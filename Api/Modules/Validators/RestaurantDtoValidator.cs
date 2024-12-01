using Api.Dtos;
using FluentValidation;

namespace Api.Modules.Validators;

public class RestaurantDtoValidator : AbstractValidator<RestaurantDto>
{
    public RestaurantDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(255).MinimumLength(3);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
    }
}
