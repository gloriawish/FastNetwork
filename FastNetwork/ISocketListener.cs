using System;
using System.Net;

namespace FastNetwork
{
    /// <summary>
    /// socket listener
    /// </summary>
    public interface ISocketListener
    {
        /// <summary>
        /// socket accepted event
        /// </summary>
        event Action<ISocketListener, IConnection> Accepted;

        /// <summary>
        /// get name
        /// </summary>
        string Name { get; }
        /// <summary>
        /// get endpoint
        /// </summary>
        EndPoint EndPoint { get; }
        /// <summary>
        /// start listen
        /// </summary>
        void Start();
        /// <summary>
        /// stop listen
        /// </summary>
        void Stop();
    }
}