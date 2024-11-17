namespace Apator.Model
{
    public class Card
    {
        public int Id { get; set; }
        public string? AccountNumber { get; set; }
        public string? PIN { get; set; }
        public string? SerialNumber { get; set; }
        public string? CardIdentifier { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
