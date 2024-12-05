using Bina.Core.Dto.Homes;
using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Bina.Core.Validation;

public class CreateHomeDtoValidation : AbstractValidator<CreateHomeDto>
{
    public CreateHomeDtoValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(x => x.Adress)
            .NotEmpty().WithMessage("Address is required.")
            .MaximumLength(200).WithMessage("Address must not exceed 200 characters.");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(x => x.Photo)
            .NotEmpty().WithMessage("At least one photo is required.")
            .Must(photos => photos.Count <= 5).WithMessage("A maximum of 5 photos is allowed.")
            .ForEach(photo =>
            {
                photo.Must(file => file.ContentType.StartsWith("image/"))
                    .WithMessage("Each file must be an image.")
                    .Must(file => file.Length <= 5 * 1024 * 1024)
                    .WithMessage("Each file must not exceed 5 MB.");
            });
    }
}
