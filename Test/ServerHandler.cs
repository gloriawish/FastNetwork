using FastNetwork;
using FastNetwork.Event;
using FastNetwork.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Test
{
    /// <summary>
    ///  实现函数处理
    /// </summary>
    public class ServerHandler : IServerHandler
    {
        int count = 0;
        public void OnConnected(IConnection connection)
        {
            Console.WriteLine("connected from:" + connection.RemoteEndPoint.ToString());

            //开始接收数据,没有这个操作将不接受该连接发送的数据
            connection.BeginReceive();
        }

        public void OnStartSending(IConnection connection, Packet packet)
        {
            //throw new NotImplementedException();
            Console.WriteLine("send beging");
        }

        public void OnSendCallback(IConnection connection, SendCallbackEventArgs e)
        {
            //throw new NotImplementedException();
            Console.WriteLine("send ok");
        }

        public void OnReceived(IConnection connection, Object obj)
        {
            //throw new NotImplementedException();
            UserInfo info = (UserInfo)obj;
            //Console.WriteLine("receive from " + connection.RemoteEndPoint.ToString()+":"+info.username);
            Interlocked.Increment(ref this.count);

            if (count % 1000 == 0)
            {
                Console.WriteLine("receive:" + count);
            }

        }

        public void OnDisconnected(IConnection connection, Exception ex)
        {
            Console.WriteLine("disconnected from:" + connection.RemoteEndPoint.ToString());
        }

        public void OnException(IConnection connection, Exception ex)
        {
            //throw new NotImplementedException();
        }
    }
}
