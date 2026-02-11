// See https://aka.ms/new-console-template for more information

using EFLesDemo;
using EFLesDemo.Entities;
using Spectre.Console;



InitializeDatabase();
ConsoleLoop();

AnsiConsole.Clear();
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
            var scrumboards = ScrumboardRepository.LoadScrumboardWithColumnAndCards();
            scrumboards.ForEach(ScrumboardConsoleUtils.RenderScrumboard);
            break;
        case Choices.DisplayScrumboardWithId:
        {
            var scumboardId = AnsiConsole.Ask<int>("Please enter a scrumboard ID to display:");
            var scrumboard = ScrumboardRepository.LoadScrumboardWithColumnAndCardsById(scumboardId);
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
            ScrumboardRepository.DeleteScrumboard(scumboardId);
            break;
        }
        case Choices.DeleteScrumCard:
        {
            var scumboardId = AnsiConsole.Ask<int>("Please enter a scrumboard ID to display:");
            var scrumboard = ScrumboardRepository.LoadScrumboardWithColumnAndCardsById(scumboardId);
            if (scrumboard is not null)
            {
                ScrumboardConsoleUtils.RenderScrumboardGrid(scrumboard);
            }
            else
            {
                AnsiConsole.MarkupLine($"No Scrumboard found for ID: {scumboardId}.");
            }
            
            var scrumCardId = AnsiConsole.Ask<int>("Please Enter a card ID to delete:");
            var deleteCard = ScrumboardRepository.DeleteScrumCard(scrumCardId);
            if (deleteCard is null) break;
            
            scrumboard = ScrumboardRepository.LoadScrumboardWithColumnAndCardsById(scumboardId);
            if (scrumboard != null) ScrumboardConsoleUtils.RenderScrumboardGrid(scrumboard);
            break;
        }
        case Choices.AddCardToTheEndOfColumn:
        {
            var scrumboardId = AnsiConsole.Ask<int>("Please enter a scrumboard ID to add a card to:");
            var scrumboard = ScrumboardRepository.LoadScrumboardWithColumnAndCardsById(scrumboardId);
            if (scrumboard is null) break;
            
            ScrumboardConsoleUtils.RenderScrumboardGrid(scrumboard);
            
            var columnId = AnsiConsole.Ask<int>("Please enter a column ID to add the card to:");

            var cardToAdd = new Card()
            {
                Name = AnsiConsole.Ask<string>("Enter the name of the card to be added:"),
                ScrumboardColumnId = columnId
            };
            // ScrumboardRepository.AddCard(cardToAdd);
            ScrumboardService.AddCardEndOfColumn(columnId, cardToAdd);

            scrumboard = ScrumboardRepository.LoadScrumboardWithColumnAndCardsById(scrumboardId);
            if (scrumboard is null) break;
            
            ScrumboardConsoleUtils.RenderScrumboardGrid(scrumboard);
            
            break;
        } 
        case Choices.AddCardAtPosition:
        {
            var scrumboardId = AnsiConsole.Ask<int>("Please enter a scrumboard ID to add a card to:");
            var scrumboard = ScrumboardRepository.LoadScrumboardWithColumnAndCardsById(scrumboardId);
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
            ScrumboardService.AddCardToOrder(columnId,  orderPosition, cardToAdd);

            scrumboard = ScrumboardRepository.LoadScrumboardWithColumnAndCardsById(scrumboardId);
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