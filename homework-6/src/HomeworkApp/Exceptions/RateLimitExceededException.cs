namespace HomeworkApp.Exceptions;

public class RateLimitExceededException(string msg) : Exception(msg);