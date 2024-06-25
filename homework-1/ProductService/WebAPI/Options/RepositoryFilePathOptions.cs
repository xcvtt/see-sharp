namespace WebAPI.Options;

public class RepositoryFilePathOptions
{
    public const string RepositoryFilePath = "RepositoryFilePath";
    
    public string ProductDailyHistoryFilePath { get; set; } = string.Empty;
    public string ProductSeasonalCoeffsFilePath { get; set; } = string.Empty;
}