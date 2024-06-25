namespace DataAccess.Exceptions;

public class WrongFilePathException(string message) : DataAccessException(message);