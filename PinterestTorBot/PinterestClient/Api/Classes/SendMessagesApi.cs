using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PinterestTorBot.PinterestClient.Api.Classes
{
    internal class SendMessagesApi : ISendMessagesApi
    {
        private PinterestApi Api { get; }

        private string MessageEntityName { get; }

        private static string SendUrl => PinterestApiConstants.ResourceSendMessage;

        public SendMessagesApi(PinterestApi api, string messageEntityName)
        {
            Api = api;
            MessageEntityName = messageEntityName;
        }

        public async Task SendMessageWithEmailAsync(List<string> emails, string text, object entityId = null)
        {
            if (!emails.Any())
            {
                throw new ArgumentException("Emails cannot be empty.", nameof(emails));
            }

            var data = new Dictionary<string, object>
            {
                {"text", text},
                {"emails", emails},
                {"user_ids", new List<string>()},
            };

            if (MessageEntityName != null && entityId != null)
            {
                data.Add(MessageEntityName, entityId);
            }

            await Api.PostAsync(SendUrl, data, true);
        }

        public async Task SendMessageWithUserIdAsync(List<string> userIds, string text, object entityId = null)
        {
            if (!userIds.Any())
            {
                throw new ArgumentException("UserIds cannot be empty.", nameof(userIds));
            }

            var data = new Dictionary<string, object>
            {
                {"text", text},
                {"emails", new List<string>()},
                {"user_ids", userIds},
            };

            if (MessageEntityName != null && entityId != null)
            {
                data.Add(MessageEntityName, entityId);
            }

            await Api.PostAsync(SendUrl, data, true);
        }
    }
}