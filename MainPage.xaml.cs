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
                    Console.WriteLine("Error: " + response.StatusCode);
                }
            }
        }
        public static void SampleREST()
        {
            string collectionUri = "http://cqmdevops03:8080/tfs";
            string teamProjectName = "CaliberRE";
            VssConnection connection = new VssConnection(new Uri(collectionUri), new VssOAuthAccessTokenCredential(CommonClass.userModel.AccessToken));

            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();
            List<TeamProjectCollectionReference> collections = (List<TeamProjectCollectionReference>)connection.GetClient<ProjectCollectionHttpClient>().GetProjectCollections().Result;

            List<QueryHierarchyItem> queryHierarchyItems = witClient.GetQueriesAsync(teamProjectName, depth: 2).Result;

            QueryHierarchyItem myQueriesFolder = queryHierarchyItems.FirstOrDefault(qhi => qhi.Name.Equals("My Queries"));
            if (myQueriesFolder != null)
            {
                string queryName = "REST Sample";

                QueryHierarchyItem newBugsQuery = null;
                if (myQueriesFolder.Children != null)
                {
                    newBugsQuery = myQueriesFolder.Children.FirstOrDefault(qhi => qhi.Name.Equals(queryName));
                }
                if (newBugsQuery == null)
                {
                    newBugsQuery = new QueryHierarchyItem()
                    {
                        Name = queryName,
                        Wiql = "SELECT [System.Id],[System.WorkItemType],[System.Title],[System.AssignedTo],[System.State],[System.Tags] FROM WorkItems WHERE [System.TeamProject] = @project AND [System.WorkItemType] = 'Bug' AND [System.State] = 'New'",
                        IsFolder = false
                    };
                    newBugsQuery = witClient.CreateQueryAsync(newBugsQuery, teamProjectName, myQueriesFolder.Name).Result;
                }
                WorkItemQueryResult result = witClient.QueryByIdAsync(newBugsQuery.Id).Result;

                if (result.WorkItems.Any())
                {
                    int skip = 0;
                    const int batchSize = 100;
                    IEnumerable<WorkItemReference> workItemRefs;
                    do
                    {
                        workItemRefs = result.WorkItems.Skip(skip).Take(batchSize);
                        if (workItemRefs.Any())
                        {
                            List<WorkItem> workItems = witClient.GetWorkItemsAsync(workItemRefs.Select(wir => wir.Id)).Result;
                            foreach (WorkItem workItem in workItems)
                            {

                            }
                        }
                        skip += batchSize;
                    }
                    while (workItemRefs.Count() == batchSize);
                }
                else
                {
                    Console.WriteLine("No work items were returned from query.");
                }
            }
        }

        private void HomeBtn_Clicked(object sender, EventArgs e)
        {
            MainContent.Content = new HomePage();
        }

        private void Dashbord_Tapped(object sender, EventArgs e)
        {

        }
    }
}