using Cursor_Installer_Creator.Data;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cursor_Installer_Creator.Utils;

public sealed class GitHubUpdater
{
    public const string RepoOwner = "Der-Floh";
    public const string RepoName = "Cursor-Installer-Creator";

    public static Version CurrentVersion { get; } = new Version(2, 1, 0);
    public static string RepoUrl => $"https://github.com/{RepoOwner}/{RepoName}";
    public static string LatestReleaseUrl => $"https://github.com/{RepoOwner}/{RepoName}/releases/latest";

    private static readonly HttpClient _client = new HttpClient();

    public static async Task<bool> HasUpdateAsync()
    {
        try
        {
            // GitHub API endpoint for the latest release
            var url = $"https://api.github.com/repos/{RepoOwner}/{RepoName}/releases/latest";
            _client.DefaultRequestHeaders.UserAgent.ParseAdd(RepoName.Replace('-', '_'));

            // Fetch latest release
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // Deserialize response
            var jsonString = await response.Content.ReadAsStringAsync();
            var jsonDocument = JsonDocument.Parse(jsonString);
            var latestVersionString = jsonDocument.RootElement.GetProperty("tag_name").GetString();
            var latestVersion = new Version(latestVersionString);

            // Compare versions
            return latestVersion > CurrentVersion;
        }
        catch
        {
            return false;
        }
    }
}
