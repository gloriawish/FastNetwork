using FastNetwork;
using FastNetwork.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Test
{
    
    class Program
    {
        static void Main(string[] args)
        {
            FastNetwork.Log.Trace.EnableConsole();
            ServerHandler handler = new ServerHandler();
            DefaultBinaryProtocol protocal = new DefaultBinaryProtocol();

            SocketServer server = new SocketServer(handler, new DefaultEncoder(),new DefaultDecoder());

            server.AddListener("tcp", new System.Net.IPEndPoint(IPAddress.Any,8008));
            try
            {
                server.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            Console.WriteLine("server start");
            Console.ReadLine();

        }
    }
}
