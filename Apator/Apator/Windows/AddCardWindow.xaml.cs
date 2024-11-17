using Apator.Model;
using Apator.Services;
using Apator.Services.Interface;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Apator
{
    public partial class AddCardWindow : Window
    {
        private readonly DataContext _dataContext;
        private readonly ICardService _cardService;
        private readonly int _userId;
        private bool _isSaveButtonEnabled;

        public bool IsSaveButtonEnabled
        {
            get => _isSaveButtonEnabled;
            set
            {
                _isSaveButtonEnabled = value;
                SaveButton.IsEnabled = value;
            }
        }

        public AddCardWindow(DataContext dataContext, ICardService cardService, int userId)
        {
            InitializeComponent();
            _dataContext = dataContext;
            _cardService = cardService;
            _userId = userId;
        }

        private void OnlyDigits_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^\d+$");
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsFormValid())
                {
                    MessageBox.Show("Please fill all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                SaveProgressBar.Visibility = Visibility.Visible;

                var cardId = _cardService.GenerateCardIdentifier();

                var newCard = new Card
                {
                    AccountNumber = AccountNumberTextBox.Text,
                    PIN = PinTextBox.Text,
                    SerialNumber = SerialNumberTextBox.Text,
                    CardIdentifier = cardId,
                    UserId = _userId
                };

                _dataContext.Cards.Add(newCard);
                _dataContext.SaveChanges();

                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while adding the card: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SaveProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private bool IsFormValid()
        {
            if (string.IsNullOrWhiteSpace(AccountNumberTextBox.Text) ||
               string.IsNullOrWhiteSpace(PinTextBox.Text) ||
               string.IsNullOrWhiteSpace(SerialNumberTextBox.Text)) return false;

            return true;
        }

        private void OnCancelClick(object sender, RoutedEventArgs e) => Close();

        private void AccountNumberTextBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateSaveButtonState();

        private void PinBox_PinChanged(object sender, RoutedEventArgs e) => UpdateSaveButtonState();

        private void SerialNumberTextBox_TextChanged(object sender, RoutedEventArgs e) => UpdateSaveButtonState();

        private void UpdateSaveButtonState() => IsSaveButtonEnabled = !string.IsNullOrWhiteSpace(AccountNumberTextBox.Text) &&
                             !string.IsNullOrWhiteSpace(PinTextBox.Text) && !string.IsNullOrWhiteSpace(SerialNumberTextBox.Text);
    }
}
