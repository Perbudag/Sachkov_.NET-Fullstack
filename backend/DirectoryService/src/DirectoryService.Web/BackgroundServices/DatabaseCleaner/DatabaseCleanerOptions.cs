namespace DirectoryService.Web.BackgroundServices.DatabaseCleaner;

public class DatabaseCleanerOptions
{
    public const string SectionName = "DatabaseCleanerSettings";

    public TimeSpan DelayTime { get; set; }
    public TimeSpan AgeOfDeletion { get; set; }
    public int BatchSize { get; set; }
}
