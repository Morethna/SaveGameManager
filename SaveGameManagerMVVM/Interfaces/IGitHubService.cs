using System.Threading.Tasks;

namespace SaveGameManager.Interfaces;

public interface IGitHubService
{
    public Task CheckGitHubNewerVersion();
}
