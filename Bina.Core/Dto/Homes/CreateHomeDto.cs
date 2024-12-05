using Microsoft.AspNetCore.Http;

namespace Bina.Core.Dto.Homes;

public class CreateHomeDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Adress { get; set; }
    public long Price { get; set; }
    public List<IFormFile> Photo {  get; set; }
}
