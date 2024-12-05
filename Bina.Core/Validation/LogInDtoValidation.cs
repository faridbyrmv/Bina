using Bina.Core.Dto.User;
using FluentValidation;

namespace Bina.Core.Validation
{
    public class LogInDtoValidator : AbstractValidator<LogInDto>
    {
        public LogInDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required.")
                .MaximumLength(50).WithMessage("UserName must not exceed 50 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
        }
    }
}
