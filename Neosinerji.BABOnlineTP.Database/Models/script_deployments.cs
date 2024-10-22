using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class script_deployments
    {
        public System.Guid deployment_id { get; set; }
        public System.Guid coordinator_id { get; set; }
        public string deployment_name { get; set; }
        public System.DateTimeOffset deployment_submitted { get; set; }
        public Nullable<System.DateTimeOffset> deployment_start { get; set; }
        public Nullable<System.DateTimeOffset> deployment_end { get; set; }
        public string status { get; set; }
        public string results_table { get; set; }
        public string retry_policy { get; set; }
        public string script { get; set; }
        public string messages { get; set; }
    }
}
