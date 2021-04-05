using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SecretNest.Hardware.VirtualKeyboard
{
    public class VirtualKeyboardHelper : IDisposable
    {
        private readonly VirtualKeyboard _keyboard;
        private readonly ManualResetEvent _lock = new ManualResetEvent(false);
        private readonly KeySimulator _up, _down, _left, _right;
        private object _syncLock;

        public VirtualKeyboardHelper(VirtualKeyboard keyboard, int dueTime, int period, object syncLock = null)
        {
            _keyboard = keyboard;
            _syncLock = syncLock ?? new object();
            _up = new KeySimulator(dueTime, period);
            _down = new KeySimulator(dueTime, period);
            _left = new KeySimulator(dueTime, period);
            _right = new KeySimulator(dueTime, period);
            _up.KeyPressed += SendUp;
            _down.KeyPressed += SendDown;
            _left.KeyPressed += SendLeft;
            _right.KeyPressed += SendRight;
        }

        public void StartAndBlock(string initialText, bool allowEnter)
        {
            _keyboard.Start(initialText, allowEnter);
            _keyboard.VirtualKeyboardResult += Keyboard_VirtualKeyboardResult;
            _lock.Reset();
            _lock.WaitOne();
            _keyboard.VirtualKeyboardResult -= Keyboard_VirtualKeyboardResult;
        }

        private void SendUp(object sender, EventArgs e)
        {
            lock (_syncLock)
            {
                _keyboard.Up();
            }
        }

        private void SendDown(object sender, EventArgs e)
        {
            lock (_syncLock)
            {
                _keyboard.Down();
            }
        }

        private void SendLeft(object sender, EventArgs e)
        {
            lock (_syncLock)
            {
                _keyboard.Left();
            }
        }

        private void SendRight(object sender, EventArgs e)
        {
            lock (_syncLock)
            {
                _keyboard.Up();
            }
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
                    _up.KeyPressed -= SendUp;
                    _down.KeyPressed -= SendDown;
                    _left.KeyPressed -= SendLeft;
                    _right.KeyPressed -= SendRight;
                    _lock.Set();
                    _lock.Dispose();
                    _up.Dispose();
                    _down.Dispose();
                    _left.Dispose();
                    _right.Dispose();
                }

                _syncLock = null;
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
