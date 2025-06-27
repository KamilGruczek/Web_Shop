using FluentValidation;
using Web_Shop.Application.DTOs;

namespace Web_Shop.Application.Validation;

public class AddUpdateProductDTOValidator : AbstractValidator<AddUpdateProductDTO>
{
    public AddUpdateProductDTOValidator()
    {
        RuleFor(request => request.Name)
            .Length(3, 255)
            .WithMessage("Pole 'Nazwa' musi zawierać od {MinLength} do {MaxLength} znaków");
        RuleFor(request => request.Price)
            .GreaterThan(0)
            .WithMessage("Pole 'Cena' musi być większe od 0");
    }
}