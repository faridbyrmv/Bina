using Bina.Core.Dto.Homes;
using Bina.DataProvider.Entity;
using Bina.DataProvider.Response;
using MailKit.Search;

namespace Bina.Core.Services.Interfaces;

public interface IHomeService
{
    Task<BaseResponse<Homes>> CreateAsync(CreateHomeDto model);
    Task<BaseResponse<List<GetHomeDto>>> GetAllAsync();
    Task<BaseResponse<Homes>> GetByIdAsync(long id);
    Task<BaseResponse<bool>> DeleteAsync(long id);
}
