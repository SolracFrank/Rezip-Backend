namespace Domain.Exceptions;

public class InfraestructureException : Exception
{
    public InfraestructureException()
    {
    }

    public InfraestructureException(string message) : base(message)
    {
    }
}