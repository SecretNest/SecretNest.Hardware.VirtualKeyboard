using System;
using System.Collections.Generic;
using System.Text;

namespace SecretNest.Hardware.VirtualKeyboard
{
    class VirtualKeyboardTextDisplayBar
    {
        //private readonly int _screenWidth;
        private readonly int _screenWidthMinusOne;
        private readonly int _screenWidthMinusTwo;
        private readonly int _pageLeadingLength;
        private readonly int _screenWidthMinusPageLeadingMinusOne;

        public VirtualKeyboardTextDisplayBar(int screenWidth, int pageLeadingLength = 4)
        {
            //_screenWidth = screenWidth;
            _pageLeadingLength = pageLeadingLength;
            _screenWidthMinusOne = screenWidth - 1;
            _screenWidthMinusTwo = screenWidth - 2;
            _screenWidthMinusPageLeadingMinusOne = _screenWidthMinusOne - pageLeadingLength;
        }

        private string _text;
        private int _index;

        //Must be set before using
        public string Text
        {
            get => _text;
            set
            {
                _text = value ?? string.Empty;
                _index = _text.Length;
                Refresh();
            }
        }

        public int Index
        {
            get => _index;
            set
            {
                if (value < 0)
                    _index = 0;
                else
                {
                    var max = _text.Length;
                    _index = value > max ? max : value;
                }
                Refresh();
            }
        }

        public string DisplayText { get; private set; }
        public int DisplayIndex { get; private set; }

        public event EventHandler<VirtualKeyboardTextDisplayBarChangedEventArgs> DisplayBarChanged;

        public void MoveLeft()
        {
            Index--;
            Refresh();
        }

        public void MoveRight()
        {
            Index++;
            Refresh();
        }

        public void EnterKey(char key)
        {
            _text = _text.Substring(0, _index) + key + _text.Substring(_index);
            _index++;
            Refresh();
        }

        public void Backspace()
        {
            if (_index == 0)
                return;
            if (_index == _text.Length)
            {
                _text = _text.Substring(0, _index - 1);
            }
            else
            {
                _text = _text.Substring(0, _index - 1) + _text.Substring(_index);
            }
            _index--;
            Refresh();
        }

        private const string BarLeading = "<";
        private const string BarTailing = ">";
        void ChangeDisplayBar(string displayText, int displayIndex)
        {
            DisplayText = displayText;
            DisplayIndex = displayIndex;
            DisplayBarChanged?.Invoke(this, new VirtualKeyboardTextDisplayBarChangedEventArgs(displayText, displayIndex));
        }

        void Refresh()
        {
            var textLength = _text.Length;
            if (textLength == 0)
            {
                //empty
                ChangeDisplayBar(string.Empty, 0);
            }
            else if (textLength <= _screenWidthMinusOne)
            {
                //full
                ChangeDisplayBar(_text, _index);
            }
            else
            {
                //paging mode

                //locate current page
                if (_index <= _screenWidthMinusOne)
                {
                    //page 1
                    ChangeDisplayBar(_text.Substring(0, _screenWidthMinusOne) + BarTailing, _index);
                }
                else
                {
                    //other page
                    /*
                     *      0, [w-1], w-2 >
                     *      < 1*(w-l-1), [w-2], w+1*(w-l-1)-2 >
                     *      < 2*(w-l-1), [w-2], w+2*(w-l-1)-2 >
                     *      < 3*(w-l-1), [w-2], w+3*(w-l-1)-2 >
                     *
                     *      < n*(w-l-1), [w-2], w+n*(w-l-1)-2 >   
                     */

                    int page = (_index - _pageLeadingLength) / _screenWidthMinusPageLeadingMinusOne;
                    int startIndex = page * _screenWidthMinusPageLeadingMinusOne;
                    int endIndex = startIndex + _screenWidthMinusTwo;
                    bool isLastPage = textLength <= endIndex;

                    string displayText;
                    if (isLastPage)
                        displayText = BarLeading + _text.Substring(startIndex);
                    else
                        displayText = BarLeading + _text.Substring(startIndex, _screenWidthMinusTwo) + BarTailing;
                    ChangeDisplayBar(displayText, _index - startIndex + 1);
                }
            }
        }
    }

    public class VirtualKeyboardTextDisplayBarChangedEventArgs : EventArgs
    {
        public VirtualKeyboardTextDisplayBarChangedEventArgs(string displayText, int displayIndex)
        {
            DisplayText = displayText;
            DisplayIndex = displayIndex;
        }

        public string DisplayText { get; }
        public int DisplayIndex { get; }
    }
}
