using Microsoft.Identity.Client;

namespace TaskTrackPro;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
        CommonClass.projectDetails = new();
    }
    private async void LoginBtn_Clicked(object sender, EventArgs e)
    {
        string clientId = "087c68d2-fc42-4758-9ab5-53a9cf8d076f";
        string redirectUri = "TaskTrackerPro://Oauth";
        string[] scopes = { "openid", "profile", "user.read" }; // Add the required scopes
        TimeSpan tokenExpiration = TimeSpan.FromHours(1);

        var app = PublicClientApplicationBuilder
            .Create(clientId)
            .WithRedirectUri(redirectUri)
            .Build();

        try
        {
            var accounts = await app.GetAccountsAsync();
            try
            {
                CommonClass.authenticationResult = await app.AcquireTokenSilent(scopes, accounts.FirstOrDefault())
                            .ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                CommonClass.authenticationResult = await app.AcquireTokenInteractive(scopes)
               .ExecuteAsync();
            }

            if (CommonClass.authenticationResult.AccessToken != null)
            {
                var mainPage = new MainPage(CommonClass.authenticationResult);
                _ = Navigation.PushAsync(mainPage);
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

}