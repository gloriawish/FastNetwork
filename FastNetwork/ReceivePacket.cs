using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastNetwork
{
    /// <summary>
    /// 表示收到的完整的一个消息
    /// </summary>
    public class ReceivePacket
    {

        public ReceivePacket(byte[] data)
        {
            this.Buffer = data;
        }

        #region Public Properties
        /// <summary>
        /// 主体内容
        /// </summary>
        public byte[] Buffer
        {
            get;
            private set;
        }
        #endregion
    }
}
