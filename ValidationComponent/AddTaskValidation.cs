using FluentValidation;
using SharedProject.Models;

namespace ValidationComponent;

public class AddTaskValidation : AbstractValidator<TaskBase>
{
    public AddTaskValidation()
    {
        RuleFor(task => task.Name)
            .NotEmpty()
            .WithMessage("Nazwa zadania nie może być pusta.");
        RuleFor(task => task.Description)
            .NotEmpty()
            .WithMessage("Opis zadania nie może być pusty.");
        RuleFor(task => task.Priority)
            .InclusiveBetween(0, 3)
            .WithMessage("Ustaw priorytet zadania");
        RuleFor(task => task.ProgramId)
            .GreaterThan(0)
            .WithMessage("Wybierz program");
    }
}
