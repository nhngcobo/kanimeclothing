using System.Globalization;
using System.Text.Json;

namespace kanimeclothing.Services;

/// <summary>
/// JSON-based localization service
/// </summary>
public interface ILocalizationService
{
    string GetString(string key);
    string this[string key] { get; }
}

public class JsonLocalizationService : ILocalizationService
{
    private readonly Dictionary<string, string> _translations;

    public JsonLocalizationService(IHttpContextAccessor httpContextAccessor)
    {
        _translations = LoadTranslations(httpContextAccessor);
    }

    public string this[string key] => GetString(key);

    private Dictionary<string, string> LoadTranslations(IHttpContextAccessor httpContextAccessor)
    {
        var culture = httpContextAccessor?.HttpContext?.Features
            .Get<Microsoft.AspNetCore.Localization.IRequestCultureFeature>()
            ?.RequestCulture.Culture ?? CultureInfo.CurrentCulture;

        var cultureName = culture.Name.Split('-')[0]; // Get language code (e.g., "en" from "en-US")
        var basePath = Directory.GetCurrentDirectory();
        var resourcePath = Path.Combine(basePath, "Resources", $"{cultureName}.json");

        if (!File.Exists(resourcePath))
        {
            resourcePath = Path.Combine(basePath, "Resources", "en.json"); // Fallback to English
        }

        var json = File.ReadAllText(resourcePath);
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
    }

    public string GetString(string key)
    {
        return _translations.TryGetValue(key, out var value) ? value : key;
    }
}
