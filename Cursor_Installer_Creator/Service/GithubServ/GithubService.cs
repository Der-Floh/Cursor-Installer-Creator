using System.Text.Json;

using Microsoft.Extensions.Logging;

namespace Cursor_Installer_Creator.Service.GithubServ;

public sealed class GithubService : IGithubService
{
    private static readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(10),
    };

    private readonly ILogger<GithubService> _logger;

    public GithubService(ILogger<GithubService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> HasUpdateAsync(string repoOwner, string repoName, Version currentVersion)
    {
        var url = $"https://api.github.com/repos/{repoOwner}/{repoName}/releases/latest";
        _logger.LogDebug("Checking for updates: {Owner}/{Repo}, current={CurrentVersion}", repoOwner, repoName, currentVersion);

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.UserAgent.ParseAdd(repoName.Replace('-', '_'));

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var jsonString = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(jsonString);
        var latestVersionString = jsonDocument.RootElement.GetProperty("tag_name").GetString()
            ?? throw new InvalidOperationException("GitHub API did not return a valid tag_name.");
        var latestVersion = new Version(latestVersionString.TrimStart('v'));

        var hasUpdate = latestVersion > currentVersion;
        _logger.LogInformation("Update check: current={Current}, latest={Latest}, hasUpdate={HasUpdate}", currentVersion, latestVersion, hasUpdate);
        return hasUpdate;
    }
}
