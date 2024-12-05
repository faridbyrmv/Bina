using System.ComponentModel.DataAnnotations;

namespace Bina.Core.Dto.User;

public class RegistrationDto
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
