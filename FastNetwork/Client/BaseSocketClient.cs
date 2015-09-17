using FastNetwork.Event;
using FastNetwork.Protocol;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace FastNetwork.Client
{
    /// <summary>
    /// socket client
    /// </summary>
    public abstract class BaseSocketClient : BaseHost
    {
        #region Private Members
        private readonly IProtocol _protocol = null;

        private readonly int _millisecondsSendTimeout;
        private readonly int _millisecondsReceiveTimeout;

        private readonly IClientHandler _handler = null;

        private readonly PendingSendQueue _pendingQueue = null;

        private readonly int _port;

        private readonly IPAddress _remote;

        static AutoResetEvent autoEvent = new AutoResetEvent(false);

        
        #endregion

        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        /// <param name="protocol"></param>
        public BaseSocketClient(IPAddress remote, int port, IClientHandler handler)
            : this(new DefaultBinaryProtocol(), 8192, 8192, 3000, 3000)
        {
            if (handler == null) throw new ArgumentNullException("handler");
            this._handler = handler;
            this._remote = remote;
            this._port = port;
        }
        /// <summary>
        /// new
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="socketBufferSize"></param>
        /// <param name="messageBufferSize"></param>
        /// <param name="millisecondsSendTimeout"></param>
        /// <param name="millisecondsReceiveTimeout"></param>
        /// <exception cref="ArgumentNullException">protocol is null</exception>
        public BaseSocketClient(IProtocol protocol,
            int socketBufferSize,
            int messageBufferSize,
            int millisecondsSendTimeout,
            int millisecondsReceiveTimeout)
            : base(socketBufferSize, messageBufferSize)
        {
            if (protocol == null) throw new ArgumentNullException("protocol");
            this._protocol = protocol;

            this._millisecondsSendTimeout = millisecondsSendTimeout;
            this._millisecondsReceiveTimeout = millisecondsReceiveTimeout;

            this._pendingQueue = new PendingSendQueue(this, millisecondsSendTimeout);
        }
        #endregion

        #region Public Method

        public override void Start()
        {
            SocketConnector node = null;
            IPEndPoint remote = new IPEndPoint(this._remote, this._port);
            //注意 这里SocketConnector的第三个参数不能是OnConnected 否则会死循环
            node = new SocketConnector("default", remote, this, base.RegisterConnection, OnDisconnected);
            node.Start();
            autoEvent.WaitOne();
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// 发送超时毫秒数
        /// </summary>
        public int MillisecondsSendTimeout
        {
            get { return this._millisecondsSendTimeout; }
        }
        /// <summary>
        /// 接收超时毫秒数
        /// </summary>
        public int MillisecondsReceiveTimeout
        {
            get { return this._millisecondsReceiveTimeout; }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// OnResponse
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="response"></param>
        protected virtual void OnResponse(IConnection connection, Object response)
        {
            try { this._handler.OnReceived(connection, response); }
            catch (Exception ex) { Log.Trace.Error(ex.Message, ex); }
        }
        /// <summary>
        /// on request send success
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="request"></param>
        protected virtual void OnSendSucess(IConnection connection, Request request)
        {
       
        }
        /// <summary>
        /// on request send failed
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="request"></param>
        protected virtual void OnSendFailed(IConnection connection, Request request)
        {
            //this.Send(request);
            //connection.BeginSend(request);
        }
        /// <summary>
        /// on request send timeout
        /// </summary>
        /// <param name="request"></param>
        protected virtual void OnSendTimeout(Request request)
        {
        }

        /// <summary>
        /// send request
        /// </summary>
        /// <param name="request"></param>
        public abstract void Send(Object obj);

        /// <summary>
        /// send packet
        /// </summary>
        /// <param name="obj"></param>
        public abstract void SendRequest(Request request);
        /// <summary>
        /// enqueue to pending queue
        /// </summary>
        /// <param name="request"></param>
        protected void EnqueueToPendingQueue(Request request)
        {
            this._pendingQueue.Enqueue(request);
        }
        /// <summary>
        /// dequeue from pending queue
        /// </summary>
        /// <returns></returns>
        protected Request DequeueFromPendingQueue()
        {
            return this._pendingQueue.Dequeue();
        }
        /// <summary>
        /// dequeue all from pending queue.
        /// </summary>
        /// <returns></returns>
        protected Request[] DequeueAllFromPendingQueue()
        {
            return this._pendingQueue.DequeueAll();
        }
        #endregion

        #region Override Methods
        /// <summary>
        /// OnConnected
        /// </summary>
        /// <param name="connection"></param>
        protected override void OnConnected(IConnection connection)
        {
            autoEvent.Set();
            connection.BeginReceive();//异步开始接收数据
            //触发事件
            try { this._handler.OnConnected(connection); }
            catch (Exception ex) { Log.Trace.Error(ex.Message, ex); }
        }
        /// <summary>
        /// OnStartSending
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="packet"></param>
        protected override void OnStartSending(IConnection connection, Packet packet)
        {
            base.OnStartSending(connection, packet);
            try { this._handler.OnStartSending(connection,packet); }
            catch (Exception ex) { Log.Trace.Error(ex.Message, ex); }
        }
        /// <summary>
        /// OnSendCallback
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="e"></param>
        protected override void OnSendCallback(IConnection connection, SendCallbackEventArgs e)
        {
            base.OnSendCallback(connection, e);

            try { this._handler.OnSendCallback(connection,e); }
            catch (Exception ex) { Log.Trace.Error(ex.Message, ex); }

            var request = e.Packet as Request;
            if (request == null) return;

            if (e.Status == SendCallbackStatus.Success)
            {
                request.CurrConnection = connection;
                request.SentTime = DateTime.UtcNow;
                this.OnSendSucess(connection, request);
                return;
            }

            request.CurrConnection = null;
            request.SentTime = DateTime.MaxValue;

            if (DateTime.UtcNow.Subtract(request.BeginTime).TotalMilliseconds < this._millisecondsSendTimeout)
            {
                this.OnSendFailed(connection, request);
                return;
            }

            //time out
            this.OnSendTimeout(request);

            ThreadPool.QueueUserWorkItem(_ =>
            {
                var rex = new RequestException(RequestException.Errors.PendingSendTimeout);
                try { request.SetException(rex); }
                catch (Exception ex) { Log.Trace.Error(ex.Message, ex); }
            });
        }
        /// <summary>
        /// OnMessageReceived
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="e"></param>
        protected override void OnMessageReceived(IConnection connection, MessageReceivedEventArgs e)
        {
            base.OnMessageReceived(connection, e);

            int readlength;
            Object response = null;
            ReceivePacket packet = null;
            try
            {
                packet = this._protocol.TryToTranslateMessage(connection, e.Buffer,DefaultConfigure.MaxMessageSize, out readlength);
            }
            catch (Exception ex)
            {
                this.OnError(connection, ex);
                connection.BeginDisconnect(ex);
                e.SetReadlength(e.Buffer.Count);
                return;
            }

            if (response != null)
            {
                this.OnResponse(connection, response);
            }
            e.SetReadlength(readlength);
        }
        /// <summary>
        /// OnDisconnected
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        protected override void OnDisconnected(IConnection connection, Exception e)
        {
            base.OnDisconnected(connection, e);
            try { this._handler.OnDisconnected(connection, e); }
            catch (Exception ex) { Log.Trace.Error(ex.Message, ex); }
        }

        protected override void OnError(IConnection connection, Exception e)
        {
            base.OnError(connection, e);
            try { this._handler.OnException(connection, e); }
            catch (Exception ex) { Log.Trace.Error(ex.Message, ex); }
        }
        #endregion

        #region Class.PendingSendQueue
        /// <summary>
        /// pending send queue
        /// </summary>
        private class PendingSendQueue
        {
            #region Private Members
            private readonly BaseSocketClient _client = null;

            private readonly int _timeout;
            private readonly Timer _timer = null;
            private readonly ConcurrentQueue<Request> _queue = new ConcurrentQueue<Request>();
            #endregion

            #region Constructors
            /// <summary>
            /// new
            /// </summary>
            ~PendingSendQueue()
            {
                this._timer.Change(Timeout.Infinite, Timeout.Infinite);
                this._timer.Dispose();
            }
            /// <summary>
            /// new
            /// </summary>
            /// <param name="client"></param>
            /// <param name="millisecondsSendTimeout"></param>
            public PendingSendQueue(BaseSocketClient client, int millisecondsSendTimeout)
            {
                this._client = client;
                this._timeout = millisecondsSendTimeout;
                this._timer = new Timer(_ =>
                {
                    this._timer.Change(Timeout.Infinite, Timeout.Infinite);
                    this.Loop();
                    this._timer.Change(1000, 0);
                }, null, 1000, 0);
            }
            #endregion

            #region Public Methods
            /// <summary>
            /// 入列
            /// </summary>
            /// <param name="request"></param>
            /// <exception cref="ArgumentNullException">request is null</exception>
            public void Enqueue(Request request)
            {
                if (request == null) throw new ArgumentNullException("request");
                this._queue.Enqueue(request);
            }
            /// <summary>
            /// dequeue
            /// </summary>
            /// <returns></returns>
            public Request Dequeue()
            {
                Request request;
                if (this._queue.TryDequeue(out request)) return request;
                return null;
            }
            /// <summary>
            /// 出列全部
            /// </summary>
            /// <returns></returns>
            public Request[] DequeueAll()
            {
                int count = this._queue.Count;
                List<Request> list = null;
                while (count-- > 0)
                {
                    Request request;
                    if (this._queue.TryDequeue(out request))
                    {
                        if (list == null) list = new List<Request>();
                        list.Add(request);
                    }
                    else break;
                }

                if (list != null) return list.ToArray();
                return new Request[0];
            }
            #endregion

            #region Private Methods
            /// <summary>
            /// loop
            /// </summary>
            private void Loop()
            {
                var dtNow = DateTime.UtcNow;
                List<Request> listSend = null;
                List<Request> listTimeout = null;

                int count = this._queue.Count;
                while (count-- > 0)
                {
                    Request request;
                    if (this._queue.TryDequeue(out request))
                    {
                        if (dtNow.Subtract(request.BeginTime).TotalMilliseconds < this._timeout)
                        {
                            if (listSend == null) listSend = new List<Request>();
                            listSend.Add(request); continue;
                        }

                        if (listTimeout == null) listTimeout = new List<Request>();
                        listTimeout.Add(request);
                    }
                    else break;
                }

                if (listSend != null)
                {
                    //send 函数实际上是调用的DefaultSocketClient的Send函数
                    //for (int i = 0, l = listSend.Count; i < l; i++) this._client.Send(listSend[i]);
                    for (int i = 0, l = listSend.Count; i < l; i++) this._client.SendRequest(listSend[i]);
                }

                if (listTimeout != null)
                {
                    for (int i = 0, l = listTimeout.Count; i < l; i++)
                    {
                        var r = listTimeout[i];
                        this._client.OnSendTimeout(r);
                        ThreadPool.QueueUserWorkItem(_ =>
                        {
                            try { r.SetException(new RequestException(RequestException.Errors.PendingSendTimeout)); }
                            catch (Exception ex) { Log.Trace.Error(ex.Message, ex); }
                        });
                    }
                }
            }
            #endregion
        }
        #endregion
    }
}