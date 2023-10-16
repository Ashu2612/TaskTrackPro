using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensibility;
using Microsoft.Identity.Client.NativeInterop;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.UserAccountMapping;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace TaskTrackPro
{
    public partial class MainPage : ContentPage
    {
        public MainPage(AuthenticationResult authenticationResult)
        {
            InitializeComponent();
            something(authenticationResult);
            MainContent.Content = new Views.DashbordView();
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
                    await CommonClass.SetUserDataAsync(content, authenticationResult.AccessToken);
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

                }
            }
        }
        private Button previouslyPressedButton = null;

        private void DashbordBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                AddTaskCation.TextColor = Color.FromRgba("#666666");
                ProjectCaption.TextColor = Color.FromRgba("#666666");
                TasksCaption.TextColor = Color.FromRgba("#666666");
                previouslyPressedButton.TextColor = Color.FromRgba("#a3a3a3");
                previouslyPressedButton.BackgroundColor = Color.FromRgba("#0000");
            }
            catch { }


            DasbordBtn.TextColor = Color.FromRgba("#88c2b0");
            DasbordBtn.BackgroundColor = Color.FromRgba("#daede7");
            DashbordCaption.TextColor = Color.FromRgba("#6aad98");
            try
            {
                MainContent.Content = new Views.DashbordView();
                previouslyPressedButton = DasbordBtn;
            }
            catch { }

        }
        private void ProjectBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                DasbordBtn.TextColor = Color.FromRgba("#a3a3a3");
                DasbordBtn.BackgroundColor = Color.FromRgba("#0000");
                AddTaskCation.TextColor = Color.FromRgba("#666666");
                DashbordCaption.TextColor = Color.FromRgba("#666666");
                TasksCaption.TextColor = Color.FromRgba("#666666");

                previouslyPressedButton.TextColor = Color.FromRgba("#a3a3a3");
                previouslyPressedButton.BackgroundColor = Color.FromRgba("#0000");
            }
            catch { }

            ProjectBtn.TextColor = Color.FromRgba("#88c2b0");
            ProjectBtn.BackgroundColor = Color.FromRgba("#daede7");
            ProjectCaption.TextColor = Color.FromRgba("#6aad98");
            try
            {
                MainContent.Content = new Views.ProjectsView();
                previouslyPressedButton = ProjectBtn;
            }
            catch { }
        }

        private void TasksBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                DasbordBtn.TextColor = Color.FromRgba("#a3a3a3");
                DasbordBtn.BackgroundColor = Color.FromRgba("#0000");
                DashbordCaption.TextColor = Color.FromRgba("#666666");
                AddTaskCation.TextColor = Color.FromRgba("#666666");
                ProjectCaption.TextColor = Color.FromRgba("#666666");
                previouslyPressedButton.TextColor = Color.FromRgba("#a3a3a3");
                previouslyPressedButton.BackgroundColor = Color.FromRgba("#0000");
            }
            catch { }

            TasksBtn.TextColor = Color.FromRgba("#88c2b0");
            TasksBtn.BackgroundColor = Color.FromRgba("#daede7");
            TasksCaption.TextColor = Color.FromRgba("#6aad98");
            try
            {
                MainContent.Content = new Views.TasksView();
                previouslyPressedButton = TasksBtn;
            }
            catch { }
        }

        private void AddTaskBtn_Clicked(object sender, EventArgs e)
        {
            try
            {
                DasbordBtn.TextColor = Color.FromRgba("#a3a3a3");
                DasbordBtn.BackgroundColor = Color.FromRgba("#0000");
                DashbordCaption.TextColor = Color.FromRgba("#666666");
                ProjectCaption.TextColor = Color.FromRgba("#666666");
                TasksCaption.TextColor = Color.FromRgba("#666666");
                previouslyPressedButton.TextColor = Color.FromRgba("#a3a3a3");
                previouslyPressedButton.BackgroundColor = Color.FromRgba("#0000");
            }
            catch { }

            AddTaskBtn.TextColor = Color.FromRgba("#88c2b0");
            AddTaskBtn.BackgroundColor = Color.FromRgba("#daede7");
            AddTaskCation.TextColor = Color.FromRgba("#6aad98");
            try
            {
                MainContent.Content = new Views.AssignTasksView();
                previouslyPressedButton = AddTaskBtn;
            }
            catch { }
        }
    }
}