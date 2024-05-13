using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHPExiled.API.Features.FileSocket
{
    internal class FileSocketClient
    {
        public string Address { get; }

        public uint Id { get; }

        public SocketClientStatus Status { get; set; }

        public FileSocketClient(string address, uint id, SocketClientStatus status)
        {
            Address = address;
            Id = id;
            Status = status;
        }
    }
}
