using FastNetwork.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace FastNetwork.Client
{
    
    public class DefaultSocketClient : BaseSocketClient
    {
        //与服务器的连接
        private IConnection _connection = null;

        //编码器
        private IEncoder _encoder = null;

        public DefaultSocketClient(IPAddress address, int port, IClientHandler handler,IEncoder encoder)
            : base(address, port, handler)
        {
            if (encoder == null) throw new ArgumentNullException("encoder");
            this._encoder = encoder;
        }
       

        /// <summary>
        /// 发送数据到服务器
        /// </summary>
        /// <param name="request"></param>
        public override void Send(Object msg)
        {
            //编码
            byte[] data= this._encoder.encode(this._connection,msg);
            //构建消息包
            byte[] payload = PacketBuilder.ToAsyncBinaryByte(data);

            Request request = new Request(payload, OnException);
            if (_connection == null)
                this.EnqueueToPendingQueue(request);//没有连接可用，放入待发送队列
            else
            {
                _connection.BeginSend(request);
            }

        }
        public override void SendRequest(Request request)
        {
            if (_connection == null)
                this.EnqueueToPendingQueue(request);//没有连接可用，放入待发送队列
            else
            {
                _connection.BeginSend(request);
            }
        }
        private void OnException(Exception ex)
        {
            base.OnError(_connection,ex);
        }
        protected override void OnConnected(IConnection connection)
        {
            this._connection = connection;
            base.OnConnected(connection);
        }
    }
}
