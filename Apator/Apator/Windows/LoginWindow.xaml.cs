using Apator.Services;
using Apator.Services.Interface;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Apator
{
    public partial class LoginWindow : Window
    {
        private readonly DataContext _dataContext;
        private readonly ICardService _cardService;
        private int _userId;
        private bool _isLoginEnabled;

        public bool IsLoginEnabled
        {
            get => _isLoginEnabled;
            set
            {
                _isLoginEnabled = value;
                LoginButton.IsEnabled = value;
            }
        }

        public LoginWindow(DataContext dataContext, ICardService cardService)
        {
            InitializeComponent();
            _dataContext = dataContext;
            _cardService = cardService;
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) OnLoginClick(LoginButton, null);
        }

        private void OnLoginClick(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text;
            var password = PasswordTextBox.Password;

            try
            {
                if (!IsLoginValid(username, password))
                {
                    MessageBox.Show("Invalid username or password. Please try again.");
                    return;
                }

                var mainWindow = new MainWindow(_userId, _dataContext, _cardService);
                mainWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while login user: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e) => UpdateLoginButtonState();

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e) => UpdateLoginButtonState();

        private void UpdateLoginButtonState() => IsLoginEnabled = !string.IsNullOrWhiteSpace(UsernameTextBox.Text) &&
                             !string.IsNullOrWhiteSpace(PasswordTextBox.Password);

        private bool IsLoginValid(string username, string password)
        {
            if (string.IsNullOrEmpty(username)) return false;

            var user = _dataContext.Users
                .Where(x => x.Login == username && x.Password == password)
                .FirstOrDefault();

            if (user is null) return false;

            _userId = user.Id;

            return true;
        }
    }
}
