namespace ScrumboardApi.Dtos.Request;

public class ScrumboardCardRequestPostDto
{
    public required string Name { get; set; }
    public int ScrumboardColumnId { get; set; }
}

public class ScrumboardCardRequestUpdateDto : ScrumboardCardRequestPostDto
{
    public int Order { get; set; }
}
