using System.Threading.Tasks;
using PinterestTorBot.PinterestClient.Models;
using PinterestTorBot.PinterestClient.Models.Inputs;

namespace PinterestTorBot.PinterestClient.Api
{
    public interface IAuthApi
    {
        Task RegisterAsync(RegisterInput input);
        
        Task RegisterBusinessAsync(BusinessRegisterInput input, BusinessRegisterSecondInput secondInput);
        
        Task ConvertToBusinessAsync(string businessName, string websiteUrl, bool login = true);

        Task<AutoRegisteredAccount> AutoRegisterAsync(string country = "US", string locale = "en-US");

        Task<AutoRegisteredAccount> AutoRegisterPersonalAsync(string country = "US", string locale = "en-US");
        
        Task ConfirmEmailAsync(string link);
        
        Task Login();

        void Logout();
    }
}