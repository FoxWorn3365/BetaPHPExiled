using Exiled.API.Features;
using MEC;
using PHPExiled.API.Features.FileSocket.Extension;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PHPExiled.API.Features.FileSocket
{
    internal class FileSocketServer
    {
        public string Address { get; }

        public uint Id { get; } = 0;

        public bool SplitDownstreamAndUpstream { get; }

        public List<FileSocketClient> Clients { get; }

        public List<FileSocketClient> Queue {  get; }

        public FileSocketStream Stream { get; }

        public CoroutineHandle GarbageCollector { get; }

        public Action<FileSocketMessage> Action { get; set; }

        public bool IsRunning { get; }

        public FileSocketServer(string address, bool splitDownstreamAndUpstream = true)
        {
            Address = address;
            SplitDownstreamAndUpstream = splitDownstreamAndUpstream;
            string UpAddress = Address + "_up";
            if (!splitDownstreamAndUpstream)
            {
                UpAddress = Address;
            }

            Clients = new();
            Queue = new();

            Action = (FileSocketMessage message) => { };

            IsRunning = true;

            Stream = new(Address, UpAddress, this);

            GarbageCollector = Timing.RunCoroutine(GarbageCollectorAct());
        }

        public void ClientTryingConnection(Dictionary<string, string> headers)
        {
            if (SocketPlugin.ValidatePluginConnectionHeaders(headers) && SocketPlugin.ValidatePluginConnectionData(headers["plugin_data"]))
            {
                // The connection can be accepted as it's valid - let's put it in the queue -> the clients need to send us a ping message to be fully accepted!
                FileSocketClient Client = new(headers["address"], uint.Parse(headers["id"]), SocketClientStatus.ConnessionEnstabilshed);
                Queue.Add(Client);
                Storage.Plugins.Add(Client.Id, new(headers["plugin_data"], Client));
            }
        }

        internal void AcceptClient(FileSocketClient client)
        {
            if (Queue.Contains(client))
            {
                Queue.Remove(client);
                Clients.Add(client);
            }
        }

        internal void ReadHandler(List<FileSocketMessage> messages)
        {
            foreach (FileSocketMessage Message in messages)
            {
                if (Message.Scope == MessageScope.Ping && Queue.Contains(Message.Sender))
                {
                    // Ping received, let's move the client to the connection list
                    Queue.Remove(Message.Sender);
                    Clients.Add(Message.Sender);
                }
                else if (Message.Scope == MessageScope.Exiting && Clients.Contains(Message.Sender))
                {
                    Clients.Remove(Message.Sender);
                }
            }
        }

        internal IEnumerator<float> GarbageCollectorAct()
        {
            while (IsRunning)
            {
                Log.Warn("Garbage Collector");
                Queue.Clear();
                yield return Timing.WaitForSeconds(10f);
            }
        }

        public bool TryGetSocketClient(uint Id, out FileSocketClient client)
        {
            client = Clients.Where(c => c.Id == Id).ToList().FirstOrDefault();
            if (client is not null)
            {
                return true;
            }
            return false;
        }

        public FileSocketClient GetSocketClient(uint Id)
        {
            TryGetSocketClient(Id, out FileSocketClient client);
            return client;
        }

        public bool IsSocketClient(uint Id)
        {
            return TryGetSocketClient(Id, out _);
        }

        public void Send(FileSocketServerMessage message) => Stream.Send(message);
    }
}
