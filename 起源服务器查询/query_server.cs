using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace 起源服务器查询
{
    //为用web查询到的ip上的服务器列表
    public class query_server
    {
        [DataContract]
        public class Server
        {
            [DataMember]
            public string addr { get; set; }
            [DataMember]
            public int gmsindex { get; set; }
            [DataMember]
            public int appid { get; set; }
            [DataMember]
            public string gamedir { get; set; }
            [DataMember]
            public int region { get; set; }
            [DataMember]
            public bool secure { get; set; }
            [DataMember]
            public bool lan { get; set; }
            [DataMember]
            public int gameport { get; set; }
            [DataMember]
            public int specport { get; set; }
        }

        [DataContract]
        public class Response
        {
            [DataMember]
            public bool success { get; set; }
            [DataMember]
            public List<Server> servers { get; set; }
        }

        [DataContract]
        public class root_message
        {
            [DataMember]
            public Response response { get; set; }
        }
    }
}
