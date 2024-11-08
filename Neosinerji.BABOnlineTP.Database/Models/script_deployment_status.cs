using System;
using System.Collections.Generic;

namespace Neosinerji.BABOnlineTP.Database.Models
{
    public partial class script_deployment_status
    {
        public System.Guid deployment_id { get; set; }
        public Nullable<System.Guid> worker_id { get; set; }
        public string logical_server { get; set; }
        public string database_name { get; set; }
        public Nullable<System.DateTimeOffset> deployment_start { get; set; }
        public Nullable<System.DateTimeOffset> deployment_end { get; set; }
        public string status { get; set; }
        public short num_retries { get; set; }
        public string messages { get; set; }
    }
}
