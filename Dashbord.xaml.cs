using Microsoft.Identity.Client;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.VisualBasic.ApplicationServices;
using System.Net.Http.Headers;
using TaskTrackPro.WinUI;

namespace TaskTrackPro;

public partial class Dashbord : ContentPage
{
    public Dashbord(AuthenticationResult authenticationResult)
    {

        InitializeComponent();
        something(authenticationResult);
    }
    public async void something(AuthenticationResult authenticationResult)
    {
        string apiEndpoint = "https://graph.microsoft.com/v1.0/me";

        // Create an HTTP client
        using (HttpClient httpClient = new HttpClient())
        {
            // Set the authorization header with the access token
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authenticationResult.AccessToken);

            // Make a GET request to the API
            HttpResponseMessage response = await httpClient.GetAsync(apiEndpoint);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                await CommonClass.SetUserDataAsync(content);
                UserNameLbl.Text = CommonClass.userModel.DisplayName;
                UserRole.Text = CommonClass.userModel.JobTitle;
            }
            else
            {
            }
        }

        string photoEndpoint = "https://graph.microsoft.com/v1.0/me/photo/$value";

        // Create an HTTP client
        using (HttpClient httpClient = new HttpClient())
        {
            // Set the authorization header with the access token
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", CommonClass.authenticationResult.AccessToken);

            // Make a GET request to the photo endpoint
            HttpResponseMessage response = await httpClient.GetAsync(photoEndpoint);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                // Retrieve the profile picture as a byte array
                byte[] photoBytes = await response.Content.ReadAsByteArrayAsync();

                BitmapImage bitmapImage = new BitmapImage();
                MemoryStream memory = new MemoryStream(photoBytes);
                UserNameImg.Source = ImageSource.FromStream(() => (Stream)memory);
            }
            else
            {
                Console.WriteLine("Error: " + response.StatusCode);
            }
        }
    }


    private void HomeBtn_Clicked(object sender, EventArgs e)
    {
        MainContent.Content = new HomePage();
    }
}