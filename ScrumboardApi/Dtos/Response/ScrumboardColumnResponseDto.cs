namespace ScrumboardApi.Dtos.Response;

public class ScrumboardColumnResponseDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    
    public int Order { get; set; }

    public List<ScrumboardCardResponseDto> Cards { get; set; } = new();
}