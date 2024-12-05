using Bina.DataProvider.Entity.BaseModel;

namespace Bina.DataProvider.Entity;

public class Photos : Base
{
    public string Photo { get; set; }
    public long HomeId { get; set; }
    public virtual Homes Home { get; set; }
}
