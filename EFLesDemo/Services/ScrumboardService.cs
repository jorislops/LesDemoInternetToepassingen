using EFLesDemo.Entities;

namespace EFLesDemo.Services;

public class ScrumboardService(ScrumboardDbContext _db) : IScrumboardService
{
    // public ScrumboardCard AddCardEndOfColumn(int columnId, ScrumboardCard scrumboardCard)
    // {
    //     //It's better to only use repo methods inside a service
    //
    //     var maxOrder = _db.Cards.Where(x => x.ScrumboardColumnId == columnId)
    //         .Max(x => x.Order);
    //     scrumboardCard.Order = maxOrder + 1;
    //
    //     scrumboardCard.ScrumboardColumnId = columnId;
    //
    //     _db.Cards.Add(scrumboardCard);
    //     _db.SaveChanges();
    //     return scrumboardCard;
    // }
    //
    // public void AddCardPreserveOrder(int columnId, int order, ScrumboardCard scrumboardCard)
    // {
    //     //moet eigenlijk met repo's!!!
    //     var cardsWithHigherOrder =
    //         _db.Cards.Where(x => x.ScrumboardColumnId == columnId
    //                              && x.Order >= order
    //             )
    //             .OrderBy(x => x.Order)
    //             .ToList();
    //
    //     cardsWithHigherOrder.ForEach(x => x.Order = x.Order + 1);
    //     //no need to update the changes!!!!
    //     //EF tracks entities (change tracker)
    //
    //     scrumboardCard.Order = order;
    //
    //     scrumboardCard.ScrumboardColumnId = columnId;
    //
    //     _db.Cards.Add(scrumboardCard);
    //
    //
    //     _db.SaveChanges();
    // }

    public ScrumboardCard InsertOrUpdateCardPreserveOrder(ScrumboardCard card)
    {
        // -----------------------------------------------------------
        // STEP 1: Determine if this is INSERT or UPDATE
        // -----------------------------------------------------------
        var existing = _db.Cards.FirstOrDefault(x => x.Id == card.Id);

        bool isInsert = existing == null;

        if (isInsert)
        {
            // -----------------------------------------------------------
            // INSERT LOGIC (new card)
            // -----------------------------------------------------------

            // SHIFT UP all cards with order >= new order
            var cardsToShift = _db.Cards
                .Where(x => x.ScrumboardColumnId == card.ScrumboardColumnId &&
                            x.Order >= card.Order)
                .ToList();

            foreach (var c in cardsToShift)
                c.Order++;

            _db.Cards.Add(card);
            _db.SaveChanges();
            return card;
        }

        // -----------------------------------------------------------
        // UPDATE LOGIC (existing card)
        // -----------------------------------------------------------

        int oldColumnId = existing.ScrumboardColumnId;
        int newColumnId = card.ScrumboardColumnId;
        int oldOrder = existing.Order;
        int newOrder = card.Order;

        bool columnChanged = oldColumnId != newColumnId;

        // Update non-order properties (e.g. name)
        existing.Name = card.Name;

        // -----------------------------------------------------------
        // Moving card to ANOTHER COLUMN
        // -----------------------------------------------------------
        if (columnChanged)
        {
            // Close gap in old column
            var oldColumnCards = _db.Cards
                .Where(x => x.ScrumboardColumnId == oldColumnId &&
                            x.Order > oldOrder)
                .ToList();

            foreach (var c in oldColumnCards)
                c.Order--;

            // Make space in new column
            var newColumnCards = _db.Cards
                .Where(x => x.ScrumboardColumnId == newColumnId &&
                            x.Order >= newOrder)
                .ToList();

            foreach (var c in newColumnCards)
                c.Order++;

            // Apply changes
            existing.ScrumboardColumnId = newColumnId;
            existing.Order = newOrder;

            _db.SaveChanges();
        }
        return existing;
    }
}