// See https://aka.ms/new-console-template for more information

using EFLesDemo;
using EFLesDemo.Entities;
using EFLesDemo.Repositories;
using Spectre.Console;
using ScrumboardService = EFLesDemo.ScrumboardService;


InitializeDatabase();
await ConsoleLoop();

AnsiConsole.Clear();

static ScrumboardRepository CreateScrumboardRepository()
{
    var db = new ScrumboardDbContext();
    var scrumboardRepository = new ScrumboardRepository(db);
    return scrumboardRepository;
}

static ScrumboardService CreateScrumboardService()
{
    var db = new ScrumboardDbContext();
    var scrumboardService = new ScrumboardService(db);
    return scrumboardService;
}

static CardRepository CreateCardRepository()
{
    var db = new ScrumboardDbContext();
    var cardRepository = new CardRepository(db);
    return cardRepository;
}



async Task ConsoleLoop()
{
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<Choices>()
            .Title("Please select a [green]option[/]:")
            .AddChoices(Enum.GetValues<Choices>())
    );

    AnsiConsole.Clear();
    AnsiConsole.MarkupLine($"You selected [yellow]{choice}.[/]");
    
    switch (choice)
    {
        case Choices.DisplayAllScrumboards:
            var scrumboards = 
                await CreateScrumboardRepository()
                    .ListAsync(new ScrumboardSpecs.ScrumboardWithColumnsAndCardsSpec());
            scrumboards.ForEach(ScrumboardConsoleUtils.RenderScrumboard);
            break;
        
        case Choices.DisplayScrumboardWithId:
        {
            var scumboardId = AnsiConsole.Ask<int>("Please enter a scrumboard ID to display:");

            var spec = new ScrumboardSpecs.ScrumboardWithColumnsAndCardsByIdSpec(scumboardId);
            var scrumboard = await CreateScrumboardRepository().FirstOrDefaultAsync(spec);
            
            if (scrumboard is not null)
            {
                ScrumboardConsoleUtils.RenderScrumboardGrid(scrumboard);
            }
            else
            {
                AnsiConsole.MarkupLine($"No Scrumboard found for ID: {scumboardId}.");
            }
            break;
        }
        case Choices.DeleteScrumboard:
        {
            var scumboardId = AnsiConsole.Ask<int>("Please Enter a scrumboard ID to delete:");

            var scrumboardToDelete = await CreateScrumboardRepository().GetByIdAsync(scumboardId);
            if (scrumboardToDelete is not null)
            {
                await CreateScrumboardRepository().DeleteAsync(scrumboardToDelete);
            }
            
            // CreateScrumboardRepository().DeleteAsync(scumboardId);
            break;
        }
        case Choices.DeleteScrumCard:
        {
            var scrumboardRepo = CreateScrumboardRepository();
            
            var scumboardId = AnsiConsole.Ask<int>("Please enter a scrumboard ID to display:");
            
            var spec = new ScrumboardSpecs.ScrumboardWithColumnsAndCardsByIdSpec(scumboardId);
            var scrumboard = await scrumboardRepo.FirstOrDefaultAsync(spec);
            if (scrumboard is not null)
            {
                ScrumboardConsoleUtils.RenderScrumboardGrid(scrumboard);
            }
            else
            {
                AnsiConsole.MarkupLine($"No Scrumboard found for ID: {scumboardId}.");
            }

            var cardRepo = CreateCardRepository();
            
            var scrumCardId = AnsiConsole.Ask<int>("Please Enter a card ID to delete:");
            var scrumCard =  await cardRepo.GetByIdAsync(scrumCardId);
            if (scrumCard is null) break;
            
            await CreateCardRepository().DeleteAsync(scrumCard);

            scrumboard = await scrumboardRepo.FirstOrDefaultAsync(
                new ScrumboardSpecs.ScrumboardWithColumnsAndCardsByIdSpec(scumboardId));
            if (scrumboard != null) ScrumboardConsoleUtils.RenderScrumboardGrid(scrumboard);
            break;
        }
        case Choices.AddCardToTheEndOfColumn:
        {
            var scrumboardId = AnsiConsole.Ask<int>("Please enter a scrumboard ID to add a card to:");
            var scrumboard = await CreateScrumboardRepository().FirstOrDefaultAsync(
                new ScrumboardSpecs.ScrumboardWithColumnsAndCardsByIdSpec(scrumboardId));
            if (scrumboard is null) break;
            
            ScrumboardConsoleUtils.RenderScrumboardGrid(scrumboard);
            
            var columnId = AnsiConsole.Ask<int>("Please enter a column ID to add the card to:");

            var cardToAdd = new Card()
            {
                Name = AnsiConsole.Ask<string>("Enter the name of the card to be added:"),
                ScrumboardColumnId = columnId
            };
            // ScrumboardRepository.AddCard(cardToAdd);
            CreateScrumboardService().AddCardEndOfColumn(columnId, cardToAdd);


            scrumboard = await CreateScrumboardRepository().FirstOrDefaultAsync(
                new ScrumboardSpecs.ScrumboardWithColumnsAndCardsByIdSpec(scrumboardId));
            if (scrumboard is null) break;
            
            ScrumboardConsoleUtils.RenderScrumboardGrid(scrumboard);
            
            break;
        } 
        case Choices.AddCardAtPosition:
        {
            var scrumboardId = AnsiConsole.Ask<int>("Please enter a scrumboard ID to add a card to:");
            var scrumboard = await CreateScrumboardRepository().FirstOrDefaultAsync(
                new ScrumboardSpecs.ScrumboardWithColumnsAndCardsByIdSpec(scrumboardId));
            if (scrumboard is null) break;
            
            ScrumboardConsoleUtils.RenderScrumboardGrid(scrumboard);
            
            var columnId = AnsiConsole.Ask<int>("Please enter a column ID to add the card to:");

            var orderPosition = AnsiConsole.Ask<int>("Please enter the position of the card to add:");
            
            var cardToAdd = new Card()
            {
                Name = AnsiConsole.Ask<string>("Enter the name of the card to be added:"),
                ScrumboardColumnId = columnId
            };
            // ScrumboardRepository.AddCard(cardToAdd);
            CreateScrumboardService().AddCardToOrder(columnId,  orderPosition, cardToAdd);

            scrumboard = await CreateScrumboardRepository().FirstOrDefaultAsync(
                new ScrumboardSpecs.ScrumboardWithColumnsAndCardsByIdSpec(scrumboardId));
            if (scrumboard is null) break;
            
            ScrumboardConsoleUtils.RenderScrumboardGrid(scrumboard);
            
            break;
        } 
        case Choices.Exit: 
            AnsiConsole.MarkupLine("[red]Exiting...[/]");
            return;
    }    
    await ConsoleLoop();
}

static void InitializeDatabase() {
    var db = new ScrumboardDbContext();
    
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();

    var scrumboard = DBSeeder.Seed();
    db.Scrumboards.AddRange(scrumboard);
    db.SaveChanges();
}

namespace EFLesDemo
{
    enum Choices
    {
        DisplayScrumboardWithId,
        DisplayAllScrumboards,
        DeleteScrumboard,
        Exit,
        DeleteScrumCard,
        AddCardToTheEndOfColumn,
        AddCardAtPosition
    }
}

