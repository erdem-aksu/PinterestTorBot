using System.Threading.Tasks;

namespace PinterestTorBot.PinterestClient.Api.Classes
{
    public interface IResolvesCurrentUser
    {
        Task<string> GetUsernameAsync();

        Task<string> GetIdAsync();
    }
}