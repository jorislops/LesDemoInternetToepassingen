using System.ComponentModel.DataAnnotations;

namespace EFLesDemo.Entities;

public class Scrumboard
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public List<ScrumboardColumn> Columns { get; set; } = new();
}

public class ScrumboardColumn
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public int Order { get; set; }
    
    public int ScrumboardId { get; set; }
    public Scrumboard Scrumboard { get; set; }

    public List<Card> Cards { get; set; } = new();
}

public class Card
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public int Order { get; set; }
    
    public DateOnly Date { get; set; }
    
    public int ScrumboardColumnId { get; set; }
    public ScrumboardColumn ScrumboardColumn { get; set; }
}

