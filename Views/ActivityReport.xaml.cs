using Microcharts;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.WebApi;
using SkiaSharp;
using TaskTrackPro.Models;

namespace TaskTrackPro.Views;

public partial class ActivityReport : ContentView
{
    public ActivityReport()
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
    private WorkItemMetrics GetWorkItemMetrics()
    {
        string collectionUri = "http://cqmdevops03:8080/tfs" + "/" + CommonClass.projectDetails.SelectedProjectCollection.Name;
        VssConnection connection = new VssConnection(new Uri(collectionUri), new VssOAuthAccessTokenCredential(CommonClass.userModel.AccessToken));

        return CalculateMetrics(connection);
    }

    private WorkItemMetrics CalculateMetrics(VssConnection connection)
    {
        try
        {
            var workItemTrackingClient = connection.GetClient<WorkItemTrackingHttpClient>();

            var wiql = new Wiql()
            {
                Query = $"SELECT [System.Id], [System.Title], [System.State], [System.WorkItemType], [Microsoft.VSTS.Scheduling.CompletedWork] FROM WorkItems WHERE [System.TeamProject] = '{CommonClass.projectDetails.SelectedProject.ProjectName}' AND [System.AssignedTo] = @Me"
            };

            var queryResult = workItemTrackingClient.QueryByWiqlAsync(wiql).Result;
            var workItemRefs = queryResult.WorkItems.ToList();
            var workItemIds = workItemRefs.Select(wir => wir.Id).ToList();

            int batchSize = 200; // Fetch work items in batches
            var workItems = new List<WorkItem>();

            for (int i = 0; i < workItemIds.Count; i += batchSize)
            {
                var batchIds = workItemIds.Skip(i).Take(batchSize).ToList();
                var batchWorkItems = workItemTrackingClient.GetWorkItemsAsync(batchIds, new[] { "System.WorkItemType", "System.State", "Microsoft.VSTS.Scheduling.CompletedWork" }).Result;
                workItems.AddRange(batchWorkItems);
            }

            var metrics = new WorkItemMetrics
            {
                TotalWorkItems = workItems.Count,
                TotalBugs = workItems.Count(wi => wi.Fields["System.WorkItemType"].ToString() == "Bug"),
                TotalWorkingHours = workItems.Where(wi => wi.Fields.ContainsKey("Microsoft.VSTS.Scheduling.CompletedWork"))
                                             .Sum(wi => (double)wi.Fields["Microsoft.VSTS.Scheduling.CompletedWork"])
            };

            foreach (var workItem in workItems)
            {
                var state = workItem.Fields["System.State"].ToString();
                if (metrics.StatusCounts.ContainsKey(state))
                {
                    metrics.StatusCounts[state]++;
                }
                else
                {
                    metrics.StatusCounts[state] = 1;
                }
            }

            return metrics;
        }
        catch (VssServiceResponseException ex)
        {
            Console.WriteLine($"VssServiceResponseException: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            throw;
        }
    }
    private List<ChartEntry> _entries;
    private void CreateChart(WorkItemMetrics metrics)
    {
        try
        {
            _entries = new List<ChartEntry>
     {
             new ChartEntry(metrics.StatusCounts.ContainsKey("Requested") ? metrics.StatusCounts["Requested"] : 0)
             {
                 Label = "Requested",
                 ValueLabel = metrics.StatusCounts.ContainsKey("Requested") ? metrics.StatusCounts["Requested"].ToString() : "0",
                 Color = SKColor.Parse("#266489")
             },
             new ChartEntry(metrics.StatusCounts.ContainsKey("Done") ? metrics.StatusCounts["Done"] : 0)
             {
                 Label = "Done",
                 ValueLabel = metrics.StatusCounts.ContainsKey("Done") ? metrics.StatusCounts["Done"].ToString() : "0",
                 Color = SKColor.Parse("#68B9C0")
             },
             new ChartEntry(metrics.StatusCounts.ContainsKey("Committed") ? metrics.StatusCounts["Committed"] : 0)
             {
                 Label = "Committed",
                 ValueLabel = metrics.StatusCounts.ContainsKey("Committed") ? metrics.StatusCounts["Committed"].ToString() : "0",
                 Color = SKColor.Parse("#90D585")
             },
             new ChartEntry(metrics.StatusCounts.ContainsKey("Removed") ? metrics.StatusCounts["Removed"] : 0)
             {
                 Label = "Removed",
                 ValueLabel = metrics.StatusCounts.ContainsKey("Removed") ? metrics.StatusCounts["Removed"].ToString() : "0",
                 Color = SKColor.Parse("#D64E4F")
             },
             new ChartEntry(metrics.StatusCounts.ContainsKey("To Do") ? metrics.StatusCounts["To Do"] : 0)
             {
                 Label = "To Do",
                 ValueLabel = metrics.StatusCounts.ContainsKey("To Do") ? metrics.StatusCounts["To Do"].ToString() : "0",
                 Color = SKColor.Parse("#a83832")
             },
             //new ChartEntry(metrics.StatusCounts.ContainsKey("Design") ? metrics.StatusCounts["Design"] : 0)
             //{
             //    Label = "Design",
             //    ValueLabel = metrics.StatusCounts.ContainsKey("Design") ? metrics.StatusCounts["Design"].ToString() : "0",
             //    Color = SKColor.Parse("#a8a632")
             //},
             new ChartEntry(metrics.StatusCounts.ContainsKey("In Progress") ? metrics.StatusCounts["In Progress"] : 0)
             {
                 Label = "In Progress",
                 ValueLabel = metrics.StatusCounts.ContainsKey("In Progress") ? metrics.StatusCounts["In Progress"].ToString() : "0",
                 Color = SKColor.Parse("#6fa832")
             }
         };
            var chart = new DonutChart
            {
                Entries = _entries,
                HoleRadius = 0.5F
            };

            TaskChartView.Chart = chart;
        }
        catch (Exception ex)
        {

        }
    }
    private void OnChartTapped(object sender, EventArgs e)
        {
            // Example logic to navigate based on tapped entry
            // You can improve this to detect the exact tapped entry
            var tappedEntry = _entries[0]; // Replace with logic to get the actual tapped entry

            if (tappedEntry.Label == "New")
            {
                // Navigate to New work items page
            }
            else if (tappedEntry.Label == "Active")
            {
                // Navigate to Active work items page
            }
            else if (tappedEntry.Label == "Resolved")
            {
                // Navigate to Resolved work items page
            }
            else if (tappedEntry.Label == "Closed")
            {
                // Navigate to Closed work items page
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

        private void ProjectPckr_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                CommonClass.projectDetails.SelectedProject = new();
                if (ProjectPckr.SelectedItem != null)
                {
                    CommonClass.projectDetails.SelectedProject = CommonClass.projectDetails.Projects.FirstOrDefault(x => x.ProjectName == ProjectPckr.SelectedItem.ToString());
                }

                var metrics = GetWorkItemMetrics();
                CreateChart(metrics);
            }
            catch (Exception ex) { }

        }
    }