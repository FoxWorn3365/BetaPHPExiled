using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHPExiled.API.Features.FileSocket
{
    internal class FileSocketMessage
    {
        public FileSocketClient Sender { get; }

        public FileSocketServer Receiver { get; }

        public uint Id { get; }

        public uint SequenceId { get; }

        public long Time { get; }

        public Dictionary<string, string> Content { get; }

        public MessageScope Scope { get; }

        public string RawContent { get; }

        public bool FromServer { get; }

        /// <summary>
        /// Create an istance of <see cref="FileSocketMessage"/> that represents a message sent to the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="receiver"></param>
        /// <param name="id"></param>
        /// <param name="content"></param>
        public FileSocketMessage(FileSocketClient sender, FileSocketServer receiver, uint id, Dictionary<string, string> content, MessageScope scope)
        {
            Sender = sender;
            Receiver = receiver;
            Id = id;
            Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Content = content;
            RawContent = JsonConvert.SerializeObject(content);
            FromServer = false;
            Scope = scope;
        }

        /// <summary>
        /// Create an istance of <see cref="FileSocketMessage"/> that represents a message sent to the server
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="receiver"></param>
        /// <param name="id"></param>
        /// <param name="content"></param>
        public FileSocketMessage(FileSocketClient sender, FileSocketServer receiver, uint id, string content, string scope)
        {
            Sender = sender;
            Receiver = receiver;
            Id = id;
            Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            Content = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            RawContent = content;
            FromServer = false;
            Enum.TryParse(scope, out MessageScope parsedScope);
            Scope = parsedScope;
        }

        public static bool ValidateSocketMessage(Dictionary<string, string> message)
        {
            return message.ContainsKey("id") && message.ContainsKey("content") && message.ContainsKey("message_id") && message.ContainsKey("scope");
        }
    }
}
