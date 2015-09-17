using System;
using System.Net;

namespace FastNetwork.Protocol
{
    /// <summary>
    /// a upd protocol
    /// </summary>
    /// <typeparam name="TCommandInfo"></typeparam>
    public interface IUdpProtocol
    {
        /// <summary>
        /// find command info
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        Object FindCommandInfo(ArraySegment<byte> buffer);
    }
}