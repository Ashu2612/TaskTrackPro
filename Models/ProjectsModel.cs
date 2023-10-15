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
        public List<ProjectCollections> SelectedProjectCollection { get; set; }
        public List<Projects> Projects { get; set; }
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


    public class Projects
    {
        public string ProjectName { get; set; }
        public string ProjectUri { get; set; }
    }
}
