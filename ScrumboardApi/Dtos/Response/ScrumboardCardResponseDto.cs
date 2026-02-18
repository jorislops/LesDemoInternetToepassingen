namespace ScrumboardApi.Dtos.Response;

public class ScrumboardCardResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Order { get; set; }
    public int ScrumboardColumnId { get; set; }
}