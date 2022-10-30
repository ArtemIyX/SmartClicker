using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartClicker_WPF.Services
{
    public class ActivityMaker
    {
        private readonly WebDriver _driver;
        private CancellationTokenSource _cancelTokenSource;
        private CancellationToken _cancellationToken;
        public ActivityMaker(WebDriver webDriver)
        {
            _driver = webDriver;
        }

        public async Task DoActivityFor(int seconds)
        {
            _cancelTokenSource = new CancellationTokenSource();
            _cancellationToken = _cancelTokenSource.Token;
            _cancelTokenSource.CancelAfter(seconds * 1000);
            while (true)
            {
                if (_cancellationToken.IsCancellationRequested)
                {
                    _cancellationToken.ThrowIfCancellationRequested();
                }
                await Task.Delay(500);
            }
        }
    }
}
