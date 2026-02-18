namespace EFLesDemo.Entities;

public class ScrumboardColumn
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public int Order { get; set; }
    
    public int ScrumboardId { get; set; }
    public Scrumboard Scrumboard { get; set; }

    public List<ScrumboardCard> Cards { get; set; } = new();
}