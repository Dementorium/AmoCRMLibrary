using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AmoCRMDownloader.Models
{
    public class TaskResponse
    {
        [JsonProperty("tasks")]
        public Tasks tasks { get; set; }
    }

    public class RootObject
    {
        [JsonProperty("response")]
        public TaskResponse response { get; set; }
    }

}
