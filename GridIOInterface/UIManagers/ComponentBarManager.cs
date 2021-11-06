using GridIOInterface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GridIOInterface {
    public class ComponentBarManager {
        private static ECSFramework framework => ECSFramework.instance;

        private readonly MainWindow window;
        private readonly StackPanel panel;
        private readonly StackPanel addPanel;
        private Entity entity;
        private readonly List<Component> components;
        private readonly HashSet<TextBox> textBoxes;
        private readonly Timer refreshTimer;
        private bool skipTextChanged = false;
        private bool initTextBoxAssignment = false;

        public static readonly DependencyProperty componentTypeProperty = DependencyProperty.RegisterAttached(
                "ComponentType",
                typeof(Type),
                typeof(Label)
            );
        public static readonly DependencyProperty componentProperty = DependencyProperty.RegisterAttached(
                "Component",
                typeof(Component),
                typeof(UIElement)
            );
        public static readonly DependencyProperty fieldProperty = DependencyProperty.RegisterAttached(
                "Field",
                typeof(FieldInfo),
                typeof(TextBox)
            );
        public static readonly DependencyProperty fieldDataProperty = DependencyProperty.RegisterAttached(
                "FieldData",
                typeof(string),
                typeof(TextBox)
            );

        public ComponentBarManager(MainWindow window, StackPanel panel) {
            this.window = window;
            this.panel = panel;
            addPanel = new StackPanel() {
                Visibility = Visibility.Collapsed
            };
            components = new List<Component>();
            textBoxes = new HashSet<TextBox>();
            refreshTimer = new Timer(1);
            refreshTimer.Elapsed += (a, b) => RefreshValues();
            LoadAddPanel();
        }

        private void LoadAddPanel() {
            ReadOnlyCollection<Type> components = framework.GetComponentTypes();
            foreach (Type componentType in components) {
                if (framework.IsComponentAddable(componentType)) {
                    Label label = new Label() {
                        Content = FormatFieldName(componentType.Name),
                        Background = Brushes.WhiteSmoke
                    };
                    label.PreviewMouseDown += ComponentLabel_PreviewMouseDown;
                    label.SetValue(componentTypeProperty, componentType);
                    addPanel.Children.Add(label);
                }
            }
        }

        private void SetTextWithoutEvent(TextBox textBox, string newText) {
            try {
                skipTextChanged = true;
                textBox.Text = newText;
            }
            finally {
                skipTextChanged = false;
            }
        }

        private void ComponentLabel_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            Label label = (Label)sender;
            Type componentType = (Type)label.GetValue(componentTypeProperty);
            Component component = (Component)componentType.GetConstructor(new Type[0]).Invoke(null);
            component.OnGUIAdd();
            framework.AddComponent(component, entity);
            ToggleAddPanel();
            Refresh();
        }

        public void SetEntity(Entity entity) {
            this.entity = entity;
            Refresh();
        }

        public void Refresh() {
            panel.Children.Clear();
            components.Clear();
            foreach (Component component in ECSFramework.instance.GetComponentsOfEntity(entity)) {
                components.Add(component);
            }

            foreach (Component component in components) {
                FieldInfo[] fields = component.GetType().GetFields();
                Grid compGrid = new Grid();
                for (int i = 0; i < fields.Length + 1; i++) {
                    compGrid.RowDefinitions.Add(new RowDefinition());
                }
                compGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(130) });
                compGrid.ColumnDefinitions.Add(new ColumnDefinition());
                panel.Children.Add(compGrid);

                Grid compLabelGrid = new Grid();
                compLabelGrid.RowDefinitions.Add(new RowDefinition());
                compLabelGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                compLabelGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(50) });

                Label compLabel = new Label() {
                    Content = component.typeName,
                    FontWeight = FontWeights.Bold
                };
                Grid.SetRow(compLabel, 0);
                Grid.SetColumn(compLabel, 0);
                compLabelGrid.Children.Add(compLabel);

                if (framework.IsComponentAddable(component.GetType())) {
                    Button removeButton = new Button() {
                        Content = "-"
                    };
                    removeButton.SetValue(componentProperty, component);
                    removeButton.Click += RemoveButton_Click;
                    Grid.SetRow(removeButton, 0);
                    Grid.SetColumn(removeButton, 1);
                    compLabelGrid.Children.Add(removeButton);
                }


                Grid.SetRow(compLabelGrid, 0);
                Grid.SetColumn(compLabelGrid, 0);
                Grid.SetColumnSpan(compLabelGrid, 2);

                compGrid.Children.Add(compLabelGrid);
                for (int i = 0; i < fields.Length; i++) {
                    Label fieldLabel = CreateLabel(FormatFieldName(fields[i].Name), i + 1, 0);

                    Grid fieldGrid = new Grid();
                    Grid.SetRow(fieldGrid, i + 1);
                    Grid.SetColumn(fieldGrid, 1);

                    bool foundType = true;
                    bool notPrimitiveType = false;
                    switch (Type.GetTypeCode(fields[i].FieldType)) {
                        case TypeCode.Byte:
                        case TypeCode.SByte:
                        case TypeCode.UInt16:
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                            DefineTable(fieldGrid, 1, 1);
                            fieldGrid.Children.Add(CreateTextBox(component, fields[i], "", fieldLabel, false, 0, 0, 1));
                            break;
                        case TypeCode.Decimal:
                        case TypeCode.Single:
                        case TypeCode.Double:
                            DefineTable(fieldGrid, 1, 1);
                            fieldGrid.Children.Add(CreateTextBox(component, fields[i], "", fieldLabel, true, 0, 0, 1));
                            break;
                        case TypeCode.String:
                            DefineTable(fieldGrid, 1, 1);
                            fieldGrid.Children.Add(CreateTextBox(component, fields[i], "", null, false, 0, 0, 1));
                            break;
                        case TypeCode.Char:
                            DefineTable(fieldGrid, 1, 1);
                            fieldGrid.Children.Add(CreateTextBox(component, fields[i], "", null, false, 0, 0, 1));
                            break;
                        case TypeCode.Boolean:
                            DefineTable(fieldGrid, 1, 1);
                            fieldGrid.Children.Add(CreateCheckBox(0, 0));
                            break;
                        default:
                            notPrimitiveType = true;
                            break;
                    }

                    if (notPrimitiveType) {
                        if (fields[i].FieldType == typeof(Vector3) || fields[i].FieldType == typeof(Vector2)) {
                            fieldGrid.RowDefinitions.Add(new RowDefinition());
                            fieldGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                            fieldGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 45 });
                            fieldGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                            fieldGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 45 });
                            fieldGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                            fieldGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 45 });

                            Label xLabel = CreateLabel("X:", 0, 0);
                            Label yLabel = CreateLabel("Y:", 0, 2);

                            TextBox xBox = CreateTextBox(component, fields[i], "x", xLabel, true, 0, 1, 1);
                            TextBox yBox = CreateTextBox(component, fields[i], "y", yLabel, true, 0, 3, 1);

                            fieldGrid.Children.Add(xLabel);
                            fieldGrid.Children.Add(xBox);
                            fieldGrid.Children.Add(yLabel);
                            fieldGrid.Children.Add(yBox);

                            if (fields[i].FieldType == typeof(Vector3)) {
                                fieldGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                                fieldGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 45 });
                                Label zLabel = CreateLabel("Z:", 0, 4);
                                TextBox zBox = CreateTextBox(component, fields[i], "z", zLabel, true, 0, 5, 1);
                                fieldGrid.Children.Add(zLabel);
                                fieldGrid.Children.Add(zBox);
                            }
                        }
                        else {
                            foundType = false;
                        }
                    }

                    if (foundType) {
                        compGrid.Children.Add(fieldLabel);
                        compGrid.Children.Add(fieldGrid);
                    }
                }

                Border border = new Border() {
                    BorderThickness = new Thickness(0, 0, 0, 1),
                    BorderBrush = Brushes.Gray,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                panel.Children.Add(border);
            }

            Button addButton = new Button() {
                Content = "Add new component..."
            };
            addButton.Click += AddButton_Click;
            panel.Children.Add(addButton);
            panel.Children.Add(addPanel);
        }

        private void RefreshValues() {
            window.Dispatcher.Invoke(() => {
                foreach (TextBox textBox in textBoxes) {
                    Component component = (Component)textBox.GetValue(componentProperty);
                    FieldInfo field = (FieldInfo)textBox.GetValue(fieldProperty);
                    string fieldData = (string)textBox.GetValue(fieldDataProperty);

                    string newValue = ProcessField(component, field, fieldData);
                    if (textBox.Text != newValue && !textBox.IsKeyboardFocused) {
                        SetTextWithoutEvent(textBox, newValue);
                    }
                }
            });
        }

        public void StartRefreshTimer() {
            refreshTimer.Start();
        }

        public void StopRefreshTimer() {
            refreshTimer.Stop();
        }

        private string ProcessField(Component component, FieldInfo field, string fieldData) {
            if (fieldData != "") {
                return field.FieldType.GetField(fieldData).GetValue(field.GetValue(component)).ToString();
            }
            else {
                return field.GetValue(component).ToString();
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e) {
            Button button = (Button)sender;
            framework.RemoveComponent((Component)button.GetValue(componentProperty), entity);
            SetEntity(entity);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e) {
            ToggleAddPanel();
        }

        private void ToggleAddPanel() {
            if (addPanel.Visibility == Visibility.Visible) {
                addPanel.Visibility = Visibility.Collapsed;
            }
            else {
                addPanel.Visibility = Visibility.Visible;
            }
        }

        private void DefineTable(Grid grid, int rows, int columns) {
            for (int i = 0; i < rows; i++) {
                grid.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < columns; i++) {
                grid.ColumnDefinitions.Add(new ColumnDefinition());
            }
        }

        private Label CreateLabel(string text, int row, int column) {
            Label label = new Label() {
                Content = text
            };

            Grid.SetRow(label, row);
            Grid.SetColumn(label, column);

            return label;
        }

        private TextBox CreateTextBox(Component component, FieldInfo field, string fieldData, Label linkedLabel, bool isDecimal, int row, int column, int columnSpan) {
            TextBox textBox = new TextBox {
                VerticalContentAlignment = VerticalAlignment.Center,
            };

            textBox.Loaded += delegate {
                try {
                    initTextBoxAssignment = true;
                    textBox.Text = ProcessField(component, field, fieldData);
                }
                finally {
                    initTextBoxAssignment = false;
                }
            };
            textBox.SizeChanged += StopTextBoxStretch;
            textBox.TextChanged += TextBox_TextChanged;
            textBox.SetValue(componentProperty, component);
            textBox.SetValue(fieldProperty, field);
            textBox.SetValue(fieldDataProperty, fieldData);

            textBoxes.Add(textBox);

            if (linkedLabel != null) {
                textBox.Loaded += (object sender, RoutedEventArgs e) => {
                    NumericLabelBehavior.SetParseNumeric(textBox, true);
                    NumericLabelBehavior.SetIsDecimal(textBox, isDecimal);
                    NumericLabelBehavior.SetDragBox(linkedLabel, textBox);
                };
            }

            Grid.SetRow(textBox, row);
            Grid.SetColumn(textBox, column);
            Grid.SetColumnSpan(textBox, columnSpan);

            return textBox;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            if (skipTextChanged) {
                return;
            }

            TextBox textBox = (TextBox)sender;
            Component component = (Component)textBox.GetValue(componentProperty);
            FieldInfo field = (FieldInfo)textBox.GetValue(fieldProperty);
            string fieldData = (string)textBox.GetValue(fieldDataProperty);

            TypeConverter converter = TypeDescriptor.GetConverter(field.FieldType);
            if (converter.CanConvertFrom(typeof(string))) {
                object value = converter.ConvertFrom(textBox.Text);
                field.SetValue(component, value);
            }
            else if (field.FieldType == typeof(Vector3)) {
                Vector3 prevValue = (Vector3)field.GetValue(component);
                bool canParseValue = float.TryParse(textBox.Text, out float value);
                if (canParseValue) {
                    switch (fieldData) {
                        case "x":
                            field.SetValue(component, new Vector3(value, prevValue.y, prevValue.z));
                            break;
                        case "y":
                            field.SetValue(component, new Vector3(prevValue.x, value, prevValue.z));
                            break;
                        case "z":
                            field.SetValue(component, new Vector3(prevValue.x, prevValue.y, value));
                            break;
                        default:
                            break;
                    }
                }
            }

            if (!initTextBoxAssignment) {
                if (component.GetType() == typeof(EntityInfo)) {
                    window.entityBarManager.RefreshEntityName(((EntityInfo)component).entity);
                }
                window.renderManager.RequestPreview();
            }
        }

        private CheckBox CreateCheckBox(int row, int column) {
            CheckBox checkBox = new CheckBox();
            Grid.SetRow(checkBox, row);
            Grid.SetColumn(checkBox, column);
            return checkBox;
        }

        private static string FormatFieldName(string name) {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < name.Length; i++) {
                if (i > 0 && name[i] >= 'A' && name[i] <= 'Z' && name[i - 1] >= 'a' && name[i - 1] <= 'z') {
                    builder.Append(' ');
                    builder.Append(name[i]);
                }
                else if (i == 0 && name[i] >= 'a' && name[i] <= 'z') {
                    builder.Append((char)(name[i] + ('A' - 'a')));
                }
                else {
                    builder.Append(name[i]);
                }
            }
            return builder.ToString();
        }

        private void StopTextBoxStretch(object sender, SizeChangedEventArgs e) {
            TextBox textBox = (TextBox)sender;
            if ((initTextBoxAssignment || textBox.CanUndo) && e.NewSize.Width > e.PreviousSize.Width) {
                textBox.Width = e.PreviousSize.Width;
            }
        }

        public static void SetComponentType(UIElement element, Type value) {
            element.SetValue(componentTypeProperty, value);
        }

        public static void SetComponent(UIElement element, Component value) {
            element.SetValue(componentProperty, value);
        }

        public static void SetField(UIElement element, FieldInfo value) {
            element.SetValue(fieldProperty, value);
        }

        public static void SetFieldData(UIElement element, string value) {
            element.SetValue(fieldDataProperty, value);
        }

        public static Type GetComponentType(UIElement element) {
            return (Type)element.GetValue(componentTypeProperty);
        }

        public static Component GetComponent(UIElement element) {
            return (Component)element.GetValue(componentProperty);
        }

        public static FieldInfo GetField(UIElement element) {
            return (FieldInfo)element.GetValue(fieldProperty);
        }

        public static string GetFieldData(UIElement element) {
            return (string)element.GetValue(fieldDataProperty);
        }

    }
}