using FastNetwork;
using FastNetwork.Protocol;
using FastNetwork.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
    public class DefaultDecoder : IDecoder
    {

        object IDecoder.decode(IConnection connection, byte[] buffer)
        {
            try
            {
                int namelen = NetworkBitConverter.ToInt32(buffer, 0);

                string name = Encoding.Default.GetString(buffer, 4, namelen);

                int age = NetworkBitConverter.ToInt32(buffer, 4 + namelen);

                return new UserInfo(name, age);
            }
            catch (Exception)
            {
                return new UserInfo("error", 0);
            }
        }
    }
}
