using Bina.Core.Dto.Homes;
using Bina.Core.Services.Interfaces;
using Bina.DataProvider.Entity;
using Bina.DataProvider.Repositories;
using Bina.DataProvider.Response;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Bina.Core.Services.Implementations;

public class HomeService : IHomeService
{
    private readonly IBaseRepository<Homes> _rep;
    private readonly IBaseRepository<Photos> _repository;

    public HomeService(IBaseRepository<Homes> rep, IBaseRepository<Photos> repository)
    {
        _rep = rep;
        _repository = repository;
    }

    public async Task<BaseResponse<Homes>> CreateAsync(CreateHomeDto model)
    {
        try
        {
            Log.Information("Starting home creation.");

            var home = new Homes()
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                Adress = model.Adress,
                CreateAt = DateTime.Now,
            };

            await _rep.Create(home);

            foreach (var item in model.Photo)
            {
                var uploadDirectory = Path.Combine("wwwroot", "upload");
                if (!Directory.Exists(uploadDirectory))
                {
                    Directory.CreateDirectory(uploadDirectory);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(item.FileName);
                var saveFilePath = Path.Combine(uploadDirectory, fileName);
                using (var stream = new FileStream(saveFilePath, FileMode.Create))
                {
                    await item.CopyToAsync(stream);
                }

                var photo = new Photos()
                {
                    HomeId = home.Id,
                    Photo = fileName
                };

                await _repository.Create(photo);
            }

            Log.Information("Home created successfully with ID: {HomeId}", home.Id);
            return new BaseResponse<Homes>(home, true, "Home created successfully.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while creating home.");
            return new BaseResponse<Homes>(null, false, "An error occurred while creating the home.");
        }
    }

    public async Task<BaseResponse<bool>> DeleteAsync(long id)
    {
        try
        {
            Log.Information("Starting deletion of home with ID: {HomeId}", id);

            var data = await _rep.GetAll().SingleOrDefaultAsync(x => x.Id == id);
            var ph = await _repository.GetAll().Where(x => x.HomeId == id).ToListAsync();

            foreach (var item in ph)
            {
                item.IsDeleted = true;
            }

            data.IsDeleted = true;
            await _rep.Update(data);

            Log.Information("Home with ID: {HomeId} marked as deleted.", id);
            return new BaseResponse<bool>(true, true, "");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while deleting home with ID: {HomeId}", id);
            return new BaseResponse<bool>(false, false, "An error occurred while deleting the home.");
        }
    }

    public async Task<BaseResponse<List<GetHomeDto>>> GetAllAsync()
    {
        try
        {
            Log.Information("Fetching all homes.");

            var data = await _rep.GetAll()
                                 .Where(x => !x.IsDeleted)
                                 .ToListAsync();

            foreach (var home in data)
            {
                home.Photos = await _repository.GetAll().Where(x => x.HomeId == home.Id).ToListAsync();
            }

            var kanalDtos = data.Select(home => new GetHomeDto
            {
                id = home.Id,
                isDeleted = home.IsDeleted,
                CreateAt = home.CreateAt,
                Name = home.Name,
                Adress = home.Adress,
                Description = home.Description,
                Price = home.Price,
                Photo = home.Photos?.FirstOrDefault()?.Photo
            }).ToList();

            Log.Information("Fetched {Count} homes with photos.", kanalDtos.Count);
            return new BaseResponse<List<GetHomeDto>>(kanalDtos, true, "Homes with photos retrieved successfully.");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while fetching homes.");
            return new BaseResponse<List<GetHomeDto>>(null, false, "An error occurred while fetching homes.");
        }
    }

    public async Task<BaseResponse<Homes>> GetByIdAsync(long id)
    {
        try
        {
            Log.Information("Fetching home with ID: {HomeId}", id);

            var data = await _rep.GetAll().SingleOrDefaultAsync(x => x.Id == id);
            data.Photos = await _repository.GetAll().Where(x => x.Id == id).ToListAsync();

            Log.Information("Fetched home with ID: {HomeId}", id);
            return new BaseResponse<Homes>(data, true, "");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Error while fetching home with ID: {HomeId}", id);
            return new BaseResponse<Homes>(null, false, "An error occurred while fetching the home.");
        }
    }
}
