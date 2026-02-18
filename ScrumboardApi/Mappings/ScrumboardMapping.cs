using EFLesDemo.Entities;
using ScrumboardApi.Dtos.Response;

namespace ScrumboardApi.Mappings;

public static class ScrumboardMapping
{
    public static ScrumboardResponseDto ToScrumboardResponseDto(this Scrumboard scrumboard) =>
        new ScrumboardResponseDto()
        {
            Id = scrumboard.Id,
            Name = scrumboard.Name,
            Columns = scrumboard.Columns.Select(columns => 
                columns.ToScrumboardColumnResponseDto()
            ).ToList()
        };

    public static ScrumboardColumnResponseDto ToScrumboardColumnResponseDto(this ScrumboardColumn column) =>
        new ScrumboardColumnResponseDto()
        {
            Id = column.Id,
            Name = column.Name,
            Order = column.Order,
            Cards = column.Cards.Select(card => new ScrumboardCardResponseDto()
            {
                Id =  card.Id,
                Name = card.Name, 
                Order =  card.Order
            }).ToList()
        };
    
    
}