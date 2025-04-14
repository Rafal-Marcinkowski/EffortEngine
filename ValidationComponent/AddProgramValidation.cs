using DataAccess.Data;
using FluentValidation;
using SharedProject.Models;

namespace ValidationComponent;

public class AddProgramValidation : AbstractValidator<Program>
{
    private readonly IProgramData programData;

    public AddProgramValidation(IProgramData programData)
    {
        this.programData = programData;

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Podaj nazwę programu");

        RuleFor(x => x.Name)
            .MustAsync(IsProgramNew)
            .When(x => !string.IsNullOrEmpty(x.Name))
            .WithMessage("Program o takiej nazwie już jest w bazie danych");
    }

    private async Task<bool> IsProgramNew(string name, CancellationToken cancellation)
    {
        var programs = await programData.GetAllProgramsAsync();
        return !programs.Any(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
