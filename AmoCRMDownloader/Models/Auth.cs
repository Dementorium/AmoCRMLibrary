using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmoCRMDownloader.Models
{
    public class Account
    {
        public string id { get; set; }
        public string name { get; set; }
        public string subdomain { get; set; }
        public string language { get; set; }
        public string timezone { get; set; }
    }

    public class User
    {
        public string id { get; set; }
        public string language { get; set; }
    }

    public class AuthResponse
    {
        public bool auth { get; set; }
        public List<Account> accounts { get; set; }
        public User user { get; set; }
        public int server_time { get; set; }
    }

    public class Auth
    {
        public AuthResponse response { get; set; }
    }

}
