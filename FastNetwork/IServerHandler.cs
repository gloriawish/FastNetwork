using FastNetwork.Event;
using FastNetwork.Protocol;
using System;

namespace FastNetwork
{
    /// <summary>
    /// 提供出来的处理函数接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IServerHandler
    {
        /// <summary>
        /// 当建立socket连接时，会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        void OnConnected(IConnection connection);
        /// <summary>
        /// 开始发送<see cref="Packet"/>
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="packet"></param>
        void OnStartSending(IConnection connection, Packet packet);
        /// <summary>
        /// 发送回调
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="e"></param>
        void OnSendCallback(IConnection connection, SendCallbackEventArgs e);
        /// <summary>
        /// 当接收到客户端新消息时，会调用此方法.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="cmdInfo"></param>
        void OnReceived(IConnection connection, Object obj);
        /// <summary>
        /// 当socket连接断开时，会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        void OnDisconnected(IConnection connection, Exception ex);
        /// <summary>
        /// 当发生异常时，会调用此方法
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="ex"></param>
        void OnException(IConnection connection, Exception ex);
    }
}