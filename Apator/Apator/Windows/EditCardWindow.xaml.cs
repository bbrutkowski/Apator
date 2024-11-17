using Apator.Model;
using Apator.Services;
using Apator.Services.Interface;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Apator.Windows
{
    public partial class EditCardWindow : Window
    {
        private readonly DataContext _dataContext;
        private readonly ICardService _cardService;
        private readonly Card _cardToEdit;

        public EditCardWindow(DataContext dataContext, Card card, ICardService cardService)
        {
            InitializeComponent();
            _dataContext = dataContext;
            _cardToEdit = card;
            _cardService = cardService;

            AccountNumberTextBox.Text = _cardToEdit.AccountNumber;
            PinTextBox.Text = _cardToEdit.PIN;
            SerialNumberTextBox.Text = _cardToEdit.SerialNumber;
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            _cardToEdit.AccountNumber = AccountNumberTextBox.Text;
            _cardToEdit.PIN = PinTextBox.Text;
            _cardToEdit.SerialNumber = SerialNumberTextBox.Text;

            var isUpdated = _cardService.EditCard(_cardToEdit);
            if (!isUpdated)
            {
                MessageBox.Show($"An error occurred while updating card", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBox.Show("The card has been updated.");
            Close();
        }

        private void OnlyDigits_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^\d+$");
        }

        private void OnCancelClick(object sender, RoutedEventArgs e) => Close();
    }
}
