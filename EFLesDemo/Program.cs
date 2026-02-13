// See https://aka.ms/new-console-template for more information

using EFLesDemo;
using EFLesDemo.Entities;
using Spectre.Console;
using ScrumboardService = EFLesDemo.ScrumboardService;


InitializeDatabase();
ConsoleLoop();

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



void ConsoleLoop()
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
            var scrumboards = CreateScrumboardRepository().LoadScrumboardWithColumnAndCards();
            scrumboards.ForEach(ScrumboardConsoleUtils.RenderScrumboard);
            break;
        case Choices.DisplayScrumboardWithId:
        {
            var scumboardId = AnsiConsole.Ask<int>("Please enter a scrumboard ID to display:");
            var scrumboard = CreateScrumboardRepository().LoadScrumboardWithColumnAndCardsById(scumboardId);
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
            CreateScrumboardRepository().DeleteScrumboard(scumboardId);
            break;
        }
        case Choices.DeleteScrumCard:
        {
            var scumboardId = AnsiConsole.Ask<int>("Please enter a scrumboard ID to display:");
            var scrumboard = CreateScrumboardRepository().LoadScrumboardWithColumnAndCardsById(scumboardId);
            if (scrumboard is not null)
            {
                ScrumboardConsoleUtils.RenderScrumboardGrid(scrumboard);
            }
            else
            {
                AnsiConsole.MarkupLine($"No Scrumboard found for ID: {scumboardId}.");
            }
            
            var scrumCardId = AnsiConsole.Ask<int>("Please Enter a card ID to delete:");
            var deleteCard = CreateScrumboardRepository().DeleteScrumCard(scrumCardId);
            if (deleteCard is null) break;
            
            scrumboard = CreateScrumboardRepository().LoadScrumboardWithColumnAndCardsById(scumboardId);
            if (scrumboard != null) ScrumboardConsoleUtils.RenderScrumboardGrid(scrumboard);
            break;
        }
        case Choices.AddCardToTheEndOfColumn:
        {
            var scrumboardId = AnsiConsole.Ask<int>("Please enter a scrumboard ID to add a card to:");
            var scrumboard = CreateScrumboardRepository().LoadScrumboardWithColumnAndCardsById(scrumboardId);
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


            scrumboard = CreateScrumboardRepository().LoadScrumboardWithColumnAndCardsById(scrumboardId);
            if (scrumboard is null) break;
            
            ScrumboardConsoleUtils.RenderScrumboardGrid(scrumboard);
            
            break;
        } 
        case Choices.AddCardAtPosition:
        {
            var scrumboardId = AnsiConsole.Ask<int>("Please enter a scrumboard ID to add a card to:");
            var scrumboard = CreateScrumboardRepository().LoadScrumboardWithColumnAndCardsById(scrumboardId);
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

            scrumboard = CreateScrumboardRepository().LoadScrumboardWithColumnAndCardsById(scrumboardId);
            if (scrumboard is null) break;
            
            ScrumboardConsoleUtils.RenderScrumboardGrid(scrumboard);
            
            break;
        } 
        case Choices.Exit: 
            AnsiConsole.MarkupLine("[red]Exiting...[/]");
            return;
    }    
    ConsoleLoop();
}

static void InitializeDatabase() {
    var db = new ScrumboardDbContext();
    
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();

    var scrumboard = DBSeeder.Seed();
    db.Scrumboards.AddRange(scrumboard);
    db.SaveChanges();
}

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

