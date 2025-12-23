using System.Globalization;
using System.Text.Json;

namespace EnterpriseTicketing.Web.Services;

public class LocalizationService : ILocalizationService
{
    private readonly IWebHostEnvironment _environment;
    private Dictionary<string, Dictionary<string, string>> _resources;
    private string _currentCulture = "fa";

    public LocalizationService(IWebHostEnvironment environment)
    {
        _environment = environment;
        LoadResources();
    }

    private void LoadResources()
    {
        var filePath = Path.Combine(_environment.WebRootPath, "Resources", $"SharedResource.{_currentCulture}.json");
        var jsonString = File.ReadAllText(filePath);
        _resources = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(jsonString);
    }

    public string GetLocalizedString(string key)
    {
        var parts = key.Split('.');
        if (parts.Length != 2) return key;

        var category = parts[0];
        var subKey = parts[1];

        if (_resources.TryGetValue(category, out var categoryDict))
        {
            if (categoryDict.TryGetValue(subKey, out var value))
            {
                return value;
            }
        }

        return key;
    }

    public void SetLanguage(string culture)
    {
        _currentCulture = culture;
        LoadResources();
        var cultureInfo = new CultureInfo(culture);
        CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
        CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
    }

    public string CurrentLanguage => _currentCulture;

    public bool IsRightToLeft => _currentCulture == "fa";
}