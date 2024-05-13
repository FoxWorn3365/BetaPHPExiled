using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using PHPExiled.API.Features.FileSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHPExiled
{
    internal class Plugin : Plugin<Config>
    {
        public override string Name => "PHPExiled";

        public override string Prefix => "PHPExiled";

        public override string Author => "FoxWorn3365";

        public override Version Version => new(0, 2, 1);

        public override Version RequiredExiledVersion => new(8, 8, 1);

        public override PluginPriority Priority => PluginPriority.Low;

        internal static Plugin Instance;

        internal static FileSocketServer SocketServer;

        internal int Index = 0;

        public override void OnEnabled()
        {
            Instance = this;
            SocketServer = new("foxhttptest", true);

            Timing.CallContinuously(2, () => SocketServer.Send(new FileSocketServerMessage(SocketServer, 10, new Dictionary<string, string>()
            {
                {
                    "index",
                    Index.ToString()
                }
            })));


            base.OnEnabled();
        }
    }
}
