using FastNetwork.Event;
using FastNetwork.Protocol;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace FastNetwork
{
    /// <summary>
    /// socket 服务器
    /// </summary>
    public class SocketServer : BaseSocketServer
    {
        #region Private Members
        private readonly List<ISocketListener> _listListener = new List<ISocketListener>();
        private readonly IServerHandler _handler = null;
        private readonly IProtocol _protocol = null;
        //编码器
        private readonly IEncoder _encoder = null;
        //解码器
        private readonly IDecoder _decoder = null;
        private readonly int _maxMessageSize;
        private readonly int _maxConnections;
        #endregion

        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        /// <param name="socketService"></param>
        /// <param name="protocol"></param>
        /// <param name="socketBufferSize"></param>
        /// <param name="messageBufferSize"></param>
        /// <param name="maxMessageSize"></param>
        /// <param name="maxConnections"></param>
        /// <exception cref="ArgumentNullException">socketService is null.</exception>
        /// <exception cref="ArgumentNullException">protocol is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">maxMessageSize</exception>
        /// <exception cref="ArgumentOutOfRangeException">maxConnections</exception>
        public SocketServer(IServerHandler handler,
            Protocol.IProtocol protocol,
            int socketBufferSize,
            int messageBufferSize,
            int maxMessageSize,
            int maxConnections)
            : base(socketBufferSize, messageBufferSize)
        {
            if (handler == null) throw new ArgumentNullException("handler");
            if (protocol == null) throw new ArgumentNullException("protocol");
            if (maxMessageSize < 1) throw new ArgumentOutOfRangeException("maxMessageSize");
            if (maxConnections < 1) throw new ArgumentOutOfRangeException("maxConnections");

            this._handler = handler;
            this._protocol = protocol;
            this._maxMessageSize = maxMessageSize;
            this._maxConnections = maxConnections;
        }
        /// <summary>
        /// 使用默认配置参数的构造函数
        /// </summary>
        /// <param name="handler"></param>
        public SocketServer(IServerHandler handler,IEncoder encoder, IDecoder decoder)
            : base(DefaultConfigure.SocketBufferSize, DefaultConfigure.MessageBufferSize)
        {
            if (handler == null) throw new ArgumentNullException("handler");
            if (encoder == null) throw new ArgumentNullException("encoder");
            if (decoder == null) throw new ArgumentNullException("decoder");
            this._handler = handler;
            this._protocol = new DefaultBinaryProtocol();
            this._encoder = encoder;
            this._decoder = decoder;
            this._maxMessageSize = DefaultConfigure.MaxMessageSize;
            this._maxConnections = DefaultConfigure.MaxConnections;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// socket accepted handler
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="connection"></param>
        private void listener_Accepted(ISocketListener listener, IConnection connection)
        {
            if (base._listConnections.Count() > this._maxConnections)
            {
                connection.BeginDisconnect(); return;
            }
            base.RegisterConnection(connection);
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// start
        /// </summary>
        public override void Start()
        {
            foreach (var child in this._listListener) child.Start();
        }
        /// <summary>
        /// stop
        /// </summary>
        public override void Stop()
        {
            foreach (var child in this._listListener) child.Stop();
            base.Stop();
        }
        /// <summary>
        /// add socket listener
        /// </summary>
        /// <param name="name"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public override ISocketListener AddListener(string name, IPEndPoint endPoint)
        {
            var listener = new SocketListener(name, endPoint, this);
            this._listListener.Add(listener);
            listener.Accepted += new Action<ISocketListener, IConnection>(this.listener_Accepted);
            return listener;
        }
        /// <summary>
        /// OnConnected
        /// </summary>
        /// <param name="connection"></param>
        protected override void OnConnected(IConnection connection)
        {
            //设置连接的编码器
            connection.Encoder = this._encoder;
            base.OnConnected(connection);
            try { this._handler.OnConnected(connection); }
            catch (Exception ex) { Log.Trace.Error(ex.Message, ex); }
        }
        /// <summary>
        /// start sending
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="packet"></param>
        protected override void OnStartSending(IConnection connection, Packet packet)
        {
            base.OnStartSending(connection, packet);
            try { this._handler.OnStartSending(connection, packet); }
            catch (Exception ex) { Log.Trace.Error(ex.Message, ex); }
        }
        /// <summary>
        /// send callback
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="e"></param>
        protected override void OnSendCallback(IConnection connection, SendCallbackEventArgs e)
        {
            base.OnSendCallback(connection, e);
            try { this._handler.OnSendCallback(connection, e); }
            catch (Exception ex) { Log.Trace.Error(ex.Message, ex); }
        }
        /// <summary>
        /// OnMessageReceived
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="e"></param>
        protected override void OnMessageReceived(IConnection connection, MessageReceivedEventArgs e)
        {
            base.OnMessageReceived(connection, e);

            int readlength=0;
            ReceivePacket packet = null;
            Object obj = null;//解析的对象
            try
            {
                packet = this._protocol.TryToTranslateMessage(connection, e.Buffer, this._maxMessageSize, out readlength);
                if (packet == null)
                    return;
                obj = this._decoder.decode(connection, packet.Buffer);
            }
            catch (Exception ex)
            {
                this.OnError(connection, ex);
                connection.BeginDisconnect(ex);
                e.SetReadlength(e.Buffer.Count);
                return;
            }

            if (packet != null)
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try { this._handler.OnReceived(connection, obj); }
                    catch (Exception ex) { Log.Trace.Error(ex.Message, ex); }
                });
            e.SetReadlength(readlength);
        }
        /// <summary>
        /// OnDisconnected
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        protected override void OnDisconnected(IConnection connection, Exception ex)
        {
            base.OnDisconnected(connection, ex);
            try { this._handler.OnDisconnected(connection, ex); }
            catch (Exception ex2) { Log.Trace.Error(ex.Message, ex2); }
        }
        /// <summary>
        /// onError
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        protected override void OnError(IConnection connection, Exception ex)
        {
            base.OnError(connection, ex);
            try { this._handler.OnException(connection, ex); }
            catch (Exception ex2) { Log.Trace.Error(ex.Message, ex2); }
        }
        #endregion
    }
}