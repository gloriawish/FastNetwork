using System;

namespace FastNetwork
{
    /// <summary>
    /// udp service interface.
    /// </summary>
    /// <typeparam name="TCommandInfo"></typeparam>
    public interface IUdpServerHandler
    {
        /// <summary>
        /// OnReceived
        /// </summary>
        /// <param name="session"></param>
        /// <param name="cmdInfo"></param>
        void OnReceived(UdpSession session, Object obj);
        /// <summary>
        /// OnError
        /// </summary>
        /// <param name="session"></param>
        /// <param name="ex"></param>
        void OnError(UdpSession session, Exception ex);
    }
}