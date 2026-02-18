// See https://aka.ms/new-console-template for more information

using Ardalis.Specification;
using EFLesDemo;
using EFLesDemo.Entities;
using EFLesDemo.Ports;
using EFLesDemo.UseCase;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using ScrumboardService = EFLesDemo.ScrumboardService;

using AskInt = System.Func<string, LanguageExt.Eff<int>>;


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
    
    // Eff<Unit> DeleteScrumboardById(Env env) => 
    //     from boardId in env.Console.AskInt("Please enter an Id for scrumboard delete") 
    //     from scrumboard in env.ScrumboardRepository.GetByIdWithColumnsAndCards(boardId) 
    //     from _ in scrumboard.Match(
    //         Some: board => env.ScrumboardRepository.Delete(board), 
    //         None: () => Eff.Success(Unit.Default)) 
    //     select Unit.Default;

    Func<int, Eff<Unit>> DisplayScrumboard(Env env) => (int scrumboardId) => 
        from scrumboard in env.ScrumboardRepository.GetByIdWithColumnsAndCards(scrumboardId) 
        from _ in 
            scrumboard.Match(
                Some: board => env.Console.RenderScrumboard(board), 
                None: () => Eff.Success(Unit.Default)) 
        select Unit.Default;

    Eff<Unit> DisplayScrumboardById(Env env) => 
        from boardId in env.Console.AskInt("Please enter a scrumboard id") 
        from _ in DisplayScrumboard(env)(boardId) 
        select Unit.Default;
    
    Eff<Unit> DeleteScrumCard(Env env) =>
        from boardId in env.Console.AskInt("Please enter a scrumboard id")
        from _ in DisplayScrumboard(env)(boardId)
        from cardId in env.Console.AskInt("Please enter a card id")
        from __ in env.CardRepository.Delete(cardId)
        select Unit.Default;
    
    
    // var scrumboardRepo = CreateScrumboardRepository();
    //
    // var scumboardId = AnsiConsole.Ask<int>("Please enter a scrumboard ID to display:");
    //
    // var spec = new ScrumboardSpecs.ScrumboardWithColumnsAndCardsByIdSpec(scumboardId);
    // var scrumboard = await scrumboardRepo.FirstOrDefaultAsync(spec);
    // if (scrumboard is not null)
    // {
    //     ScrumboardConsoleUtils.RenderScrumboardGrid(scrumboard);
    // }
    // else
    // {
    //     AnsiConsole.MarkupLine($"No Scrumboard found for ID: {scumboardId}.");
    // }
    //
    // var cardRepo = CreateCardRepository();
    //
    // var scrumCardId = AnsiConsole.Ask<int>("Please Enter a card ID to delete:");
    // var scrumCard =  await cardRepo.GetByIdAsync(scrumCardId);
    // if (scrumCard is null) break;
    //
    // await CreateCardRepository().DeleteAsync(scrumCard);
    //
    // scrumboard = await scrumboardRepo.FirstOrDefaultAsync(
    //     new ScrumboardSpecs.ScrumboardWithColumnsAndCardsByIdSpec(scumboardId));
    // if (scrumboard != null) ScrumboardConsoleUtils.RenderScrumboardGrid(scrumboard);
    // break;    
    

    Func<ScrumboardRepository> createScrumboardRepository = () => CreateScrumboardRepository();
    Func<CardRepository> cardRepoFactory = () => CreateCardRepository();
    
    var env = new Env(
        Console: new SpectreConsolePort(), 
        ScrumboardRepository: new ScrumboardRepoPort(createScrumboardRepository), 
        CardRepository: new CardRepositoryPort(cardRepoFactory) 
    );
    
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
            var run = DisplayScrumboardById(env).Run();
            run.IfFail(error => Console.WriteLine(error));
            break;
        }
        case Choices.DeleteScrumboard:
        {
            var w = 
                from id in env.Console.AskInt("Please enter a scrumboard ID to delete:")
                from _ in DeleteScrumboardByIdUseCase.DeleteIfFound(
                    env.ScrumboardRepository.GetByIdWithColumnsAndCards,
                    env.ScrumboardRepository.Delete)(id)
                select Unit.Default;
            var r = w.Run();
            r.Match(Succ: unit => Console.WriteLine(unit), Console.WriteLine);
            
            
            // DeleteScrumboardByIdUseCase
            //     .DeleteScrumboardById(
            //         env.Console.AskInt, 
            //         env.ScrumboardRepository.GetByIdWithColumnsAndCards, 
            //         env.ScrumboardRepository.Delete
            //     )
            //     .Run();
            // throw new NotImplementedException();
            // var run = DeleteScrumboardById(env).Run();
            // run.IfFail(error => Console.WriteLine(error));
            
            break;
        }
        case Choices.DeleteScrumCard:
        {
            var run = DeleteScrumCard(env).Run();
            run.IfFail(error => Console.WriteLine(error));
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

