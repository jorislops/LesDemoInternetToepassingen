using Ardalis.Specification.EntityFrameworkCore;
using EFLesDemo.Entities;

namespace EFLesDemo.Repositories;

public class ScrumCardRepository(ScrumboardDbContext db) : RepositoryBase<ScrumboardCard>(db), IScrumCardRepository
{
}