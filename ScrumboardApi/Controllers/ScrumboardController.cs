using EFLesDemo.Entities;
using EFLesDemo.Repositories;
using Microsoft.AspNetCore.Mvc;
using ScrumboardApi.Dtos.Response;
using ScrumboardApi.Mappings;

namespace ScrumboardApi.Controllers;

public class ScrumboardController(IScrumboardRepository scrumboardRepository) : ApiBaseController
{
    // [HttpGet("getscrumboardnotworking")]
    // public async Task<List<Scrumboard>> GetNotWorkingAsync()
    // {
    //     // this is not working (cycle in the datamodel), of course I can fix this be chancing the json properties.
    //     // it's better to use mapping instead
    //     var scrumboards = await scrumboardRepository.ListAsync(new ScrumboardSpecs.ScrumboardWithColumnsAndCardsSpec());
    //     return scrumboards;
    // }

    [HttpGet]
    public async Task<List<ScrumboardResponseDto>> GetAsync()
    {
        var scrumboards = await scrumboardRepository.ListAsync(new ScrumboardSpecs.ScrumboardWithColumnsAndCardsSpec());
        // manual mapping
        var scrumboardResponseDtos = scrumboards.Select(board => board.ToScrumboardResponseDto()).ToList();
        return scrumboardResponseDtos;
    }
}