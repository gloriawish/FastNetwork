using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastNetwork
{
    /// <summary>
    /// 系统默认值
    /// </summary>
    public class DefaultConfigure
    {

        public static readonly int SocketBufferSize = 8192;

        public static readonly int MessageBufferSize = 8192;

        public static readonly int MaxMessageSize = 102400;

        public static readonly int MaxConnections = 20000;

    }
}
