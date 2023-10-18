using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackPro.Models
{
    public class ProjectDetails
    {
        public List<ProjectCollections> ProjectCollections { get; set; }
        public ProjectCollections SelectedProjectCollection { get; set; }
        public List<Projects> Projects { get; set; }
        public List<WorkItems> WorkItems { get; set; }
        public WorkItems SelectedWorkItem { get; set; }
        public List<int> SelectedWorkItemId { get; set; }
        public Projects SelectedProject { get; set; }
        public List<Guid> SelectedProjectId { get; set; }
        public List<Guid> SelectedCollectionId { get; set; }
    }
    public class ApplicationData
    {
        public List<HardwareVerification> HardwareVerifications { get; set; }
    }
    public class HardwareVerification
    {
        public string Name { get; set; }
        public string Time { get; set; }
        public string Actual { get; set; }
        public string Recommended { get; set; }
    }
    public class ProjectCollections
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
    public class WorkItems
    {
        public int? Id { get; set; }
        public string IterationPath { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public int RemainingHours { get; set; }
    }

    public class Projects
    {
        public string ProjectName { get; set; }
        public Guid Id { get; set; }
    }
}
