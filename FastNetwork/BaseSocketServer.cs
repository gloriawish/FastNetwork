using System.Net;

namespace FastNetwork
{
    /// <summary>
    /// abstract socket server
    /// </summary>
    public abstract class BaseSocketServer : BaseHost
    {
        /// <summary>
        /// new
        /// </summary>
        /// <param name="socketBufferSize"></param>
        /// <param name="messageBufferSize"></param>
        protected BaseSocketServer(int socketBufferSize, int messageBufferSize)
            : base(socketBufferSize, messageBufferSize)
        {
        }

        /// <summary>
        /// add socket listener
        /// </summary>
        /// <param name="name"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public abstract ISocketListener AddListener(string name, IPEndPoint endPoint);
    }
}