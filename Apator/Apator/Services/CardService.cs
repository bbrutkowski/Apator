using Apator.Model;
using Apator.Services.Interface;
using System.Text;

namespace Apator.Services
{
    class CardService : ICardService
    {
        private readonly DataContext _dataContext;
        private readonly char[] AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
        private readonly Random Random = new();

        public CardService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public ICollection<Card> GetCardsByUserId(int userId) => [.. _dataContext.Cards.Where(x => x.UserId == userId)];

        public ICollection<Card> GetCards() => [.. _dataContext.Cards]; // Possibility to get all cards

        public string GenerateCardIdentifier()
        {
            // Possible addition of key generation method from library

            var length = 32;
            string identifier = string.Empty;

            var existingIdentifiers = _dataContext.Cards
                .Select(x => x.CardIdentifier)
                .ToList();

            do
            {
                identifier = GenerateRandomString(length);
            }
            while (existingIdentifiers.Any(x => x == identifier));

            return identifier;
        }

        private string GenerateRandomString(int length)
        {
            var stringBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                char randomChar = AllowedChars[Random.Next(AllowedChars.Length)];
                stringBuilder.Append(randomChar);
            }

            return stringBuilder.ToString();
        }

        public bool DeleteCard(Card card)
        {
            try
            {
                _dataContext.Cards.Remove(card);
                _dataContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EditCard(Card card)
        {
            try
            {
                var existingCard = _dataContext.Cards.First(x => x.Id == card.Id);
                if (existingCard is null) return false;

                existingCard.AccountNumber = card.AccountNumber;
                existingCard.SerialNumber = card.SerialNumber;
                existingCard.PIN = card.PIN;

                _dataContext.SaveChanges();
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
