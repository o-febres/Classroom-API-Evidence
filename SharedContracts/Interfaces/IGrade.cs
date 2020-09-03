namespace SharedContracts.Interfaces
{
    public interface IGrade : IEntity
    {
        int StudentId { get; set; }
        string Subject { get; set; }
        int Score { get; set; }
    }
}