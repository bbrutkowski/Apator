using Apator.Model;
using Apator.Services;
using Apator.Services.Interface;
using Apator.Windows;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Apator
{
    public partial class MainWindow : Window
    {
        private readonly DataContext _dataContext;
        private int _userId;
        private string _searchText = "";
        private DispatcherTimer _debounceTimer;
        private ICollection<Card> _allCards;
        private ICardService _cardService;

        public MainWindow(int userId, DataContext dataContext, ICardService cardService)
        {
            InitializeComponent();
            _dataContext = dataContext;
            _userId = userId;
            _cardService = cardService;
            _allCards = GetUserCards();
            UserCardsListView.ItemsSource = _allCards;

            _debounceTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2) 
            };
            _debounceTimer.Tick += OnDebounceTick;
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                _debounceTimer.Stop(); 
                _debounceTimer.Start();
            }
        }

        private void OnDebounceTick(object sender, EventArgs e)
        {
            _debounceTimer.Stop();
            FilterCards(); 
        }

        private void OnAddNewCardClick(object sender, RoutedEventArgs e)
        {
            var addCardWindow = new AddCardWindow(_dataContext, _cardService, _userId);
            addCardWindow.ShowDialog();

            RefreshCards();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchText = ((TextBox)sender).Text;
        }

        private void OnLogoutClick(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to log out?",
                "Logout Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _userId = 0;
                var loginWindow = new LoginWindow(_dataContext, _cardService);
                loginWindow.Show();

                Close();
            }
        }

        private void FilterCards()
        {
            var filteredCards = _allCards
                .Where(c => c.AccountNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                            c.SerialNumber.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                            c.CardIdentifier.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .ToList();

            UserCardsListView.ItemsSource = filteredCards;
        }

        private void OnClearSearchClick(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
            UserCardsListView.ItemsSource = _allCards;
        }

        private void OnEditCardClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var card = button?.DataContext as Card;

            if (card != null)
            {
                var editCardWindow = new EditCardWindow(_dataContext, card, _cardService);
                editCardWindow.ShowDialog();

                RefreshCards();
            }
        }

        private void OnDeleteCardClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var card = button?.DataContext as Card;

                if (card is not null)
                {
                    var result = MessageBox.Show(
                    "Are you sure you want to delete this card?",
                    "Delete Card",
                    MessageBoxButton.YesNo);

                    if (result == MessageBoxResult.Yes)
                    {
                        _cardService.DeleteCard(card);
                        RefreshCards();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while removing card: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ICollection<Card> GetUserCards() => _cardService.GetCardsByUserId(_userId);

        private void RefreshCards()
        {
            _allCards = GetUserCards();
            UserCardsListView.ItemsSource = _allCards;
        }
    }
}
