using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppStoreApiWithIdentityServer4.Models
{
    public class DownloadLog
    { public int app_id { get; set; }
        public String app_version { get; set; }
        public String time_range { get; set; }
        public int hits { get; set; }
    }
}
