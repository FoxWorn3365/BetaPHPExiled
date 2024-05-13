using Newtonsoft.Json;
using System.Collections.Generic;

namespace PHPExiled.API.Features.FileSocket
{
    internal class FileSocketServerMessage
    {
        public FileSocketServer Sender { get; }

        public FileSocketClient Receiver { get; }

        public uint Id { get; }

        public string RawContent { get; }

        public Dictionary<string, string> Content { get; }

        public FileSocketServerMessage(FileSocketServer sender, FileSocketClient receiver, uint id, Dictionary<string, string> content)
        {
            Sender = sender;
            Receiver = receiver;
            Id = id;
            RawContent = JsonConvert.SerializeObject(content);
            Content = content;
        }

        public FileSocketServerMessage(FileSocketServer sender, FileSocketClient receiver, uint id, string content)
        {
            Sender = sender;
            Receiver = receiver;
            Id = id;
            RawContent = content;
            Content = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
        }

        public FileSocketServerMessage(FileSocketServer sender, uint id, string content)
        {
            // The broadcast mode
            Sender = sender;
            Receiver = null;
            Id = id;
            RawContent = content;
            Content = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
        }

        public FileSocketServerMessage(FileSocketServer sender, uint id, Dictionary<string, string> content)
        {
            Sender = sender;
            Receiver = null;
            Id = id;
            RawContent = JsonConvert.SerializeObject(content);
            Content = content;
        }

        public string Encode()
        {
            return JsonConvert.SerializeObject(new Dictionary<string, string>()
            {
                {
                    "id",
                    Sender.Id.ToString()
                },
                {
                    "receiver",
                    Receiver?.Id.ToString() ?? ""
                },
                {
                    "message_id",
                    Id.ToString()
                },
                {
                    "content",
                    RawContent
                }
            });
        }
    }
}
