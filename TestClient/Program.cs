using FastNetwork.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            FastNetwork.Log.Trace.EnableConsole();
            for (int i = 0; i < 50; i++)
            {
                Thread td1 = new Thread(run);
                td1.Start();
            }

        }
        static void run()
        {
            ClientHandler handler = new ClientHandler();

            DefaultSocketClient client = new DefaultSocketClient(IPAddress.Parse("127.0.0.1"), 8008, handler, new DefaultEncoder());
            handler._client = client;
            client.Start();

            UserInfo info = null;
            string msg = "messagemessagemessagemessagemessagemessagemessagemessagemessagemessagemessagemessagemessagemessagemessagemessagemessagemessagemessagemessagemessagemessagemessagemessagemessagemessagemessagemessagemessagemessage";
            for (int i = 0; ; i++)
            {
                info = new UserInfo(msg + msg + msg, 18);
                client.Send(info);
                Thread.Sleep(1);
            }
        }
    }
}
