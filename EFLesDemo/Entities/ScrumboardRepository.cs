using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;

namespace EFLesDemo.Entities;

public class ScrumboardRepository(ScrumboardDbContext db) : RepositoryBase<Scrumboard>(db)
{
    // public List<Scrumboard> ListScrumboards(ISpecification<Scrumboard> spec)
    // {
    //     var query = SpecificationEvaluator.Default.GetQuery(db.Scrumboards, spec);
    //     return query.ToList();
    // }
    //
    // public Scrumboard? GetFirstOrDefault(ISpecification<Scrumboard> spec)
    // {
    //     var query = SpecificationEvaluator.Default.GetQuery(db.Scrumboards, spec);
    //     return query.FirstOrDefault();
    // }
}