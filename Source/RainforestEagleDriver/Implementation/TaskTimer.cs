using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Elve.Driver.RainforestEagle.Implementation
{
    /// <summary>
    /// Scheduler class for repeatedly invoking a Non-Reentrant action.
    /// </summary>
    internal sealed class TaskTimer : IDisposable
    {
        #region Private Fields

        private readonly CancellationToken _cancellationToken;
        private readonly Func<CancellationToken, Task> _task;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private readonly Timer _timer;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskTimer" /> class.
        /// </summary>
        /// <param name="task">The task to be executed..</param>
        /// <param name="dueTime">The amount of time to delay before <paramref name="task" /> is invoked, in milliseconds. Specify zero (0) to start the timer immediately.</param>
        /// <param name="period">The time interval between invocations of <paramref name="task" /> once ScheduleNext() method is invoked.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public TaskTimer(Func<CancellationToken, Task> task, long dueTime, long period, CancellationToken cancellationToken)
        {
            Interval = period;
            _task = task;
            _cancellationToken = cancellationToken;
            _timer = new Timer(TimerCallback, null, dueTime, Timeout.Infinite);

            // Register Dispose on Cancellation to stop the timer from firing
            cancellationToken.Register(Dispose);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskTimer" /> class.
        /// </summary>
        /// <param name="task">The task to be executed..</param>
        /// <param name="dueTime">The amount of time to delay before <paramref name="task" /> is invoked, in milliseconds. Specify TimeSpan.Zero to start the timer immediately.</param>
        /// <param name="period">The time interval between invocations of <paramref name="task" />, in milliseconds once ScheduleNext() method is invoked.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public TaskTimer(Func<CancellationToken, Task> task, TimeSpan dueTime, TimeSpan period, CancellationToken cancellationToken) : 
            this(task, (long) dueTime.TotalMilliseconds, (long) period.TotalMilliseconds, cancellationToken)
        {
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets the completed execution counter.
        /// </summary>
        public long Count { get; private set; }

        /// <summary>
        /// Gets or sets the interval. This can be changed at any time and will take effect after the current execution.
        /// </summary>
        public long Interval { get; set; }

        /// <summary>
        /// Gets the last exception thrown inside the action (if any).
        /// </summary>
        public Exception LastException { get; private set; }

        /// <summary>
        /// Gets the elapsed time since the action was most recently invoked.
        /// </summary>
        public TimeSpan ElapsedTime { get { return _stopwatch.Elapsed; } }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Disposes the <see cref="TaskTimer" /> class. This will stop any pending and future executions. 
        /// </summary>
        public void Dispose()
        {
            if (_timer != null)
                _timer.Dispose();
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Executes the task for the timer and schedules the next invocation.
        /// </summary>
        /// <param name="state">The state.</param>
        private void TimerCallback(object state)
        {
            if (_cancellationToken.IsCancellationRequested) return;
            _stopwatch.Reset();
            _stopwatch.Start();

            try
            {
                _task.Invoke(_cancellationToken)
                     .ContinueWith(t => ScheduleNext(), TaskContinuationOptions.None);
            }
            catch (Exception ex)
            {
                LastException = ex;
                ScheduleNext();
            }
        }

        /// <summary>
        /// Schedules the next invocation with optional ability to change interval.
        /// </summary>
        /// <returns></returns>
        private void ScheduleNext()
        {
            if (_cancellationToken.IsCancellationRequested) return;

            try
            {
                _timer.Change(Interval, Timeout.Infinite);
                Count++;
            }
            catch (ObjectDisposedException)
            {
            }
        }

        #endregion Private Methods
    }
}
