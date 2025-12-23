using System.Globalization;

namespace EnterpriseTicketing.Web.Services;

public interface ILocalizationService
{
    string GetLocalizedString(string key);
    void SetLanguage(string culture);
    string CurrentLanguage { get; }
    bool IsRightToLeft { get; }
}