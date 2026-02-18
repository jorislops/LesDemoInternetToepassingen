using System.ComponentModel.DataAnnotations;

namespace EFLesDemo.Entities;

public class Scrumboard
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public List<ScrumboardColumn> Columns { get; set; } = new();
}