using Octokit;
using SaveGameManager.Interfaces;
using SaveGameManager.Viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SaveGameManager.Services;

public class GitHubService : IGitHubService
{
    private readonly IWindowService _windowService;
    private readonly GitHubViewModel _gitHubView;

    public GitHubService(IWindowService windowService, GitHubViewModel gitHubView)
    {
        _windowService = windowService;
        _gitHubView = gitHubView;
    }
    public async Task CheckGitHubNewerVersion()
    {
        GitHubClient client = new(new ProductHeaderValue("SaveGameManager"));
        IReadOnlyList<Release> releases = await client.Repository.Release.GetAll("Morethna", "SaveGameManager");

        var release = releases.ToList()
            .OrderByDescending(item => item.CreatedAt)
            .FirstOrDefault();

        if (release == null)
            return;

        string tag = release.TagName;
        tag = Regex.Replace(tag, "[^0-9.]", "");
        var latestGitHubVersion = new Version(tag);
        var localVersion = Assembly.GetExecutingAssembly().GetName().Version;

        if (localVersion is null)
            return;

        int versionComparison = localVersion.CompareTo(latestGitHubVersion);
        if (versionComparison < 0)
        {
            //The version on GitHub is more up to date than this local release.
            _gitHubView.CurrentVersion = localVersion.ToString();
            _gitHubView.NewVersion = latestGitHubVersion.ToString();
            _gitHubView.Url = release.HtmlUrl;
            _windowService.OpenWindow(IWindowService.Windows.GitHub, _gitHubView);
        }
    }
}
