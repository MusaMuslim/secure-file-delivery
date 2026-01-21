using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecureFileDelivery.Domain.Exceptions;

// Base exception for all domain-specific errors
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

// Thrown when a requested entity is not found
public class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entityName, object id)
        : base($"{entityName} with id '{id}' was not found")
    {
    }
}

// Thrown when a download link has expired or is invalid
public class InvalidLinkException : DomainException
{
    public InvalidLinkException(string message) : base(message)
    {
    }
}

// Thrown when file validation fails
public class InvalidFileException : DomainException
{
    public InvalidFileException(string message) : base(message)
    {
    }
}
