namespace Cursor_Installer_Creator.Service.GithubServ;

public interface IGithubService
{
    Task<bool> HasUpdateAsync(string repoOwner, string repoName, Version currentVersion);
}
