using OpenQA.Selenium;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SmartClicker_WPF.Extensions
{
    public class Waiter
    {
        private readonly IWebDriver _webDriver;
        private readonly int _timeOut;
        private readonly Func<IWebDriver, IWebElement?> _condition;

        private CancellationTokenSource? _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private Task<IWebElement>? _task;

        private int SleepingDelayMs { get; set; } = 500;
        public Waiter(IWebDriver webDriver, int timeOutS, Func<IWebDriver, IWebElement?> condition)
        {
            _webDriver = webDriver;
            _timeOut = timeOutS;
            _condition = condition;
        }

        public async Task<IWebElement?> Wait(CancellationToken ct)
        {
            //Check if already in proccess
            if (_task != null)
            {
                if (_task.Status == TaskStatus.Running)
                {
                    throw new Exception("Already in the process of waiting");
                }
            }

            if (_condition == null)
            {
                throw new ArgumentNullException(nameof(ct), "condition cannot be null");
            }

            //Init cancelation
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            //Cancel after timeout
            _cancellationTokenSource.CancelAfter(_timeOut * 1000);
            //Run finding task

            //If all ok element will be found element
            IWebElement? el;
            try
            {
                _task = Task.Run(() =>
                {
                    while (true)
                    {
                        //Main cancel
                        ct.ThrowIfCancellationRequested();
                        //Time out cancel
                        _cancellationToken.ThrowIfCancellationRequested();
                        IWebElement? element = _condition(_webDriver);
                        if (element != null)
                        {
                            return element;
                        }
                        Task.Delay(SleepingDelayMs, ct);
                    }
                }, ct);
                el = await _task;
            }
            //But if it is error (time out exception for example)
            //Element will be null
            catch
            {
                el = null;
            }
            //Clear data

            _cancellationTokenSource = null;
            //Return result
            return el;
        }

    }
}
