using PHPExiled.API.Features.FileSocket.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHPExiled.API
{
    internal class Storage
    {
        public static Dictionary<uint, SocketPlugin> Plugins { get; } = new();
    }
}
