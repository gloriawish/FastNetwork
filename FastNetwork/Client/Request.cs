using System;

namespace FastNetwork.Client
{
    /// <summary>
    /// request
    /// </summary>
    public class Request : Packet
    {
        #region Members

        /// <summary>
        /// get or set receive time out
        /// </summary>
        public int MillisecondsReceiveTimeout;

        /// <summary>
        /// connectionID
        /// </summary>
        internal IConnection CurrConnection = null;
        /// <summary>
        /// sent time
        /// </summary>
        internal DateTime SentTime = DateTime.MaxValue;

        /// <summary>
        /// 异常回调
        /// </summary>
        private readonly Action<Exception> _onException = null;
        #endregion

        #region Constructors
        /// <summary>
        /// new
        /// </summary>
        /// <param name="seqID">seqID</param>
        /// <param name="cmdName">command name</param>
        /// <param name="payload">发送内容</param>
        /// <param name="onException">异常回调</param>
        /// <exception cref="ArgumentNullException">onException is null</exception>
        /// <exception cref="ArgumentNullException">onResult is null</exception>
        public Request(byte[] payload, Action<Exception> onException)
            : base(payload)
        {
            if (onException == null) throw new ArgumentNullException("onException");
            this._onException = onException;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// set Exception
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public bool SetException(Exception ex)
        {
            this._onException(ex);
            return true;
        }
        #endregion
    }
}