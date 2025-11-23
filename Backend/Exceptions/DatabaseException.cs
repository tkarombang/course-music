namespace Backend.Exceptions
{
  public class DatabaseException : Exception
  {
    public DatabaseException(string? message, Exception err) : base(message)
    {
    }
  }
}