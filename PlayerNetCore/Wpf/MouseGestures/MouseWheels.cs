using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace NekoPlayer.Wpf.MouseGestures
{
    public class MouseWheelUp : MouseGesture
    {
        public MouseWheelUp() : base(MouseAction.WheelClick)
        {
        }

        public MouseWheelUp(ModifierKeys modifiers) : base(MouseAction.WheelClick, modifiers)
        {
        }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if (!base.Matches(targetElement, inputEventArgs)) return false;
            if (!(inputEventArgs is MouseWheelEventArgs)) return false;
            var args = (MouseWheelEventArgs)inputEventArgs;
            return args.Delta > 0;
        }
    }
    public class MouseWheelDown : MouseGesture
    {
        public MouseWheelDown() : base(MouseAction.WheelClick)
        {
        }

        public MouseWheelDown(ModifierKeys modifiers) : base(MouseAction.WheelClick, modifiers)
        {
        }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            if (!base.Matches(targetElement, inputEventArgs)) return false;
            if (!(inputEventArgs is MouseWheelEventArgs)) return false;
            var args = (MouseWheelEventArgs)inputEventArgs;
            return args.Delta < 0;
        }
    }
}
