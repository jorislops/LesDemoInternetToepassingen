using System.ComponentModel.DataAnnotations;

namespace EFLesDemo.Entities;

public class Scrumboard
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public ICollection<ScrumboardColumn> Columns { get; set; }
}

public class ScrumboardColumn
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public int Order { get; set; }
    
    public int ScrumboardId { get; set; }
    public Scrumboard Scrumboard { get; set; }
    
    public ICollection<Card> Cards { get; set; } = new List<Card>();
}

public class Card
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public int Order { get; set; }
    
    public int ScrumboardColumnId { get; set; }
    public ScrumboardColumn ScrumboardColumn { get; set; }
}

