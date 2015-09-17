using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastNetwork.Protocol
{
    public interface IEncoder
    {

        byte[] encode(IConnection connection, Object msg);

    }
}
