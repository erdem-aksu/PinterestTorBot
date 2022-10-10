using System.Threading.Tasks;

namespace PinterestTorBot.PinterestClient.Api.Classes
{
    public interface ICanBeDeletedApi
    {
        Task DeleteAsync(object entityId);
    }
}