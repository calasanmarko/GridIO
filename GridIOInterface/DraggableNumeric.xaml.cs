using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Timers;
using System;
using System.Runtime.InteropServices;

namespace BlockIOInterface {
    /// <summary>
    /// Interaction logic for DraggableNumeric.xaml
    /// </summary>
    public partial class DraggableNumeric : UserControl {
        [DllImport("User32.dll")]
        private static extern bool GetCursorPos(out POINT point);
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        public static readonly DependencyProperty textProperty = DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(DraggableNumeric),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnLabelChanged))
            );
        public static readonly DependencyProperty valueProperty = DependencyProperty.Register(
                "Value",
                typeof(string),
                typeof(DraggableNumeric),
                new FrameworkPropertyMetadata(new PropertyChangedCallback(OnValueChanged))
            );
        public static readonly DependencyProperty decimalProperty = DependencyProperty.Register(
                "IsDecimal",
                typeof(bool),
                typeof(DraggableNumeric)
            );
        private float prevValue = 0;
        private readonly Timer dragTimer;
        private Point mousePosStartDrag;
        private int dragXState = 0;

        public string Text {
            get {
                return (string)GetValue(textProperty);
            }
            set {
                SetValue(textProperty, value);
            }
        }

        public string Value {
            get {
                return valueBox.Text;
            }
            set {
                valueBox.Text = value;
            }
        }

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

        public bool IsDecimal {
            get {
                return (bool)GetValue(decimalProperty);
            }
            set {
                SetValue(decimalProperty, value);
            }
        }

        public TextChangedEventHandler TextChanged {
            set {
                valueBox.TextChanged += value;
            }
        }

        public DraggableNumeric() {
            dragTimer = new Timer(25);
            dragTimer.Elapsed += DragTimer_Elapsed;
            InitializeComponent();
        }

        private void CommitValue() {
            bool valid = float.TryParse(valueBox.Text, out float valFloat);
            if (valid) {
                prevValue = valFloat;
            }
            else {
                valueBox.Text = prevValue.ToString();
                valueBox.CaretIndex = valueBox.Text.Length;
            }
        }

        private void valueBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            CommitValue();
        }

        private void valueBox_TextChanged(object sender, TextChangedEventArgs e) {
            SetValue(valueProperty, Value);
        }

        private static void OnLabelChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            DraggableNumeric control = (DraggableNumeric)obj;
            control.nameLabel.Content = args.NewValue;
        }

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args) {
            DraggableNumeric control = (DraggableNumeric)obj;
            control.valueBox.Text = (string)args.NewValue;
        }

        private Point ScreenMousePos() {
            GetCursorPos(out POINT point);
            return (Point)point;
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
                Value = (IsDecimal ? newVal : (int)newVal).ToString();
            }));
        }

        private void nameLabel_MouseDown(object sender, MouseButtonEventArgs e) {
            mousePosStartDrag = ScreenMousePos();
            UIElement el = (UIElement)sender;
            el.CaptureMouse();
            Mouse.OverrideCursor = Cursors.SizeWE;
            dragTimer.Start();
        }

        private void nameLabel_MouseUp(object sender, MouseButtonEventArgs e) {
            UIElement el = (UIElement)sender;
            el.ReleaseMouseCapture();
            Mouse.OverrideCursor = null;
            dragTimer.Stop();
            CommitValue();
            dragXState = 0;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {
            CommitValue();
        }
    }
}
