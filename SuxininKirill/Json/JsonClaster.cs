using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SuxininKirill.Json
{
    public class JsonSqlSetting
    {
        
        public class RootSettingServer
        {
            [JsonProperty("Server",NullValueHandling = NullValueHandling.Ignore)]
            public Server Server { get; set; }
        }

        public class Server
        {
            [JsonProperty("Windows", NullValueHandling = NullValueHandling.Ignore)]
            public Windows Windows { get; set; }
            [JsonProperty("Ubuntu/Linux", NullValueHandling = NullValueHandling.Ignore)]
            public UbuntuLinux UbuntuLinux { get; set; }

            [JsonProperty("Invoke",NullValueHandling = NullValueHandling.Ignore)]
            public string Invoke;
        }

        public class Windows
        {
            [JsonProperty("DataSource", NullValueHandling = NullValueHandling.Ignore)]
            public string DataSource { get; set; }
            [JsonProperty("InitialCatalog", NullValueHandling = NullValueHandling.Ignore)]
            public string InitialCatalog { get; set; }
            [JsonProperty("IntegratedSecurity", NullValueHandling = NullValueHandling.Ignore)]
            public string IntegratedSecurity { get; set; }
            [JsonProperty("NamespaceType", NullValueHandling = NullValueHandling.Ignore)]
            public string NamespaceType { get; set; }
        }

        public class UbuntuLinux
        {
            [JsonProperty("Server", NullValueHandling = NullValueHandling.Ignore)]
            public string Server { get; set; }
            [JsonProperty("IdUser", NullValueHandling = NullValueHandling.Ignore)]
            public string IdUser { get; set; }
            [JsonProperty("Password", NullValueHandling = NullValueHandling.Ignore)]
            public string Password { get; set; }
            [JsonProperty("Database", NullValueHandling = NullValueHandling.Ignore)]
            public string Database { get; set; }
            [JsonProperty("NamespaceType", NullValueHandling = NullValueHandling.Ignore)]
            public string NamespaceType { get; set; }
        }

    }
}
