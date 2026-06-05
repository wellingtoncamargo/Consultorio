using System;

namespace Consultorio.Application.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }

    public class NotFoundException : DomainException
    {
        public NotFoundException(string resource, Guid id) 
            : base($"{resource} com ID '{id}' nao encontrado.") { }
    }

    public class ValidationException : DomainException
    {
        public ValidationException(string message) : base(message) { }
    }

    public class UnauthorizedException : DomainException
    {
        public UnauthorizedException(string message = "Acesso nao autorizado.") : base(message) { }
    }

    public class ConflictException : DomainException
    {
        public ConflictException(string message) : base(message) { }
    }
}
