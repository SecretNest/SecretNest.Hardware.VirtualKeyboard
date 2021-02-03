using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using SecretNest.Hardware.Text;
using SecretNest.Hardware.VirtualKeyboard.Properties;

namespace SecretNest.Hardware.VirtualKeyboard
{
    public class VirtualKeyboard_128x64_Monochrome : VirtualKeyboard, IDisposable
    {
        private readonly VirtualKeyboardTextDisplayBar _textDisplayBar;
        private bool _textDisplayBarHasControl = false;
        private readonly TextRenderer _textRenderer;
        private readonly Bitmap _sourceBitmap;
        private readonly Dictionary<char, Point> _whites;
        private readonly Dictionary<char, Point> _blacks;
        private readonly Dictionary<char, Point> _locations;
        private readonly TextRendererSource[] _textRendererSources;
        private readonly char[][] _keyboard1;
        private readonly char[][] _keyboard2;
        private bool _keyboard1Selected;
        private int _keyboardX, _keyboardY;
        private readonly Size _charSize = new Size(8, 12);
        private const int CharCountPerLine = 16;
        private const int PageLeadingLength = 4;

        public Bitmap CharImage => _sourceBitmap;

        public VirtualKeyboard_128x64_Monochrome(Graphics screenGraphics) : base(screenGraphics)
        {
            using (MemoryStream bitmapData = new MemoryStream(Resources.Characters))
            {
                _sourceBitmap = new Bitmap(bitmapData);
            }
            _whites = new Dictionary<char, Point>
            {
                #region Points

                {'1', new Point(0, 72)},
                {'2', new Point(8, 0)},
                {'3', new Point(16, 0)},
                {'4', new Point(24, 0)},
                {'5', new Point(32, 0)},
                {'6', new Point(40, 0)},
                {'7', new Point(48, 0)},
                {'8', new Point(56, 0)},
                {'9', new Point(64, 0)},
                {'0', new Point(72, 0)},
                {',', new Point(80, 0)},
                {'.', new Point(88, 0)},
                {'(', new Point(96, 0)},
                {')', new Point(104, 0)},
                {';', new Point(112, 0)},
                {' ', new Point(120, 0)},

                {'a', new Point(0, 12)},
                {'b', new Point(8, 12)},
                {'c', new Point(16, 12)},
                {'d', new Point(24, 12)},
                {'e', new Point(32, 12)},
                {'f', new Point(40, 12)},
                {'g', new Point(48, 12)},
                {'h', new Point(56, 12)},
                {'i', new Point(64, 12)},
                {'j', new Point(72, 12)},
                {'k', new Point(80, 12)},
                {'l', new Point(88, 12)},
                {'m', new Point(96, 12)},
                {'n', new Point(104, 12)},
                {'o', new Point(112, 12)},
                {'p', new Point(120, 12)},

                {'q', new Point(0, 24)},
                {'r', new Point(8, 24)},
                {'s', new Point(16, 24)},
                {'t', new Point(24, 24)},
                {'u', new Point(32, 24)},
                {'v', new Point(40, 24)},
                {'w', new Point(48, 24)},
                {'x', new Point(56, 24)},
                {'y', new Point(64, 24)},
                {'z', new Point(72, 24)},
                {'+', new Point(80, 24)},
                {'-', new Point(88, 24)},
                {'%', new Point(96, 24)},
                {'#', new Point(104, 24)},
                {'&', new Point(112, 24)},

                {'?', new Point(0, 36)},
                {'!', new Point(8, 36)},
                {':', new Point(16, 36)},
                {'_', new Point(24, 36)},
                {'*', new Point(32, 36)},
                {'/', new Point(40, 36)},
                {'\\', new Point(48, 36)},
                {'=', new Point(56, 36)},
                {'[', new Point(64, 36)},
                {']', new Point(72, 36)},
                {'{', new Point(80, 36)},
                {'}', new Point(88, 36)},
                {'<', new Point(96, 36)},
                {'>', new Point(104, 36)},
                {'\"', new Point(112, 36)},
                {'\'', new Point(120, 36)},

                {'A', new Point(0, 48)},
                {'B', new Point(8, 48)},
                {'C', new Point(16, 48)},
                {'D', new Point(24, 48)},
                {'E', new Point(32, 48)},
                {'F', new Point(40, 48)},
                {'G', new Point(48, 48)},
                {'H', new Point(56, 48)},
                {'I', new Point(64, 48)},
                {'J', new Point(72, 48)},
                {'K', new Point(80, 48)},
                {'L', new Point(88, 48)},
                {'M', new Point(96, 48)},
                {'N', new Point(104, 48)},
                {'O', new Point(112, 48)},
                {'P', new Point(120, 48)},

                {'Q', new Point(0, 60)},
                {'R', new Point(8, 60)},
                {'S', new Point(16, 60)},
                {'T', new Point(24, 60)},
                {'U', new Point(32, 60)},
                {'V', new Point(40, 60)},
                {'W', new Point(48, 60)},
                {'X', new Point(56, 60)},
                {'Y', new Point(64, 60)},
                {'Z', new Point(72, 60)},
                {'^', new Point(80, 60)},
                {'~', new Point(88, 60)},
                {'`', new Point(96, 60)},
                {'|', new Point(104, 60)},
                {'$', new Point(112, 60)},
                {'@', new Point(120, 60)}

                #endregion
            };
            _blacks = new Dictionary<char, Point>
            {
                #region Points

                {'1', new Point(0, 0)},
                {'2', new Point(8, 72)},
                {'3', new Point(16, 72)},
                {'4', new Point(24, 72)},
                {'5', new Point(32, 72)},
                {'6', new Point(40, 72)},
                {'7', new Point(48, 72)},
                {'8', new Point(56, 72)},
                {'9', new Point(64, 72)},
                {'0', new Point(72, 72)},
                {',', new Point(80, 72)},
                {'.', new Point(88, 72)},
                {'(', new Point(96, 72)},
                {')', new Point(104, 72)},
                {';', new Point(112, 72)},
                {' ', new Point(120, 72)},

                {'a', new Point(0, 84)},
                {'b', new Point(8, 84)},
                {'c', new Point(16, 84)},
                {'d', new Point(24, 84)},
                {'e', new Point(32, 84)},
                {'f', new Point(40, 84)},
                {'g', new Point(48, 84)},
                {'h', new Point(56, 84)},
                {'i', new Point(64, 84)},
                {'j', new Point(72, 84)},
                {'k', new Point(80, 84)},
                {'l', new Point(88, 84)},
                {'m', new Point(96, 84)},
                {'n', new Point(104, 84)},
                {'o', new Point(112, 84)},
                {'p', new Point(120, 84)},

                {'q', new Point(0, 96)},
                {'r', new Point(8, 96)},
                {'s', new Point(16, 96)},
                {'t', new Point(24, 96)},
                {'u', new Point(32, 96)},
                {'v', new Point(40, 96)},
                {'w', new Point(48, 96)},
                {'x', new Point(56, 96)},
                {'y', new Point(64, 96)},
                {'z', new Point(72, 96)},
                {'+', new Point(80, 96)},
                {'-', new Point(88, 96)},
                {'%', new Point(96, 96)},
                {'#', new Point(104, 96)},
                {'&', new Point(112, 96)},

                {'?', new Point(0, 108)},
                {'!', new Point(8, 108)},
                {':', new Point(16, 108)},
                {'_', new Point(24, 108)},
                {'*', new Point(32, 108)},
                {'/', new Point(40, 108)},
                {'\\', new Point(48, 108)},
                {'=', new Point(56, 108)},
                {'[', new Point(64, 108)},
                {']', new Point(72, 108)},
                {'{', new Point(80, 108)},
                {'}', new Point(88, 108)},
                {'<', new Point(96, 108)},
                {'>', new Point(104, 108)},
                {'\"', new Point(112, 108)},
                {'\'', new Point(120, 108)},

                {'A', new Point(0, 120)},
                {'B', new Point(8, 120)},
                {'C', new Point(16, 120)},
                {'D', new Point(24, 120)},
                {'E', new Point(32, 120)},
                {'F', new Point(40, 120)},
                {'G', new Point(48, 120)},
                {'H', new Point(56, 120)},
                {'I', new Point(64, 120)},
                {'J', new Point(72, 120)},
                {'K', new Point(80, 120)},
                {'L', new Point(88, 120)},
                {'M', new Point(96, 120)},
                {'N', new Point(104, 120)},
                {'O', new Point(112, 120)},
                {'P', new Point(120, 120)},

                {'Q', new Point(0, 132)},
                {'R', new Point(8, 132)},
                {'S', new Point(16, 132)},
                {'T', new Point(24, 132)},
                {'U', new Point(32, 132)},
                {'V', new Point(40, 132)},
                {'W', new Point(48, 132)},
                {'X', new Point(56, 132)},
                {'Y', new Point(64, 132)},
                {'Z', new Point(72, 132)},
                {'^', new Point(80, 132)},
                {'~', new Point(88, 132)},
                {'`', new Point(96, 132)},
                {'|', new Point(104, 132)},
                {'$', new Point(112, 132)},
                {'@', new Point(120, 132)}

                #endregion
            };
            _locations = new Dictionary<char, Point>
            {
                #region Points

                {'1', new Point(0, 16)},
                {'2', new Point(8, 16)},
                {'3', new Point(16, 16)},
                {'4', new Point(24, 16)},
                {'5', new Point(32, 16)},
                {'6', new Point(40, 16)},
                {'7', new Point(48, 16)},
                {'8', new Point(56, 16)},
                {'9', new Point(64, 16)},
                {'0', new Point(72, 16)},
                {',', new Point(80, 16)},
                {'.', new Point(88, 16)},
                {'(', new Point(96, 16)},
                {')', new Point(104, 16)},
                {';', new Point(112, 16)},
                {' ', new Point(120, 16)},

                {'a', new Point(0, 28)},
                {'b', new Point(8, 28)},
                {'c', new Point(16, 28)},
                {'d', new Point(24, 28)},
                {'e', new Point(32, 28)},
                {'f', new Point(40, 28)},
                {'g', new Point(48, 28)},
                {'h', new Point(56, 28)},
                {'i', new Point(64, 28)},
                {'j', new Point(72, 28)},
                {'k', new Point(80, 28)},
                {'l', new Point(88, 28)},
                {'m', new Point(96, 28)},
                {'n', new Point(104, 28)},
                {'o', new Point(112, 28)},
                {'p', new Point(120, 28)},

                {'q', new Point(0, 40)},
                {'r', new Point(8, 40)},
                {'s', new Point(16, 40)},
                {'t', new Point(24, 40)},
                {'u', new Point(32, 40)},
                {'v', new Point(40, 40)},
                {'w', new Point(48, 40)},
                {'x', new Point(56, 40)},
                {'y', new Point(64, 40)},
                {'z', new Point(72, 40)},
                {'+', new Point(80, 40)},
                {'-', new Point(88, 40)},
                {'%', new Point(96, 40)},
                {'#', new Point(104, 40)},
                {'&', new Point(112, 40)},
                {'\n', new Point(120, 40)},

                {'?', new Point(0, 52)},
                {'!', new Point(8, 52)},
                {':', new Point(16, 52)},
                {'_', new Point(24, 52)},
                {'*', new Point(32, 52)},
                {'/', new Point(40, 52)},
                {'\\', new Point(48, 52)},
                {'=', new Point(56, 52)},
                {'[', new Point(64, 52)},
                {']', new Point(72, 52)},
                {'{', new Point(80, 52)},
                {'}', new Point(88, 52)},
                {'<', new Point(96, 52)},
                {'>', new Point(104, 52)},
                {'\"', new Point(112, 52)},
                {'\'', new Point(120, 52)},

                {'A', new Point(0, 28)},
                {'B', new Point(8, 28)},
                {'C', new Point(16, 28)},
                {'D', new Point(24, 28)},
                {'E', new Point(32, 28)},
                {'F', new Point(40, 28)},
                {'G', new Point(48, 28)},
                {'H', new Point(56, 28)},
                {'I', new Point(64, 28)},
                {'J', new Point(72, 28)},
                {'K', new Point(80, 28)},
                {'L', new Point(88, 28)},
                {'M', new Point(96, 28)},
                {'N', new Point(104, 28)},
                {'O', new Point(112, 28)},
                {'P', new Point(120, 28)},

                {'Q', new Point(0, 40)},
                {'R', new Point(8, 40)},
                {'S', new Point(16, 40)},
                {'T', new Point(24, 40)},
                {'U', new Point(32, 40)},
                {'V', new Point(40, 40)},
                {'W', new Point(48, 40)},
                {'X', new Point(56, 40)},
                {'Y', new Point(64, 40)},
                {'Z', new Point(72, 40)},
                {'^', new Point(80, 40)},
                {'~', new Point(88, 40)},
                {'`', new Point(96, 40)},
                {'|', new Point(104, 40)},
                {'$', new Point(112, 40)},
                {'@', new Point(120, 40)}

                #endregion
            };
            _textRendererSources = new TextRendererSource[]
                {new TextRendererSource(_sourceBitmap, _whites), new TextRendererSource(_sourceBitmap, _blacks)};
            _keyboard1 = new[]
            {
                new[] {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0', ',', '.', '(', ')', ';', ' '},
                new[] {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p'},
                new[] {'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '+', '-', '%', '#', '&', '\n'},
                new[] {'?', '!', ':', '_', '*', '/', '\\', '=', '[', ']', '{', '}', '<', '>', '\"', '\''}
            };
            _keyboard2 = new[]
            {
                _keyboard1[0],
                new[] {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P'},
                new[] {'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '^', '~', '`', '|', '$', '@'},
                _keyboard1[3]
            };

            _textRenderer = new TextRenderer(Graphics, 0, 0, _charSize, CharCountPerLine, _textRendererSources);

            _textDisplayBar = new VirtualKeyboardTextDisplayBar(CharCountPerLine, PageLeadingLength);
            _textDisplayBar.DisplayBarChanged += DisplayBarTextChanged;
        }

        private bool _suppressOnScreenChanged = false;
        private readonly Rectangle _keyboardZoneFull = new Rectangle(0, 16, 128, 48);
        private readonly Rectangle _keyboardZoneLine1 = new Rectangle(0, 16, 128, 12);
        private readonly Rectangle _keyboardZoneLine23 = new Rectangle(0, 28, 128, 24);
        private readonly Rectangle _keyboardZoneLine4 = new Rectangle(0, 52, 128, 12);
        private readonly Rectangle _keyboardSourceZoneKeyboard1Full = new Rectangle(0, 0, 128, 48);
        private readonly Rectangle _keyboardSourceZoneKeyboardLine1 = new Rectangle(0, 0, 128, 12);
        private readonly Rectangle _keyboardSourceZoneKeyboard1Line23 = new Rectangle(0, 12, 128, 24);
        private readonly Rectangle _keyboardSourceZoneKeyboardLine4 = new Rectangle(0, 36, 128, 12);
        private readonly Rectangle _keyboardSourceZoneKeyboard2Line23 = new Rectangle(0, 48, 128, 24);

        private string _initialText;
        private bool _allowEnter;
        private bool _helpMode, _confirmMode;

        protected override void StartInternal(string initialText, bool allowEnter)
        {
            _initialText = initialText;
            _allowEnter = allowEnter;
            _confirmMode = false;

            //Help
            if (ShowHelp)
            {
                InitializeHelp();
            }
            else
            {
                InitializeFunction();
            }
        }

        #region Help
        void InitializeHelp()
        {
            _helpMode = true;
            Graphics.Clear(Color.White);

            TextRenderer line1 = new TextRenderer(Graphics, 0, 2, _charSize, CharCountPerLine, _textRendererSources);
            TextRenderer line2 = new TextRenderer(Graphics, 0, 14, _charSize, CharCountPerLine, _textRendererSources);
            TextRenderer line3 = new TextRenderer(Graphics, 0, 26, _charSize, CharCountPerLine, _textRendererSources);
            TextRenderer line4 = new TextRenderer(Graphics, 0, 38, _charSize, CharCountPerLine, _textRendererSources);
            TextRenderer line5 = new TextRenderer(Graphics, 0, 50, _charSize, CharCountPerLine, _textRendererSources);
            line1.Render("JS: Move cursor ", i => i <= 3 ? 1 : 0);
            line2.Render("JS Press: Select", i => i <= 9 ? 1 : 0);
            line3.Render("Key1: Backspace ", i => i <= 4 ? 1 : 0);
            line4.Render("Key2: Shift Keys", i => i <= 4 ? 1 : 0);
            line5.Render("Key3: OK/Cancel ", i => i <= 4 ? 1 : 0);

            OnScreenChanged(0, 63);
        }

        bool TryCloseHelp()
        {
            if (_helpMode)
            {
                _helpMode = false;
                InitializeFunction();

                return true;
            }

            return false;
        }
        #endregion

        #region OK/Cancel

        void ExitFunction()
        {
            if (ShowConfirm)
            {
                _confirmMode = true;

                TextRenderer line1 = new TextRenderer(Graphics, 0, 16, _charSize, CharCountPerLine, _textRendererSources);
                TextRenderer line2 = new TextRenderer(Graphics, 0, 28, _charSize, CharCountPerLine, _textRendererSources);
                TextRenderer line3 = new TextRenderer(Graphics, 0, 40, _charSize, CharCountPerLine, _textRendererSources);
                TextRenderer line4 = new TextRenderer(Graphics, 0, 52, _charSize, CharCountPerLine, _textRendererSources);
                line1.Render("JS: Move cursor ", i => i <= 3 ? 1 : 0);
                line2.Render("Key1: OK        ", i => i <= 4 ? 1 : 0);
                line3.Render("Key2: Cancel    ", i => i <= 4 ? 1 : 0);
                line4.Render("Key3: Back      ", i => i <= 4 ? 1 : 0);

                if (_textDisplayBarHasControl)
                    OnScreenChanged(16, 63);
                else
                {
                    _textDisplayBarHasControl = true;
                    OnScreenChanged(0, 63);
                }
            }
            else
            {
                SetVirtualKeyboardResult(_textDisplayBar.Text, false, true);
            }
        }

        bool TryOk()
        {
            if (_confirmMode)
            {
                _confirmMode = false;
                SetVirtualKeyboardResult(_textDisplayBar.Text, false, true);

                return true;
            }

            return false;
        }

        bool TryCancel()
        {
            if (_confirmMode)
            {
                _confirmMode = false;
                SetVirtualKeyboardResult(null, true, true);

                return true;
            }

            return false;
        }

        bool TryBack()
        {
            if (_confirmMode)
            {
                _confirmMode = false;

                //reinitialize keyboard
                if (_keyboard1Selected)
                {
                    Graphics.DrawImage(_sourceBitmap, _keyboardZoneFull, _keyboardSourceZoneKeyboard1Full, GraphicsUnit.Pixel);
                }
                else
                {
                    Graphics.DrawImage(_sourceBitmap, _keyboardZoneLine1, _keyboardSourceZoneKeyboardLine1, GraphicsUnit.Pixel);
                    Graphics.DrawImage(_sourceBitmap, _keyboardZoneLine23, _keyboardSourceZoneKeyboard2Line23, GraphicsUnit.Pixel);
                    Graphics.DrawImage(_sourceBitmap, _keyboardZoneLine4, _keyboardSourceZoneKeyboardLine4, GraphicsUnit.Pixel);
                }

                SetWhite('1');

                OnScreenChanged(16, 63);

                return true;
            }

            return false;
        }

        #endregion

        void InitializeFunction()
        {
            _helpMode = false;
            Graphics.Clear(Color.White);

            //Text bar
            if (_allowEnter)
            {
                _whites['\n'] = new Point(120, 24);
                _blacks['\n'] = new Point(120, 96);
                _keyboard1[2][15] = '\n';
            }
            else
            {
                _whites['\n'] = new Point(120, 0); //space
                _blacks['\n'] = new Point(120, 72); //space
                _keyboard1[2][15] = ' ';
            }
            
            _suppressOnScreenChanged = true;
            _textDisplayBar.Text = _initialText;
            _suppressOnScreenChanged = false;

            //Keyboard zone
            Graphics.DrawLine(new Pen(Color.Black, 2), 0, 13, 127, 13);

            _keyboard1Selected = true;
            _keyboardX = 0;
            _keyboardY = 0;
            Graphics.DrawImage(_sourceBitmap, _keyboardZoneFull, _keyboardSourceZoneKeyboard1Full, GraphicsUnit.Pixel);

            OnScreenChanged(0, 63);
        }

        private void DisplayBarTextChanged(object sender, VirtualKeyboardTextDisplayBarChangedEventArgs e)
        {
            var displayText = e.DisplayText.PadRight(16);
            _textRenderer.Render(displayText.PadRight(16), i => i == e.DisplayIndex ? 1 : 0);

            if (_suppressOnScreenChanged) return;
            OnScreenChanged(0, 11);
        }

        #region Keyboard Moving

        void SetWhite(char key)
        {
            Graphics.DrawImage(_sourceBitmap, new Rectangle(_locations[key], _charSize),
                new Rectangle(_whites[key], _charSize), GraphicsUnit.Pixel);
        }

        void SetBlack(char key)
        {
            Graphics.DrawImage(_sourceBitmap, new Rectangle(_locations[key], _charSize),
                new Rectangle(_blacks[key], _charSize), GraphicsUnit.Pixel);
        }

        void KeyShift()
        {
            if (_keyboard1Selected)
            {
                _keyboard1Selected = false;
                //Graphics.DrawImage(_sourceBitmap, _keyboardZoneLine1, _keyboardSourceZoneKeyboardLine1, GraphicsUnit.Pixel);
                Graphics.DrawImage(_sourceBitmap, _keyboardZoneLine23, _keyboardSourceZoneKeyboard2Line23, GraphicsUnit.Pixel);
                //Graphics.DrawImage(_sourceBitmap, _keyboardZoneLine4, _keyboardSourceZoneKeyboardLine4, GraphicsUnit.Pixel);
            }
            else
            {
                _keyboard1Selected = true;
                //Graphics.DrawImage(_sourceBitmap, _keyboardZoneLine1, _keyboardSourceZoneKeyboardLine1, GraphicsUnit.Pixel);
                Graphics.DrawImage(_sourceBitmap, _keyboardZoneLine23, _keyboardSourceZoneKeyboard1Line23, GraphicsUnit.Pixel);
                //Graphics.DrawImage(_sourceBitmap, _keyboardZoneLine4, _keyboardSourceZoneKeyboardLine4, GraphicsUnit.Pixel);
            }

            if (!(_textDisplayBarHasControl || _keyboardY == 0 || _keyboardY == 3))
            {
                var selectedKey = (_keyboard1Selected ? _keyboard1 : _keyboard2)[_keyboardY][_keyboardX];

                Graphics.DrawImage(_sourceBitmap, new Rectangle(_locations[selectedKey], _charSize),
                    new Rectangle(_blacks[selectedKey], _charSize), GraphicsUnit.Pixel);
            }

            OnScreenChanged(28, 51);
        }

        #endregion

        public override string GetResult(bool raiseEvent)
        {
            var text = _textDisplayBar.Text;
            SetVirtualKeyboardResult(text, false, raiseEvent);
            return text;
        }

        public override void Key1()
        {
            if (TryCloseHelp()) return;
            if (TryOk()) return;

            //backspace
            _textDisplayBar.Backspace();
        }

        public override void Key2()
        {
            if (TryCloseHelp()) return;
            if (TryCancel()) return;

            //shift
            KeyShift();
        }

        public override void Key3()
        {
            if (TryCloseHelp()) return;
            if (TryBack()) return;

            //ok/cancel
            ExitFunction();
        }

        public override void Up()
        {
            if (TryCloseHelp()) return;
            if (_confirmMode) return;

            if (_textDisplayBarHasControl) return;
            if (_keyboardY == 0)
            {
                _textDisplayBarHasControl = true;

                var selectedKey = _keyboard1[0][_keyboardX]; //same as keyboard2
                SetWhite(selectedKey);
                OnScreenChanged(16, 27);
            }
            else
            {
                var selectedKey = (_keyboard1Selected ? _keyboard1 : _keyboard2)[_keyboardY][_keyboardX];
                SetWhite(selectedKey);
                _keyboardY--;
                selectedKey = (_keyboard1Selected ? _keyboard1 : _keyboard2)[_keyboardY][_keyboardX];
                SetBlack(selectedKey);
                int start = _keyboardY * 12 + 16;
                OnScreenChanged(start, start + 23);
            }
        }

        public override void Down()
        {
            if (TryCloseHelp()) return;
            if (_confirmMode) return;

            if (_textDisplayBarHasControl)
            {
                _textDisplayBarHasControl = false;
                var selectedKey = _keyboard1[0][_keyboardX]; //same as keyboard2
                SetBlack(selectedKey);
                OnScreenChanged(16, 27);
            }
            else if (_keyboardY != 3)
            {
                var selectedKey = (_keyboard1Selected ? _keyboard1 : _keyboard2)[_keyboardY][_keyboardX];
                SetWhite(selectedKey);
                _keyboardY++;
                selectedKey = (_keyboard1Selected ? _keyboard1 : _keyboard2)[_keyboardY][_keyboardX];
                SetBlack(selectedKey);
                int start = _keyboardY * 12 + 4;
                OnScreenChanged(start, start + 23);
            }
        }

        public override void Left()
        {
            if (TryCloseHelp()) return;

            if (_textDisplayBarHasControl)
            {
                _textDisplayBar.MoveLeft();
            }
            else
            {
                if (_keyboardX == 0) return;
                var selectedKey = (_keyboard1Selected ? _keyboard1 : _keyboard2)[_keyboardY][_keyboardX];
                SetWhite(selectedKey);
                _keyboardX--;
                selectedKey = (_keyboard1Selected ? _keyboard1 : _keyboard2)[_keyboardY][_keyboardX];
                SetBlack(selectedKey);
                int start = _keyboardY * 12 + 16;
                OnScreenChanged(start, start + 11);
            }
        }

        public override void Right()
        {
            if (TryCloseHelp()) return;

            if (_textDisplayBarHasControl)
            {
                _textDisplayBar.MoveRight();
            }
            else
            {
                if (_keyboardX == 15) return;
                var selectedKey = (_keyboard1Selected ? _keyboard1 : _keyboard2)[_keyboardY][_keyboardX];
                SetWhite(selectedKey);
                _keyboardX++;
                selectedKey = (_keyboard1Selected ? _keyboard1 : _keyboard2)[_keyboardY][_keyboardX];
                SetBlack(selectedKey);
                int start = _keyboardY * 12 + 16;
                OnScreenChanged(start, start + 11);
            }
        }

        public override void Press()
        {
            if (TryCloseHelp()) return;
            if (_textDisplayBarHasControl) return;

            var selectedKey = (_keyboard1Selected ? _keyboard1 : _keyboard2)[_keyboardY][_keyboardX];
            _textDisplayBar.EnterKey(selectedKey);
        }

        public void Dispose()
        {
            ((IDisposable)_sourceBitmap).Dispose();
        }
    }
}
