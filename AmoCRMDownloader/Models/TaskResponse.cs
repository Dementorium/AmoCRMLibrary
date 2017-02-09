using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmoCRMDownloader.Models
{

    public class TaskResponse
    {
        public Tasks tasks { get; set; }
    }

    public class RootObject
    {
        public TaskResponse response { get; set; }
    }

}
