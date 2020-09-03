using SharedContracts.Interfaces;

namespace SharedContracts.Models
{
    public class Grade : IGrade
    {
        public int StudentId { get; set; }
        public string Subject { get; set; }
        public int Score { get; set; }
        public long Id { get; set; }
    }
}