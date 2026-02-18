using EFLesDemo.Entities;
using ScrumboardApi.Dtos.Request;
using ScrumboardApi.Dtos.Response;

namespace ScrumboardApi.Mappings;

public static class ScrumboardCardMappings
{
    public static ScrumboardCardResponseDto ToResponseDto(this ScrumboardCard entity)
    {
        return new ScrumboardCardResponseDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Order = entity.Order,
            ScrumboardColumnId = entity.ScrumboardColumnId
        };
    }

    public static ScrumboardCard ToEntity(this ScrumboardCardRequestPostDto postDto)
    {
        return new ScrumboardCard
        {
            Name = postDto.Name,
            ScrumboardColumnId = postDto.ScrumboardColumnId
        };
    }

    // public static void MapToEntity(this ScrumboardCardRequestPostDto postDto, ScrumboardCard entity)
    // {
    //     entity.Name = postDto.Name;
    //     entity.ScrumboardColumnId = postDto.ScrumboardColumnId;
    // }
}