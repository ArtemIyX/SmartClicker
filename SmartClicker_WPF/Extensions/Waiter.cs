using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SmartClicker_WPF.Extensions
{
    internal class Waiter
    {
        private readonly IWebDriver _webDriver;
        private readonly int _timeOut;
        private readonly Func<IWebDriver, IWebElement?> _condition;

        private CancellationTokenSource? _cancellationTokenSource;
        private CancellationToken _cancellationToken;
        private Task<IWebElement>? _task;

        public int SleepingDelayMs { get; set; } = 500;
        public Waiter(IWebDriver webDriver, int timeOutS, Func<IWebDriver, IWebElement?> condition)
        {
            _webDriver = webDriver;
            _timeOut = timeOutS;
            _condition = condition;
        }

        public async Task<IWebElement?> Wait()
        {
            //Check if already in proccess
            if(_task != null)
            {
                if(_task.Status == TaskStatus.Running)
                {
                    throw new Exception("Already in the process of waiting");
                }
            }

            //Init cancelation
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancellationTokenSource.Token;
            //Cancel after timeout
            _cancellationTokenSource.CancelAfter(_timeOut * 1000);
            //Run finding task
            _task = RunMe();

            IWebElement? el;
            //If all ok element will be found element
            try
            {
                el = await _task;
            }
            //But if it is error (time out exception for example)
            //Element will be null
            catch 
            {
                el = null;
            }
            //Clear data
            _task = null;
            _cancellationTokenSource = null;
            //Return result
            return el;
        }

        private Task<IWebElement> RunMe()
        {
            return Task.Run(() =>
            {
                if (_condition == null)
                {
                    throw new ArgumentNullException("condition", "condition cannot be null");
                }
                while (true)
                {
                    if (_cancellationToken.IsCancellationRequested)
                    {
                        //string text = $"Timed out after {_timeOut} seconds";
                        _cancellationToken.ThrowIfCancellationRequested();
                    }
                    IWebElement? element = _condition(_webDriver);
                    if(element != null)
                    {
                        return element;
                    }
                    Task.Delay(SleepingDelayMs);
                }
            });
            
        }
    }
}
