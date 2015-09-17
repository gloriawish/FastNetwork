using System;
using System.Net;
using System.Net.Sockets;

namespace FastNetwork
{
    /// <summary>
    /// 连接器
    /// </summary>
    public sealed class SocketConnector
    {
        #region Members
        /// <summary>
        /// get node name
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// get node endpoint
        /// </summary>
        private readonly EndPoint EndPoint;
        /// <summary>
        /// get node owner host
        /// </summary>
        private readonly IHost Host = null;

        private Action<IConnection> _onConnected;
        private Action<IConnection,Exception> _onDisconnected;
        private volatile bool _isStop = false;
        #endregion

        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        /// <param name="name"></param>
        /// <param name="endPoint"></param>
        /// <param name="host"></param>
        /// <param name="onConnected"></param>
        /// <param name="onDisconnected"></param>
        public SocketConnector(string name,
            EndPoint endPoint,
            IHost host,
            Action<IConnection> onConnected,
            Action<IConnection,Exception> onDisconnected)
        {
            this.Name = name;
            this.EndPoint = endPoint;
            this.Host = host;
            this._onConnected = onConnected;
            this._onDisconnected = onDisconnected;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// start
        /// </summary>
        public void Start()
        {
            BeginConnect(this.EndPoint, this.Host, connection =>
            {
                if (this._isStop)
                {
                    if (connection != null) connection.BeginDisconnect(); return;
                }
                if (connection == null)
                {
                    Utils.TaskEx.Delay(new Random().Next(1500, 3000), this.Start); return;
                }
                connection.Disconnected += this.OnDisconnected;
                this._onConnected(connection);
            });
        }
        /// <summary>
        /// stop
        /// </summary>
        public void Stop()
        {
            this._isStop = true;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        private void OnDisconnected(IConnection connection, Exception ex)
        {
            connection.Disconnected -= this.OnDisconnected;
            //delay reconnect 20ms ~ 200ms
            if (!this._isStop) Utils.TaskEx.Delay(new Random().Next(20, 200), this.Start);
            //fire disconnected event
            this._onDisconnected(connection,ex);
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// begin connect
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="host"></param>
        /// <param name="callback"></param>
        /// <exception cref="ArgumentNullException">endPoint is null</exception>
        /// <exception cref="ArgumentNullException">host is null</exception>
        /// <exception cref="ArgumentNullException">callback is null</exception>
        static public void BeginConnect(EndPoint endPoint, IHost host, Action<IConnection> callback)
        {
            if (endPoint == null) throw new ArgumentNullException("endPoint");
            if (host == null) throw new ArgumentNullException("host");
            if (callback == null) throw new ArgumentNullException("callback");

            Log.Trace.Debug(string.Concat("begin connect to ", endPoint.ToString()));

            var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.BeginConnect(endPoint, ar =>
                {
                    try
                    {
                        socket.EndConnect(ar);
                        socket.NoDelay = true;
                        socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                        socket.ReceiveBufferSize = host.SocketBufferSize;
                        socket.SendBufferSize = host.SocketBufferSize;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            socket.Close();
                            socket.Dispose();
                        }
                        catch { }

                        Log.Trace.Error(ex.Message, ex);
                        callback(null); return;
                    }

                    callback(new DefaultConnection(host.NextConnectionID(), socket, host));
                }, null);
            }
            catch (Exception ex)
            {
                Log.Trace.Error(ex.Message, ex);
                callback(null);
            }
        }
        #endregion
    }
}