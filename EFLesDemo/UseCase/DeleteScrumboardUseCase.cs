using EFLesDemo.Entities;
using EFLesDemo.Ports;
using LanguageExt;
using LanguageExt.Common;

namespace EFLesDemo.UseCase;

public static class DeleteScrumboardByIdUseCase
{
    public static Eff<Unit> DeleteScrumboardById(  Func<string, Eff<int>> askInt, 
                                            Func<int, Eff<Option<Scrumboard>>> getByIdWithColumnsAndCards, 
                                            Func<Scrumboard, Eff<Unit>> delete) =>
        from scrumboardId in askInt("Scrumboard ID to delete")
        from _id in scrumboardId > 0 ? Eff<int>.Pure(scrumboardId) : Eff<int>.Fail(Error.New("Scrumboard ID must be a positive integer")) 
        from scrumboard in getByIdWithColumnsAndCards(_id)
        from _ in
            scrumboard.Match(
                Some: delete, 
                None: Eff<Unit>.Fail(Error.New("scrumboard not found for id")))
        select Unit.Default; 

    public static Func<int, Eff<Unit>> DeleteIfFound<T>(
        Func<int,  Eff<Option<T>>> getById,
        Func<T, Eff<Unit>> delete,
        Func<int, Error> notFound) => 
            (int id) =>
                from opt in getById(id)
                from _   in opt.Match(
                    Some: delete,
                    None: () => notFound(id))
                select Unit.Default;

    public static Func<int, Eff<Unit>> DeleteIfFound<T>(
        Func<int, Eff<Option<T>>> getById,
        Func<T, Eff<Unit>> delete) => 
            (int id) =>
                DeleteIfFound(getById, delete, idNotFound => Error.New($"Id not found {idNotFound}"))(id);
}