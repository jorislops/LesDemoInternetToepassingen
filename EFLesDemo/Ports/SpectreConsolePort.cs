using EFLesDemo.Entities;
using EFLesDemo.Ports;
using LanguageExt;
using LanguageExt.Common;
using Spectre.Console;
using static LanguageExt.Prelude;

namespace EFLesDemo.Ports;

public interface IConsole
{
    Eff<int> AskInt(string prompt);
    Eff<Unit> RenderScrumboard(Scrumboard scrumboard);
}



public class SpectreConsolePort : IConsole
{
    public Eff<int> AskInt(string prompt) =>
        Eff.lift(() => AnsiConsole.Ask<int>(prompt));

    public Eff<Unit> RenderScrumboard(Scrumboard scrumboard) =>
        Eff.lift(() =>
        {
            ScrumboardConsoleUtils.RenderScrumboardGrid(scrumboard);
            return Unit.Default;
        });
}

public interface IScrumboardRepoPort
{
    Eff<Option<Scrumboard>> GetByIdWithColumnsAndCards(int boardId);
    Eff<Unit> Delete(Scrumboard board);
}

public class ScrumboardRepoPort(Func<ScrumboardRepository>  scrumboardRepositoryFactory) : 
    IScrumboardRepoPort
{
    public Eff<Option<Scrumboard>> GetByIdWithColumnsAndCards(int boardId) =>
        Eff.lift(async () =>
        {
            var scrumboardRepo = scrumboardRepositoryFactory();
            Option<Scrumboard> scrumboard =
                await scrumboardRepo.FirstOrDefaultAsync(
                    new ScrumboardSpecs.ScrumboardWithColumnsAndCardsByIdSpec(boardId));

            return scrumboard;
        });

    public Eff<Unit> Delete(Scrumboard board) =>
        Eff.lift(async () =>
        {
            await scrumboardRepositoryFactory().DeleteAsync(board);
            return Unit.Default;
        });

   
}

public interface ICardRepositoryPort
{
    Eff<Unit> Delete(int cardId);
}

public class CardRepositoryPort(Func<CardRepository> cardRepositoryFactory) : ICardRepositoryPort
{
    public Eff<Unit> Delete(int cardId) =>
        Eff.lift(async () =>
        {
            var repo = cardRepositoryFactory();
            var card = await repo.GetByIdAsync(cardId);
            if (card is not null)
            {
                await repo.DeleteAsync(card);
            }
            return Unit.Default;
        });
}

public record Env(IConsole Console, IScrumboardRepoPort ScrumboardRepository, ICardRepositoryPort CardRepository)
{
    
}

public static class ScrumboardDisplayerProgram
{
    public static Func<Env, Eff<int>> AskInt(string prompt) => env =>
        env.Console.AskInt(prompt);

    public static Func<Env, Eff<Unit>> RenderScrumboard(Scrumboard scrumboard) => env =>
        env.Console.RenderScrumboard(scrumboard);

    public static Func<Env, Eff<Option<Scrumboard>>> GetByIdWithColumnsAndCards(int boardId) =>
        env => env.ScrumboardRepository.GetByIdWithColumnsAndCards(boardId);

    public static Func<Env, Eff<Unit>> DeleteCard(int cardId) => env =>
        env.CardRepository.Delete(cardId);
}