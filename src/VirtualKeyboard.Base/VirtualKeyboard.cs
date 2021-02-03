﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace SecretNest.Hardware.VirtualKeyboard
{
    public abstract class VirtualKeyboard
    {
        public event EventHandler<VirtualKeyboardResultEventArgs> VirtualKeyboardResult;

        protected void SetVirtualKeyboardResult(string text, bool isCancelled, bool raiseEvent)
        {
            Text = ReplaceEnterFromDisplay(text);
            IsCancelled = isCancelled;
            if (raiseEvent)
                VirtualKeyboardResult?.Invoke(this, new VirtualKeyboardResultEventArgs(isCancelled, text));
        }

        private string ReplaceEntersToDisplay(string text)
        {
            if (EnterKeyTexts != null)
                foreach (var s in EnterKeyTexts)
                {
                    text = text.Replace(s, "\n");
                }

            return text;
        }

        private string ReplaceEnterFromDisplay(string text)
        {
            if (EnterKeyTexts != null && EnterKeyTexts.Length > 0)
                return text.Replace("\n", EnterKeyTexts[0]);
            else
                return text;
        }

        public string Text { get; private set; }
        public string[] EnterKeyTexts { get; set; }
        public bool ShowHelp { get; set; }
        public bool ShowConfirm { get; set; }
        public bool IsCancelled { get; private set; }
        //public Bitmap Image { get; protected set; }
        protected Graphics Graphics { get; private set; }

        protected VirtualKeyboard(Graphics screenGraphics)
        {
            Graphics = screenGraphics;
        }

        public event EventHandler<ScreenChangedEventArgs> ScreenChanged;

        protected void OnScreenChanged(int startY, int endY)
        {
            ScreenChanged?.Invoke(this, new ScreenChangedEventArgs(startY, endY));
        }

        public void Start(string initialText, bool allowEnter) =>
            StartInternal(ReplaceEntersToDisplay(initialText), allowEnter);

        protected abstract void StartInternal(string initialText, bool allowEnter);

        public abstract string GetResult(bool raiseEvent);

        public abstract void Key1();

        public abstract void Key2();

        public abstract void Key3();

        public abstract void Up();

        public abstract void Down();

        public abstract void Left();

        public abstract void Right();

        public abstract void Press();
    }

    public class VirtualKeyboardResultEventArgs : EventArgs
    {
        public VirtualKeyboardResultEventArgs(bool isCancelled, string text)
        {
            IsCancelled = isCancelled;
            Text = text;
        }

        public bool IsCancelled { get; }
        public string Text { get; }
    }

    public class ScreenChangedEventArgs : EventArgs
    {
        public ScreenChangedEventArgs(int startY, int endY)
        {
            StartY = startY;
            EndY = endY;
        }

        public int StartY { get; }
        public int EndY { get; }
    }
}