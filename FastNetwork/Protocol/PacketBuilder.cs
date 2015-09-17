using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastNetwork.Protocol
{
    static public class PacketBuilder
    {

        #region ToAsyncBinary
        /// <summary>
        /// to async binary <see cref="SocketBase.Packet"/>
        /// </summary>
        /// <param name="responseFlag"></param>
        /// <param name="seqID"></param>
        /// <param name="buffer"></param>
        /// <returns></returns>
        static public Packet ToAsyncBinary(byte[] buffer)
        {
            var messageLength = (buffer == null ? 0 : buffer.Length) + 4;
            var sendBuffer = new byte[messageLength + 4];

            //write message length
            Buffer.BlockCopy(Utils.NetworkBitConverter.GetBytes(messageLength), 0, sendBuffer, 0, 4);
            //write body buffer
            if (buffer != null && buffer.Length > 0) 
                Buffer.BlockCopy(buffer, 0, sendBuffer, 4, buffer.Length);

            return new Packet(sendBuffer);
        }

        static public byte[] ToAsyncBinaryByte(byte[] buffer)
        {
            var messageLength = (buffer == null ? 0 : buffer.Length) + 4;
            var sendBuffer = new byte[messageLength + 4];

            //write message length
            Buffer.BlockCopy(Utils.NetworkBitConverter.GetBytes(messageLength), 0, sendBuffer, 0, 4);
            //write body buffer
            if (buffer != null && buffer.Length > 0)
                Buffer.BlockCopy(buffer, 0, sendBuffer, 4, buffer.Length);

            return sendBuffer;
        }
        #endregion
    }
}
