using Ardalis.Specification;

namespace EFLesDemo.Entities;

public class ScrumboardSpecs
{
    public class ScrumboardWithColumnsAndCardsSpec : Specification<Scrumboard>
    {
        public ScrumboardWithColumnsAndCardsSpec()
        {
            Query
                .AsNoTracking()
                .Include(scrumboard => scrumboard.Columns.OrderBy(column => column.Order))
                .ThenInclude(column => column.Cards.OrderBy(card => card.Order));
        }
    }

    public class ScrumboardWithColumnsAndCardsByIdSpec : Specification<Scrumboard>
    {
        public ScrumboardWithColumnsAndCardsByIdSpec(int scrumboardId)
        {
            Query
                .AsNoTracking()
                .Where(scrumboard => scrumboard.Id == scrumboardId)
                .Include(scrumboard => scrumboard.Columns.OrderBy(column => column.Order))
                .ThenInclude(column => column.Cards.OrderBy(card => card.Order));
        }
    }
}