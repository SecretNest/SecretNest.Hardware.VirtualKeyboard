using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace SecretNest.Hardware.VirtualKeyboard
{
    class KeySimulator : IDisposable
    {
        private readonly Timer _timer;
        private bool _disposedValue;

        private readonly int _dueTime;
        private readonly int _period;

        public KeySimulator(int dueTime, int period)
        {
            _dueTime = dueTime;
            _period = period;
            _timer = new Timer(Timer);
        }

        private protected virtual void Timer(object state)
        {
            KeyPressed?.Invoke(this, EventArgs.Empty);
        }

        public virtual void Start()
        {
            Timer(null);
            _timer.Change(_dueTime, _period);
        }

        public void Stop()
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public event EventHandler KeyPressed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _timer.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    class KeySimulator<T> : KeySimulator
    {
        private readonly T _state;

        public KeySimulator(T state, int dueTime, int period) : base(dueTime, period)
        {
            _state = state;
        }

        public override void Start()
        {
            Timer(_state);
            base.Start();
        }

        private protected override void Timer(object state)
        {
            KeyPressedWithState?.Invoke(this, new KeyPressedEventArgs<T>((T) _state));
        }

        public event EventHandler<KeyPressedEventArgs<T>> KeyPressedWithState;

    }

    class KeyPressedEventArgs<T> : EventArgs
    {
        public T State { get; }

        public KeyPressedEventArgs(T state)
        {
            State = state;
        }
    }
}
