using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace PHPExiled.API.Features.FileSocket.Extension
{
    internal class SocketPlugin
    {
        public uint Id { get; }

        public string Name { get; }
        
        public string Prefix { get; }

        public string Author { get; }

        public Version Version { get; }

        public List<string> SubscribedEvents { get; }

        public FileSocketClient Client { get; }

        public SocketPlugin(string name, string prefix, string author, Version version, List<string> subscribedEvents, FileSocketClient client)
        {
            Name = name;
            Prefix = prefix;
            Author = author;
            Version = version;
            SubscribedEvents = subscribedEvents;
            Client = client;
            Id = Client.Id;
        }

        public SocketPlugin(Dictionary<string, string> data, FileSocketClient connection)
        {
            Name = data["name"];
            Prefix = data["prefix"];
            Author = data["author"];
            Version = new(data["version"]);
            SubscribedEvents = data["events"].Split(',').ToList();
            Client = connection;
        }

        public SocketPlugin(string rawData, FileSocketClient connection)
        {
            Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(rawData);
            Name = data["name"];
            Prefix = data["prefix"];
            Author = data["author"];
            Version = new(data["version"]);
            SubscribedEvents = data["events"].Split(',').ToList();
            Client = connection;
        }

        public static bool ValidatePluginConnectionHeaders(Dictionary<string, string> headers)
        {
            return headers.ContainsKey("id") && headers.ContainsKey("plugin_data") && headers.ContainsKey("address");
        }

        public static bool ValidatePluginConnectionData(Dictionary<string, string> data)
        {
            return data.ContainsKey("name") && data.ContainsKey("prefix") && data.ContainsKey("author") && data.ContainsKey("version") && data.ContainsKey("events");
        }

        public static bool ValidatePluginConnectionData(string data)
        {
            return ValidatePluginConnectionData(JsonConvert.DeserializeObject<Dictionary<string, string>>(data));
        }
    }
}
