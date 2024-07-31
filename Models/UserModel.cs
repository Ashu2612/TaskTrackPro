using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTrackPro.Models
{
    public class UserModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string DisplayName { get; set; }
        public string AccessToken { get; set; }
        public string JobTitle { get; set; }
        public byte[] UserProfilePic { get; set; }
    }

    public class Icons
    {
        public static string DashbordIcon = "\ue871";
        public static string ProjectIcon = "\ue8f9";
        public static string TaskIcon = "\uf075";
        public static string AddTaskIcon = "\uf848";
        public static string BackIcon = "\ue2ea";
        public static string SelfActivityIcon = "\uf20c";
        public static string TeamActivityIcon = "\ue50a";

    }


}
