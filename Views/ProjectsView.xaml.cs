using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net.Http.Headers;
using TaskTrackPro.Models;

namespace TaskTrackPro.Views;

public partial class ProjectsView : ContentView
{
    public ProjectsView()
    {
        InitializeComponent();
        ReadProjectCollections();
        DisplayCollections();

    }
    public void ReadProjectCollections()
    {
        string collectionUri = "http://cqmdevops03:8080/tfs";

        VssConnection connection = new VssConnection(new Uri(collectionUri), new VssOAuthAccessTokenCredential(CommonClass.userModel.AccessToken));

        WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();

        try
        {
            List<TeamProjectCollectionReference> collections = (List<TeamProjectCollectionReference>)connection.GetClient<ProjectCollectionHttpClient>().GetProjectCollections().Result;
            CommonClass.projectDetails.ProjectCollections = new();
            foreach (TeamProjectCollectionReference teamProjectCollection in collections)
            {
                CommonClass.projectDetails.ProjectCollections.Add(new ProjectCollections { Name = teamProjectCollection.Name, Id = teamProjectCollection.Id });
            }

            //    List<QueryHierarchyItem> queryHierarchyItems = witClient.GetQueriesAsync(teamProjectName, depth: 2).Result;



            //QueryHierarchyItem myQueriesFolder = queryHierarchyItems.FirstOrDefault(qhi => qhi.Name.Equals("My Queries"));
            //if (myQueriesFolder != null)
            //{
            //    string queryName = "REST Sample";

            //    QueryHierarchyItem newBugsQuery = null;
            //    if (myQueriesFolder.Children != null)
            //    {
            //        newBugsQuery = myQueriesFolder.Children.FirstOrDefault(qhi => qhi.Name.Equals(queryName));
            //    }
            //    if (newBugsQuery == null)
            //    {
            //        newBugsQuery = new QueryHierarchyItem()
            //        {
            //            Name = queryName,
            //            Wiql = "SELECT [System.Id],[System.WorkItemType],[System.Title],[System.AssignedTo],[System.State],[System.Tags] FROM WorkItems WHERE [System.TeamProject] = @project AND [System.WorkItemType] = 'Bug' AND [System.State] = 'New'",
            //            IsFolder = false
            //        };
            //        newBugsQuery = witClient.CreateQueryAsync(newBugsQuery, teamProjectName, myQueriesFolder.Name).Result;
            //    }
            //    WorkItemQueryResult result = witClient.QueryByIdAsync(newBugsQuery.Id).Result;

            //    if (result.WorkItems.Any())
            //    {
            //        int skip = 0;
            //        const int batchSize = 100;
            //        IEnumerable<WorkItemReference> workItemRefs;
            //        do
            //        {
            //            workItemRefs = result.WorkItems.Skip(skip).Take(batchSize);
            //            if (workItemRefs.Any())
            //            {
            //                List<WorkItem> workItems = witClient.GetWorkItemsAsync(workItemRefs.Select(wir => wir.Id)).Result;
            //                foreach (WorkItem workItem in workItems)
            //                {

            //                }
            //            }
            //            skip += batchSize;
            //        }
            //        while (workItemRefs.Count() == batchSize);
            //    }
            //    else
            //    {
            //        Console.WriteLine("No work items were returned from query.");
            //    }

            //}
        }
        catch (Exception ex) { }
    }
    private void DisplayCollections()
    {
        int colCount = 3, setRow = -1, setColumn = 0;
        for (int i = 0; i < colCount; i++)
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition());
        for (int i = 0; i < CommonClass.projectDetails.ProjectCollections.Count; i++)
        {
            if (i % colCount == 0)
            {
                setColumn = 0;
                setRow++;
                MainGrid.RowDefinitions.Add(new RowDefinition() { Height = 120 });
            }
            Button ProjectCollection = new()
            {
                Text = CommonClass.projectDetails.ProjectCollections[i].Name,
                FontSize = 14,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                BackgroundColor = Colors.Transparent,
                MinimumHeightRequest = 100,
                MinimumWidthRequest = 260,
                TextColor = Colors.Black,
                BorderColor = Colors.Transparent,
                StyleId = CommonClass.projectDetails.ProjectCollections[i].Id.ToString(),
            };
            ProjectCollection.Clicked += (sender, e) => ProjectCollection_Clicked(sender, e);
            BoxView boxView1 = new()
            {
                Shadow = new Shadow() { Brush = Color.FromRgba("#C8C8C8") },
                MinimumHeightRequest = 100,
            };
            Grid views = new()
            {
                HorizontalOptions = LayoutOptions.Center,
                Margin = 10
            };

            MainGrid.SetRow(views, setRow);
            MainGrid.SetColumn(views, setColumn);
            views.Add(boxView1);
            views.Add(ProjectCollection);
            MainGrid.Add(views);
            setColumn++;
        }

    }
    private void DisplayProjects()
    {
        MainGrid.Children.Clear();
        int colCount = 3, setRow = -1, setColumn = 0;
        for (int i = 0; i < colCount; i++)
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 0});
        for (int i = 0; i < CommonClass.projectDetails.Projects.Count; i++)
        {
            if (i % colCount == 0)
            {
                setColumn = 0;
                setRow++;
                MainGrid.RowDefinitions.Add(new RowDefinition() { Height = 120 });
            }
            Button Project = new()
            {
                Text = CommonClass.projectDetails.Projects[i].ProjectName,
                FontSize = 14,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                BackgroundColor = Colors.Transparent,
                MinimumHeightRequest = 100,
                MinimumWidthRequest = 260,
                TextColor = Colors.Black,
                BorderColor = Colors.Transparent,
                StyleId = CommonClass.projectDetails.Projects[i].Id.ToString(),
            };
            Project.Clicked += (sender, e) => Project_Clicked(sender, e);
            BoxView boxView1 = new()
            {
                Shadow = new Shadow() { Brush = Color.FromRgba("#C8C8C8") },
                MinimumHeightRequest = 100,

            };
            Grid views = new()
            {
                HorizontalOptions = LayoutOptions.Center,
                Margin = 10
            };

            MainGrid.SetRow(views, setRow);
            MainGrid.SetColumn(views, setColumn);
            views.Add(boxView1);
            views.Add(Project);
            MainGrid.Add(views);
            setColumn++;
        }

    }
    private void Project_Clicked(object sender, EventArgs e)
    {
        CommonClass.projectDetails.SelectedProjectId = new();
        Button button = (Button)sender;
        CommonClass.projectDetails.SelectedProjectId.Add(Guid.Parse(button.StyleId));
        ReadWorkItems();
    }
    private void ProjectCollection_Clicked(object sender, EventArgs e)
    {
        CommonClass.projectDetails.SelectedCollectionId = new();
        Button button = (Button)sender;
        CommonClass.projectDetails.SelectedCollectionId.Add(Guid.Parse(button.StyleId));
        ReadProjects();
        DisplayProjects();
    }
    private void ReadProjects()
    {
        try
        {
            CommonClass.projectDetails.SelectedProjectCollection = new();

            CommonClass.projectDetails.SelectedProjectCollection = CommonClass.projectDetails.ProjectCollections.Find(x => CommonClass.projectDetails.SelectedCollectionId.Contains(Guid.Parse(x.Id.ToString())));
            string collectionUri = "http://cqmdevops03:8080/tfs" + "/" + CommonClass.projectDetails.SelectedProjectCollection.Name;
            string teamProjectName = "IT022100";

            VssConnection connection = new VssConnection(new Uri(collectionUri), new VssOAuthAccessTokenCredential(CommonClass.userModel.AccessToken));


            WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();

            List<TeamProjectReference> teamProjectReferences = (List<TeamProjectReference>)connection.GetClient<ProjectHttpClient>().GetProjects().Result;
            CommonClass.projectDetails.Projects = new();
            foreach(TeamProjectReference teamProjectReference in teamProjectReferences)
            {
                CommonClass.projectDetails.Projects.Add(new Projects { ProjectName = teamProjectReference.Name, Id = teamProjectReference.Id });
            }


        }
        catch (Exception ex) { }
    }
    private void ReadWorkItems()
    {
        CommonClass.projectDetails.SelectedProject = new();
        CommonClass.projectDetails.SelectedProject = CommonClass.projectDetails.Projects.Find(x => CommonClass.projectDetails.SelectedProjectId.Contains(Guid.Parse(x.Id.ToString())));

        string collectionUri = "http://cqmdevops03:8080/tfs" + "/" + CommonClass.projectDetails.SelectedProjectCollection.Name;

        VssConnection connection = new VssConnection(new Uri(collectionUri), new VssOAuthAccessTokenCredential(CommonClass.userModel.AccessToken));

        WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();

        try
        {
            List<QueryHierarchyItem> queryHierarchyItems = witClient.GetQueriesAsync(CommonClass.projectDetails.SelectedProject.ProjectName, depth: 2).Result;



            QueryHierarchyItem myQueriesFolder = queryHierarchyItems.FirstOrDefault(qhi => qhi.Name.Equals("My Queries"));
            if (myQueriesFolder != null)
            {
                CommonClass.projectDetails.WorkItems = new();
                string queryName = "REST Sample4";

                QueryHierarchyItem newBugsQuery = null;
                if (myQueriesFolder.Children != null)
                {
                    newBugsQuery = myQueriesFolder.Children.FirstOrDefault(qhi => qhi.Name.Equals(queryName));
                }
                //if (newBugsQuery == null)
                //{
                    newBugsQuery = new QueryHierarchyItem()
                    {
                        Name = queryName,
                        Wiql = $"SELECT [System.Id],[System.WorkItemType],[System.Title],[System.State],[System.Tags],[Microsoft.VSTS.Scheduling.RemainingWork] FROM WorkItems WHERE [System.TeamProject] = @project AND [System.AssignedTo] = '{CommonClass.userModel.UserName}'",
                        IsFolder = false
                    };
                try
                {
                    newBugsQuery = witClient.CreateQueryAsync(newBugsQuery, CommonClass.projectDetails.SelectedProject.ProjectName, myQueriesFolder.Name).Result;

                }
                catch
                {
                    _ = witClient.DeleteQueryAsync(CommonClass.projectDetails.SelectedProject.ProjectName, queryName);
                    newBugsQuery = witClient.CreateQueryAsync(newBugsQuery, CommonClass.projectDetails.SelectedProject.ProjectName, myQueriesFolder.Name).Result;
                }
                //}
                WorkItemQueryResult result = witClient.QueryByIdAsync(newBugsQuery.Id).Result;

                if (result.WorkItems.Any())
                {
                    int skip = 0;
                    const int batchSize = 100;
                    IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemReference> workItemRefs;
                    do
                    {
                        workItemRefs = result.WorkItems.Skip(skip).Take(batchSize);
                        if (workItemRefs.Any())
                        {
                            List<WorkItem> workItems = witClient.GetWorkItemsAsync(workItemRefs.Select(wir => wir.Id)).Result;
                            foreach (WorkItem workItem in workItems)
                            {
                                CommonClass.projectDetails.WorkItems.Add(new() { Title = workItem.Fields["System.Title"].ToString() /*RemainingHours = TimeOnly.Parse(workItem.Fields["Microsoft.VSTS.Scheduling.RemainingWork"].ToString())*/, Status = workItem.Fields["System.State"].ToString() });
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
        catch (Exception ex) { }
    }
}