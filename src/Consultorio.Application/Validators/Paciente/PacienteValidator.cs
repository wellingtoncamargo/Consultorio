using FluentValidation;
using Consultorio.Application.Dtos.Paciente;

namespace Consultorio.Application.Validators.Paciente
{
    public class CreatePacienteValidator : AbstractValidator<CreatePacienteDto>
    {
        public CreatePacienteValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("Nome e obrigatorio")
                .MinimumLength(3).WithMessage("Nome deve ter no minimo 3 caracteres")
                .MaximumLength(255).WithMessage("Nome nao pode exceder 255 caracteres");

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email invalido");

            RuleFor(x => x.CPF)
                .NotEmpty().WithMessage("CPF e obrigatorio")
                .Matches(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$|^\d{11}$").WithMessage("CPF em formato invalido");

            RuleFor(x => x.DataNascimento)
                .NotEmpty().WithMessage("Data de nascimento e obrigatoria")
                .LessThan(System.DateTime.Today).WithMessage("Data de nascimento invalida");

            RuleFor(x => x.Celular)
                .Matches(@"^\(\d{2}\)\s?9\d{4}-\d{4}$|^\d{11}$").WithMessage("Celular em formato invalido").When(x => !string.IsNullOrEmpty(x.Celular));
        }
    }

    public class UpdatePacienteValidator : AbstractValidator<UpdatePacienteDto>
    {
        public UpdatePacienteValidator()
        {
            Include(new CreatePacienteValidator());
            RuleFor(x => x.Id).NotEqual(System.Guid.Empty).WithMessage("ID invalido");
        }
    }
}
