using Microsoft.Identity.Client;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.UserAccountMapping;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net.Http.Headers;

namespace TaskTrackPro.ViewModels;

public class ProjectsView : ContentView
{
    public ProjectsView()
    {
        Content = new VerticalStackLayout
        {
            Children = {
                new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, Text = "Welcome to .NET MAUI!"
                }
            }
        };
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
}