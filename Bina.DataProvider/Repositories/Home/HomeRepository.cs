using Bina.DataProvider.Entity;
using Rooms.Context;

namespace Bina.DataProvider.Repositories.Home;

public class HomeRepository : IBaseRepository<Homes>
{
    private readonly ApplicationDbContext _db;
    public HomeRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<bool> Create(Homes entity)
    {
        await _db.Home.AddAsync(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(Homes entity)
    {
        _db.Home.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public IQueryable<Homes> GetAll()
    {   
        return _db.Home;   
    }

    public async Task<Homes> Update(Homes entity)
    {
        _db.Home.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}
