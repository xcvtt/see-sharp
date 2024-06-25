namespace DataAccess.Exceptions;

public class DataAccessException(string message) : Exception(message)
{
    public static WrongFilePathException WrongFilePath(string filePath)
    {
        return new WrongFilePathException($"File {filePath} doesn't exist");
    }
}