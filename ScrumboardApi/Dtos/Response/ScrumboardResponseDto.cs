namespace ScrumboardApi.Dtos.Response;

public class ScrumboardResponseDto
{
    public int Id { get; set; }
    public required string Name { get; set; }

    public List<ScrumboardColumnResponseDto> Columns { get; set; } = new();
}