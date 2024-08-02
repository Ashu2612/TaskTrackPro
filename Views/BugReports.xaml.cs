using Microcharts;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;
using SkiaSharp;
using TaskTrackPro.Models;
using Microcharts.Maui;

namespace TaskTrackPro.Views;

public partial class BugReports : ContentView
{
    public BugReports()
    {
        InitializeComponent();
        ProjectCollection();
        CollectionPckr.ItemsSource = CommonClass.projectDetails.ProjectCollections.Select(x => x.Name).ToList();
    }
    private void ProjectCollection()
    {
        string collectionUri = "http://cqmdevops03:8080/tfs";

        VssConnection connection = new VssConnection(new Uri(collectionUri), new VssOAuthAccessTokenCredential(CommonClass.userModel.AccessToken));

        WorkItemTrackingHttpClient witClient = connection.GetClient<WorkItemTrackingHttpClient>();


        List<TeamProjectCollectionReference> collections = (List<TeamProjectCollectionReference>)connection.GetClient<ProjectCollectionHttpClient>().GetProjectCollections().Result;
        CommonClass.projectDetails.ProjectCollections = new();
        foreach (TeamProjectCollectionReference teamProjectCollection in collections)
        {
            CommonClass.projectDetails.ProjectCollections.Add(new ProjectCollections { Name = teamProjectCollection.Name, Id = teamProjectCollection.Id });
        }
    }
    public class WorkItemMetrics
    {
        public int TotalWorkItems { get; set; }
        public int TotalBugs { get; set; }
        public double TotalWorkingHours { get; set; }
        public Dictionary<string, int> StatusCounts { get; set; } = new Dictionary<string, int>();
    }

    private async Task CalculateMetricsAsync()
    {
        try
        {
            string collectionUri = "http://cqmdevops03:8080/tfs" + "/" + CommonClass.projectDetails.SelectedProjectCollection.Name;
            VssConnection connection = new VssConnection(new Uri(collectionUri), new VssOAuthAccessTokenCredential(CommonClass.userModel.AccessToken));

            var workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            var query = $"SELECT [System.Id], [System.Title] FROM WorkItems WHERE [System.WorkItemType] = 'Bug' AND [System.TeamProject] = '{CommonClass.projectDetails.SelectedProject.ProjectName}'";
            var workItemQueryResult = await workItemTrackingClient.QueryByWiqlAsync(new Wiql() { Query = query });

            var bugIds = workItemQueryResult.WorkItems.Select(wi => wi.Id).ToArray();

            List<WorkItem> bugs = new List<WorkItem>();
            const int batchSize = 200;
            for (int i = 0; i < bugIds.Length; i += batchSize)
            {
                var batch = bugIds.Skip(i).Take(batchSize).ToArray();
                var batchBugs = await workItemTrackingClient.GetWorkItemsAsync(batch, expand: WorkItemExpand.Relations);
                bugs.AddRange(batchBugs);
            }
            var filteredBugs = new List<(WorkItem Bug, string DeveloperName, int ReassignmentCount)>();

            foreach (var bug in bugs)
            {
                if (!bug.Id.HasValue) continue;

                var revisions = await workItemTrackingClient.GetRevisionsAsync(bug.Id.Value);

                // Track the history of assignments and state changes
                var assignmentHistory = new List<string>();
                string developerName = null;
                string testerName = null;
                bool initiallyResolvedByDev = false;
                bool reassignedToDev = false;
                int reassignmentCount = 0;

                foreach (var revision in revisions)
                {
                    if (revision.Fields.ContainsKey("System.AssignedTo"))
                    {
                        var assignedToObject = revision.Fields["System.AssignedTo"];
                        var assignedToDisplayName = assignedToObject.GetType().GetProperty("DisplayName")?.GetValue(assignedToObject, null)?.ToString();

                        assignmentHistory.Add(assignedToDisplayName);

                        if (assignmentHistory.Count(a => a == assignedToDisplayName) > 2)
                        {
                            if (revision.Fields.ContainsKey("System.State") && revision.Fields["System.State"].ToString() == "Resolved")
                            {
                                initiallyResolvedByDev = true;
                                developerName = assignedToDisplayName;
                            }
                            else if (initiallyResolvedByDev && assignedToDisplayName == developerName)
                            {
                                reassignedToDev = true;
                                reassignmentCount++;
                            }
                        }
                    }
                }

                if (initiallyResolvedByDev && reassignedToDev)
                {
                    filteredBugs.Add((Bug: bug, DeveloperName: developerName, ReassignmentCount : reassignmentCount));
                }
            }

            var aggregatedData = filteredBugs
                .Where(b => b.ReassignmentCount >= 3).OrderBy(x => x.ReassignmentCount)
                .ToList();


            var random = new Random();
            List<SKColor> GenerateRandomColors(int count)
            {
                var colors = new List<SKColor>();
                for (int i = 0; i < count; i++)
                {
                    var color = SKColor.FromHsl(
                        (float)random.NextDouble() * 360,  // Hue
                        100,                              // Saturation
                        70);                              // Lightness
                    colors.Add(color);
                }
                return colors;
            }
            var colors = GenerateRandomColors(aggregatedData.Count);

            var data = aggregatedData.Select((item, index) =>
            {
                var developer = item.DeveloperName;
                var bugCount = item.ReassignmentCount;
                var bugId = item.Bug.Id.ToString(); // Assuming WorkItem has an Id property

                var label = developer + $" ({bugCount} reassignments)";

                return new ChartEntry(bugCount)
                {
                    Label = bugId,
                    ValueLabel = label,
                    Color = colors[index],
                };
            }).ToList();

            // Assuming you have a ChartView named TaskChartView
            var chart = new LineChart
            {
                Entries = data
            };


            TaskChartView.Chart = chart;
        }
        catch (VssServiceResponseException ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    private void Projects()
    {
        try
        {
            string collectionUri = "http://cqmdevops03:8080/tfs" + "/" + CommonClass.projectDetails.SelectedProjectCollection.Name;

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
    private void CollectionPckr_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            CommonClass.projectDetails.SelectedProjectCollection = new();
            if (CollectionPckr.SelectedItem != null)
            {
                CommonClass.projectDetails.SelectedProjectCollection = CommonClass.projectDetails.ProjectCollections.FirstOrDefault(x => x.Name == CollectionPckr.SelectedItem.ToString());
            }
            Projects();
            ProjectPckr.SelectedItem = null;
            TaskChartView.Chart = null;
            ProjectPckr.ItemsSource = CommonClass.projectDetails.Projects.Select(x => x.ProjectName).ToList();
            ProjectPckr.IsEnabled = true;
        }
        catch (Exception ex) { }
    }

    private async void ProjectPckr_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            CommonClass.projectDetails.SelectedProject = new();
            if (ProjectPckr.SelectedItem != null)
            {
                CommonClass.projectDetails.SelectedProject = CommonClass.projectDetails.Projects.FirstOrDefault(x => x.ProjectName == ProjectPckr.SelectedItem.ToString());
            }
            TaskChartView.Chart = null;
            await CalculateMetricsAsync();

        }
        catch (Exception ex) { }
    }
}