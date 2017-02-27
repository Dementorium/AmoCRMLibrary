using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AmoDownloaderCLR.Models
{
    public class Account
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("subdomain")]
        public string subdomain { get; set; }

        [JsonProperty("language")]
        public string language { get; set; }

        [JsonProperty("timezone")]
        public string timezone { get; set; }
    }

    public class User
    {
        [JsonProperty("id")]
        public string id { get; set; }

        [JsonProperty("language")]
        public string language { get; set; }
    }

    public class AuthResponse
    {
        [JsonProperty("auth")]
        public bool auth { get; set; }

        [JsonProperty("accounts")]
        public List<Account> accounts { get; set; }

        [JsonProperty("user")]
        public User user { get; set; }

        [JsonProperty("server_time")]
        public int server_time { get; set; }
    }

    public class Auth
    {
        [JsonProperty("response")]
        public AuthResponse response { get; set; }
    }

}
