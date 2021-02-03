using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SecretNest.Hardware.VirtualKeyboard
{
    class VirtualKeyboardHelper : IDisposable
    {
        private readonly VirtualKeyboard _keyboard;
        private readonly AutoResetEvent _lock = new AutoResetEvent(false);
        private readonly KeySimulator _up, _down, _left, _right;

        public VirtualKeyboardHelper(VirtualKeyboard keyboard, int dueTime, int period)
        {
            _keyboard = keyboard;
            _up = new KeySimulator(dueTime, period);
            _down = new KeySimulator(dueTime, period);
            _left = new KeySimulator(dueTime, period);
            _right = new KeySimulator(dueTime, period);
            _up.KeyPressed += new EventHandler((sender, e) => keyboard.Up());
            _down.KeyPressed += new EventHandler((sender, e) => keyboard.Down());
            _left.KeyPressed += new EventHandler((sender, e) => keyboard.Left());
            _right.KeyPressed += new EventHandler((sender, e) => keyboard.Right());
        }

        public void StartAndBlock(string initialText, bool allowEnter)
        {
            _keyboard.Start(initialText, allowEnter);
            _keyboard.VirtualKeyboardResult += Keyboard_VirtualKeyboardResult;
            _lock.WaitOne();
        }

        private void Keyboard_VirtualKeyboardResult(object sender, VirtualKeyboardResultEventArgs e)
        {
            _lock.Set();
        }

        public void ReleaseBlock()
        {
            _lock.Set();
        }

        public void UpPressed()
        {
            _up.Start();
        }

        public void UpReleased()
        {
            _up.Stop();
        }

        public void DownPressed()
        {
            _down.Start();
        }

        public void DownReleased()
        {
            _down.Stop();
        }

        public void LeftPressed()
        {
            _left.Start();
        }

        public void LeftReleased()
        {
            _left.Stop();
        }
        
        public void RightPressed()
        {
            _right.Start();
        }

        public void RightReleased()
        {
            _right.Stop();
        }

        private bool _disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _lock.Set();
                    _lock.Dispose();
                    _up.Dispose();
                    _down.Dispose();
                    _left.Dispose();
                    _right.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
