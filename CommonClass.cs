using Microsoft.Identity.Client;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TaskTrackPro.Models;

namespace TaskTrackPro
{
    public class CommonClass
    {
        public static AuthenticationResult authenticationResult;
        public static UserModel userModel { get; set; }

        public static Task SetUserDataAsync(string UseraDtaJson, string accesstoken)
        {
            JObject UserData = JObject.Parse(JsonConvert.DeserializeObject(UseraDtaJson).ToString());
            userModel = new()
            {
                UserEmail = UserData["mail"].ToString(),
                UserName = UserData["displayName"].ToString(),
                UserId = UserData["id"].ToString(),
                JobTitle = UserData["jobTitle"].ToString(),
                DisplayName = UserData["displayName"].ToString(),
                AccessToken = accesstoken
            };
            return Task.CompletedTask;
        }
    }
}
