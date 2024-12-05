using Bina.DataProvider.Entity.BaseModel;

namespace Bina.DataProvider.Entity;

public class Homes : Base
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Adress { get; set; }
    public long Price { get; set; }

    public virtual List<Photos> Photos { get; set; }

}
