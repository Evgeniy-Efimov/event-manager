namespace EventManager.WebApi.Configuration;

public record CorsSettings(string Origins, string Methods, string Headers)
{
    public const string SectionName = "Cors";
}
