using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Coleseus.Shared.App.Impl
{
    public abstract class AbstractScheduleTask : ScheduleTask
    {
        private readonly ILogger _logger = Serilog.Log.Logger.ForContext<AbstractScheduleTask>();

        private CancellationTokenSource _cancellationTokenSource;
        private Task _task;

        public Object Id { get; set; }
        public string TaskName { get; set; }
        public TimeSpan TaskTimeSpan { get; set; }
        public bool TaskRunAtStart { get; set; }

        public TimeSpan InitialDelay { get; set; }

        public abstract void Execute();

        public void Start()
        {
            if (_task != null)
            {
                _logger.Error($"Task {TaskName} already started");
                return;
            }


            _cancellationTokenSource = new CancellationTokenSource();
            _task = new Task(Run, _cancellationTokenSource.Token);
            _task.Start();
        }

        public void Stop()
        {
            if (_task == null)
            {
                _logger.Error($"Task {TaskName} already stopped");
                return;
            }

            _cancellationTokenSource.Cancel();
            _task = null;
        }

        private async void Run()
        {
            _logger.Debug($"Task {TaskName} delay");
            await Task.Delay(InitialDelay, _cancellationTokenSource.Token);
            _logger.Debug($"Task {TaskName} started");
            if (TaskRunAtStart)
            {
                _logger.Verbose($"Task {TaskName} run");
                ExecuteUserCode();
                _logger.Verbose($"Task {TaskName} completed");
            }

            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TaskTimeSpan, _cancellationTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    _logger.Debug($"Task {TaskName} canceled");
                }

                if (!_cancellationTokenSource.Token.IsCancellationRequested)
                {
                    _logger.Verbose($"Task {TaskName} run");
                    ExecuteUserCode();
                    _logger.Verbose($"Task {TaskName} completed");
                }
            }

            _logger.Debug($"Task {TaskName} ended");
        }

        private void ExecuteUserCode()
        {
            try
            {
                Execute();
            }
            catch (Exception ex)
            {
                _logger.Error($"Task {TaskName} crashed.  Stopping Task.\n Exception: {ex}");
                Stop();
            }
        }





    }
}
