namespace EFLesDemo.Entities;

public class ScrumboardCard
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public int Order { get; set; }
    
    public int ScrumboardColumnId { get; set; }
    public ScrumboardColumn ScrumboardColumn { get; set; }
}