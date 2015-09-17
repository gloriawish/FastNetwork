using System;

namespace FastNetwork.Protocol
{
    /// <summary>
    /// 协议接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IProtocol
    {
        /// <summary>
        /// Find CommandInfo
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="buffer"></param>
        /// <param name="maxMessageSize"></param>
        /// <param name="readlength"></param>
        /// <returns></returns>
        ReceivePacket TryToTranslateMessage(IConnection connection, ArraySegment<byte> buffer,
            int maxMessageSize, out int readlength);
    }
}