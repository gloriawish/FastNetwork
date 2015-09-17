using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastNetwork.Protocol
{
    public interface IDecoder
    {
        Object decode(IConnection connection, byte[] buffer);
    }
}
