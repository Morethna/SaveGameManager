using System.Threading.Tasks;

namespace SaveGameManagerMVVM.Interfaces;

public interface IGitHubService
{
    public Task CheckGitHubNewerVersion();
}
