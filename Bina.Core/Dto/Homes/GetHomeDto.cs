using Microsoft.AspNetCore.Http;

namespace Bina.Core.Dto.Homes;

public class GetHomeDto
{
    public long id { get; set; }
    public bool isDeleted { get; set; }
    public DateTime CreateAt { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Adress { get; set; }
    public long Price { get; set; }
    public string Photo { get; set; }
}
