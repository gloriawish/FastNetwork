using FastNetwork;
using FastNetwork.Protocol;
using FastNetwork.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    public class DefaultEncoder : IEncoder
    {
        public byte[] encode(IConnection connection, object obj)
        {
            UserInfo msg = (UserInfo)obj;
            byte[] name = Encoding.Default.GetBytes(msg.username);

            byte[] age = NetworkBitConverter.GetBytes(msg.age);

            byte[] data = new byte[4 + name.Length + 4];

            Buffer.BlockCopy(NetworkBitConverter.GetBytes(name.Length), 0, data, 0, 4);

            Buffer.BlockCopy(name, 0, data, 4, name.Length);

            Buffer.BlockCopy(age, 0, data, 4 + name.Length, 4);

            return data;
        }
    }
}
