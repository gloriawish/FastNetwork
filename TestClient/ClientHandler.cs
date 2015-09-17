using FastNetwork.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestClient
{
    public class ClientHandler:IClientHandler
    {
        public BaseSocketClient _client = null;
        

        public void OnConnected(FastNetwork.IConnection connection)
        {
            Console.WriteLine("connected server");

            
        }

        public void OnStartSending(FastNetwork.IConnection connection, FastNetwork.Packet packet)
        {
            //Console.WriteLine("start sending");
        }

        public void OnSendCallback(FastNetwork.IConnection connection, FastNetwork.Event.SendCallbackEventArgs e)
        {
            //Console.WriteLine("send callback");

            //UserInfo info = new UserInfo("zhujun", 18);

            //_client.Send(info);
        }

        public void OnReceived(FastNetwork.IConnection connection, object obj)
        {
            Console.WriteLine("receive data");
        }

        public void OnDisconnected(FastNetwork.IConnection connection, Exception ex)
        {
            Console.WriteLine("disconnect with server");
        }

        public void OnException(FastNetwork.IConnection connection, Exception ex)
        {
            Console.WriteLine("error");
        }
    }
}
