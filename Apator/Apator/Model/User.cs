namespace Apator.Model
{
    public class User
    {
        public int Id { get; set; }
        public string? Login { get; set; }
        public string? Password { get; set; }
        public ICollection<Card> Cards { get; set; }
    }
}
