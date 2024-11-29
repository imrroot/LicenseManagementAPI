namespace LicenseManagementAPI.Core.Exceptions;

public class EntityDuplicateException : Exception
{
    public EntityDuplicateException(string message) : base(message) { }
}
public class EntityNotFoundException : Exception
{
    public EntityNotFoundException(string message) : base(message) { }
}
public class EntityValidationException : Exception
{
    public EntityValidationException(string message) : base(message) { }
}
public class DatabaseConstraintException : Exception
{
    public DatabaseConstraintException(string message, Exception innerException) 
        : base(message, innerException) { }
}
public class ConcurrencyException : Exception
{
    public ConcurrencyException(string message) : base(message) { }
}
public class ResourceLimitExceededException : Exception
{
    public ResourceLimitExceededException(string message) : base(message) { }
}
public class DataIntegrityException : Exception
{
    public DataIntegrityException(string message) : base(message) { }
}



