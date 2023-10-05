using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensibility;
using System.Security.Claims;

namespace TaskTrackPro
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void LoginBtn_Clicked(object sender, EventArgs e)
        {
            string clientId = "087c68d2-fc42-4758-9ab5-53a9cf8d076f";
            string redirectUri = "TaskTrackerPro://Oauth";
            string[] scopes = { "openid", "profile" }; // Add the required scopes
            TimeSpan tokenExpiration = TimeSpan.FromHours(1);

            var app = PublicClientApplicationBuilder
                .Create(clientId)
                .WithRedirectUri(redirectUri)
                .Build();

            try
            {
                var authResult = await app.AcquireTokenInteractive(scopes)
                    .ExecuteAsync();
            
                if (authResult.AccessToken != null)
                {
                    var dashbord = new Dashbord();
                    _ = Navigation.PushAsync(dashbord);
                }
                else
                {
                    await DisplayAlert("Unable to authenticate the user.", "Check your email id and try again !", "Okay");
                }
            }
            catch (MsalException ex)
            {
                await DisplayAlert("Authentication Exception !", ex.Message, "Okay");
            }
        }

        private void LoginBtn_Clicked_1(object sender, EventArgs e)
        {

        }
    }
}