using Apator.Model;

namespace Apator.Services.Interface
{
    public interface ICardService
    {
        ICollection<Card> GetCardsByUserId(int userId);
        string GenerateCardIdentifier();
        bool DeleteCard(Card card);
        bool EditCard(Card card);
        ICollection<Card> GetCards();
    }
}
