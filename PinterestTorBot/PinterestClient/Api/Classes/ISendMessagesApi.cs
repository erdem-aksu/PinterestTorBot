using System.Collections.Generic;
using System.Threading.Tasks;

namespace PinterestTorBot.PinterestClient.Api.Classes
{
    public interface ISendMessagesApi
    {
        Task SendMessageWithEmailAsync(List<string> emails, string text, object entityId = null);

        Task SendMessageWithUserIdAsync(List<string> userIds, string text, object entityId = null);
    }
}