namespace Catalog.ReadModels;

public class ReviewReadModel
{
    public Guid Id { get; set; }
    public int Rating { get; set; }
    public string Content { get; set; }
    public Guid ProductId { get; set; }
    public Guid UserId { get; set; }
    public bool Approved { get; set; }
    public DateTime CreatedAt { get; set; }
}
