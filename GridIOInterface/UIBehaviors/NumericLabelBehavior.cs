using GridIOInterface;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GridIOInterface {
    class NumericLabelBehavior : DependencyObject {
        [DllImport("User32.dll")]
        private static extern bool GetCursorPos(out POINT point);
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        public static readonly DependencyProperty linkedBoxProperty = DependencyProperty.RegisterAttached(
                "DragBox",
                typeof(TextBox),
                typeof(Label)
            );
        public static readonly DependencyProperty parseNumericProperty = DependencyProperty.RegisterAttached(
                "ParseNumeric",
                typeof(bool),
                typeof(TextBox)
            );
        public static readonly DependencyProperty isDecimalProperty = DependencyProperty.RegisterAttached(
                "IsDecimal",
                typeof(bool),
                typeof(TextBox)
            );
        public static readonly DependencyProperty minValProperty = DependencyProperty.RegisterAttached(
                "MinVal",
                typeof(float),
                typeof(TextBox),
                new UIPropertyMetadata(float.MinValue)
            );
        public static readonly DependencyProperty maxValProperty = DependencyProperty.RegisterAttached(
                "MaxVal",
                typeof(float),
                typeof(TextBox),
                new UIPropertyMetadata(float.MaxValue)
            );

        private readonly static int decimalPrecision = 5;
        private Label nameLabel;
        private TextBox valueBox;
        private Point mousePosStartDrag;
        private Timer dragTimer;
        private int dragXState = 0;
        private float prevValue = 0;
        private bool isDecimal => (bool)valueBox.GetValue(isDecimalProperty);

        public float DecimalValue {
            get {
                bool valid = float.TryParse(valueBox.Text, out float val);
                return valid ? val : prevValue;
            }
        }

        public int IntValue {
            get {
                bool valid = int.TryParse(valueBox.Text, out int val);
                return valid ? val : (int)prevValue;
            }
        }

        public NumericLabelBehavior(Label nameLabel, TextBox valueBox) {
            this.nameLabel = nameLabel;
            this.valueBox = valueBox;

            if ((bool)valueBox.GetValue(parseNumericProperty)) {
                dragTimer = new Timer(25);
                dragTimer.Elapsed += DragTimer_Elapsed;

                nameLabel.MouseLeftButtonDown += Tag_MouseDown;
                nameLabel.MouseLeftButtonUp += Tag_MouseUp;

                valueBox.LostKeyboardFocus += ValueBox_LostKeyboardFocus;

                CommitValue();
            }
            else {
                throw new Exception();
            }
        }

        private void ValueBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            CommitValue();
        }

        private void CommitValue() {
            bool valid = float.TryParse(valueBox.Text, out float valFloat);
            if (valid) {
                prevValue = Math.Clamp(valFloat, (float)valueBox.GetValue(minValProperty), (float)valueBox.GetValue(maxValProperty));
                if (prevValue != valFloat) {
                    valueBox.Text = prevValue.ToString();
                }
            }
            else {
                valueBox.Text = Math.Round(prevValue, decimalPrecision).ToString();
                valueBox.CaretIndex = valueBox.Text.Length;
            }
        }

        private void DragTimer_Elapsed(object sender, ElapsedEventArgs e) {
            Dispatcher.Invoke(new Action(() => {
                Point mousePos = ScreenMousePos();
                int screenWidth = (int)SystemParameters.PrimaryScreenWidth;
                if (mousePos.X == 0) {
                    SetCursorPos(screenWidth - 3, (int)mousePos.Y);
                    mousePos.X = screenWidth - 3;
                    dragXState--;
                }
                else if (mousePos.X == screenWidth - 1) {
                    SetCursorPos(2, (int)mousePos.Y);
                    mousePos.X = 2;
                    dragXState++;
                }
                double mouseDiff = (mousePos - mousePosStartDrag).X;
                double newVal = prevValue + (mouseDiff + screenWidth * dragXState) / 100;
                valueBox.Text = Math.Round((isDecimal ? newVal : (int)newVal), decimalPrecision).ToString();
            }));
        }

        private void Tag_MouseDown(object sender, MouseButtonEventArgs e) {
            mousePosStartDrag = ScreenMousePos();
            UIElement el = (UIElement)sender;
            el.CaptureMouse();
            Mouse.OverrideCursor = Cursors.SizeWE;
            dragTimer.Start();
        }

        private void Tag_MouseUp(object sender, MouseButtonEventArgs e) {
            UIElement el = (UIElement)sender;
            el.ReleaseMouseCapture();
            Mouse.OverrideCursor = null;
            dragTimer.Stop();
            CommitValue();
            dragXState = 0;
        }

        public static void SetDragBox(UIElement element, TextBox value) {
            Label nameLabel = (Label)element;

            new NumericLabelBehavior(nameLabel, value);
            element.SetValue(linkedBoxProperty, value);
        }

        public static void SetDragBox(UIElement element, string value) {
            Label nameLabel = (Label)element;
            TextBox valueBox = (TextBox)((FrameworkElement)nameLabel.Parent).FindName(value);

            new NumericLabelBehavior(nameLabel, valueBox);
            element.SetValue(linkedBoxProperty, value);
        }

        public static string GetDragBox(UIElement element) {
            return (string)element.GetValue(linkedBoxProperty);
        }

        public static void SetParseNumeric(UIElement element, bool value) {
            element.SetValue(parseNumericProperty, value);
        }

        public static bool GetParseNumeric(UIElement element) {
            return (bool)element.GetValue(parseNumericProperty);
        }

        public static void SetIsDecimal(UIElement element, bool value) {
            element.SetValue(isDecimalProperty, value);
        }

        public static void SetMinVal(UIElement element, float value) {
            element.SetValue(minValProperty, value);
        }

        public static void SetMaxVal(UIElement element, float value) {
            element.SetValue(maxValProperty, value);
        }

        public static bool GetIsDecimal(UIElement element) {
            return (bool)element.GetValue(isDecimalProperty);
        }

        public static float GetMinVal(UIElement element) {
            return (float)element.GetValue(minValProperty);
        }

        public static float GetMaxVal(UIElement element) {
            return (float)element.GetValue(maxValProperty);
        }

        private Point ScreenMousePos() {
            GetCursorPos(out POINT point);
            return (Point)point;
        }
    }
}
