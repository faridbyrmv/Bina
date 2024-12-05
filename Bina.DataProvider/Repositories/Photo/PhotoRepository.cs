using Bina.DataProvider.Entity;
using Rooms.Context;

namespace Bina.DataProvider.Repositories.Photo;

public class PhotoRepository : IBaseRepository<Photos>
{
    private readonly ApplicationDbContext _db;
    public PhotoRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<bool> Create(Photos entity)
    {
        await _db.Photos.AddAsync(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(Photos entity)
    {
        _db.Photos.Remove(entity);
        await _db.SaveChangesAsync();
        return true;
    }

    public IQueryable<Photos> GetAll()
    {
        return _db.Photos;
    }

    public async Task<Photos> Update(Photos entity)
    {
        _db.Photos.Update(entity);
        await _db.SaveChangesAsync();
        return entity;
    }
}
