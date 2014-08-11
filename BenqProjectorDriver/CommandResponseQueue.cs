using System.Collections.Generic;

namespace Elve.Driver.BenqProjector
{
    internal class CommandResponseQueue<TCommand, TResponse>
    {
        #region Private Fields

        private readonly Queue<TCommand> _commandQ;
        private readonly Queue<TResponse> _responseQ;
        private readonly object _lock = new object();

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandResponseQueue{TCommand, TResponse}"/> class.
        /// </summary>
        /// <param name="maxCapacity">The maximum capacity.</param>
        public CommandResponseQueue(int maxCapacity)
        {
            _commandQ = new Queue<TCommand>(maxCapacity);
            _responseQ = new Queue<TResponse>(maxCapacity);
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the command count.
        /// </summary>
        /// <value>
        /// The command count.
        /// </value>
        public int CommandCount
        {
            get
            {
                lock (_lock)
                    return _commandQ.Count;
            }
        }

        /// <summary>
        /// Gets the response count.
        /// </summary>
        /// <value>
        /// The response count.
        /// </value>
        public int ResponseCount
        {
            get
            {
                lock (_lock)
                    return _responseQ.Count;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Clears this instance queues.
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _commandQ.Clear();
                _responseQ.Clear();
            }
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <returns></returns>
        public TCommand DequeueCommand()
        {
            lock (_lock)
                return _commandQ.Dequeue();
        }

        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <returns></returns>
        public TResponse DequeueResponse()
        {
            lock (_lock)
                return _responseQ.Dequeue();
        }

        /// <summary>
        /// Enqueues the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="response">The response.</param>
        public void Enqueue(TCommand command, TResponse response)
        {
            lock (_lock)
            {
                _commandQ.Enqueue(command);
                _responseQ.Enqueue(response);
            }
        }

        /// <summary>
        /// Determines whether [is command empty].
        /// </summary>
        /// <returns></returns>
        public bool IsCommandEmpty()
        {
            lock (_lock)
                return _commandQ.Count == 0;
        }
        /// <summary>
        /// Determines whether [is response empty].
        /// </summary>
        /// <returns></returns>
        public bool IsResponseEmpty()
        {
            lock (_lock)
                return _responseQ.Count == 0;
        }

        /// <summary>
        /// Peeks the command.
        /// </summary>
        /// <returns></returns>
        public TCommand PeekCommand()
        {
            lock (_lock)
                return _commandQ.Peek();
        }
        /// <summary>
        /// Peeks the response.
        /// </summary>
        /// <returns></returns>
        public TResponse PeekResponse()
        {
            lock (_lock)
                return _responseQ.Peek();
        }

        #endregion Public Methods
    }
}
