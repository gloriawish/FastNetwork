using FastNetwork.Utils;
using System;
using System.Text;

namespace FastNetwork.Protocol
{
    /// <summary>
    /// 异步二进制协议
    /// 协议格式
    /// [Message Length(int32)][SeqID(int32)][Body Buffer]
    /// </summary>
    public sealed class DefaultBinaryProtocol : IProtocol
    {
        #region IProtocol Members
        /// <summary>
        /// find command
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="buffer"></param>
        /// <param name="maxMessageSize"></param>
        /// <param name="readlength"></param>
        /// <returns></returns>
        /// <exception cref="BadProtocolException">bad async binary protocl</exception>
        public ReceivePacket TryToTranslateMessage(IConnection connection, ArraySegment<byte> buffer,
            int maxMessageSize, out int readlength)
        {
            if (buffer.Count < 4) { readlength = 0; return null; }

            var payload = buffer.Array;

            //获取message length
            var messageLength = Utils.NetworkBitConverter.ToInt32(payload, buffer.Offset);
            if (messageLength < 4) throw new BadProtocolException("bad async binary protocl");
            if (messageLength > maxMessageSize) throw new BadProtocolException("message is too long");

            readlength = messageLength + 4;
            if (buffer.Count < readlength)
            {
                readlength = 0; return null;
            }
            byte[] data = null;
            if (messageLength > 0)
            {
                data = new byte[messageLength];
                Buffer.BlockCopy(payload, buffer.Offset + 4, data, 0, messageLength);
            }
            //TODO 如何返回一个数据
            return new ReceivePacket(data);
        }
        #endregion
    }
}