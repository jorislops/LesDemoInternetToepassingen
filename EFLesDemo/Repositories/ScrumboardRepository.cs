using Ardalis.Specification.EntityFrameworkCore;
using EFLesDemo.Entities;

namespace EFLesDemo.Repositories;

public class ScrumboardRepository(ScrumboardDbContext db) : RepositoryBase<Scrumboard>(db), IScrumboardRepository
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