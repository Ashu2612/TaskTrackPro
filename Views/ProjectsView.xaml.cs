using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using TaskTrackPro.Models;
using System.Windows.Input;

namespace TaskTrackPro.Views;

public partial class ProjectsView : ContentView
{

    public ProjectsView()
    {
        InitializeComponent();
        Grid views = new()
        {
            HorizontalOptions = LayoutOptions.Center
        };
        CommonClass.backButton.IsVisible = false;
        CommonClass.RunIndicator(true);
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
        MainGrid.Children.Clear();
        int colCount = 3, setRow = -1, setColumn = 0;
        for (int i = 0; i < colCount; i++)
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2, GridUnitType.Auto) });
        for (int i = 0; i < CommonClass.projectDetails.ProjectCollections.Count; i++)
        {
            if (i % colCount == 0)
            {
                setColumn = 0;
                setRow++;
                MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2, GridUnitType.Auto) });
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
            ProjectCollection.Clicked += ProjectCollection_Clicked;
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
        CommonClass.activityInd.IsRunning = false;
        CommonClass.activityInd.IsVisible = false;
    }
    private void DisplayProjects()
    {
        CommonClass.backButton.IsVisible = true;
        MainGrid.Children.Clear();
        int colCount = 3, setRow = -1, setColumn = 0;
        for (int i = 0; i < colCount; i++)
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 0 });
        for (int i = 0; i < CommonClass.projectDetails.Projects.Count; i++)
        {
            if (i % colCount == 0)
            {
                setColumn = 0;
                setRow++;
                MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2, GridUnitType.Auto) });
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
            Project.Clicked += Project_Clicked;
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
        CommonClass.activityInd.IsRunning = false;
        CommonClass.activityInd.IsVisible = false;

    }

    private void DisplayWorkItems()
    {
        MainGrid.Children.Clear();
        int colCount = 3, setRow = -1, setColumn = 0;
        for (int i = 0; i < colCount; i++)
            MainGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = 0 });
        for (int i = 0; i < CommonClass.projectDetails.WorkItems.Count; i++)
        {
            if (i % colCount == 0)
            {
                setColumn = 0;
                setRow++;
                MainGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2, GridUnitType.Auto) });
            }
            Button WorkItems = new()
            {
                Text = "Start",
                FontSize = 10,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.End,
                BackgroundColor = Colors.AliceBlue,
                MinimumHeightRequest = 15,
                MinimumWidthRequest = 60,
                Margin = 5,
                TextColor = Colors.Black,
                StyleId = CommonClass.projectDetails.WorkItems[i].Id.ToString()
            };
            WorkItems.Clicked += (sender, e) => WorkItem_Clicked(sender, e);
            Label Title = new()
            {
                Text = CommonClass.projectDetails.WorkItems[i].Title,
                FontSize = 14,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                Margin = 5
            };
            Label State = new()
            {
                Text = "State - " + CommonClass.projectDetails.WorkItems[i].Status,
                TextColor = Color.FromRgba("#6E6E6E"),
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                Margin = new Thickness(15, 5, 0, 0)
            };
            var StateList = new List<string>();
            StateList.Add("To Do");
            StateList.Add("Removed");
            StateList.Add("In Progress");
            StateList.Add("Done");
            Picker picker = new()
            {
                MinimumHeightRequest = 10,
                WidthRequest = 10
            };
            picker.ItemsSource = StateList;
            picker.SelectedIndexChanged += (sender, e) => Picker_SelectedIndexChanged(sender, e);

            Label Hours = new()
            {
                Text = "Remaining Hours - " + CommonClass.projectDetails.WorkItems[i].RemainingHours.ToString(),
                TextColor = Color.FromRgba("#6E6E6E"),
                FontSize = 12,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                Margin = new Thickness(15, 5, 0, 0)
            };
            BoxView boxView1 = new()
            {
                Shadow = new Shadow() { Brush = Color.FromRgba("#C8C8C8") },
                MinimumHeightRequest = 150

            };
            Grid views = new()
            {
                HorizontalOptions = LayoutOptions.Center,
                Margin = 10
            };

            views.AddRowDefinition(new() { Height = new GridLength(2, GridUnitType.Auto) });
            views.AddRowDefinition(new() { Height = new GridLength(2, GridUnitType.Auto) });
            views.AddRowDefinition(new() { Height = new GridLength(2, GridUnitType.Auto) });
            views.AddColumnDefinition(new() { Width = 170 });
            views.AddColumnDefinition(new() { Width = 80 });
            MainGrid.SetRow(views, setRow);
            MainGrid.SetColumn(views, setColumn);
            views.SetColumnSpan(boxView1, 2);
            views.SetRowSpan(boxView1, 3);
            views.Add(boxView1);
            views.Add(WorkItems, 1, 2);
            views.SetColumnSpan(State, 2);
            views.Add(State, 0, 1);
            views.Add(picker, 1, 1);
            views.Add(Hours, 0, 2);
            views.SetColumnSpan(Title, 2);
            views.Add(Title, 0, 0);
            MainGrid.Add(views);
            setColumn++;

        }
        CommonClass.activityInd.IsRunning = false;
        CommonClass.activityInd.IsVisible = false;

    }

    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
    private void UpdateTaskTimer()
    {
        Label State = new()
        {
            Text = "",
            TextColor = Color.FromRgba("#6E6E6E"),
            FontSize = 12,
            HorizontalOptions = LayoutOptions.Start,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(15, 5, 0, 0)
        };
        BoxView boxView1 = new()
        {
            Shadow = new Shadow() { Brush = Color.FromRgba("#C8C8C8") },
            MinimumHeightRequest = 150

        };
        Grid views = new()
        {
            HorizontalOptions = LayoutOptions.Center,
            Margin = 10
        };
        views.Add(boxView1);
        MainGrid.Add(views);
    }

    private void WorkItem_Clicked(object sender, EventArgs e)
    {
        MainGrid.Children.Clear();
        this.Loaded += CurrentTime;
        CommonClass.backButton.IsVisible = false;
        CommonClass.projectDetails.SelectedWorkItemId = new();
        Button button = (Button)sender;
        CommonClass.projectDetails.SelectedWorkItemId.Add(int.Parse(button.StyleId));
    }

    private void CurrentTime(object sender, EventArgs e)
    {
         CommonClass.TaskTimer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
    }

    private void TimerCallback(object state)
    {
        string currentTime = DateTime.UtcNow.ToString("T");
        MainThread.BeginInvokeOnMainThread(() =>
        {
            TaskTimer.Text = currentTime;
        });
    }

    private void Project_Clicked(object sender, EventArgs e)
    {
        CommonClass.activityInd.IsRunning = true;
        CommonClass.backButton.IsVisible = true;
        CommonClass.backButton.Clicked += (sender, e) => ProjectCollection_Clicked(sender, e);
        CommonClass.projectDetails.SelectedProjectId = new();
        Button button = (Button)sender;
        try
        {
            CommonClass.projectDetails.SelectedProjectId.Add(Guid.Parse(button.StyleId));
        }
        catch
        {
            CommonClass.projectDetails.SelectedProjectId.Add(Guid.Parse(CommonClass.projectDetails.SelectedProject.Id.ToString()));
        }
        ReadWorkItems();
        DisplayWorkItems();
    }
    private void ProjectCollectionBack_Clicked(object sender, EventArgs e)
    {
        CommonClass.backButton.IsVisible=false;
        CommonClass.activityInd.IsRunning = true;
        ReadProjectCollections();
        DisplayCollections();
    }
    private void ProjectCollection_Clicked(object sender, EventArgs e)
    {
        CommonClass.backButton.IsVisible = true;
        CommonClass.backButton.Clicked += (sender, e) => ProjectCollectionBack_Clicked(sender, e);
        CommonClass.projectDetails.SelectedCollectionId = new();
        Button button = (Button)sender;
        try
        {
            CommonClass.projectDetails.SelectedCollectionId.Add(Guid.Parse(button.StyleId));
        }
        catch
        {
            CommonClass.projectDetails.SelectedCollectionId.Add(Guid.Parse(CommonClass.projectDetails.SelectedProjectCollection.Id.ToString()));
        }
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
            foreach (TeamProjectReference teamProjectReference in teamProjectReferences)
            {
                CommonClass.projectDetails.Projects.Add(new Projects { ProjectName = teamProjectReference.Name, Id = teamProjectReference.Id });
            }


        }
        catch (Exception ex) { }
    }
    private async void UpdateWokItemState()
    {
        CommonClass.projectDetails.SelectedProject = new();
        CommonClass.projectDetails.SelectedProject = CommonClass.projectDetails.Projects.Find(x => CommonClass.projectDetails.SelectedProjectId.Contains(Guid.Parse(x.Id.ToString())));

        string collectionUri = "http://cqmdevops03:8080/tfs" + "/" + CommonClass.projectDetails.SelectedProjectCollection.Name;

        VssConnection connection = new VssConnection(new Uri(collectionUri), new VssOAuthAccessTokenCredential(CommonClass.userModel.AccessToken));

        WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();

        try
        {
            List<QueryHierarchyItem> queryHierarchyItems = witClient.GetQueriesAsync(CommonClass.projectDetails.SelectedProject.ProjectName, depth: 2).Result;
            JsonPatchDocument patchDocument = new JsonPatchDocument();
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Replace,
                    Path = "/fields/System.State",
                    Value = "InProgress" // Replace with the desired state
                });

            WorkItem updatedWorkItem = await witClient.UpdateWorkItemAsync(patchDocument, int.Parse(CommonClass.projectDetails.SelectedWorkItem.Id.ToString()));

        }
        catch (Exception ex) { }
    }
    private async Task UpdateWokItemRemainingHoursAsync()
    {
        CommonClass.projectDetails.SelectedProject = new();
        CommonClass.projectDetails.SelectedProject = CommonClass.projectDetails.Projects.Find(x => CommonClass.projectDetails.SelectedProjectId.Contains(Guid.Parse(x.Id.ToString())));

        string collectionUri = "http://cqmdevops03:8080/tfs" + "/" + CommonClass.projectDetails.SelectedProjectCollection.Name;

        VssConnection connection = new VssConnection(new Uri(collectionUri), new VssOAuthAccessTokenCredential(CommonClass.userModel.AccessToken));

        WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();

        try
        {
            List<QueryHierarchyItem> queryHierarchyItems = witClient.GetQueriesAsync(CommonClass.projectDetails.SelectedProject.ProjectName, depth: 2).Result;
            JsonPatchDocument patchDocument = new JsonPatchDocument();
            patchDocument.Add(
                new JsonPatchOperation()
                {
                    Operation = Operation.Add,
                    Path = "/fields/Microsoft.VSTS.Scheduling.RemainingWork",
                    Value = 8 // Replace with the desired remaining work hours
                });

            WorkItem updatedWorkItem = await witClient.UpdateWorkItemAsync(patchDocument, int.Parse(CommonClass.projectDetails.SelectedWorkItem.Id.ToString()));

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
                string queryName = "REST Sample6";

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
                        Wiql = $"SELECT [System.Id],[System.WorkItemType],[System.Title],[System.State],[System.Tags],[Microsoft.VSTS.Scheduling.CompletedWork] FROM WorkItems WHERE [System.TeamProject] = @project AND [System.State] <> 'Done' AND [System.AssignedTo] = @Me",
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
                }
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
                                CommonClass.projectDetails.WorkItems.Add(new() { Id = workItem.Id, IterationPath = workItem.Fields["System.IterationPath"].ToString(), Title = workItem.Fields["System.Title"].ToString(), RemainingHours = 1, Status = workItem.Fields["System.State"].ToString() });
                            }
                        }
                        skip += batchSize;
                    }
                    while (workItemRefs.Count() == batchSize);
                }
                else
                {

                }

            }
        }
        catch (Exception ex) { }
    }
}