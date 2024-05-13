using Exiled.API.Features;
using MEC;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHPExiled.API.Features.FileSocket
{
    /// <summary>
    /// This class represent the communication between the server and the client, from the "POV" of the server.
    /// Read will read the messages in the file buffer while write will write something in the buffer
    /// </summary>
    internal class FileSocketStream
    {
        public string DownstreamAddress { get; }

        public string UpstreamAddress { get; }

        public FileStream FileUpstream { get; }

        public FileStream FileDownstream { get; }

        public List<FileSocketMessage> History { get; } 

        public List<FileSocketServerMessage> WriteQueue { get; }

        public CoroutineHandle Coroutine { get; }

        public readonly float RefreshRate = 0.025f;

        public bool IsRunning { get; internal set; } = false;

        public FileSocketServer Server { get; }

        public FileSocketStream(string downstreamAddress, string upstreamAddress, FileSocketServer server)
        {
            Server = server;
            DownstreamAddress = downstreamAddress;
            UpstreamAddress = upstreamAddress;

            FileDownstream = new(Path.Combine(Paths.Configs, ".temprun", DownstreamAddress), FileMode.OpenOrCreate, FileAccess.ReadWrite);

            if (DownstreamAddress == UpstreamAddress)
            {
                FileUpstream = FileDownstream;
            }
            else
            {
                FileUpstream = new(Path.Combine(Paths.Configs, ".temprun", UpstreamAddress), FileMode.OpenOrCreate, FileAccess.Write);
            }

            History = new();
            WriteQueue = new();

            IsRunning = true;

            Coroutine = Timing.RunCoroutine(Loop());
        }

        internal IEnumerator<float> Loop()
        {
            while (IsRunning)
            {
                // First doing the reading task, then the writing one
                Read();
                // resets the read stream after the reading
                // Now let's read the thing
                Write();
                yield return Timing.WaitForSeconds(RefreshRate);
            }
        }

        public List<FileSocketMessage> Read()
        {
            List<FileSocketMessage> Messages = new();

            foreach (string RawMessage in new StreamReader(FileDownstream).ReadToEnd().Split(new string[1] { "== END MESSAGE ==" }, System.StringSplitOptions.RemoveEmptyEntries))
            {
                if (RawMessage.Length < 7)
                {
                    continue;
                }
                Log.Warn($"New message got: {RawMessage}");
                Dictionary<string, string> Content = JsonConvert.DeserializeObject<Dictionary<string, string>>(RawMessage);
                Log.Warn("READ");
                if (FileSocketMessage.ValidateSocketMessage(Content))
                {
                    FileSocketMessage Message = new(Server.GetSocketClient(uint.Parse(Content["id"])), Server, uint.Parse(Content["message_id"]), Content["content"], Content["scope"]);
                    History.Add(Message);
                    Messages.Add(Message);
                }
            }

            File.WriteAllText

            StreamWriter Writer = new()

            return Messages;
        }

        internal void Write()
        {
            foreach (FileSocketServerMessage Message in WriteQueue)
            {
                Send(Message.Encode());
            }
            WriteQueue.Clear();
        }

        public void Send(FileSocketServerMessage message)
        {
            WriteQueue.Add(message);
        }

        internal void Send(string message)
        {
            StreamWriter Writer = new(FileUpstream);
            Writer.Write($"{message}\n== END MESSAGE ==");
            Writer.Flush();
        }
    }
}
