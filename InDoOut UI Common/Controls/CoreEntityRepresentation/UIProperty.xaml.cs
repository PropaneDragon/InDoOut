using InDoOut_Core.Entities.Functions;
using InDoOut_UI_Common.Actions.Copying;
using InDoOut_UI_Common.InterfaceElements;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace InDoOut_UI_Common.Controls.CoreEntityRepresentation
{
    public partial class UIProperty : UserControl, IUIProperty
    {
        private IProperty _property = null;
        private DispatcherTimer _valueUpdateTimer = null;

        public IProperty AssociatedProperty { get => _property; set => SetProperty(value); }

        public UIProperty() : base()
        {
            InitializeComponent();
        }

        public UIProperty(IProperty property) : this()
        {
            AssociatedProperty = property;
        }

        public void PositionUpdated(Point position)
        {
            if (AssociatedProperty != null)
            {
                AssociatedProperty.Metadata["x"] = position.X.ToString();
                AssociatedProperty.Metadata["y"] = position.Y.ToString();
                AssociatedProperty.Metadata["w"] = ActualWidth.ToString();
                AssociatedProperty.Metadata["h"] = ActualHeight.ToString();
            }
        }

        public bool CopyTo(ICopyable other)
        {
            if (other != null && other is UIProperty otherProperty)
            {
                var otherAssociatedProperty = otherProperty.AssociatedProperty;
                if (otherAssociatedProperty != null && AssociatedProperty != null)
                {
                    otherAssociatedProperty.LastSetValue = AssociatedProperty.LastSetValue;
                    otherAssociatedProperty.RawValue = AssociatedProperty.RawValue;

                    return true;
                }
            }

            return false;
        }

        public bool CanCopy(ICommonDisplay display) => true;

        public ICopyable CreateCopy(ICommonDisplay display) => null;

        private void SetProperty(IProperty property)
        {
            if (_property != null)
            {
                //Todo: Teardown old property
            }

            _property = property;

            if (_property != null)
            {
                //Warning: Potential bug? INamed has no safe version.
                IO_Main.Text = _property.Name;
                IO_Main.Value = "";
            }
        }

        private void UpdateEditorFromProperty()
        {
            if (AssociatedProperty != null)
            {
                TextBox_ValueEdit.Text = AssociatedProperty?.RawValue ?? "";
            }
        }

        private void UpdatePropertyFromEditor()
        {
            if (AssociatedProperty != null)
            {
                AssociatedProperty.RawValue = TextBox_ValueEdit.Text;
            }
        }

        private void ShowEditor()
        {
            UpdateEditorFromProperty();

            var fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromMilliseconds(200));

            IO_Overlay.Visibility = Visibility.Visible;
            IO_Overlay.BeginAnimation(OpacityProperty, fadeInAnimation);

            TextBox_ValueEdit.Visibility = Visibility.Visible;
            TextBox_ValueEdit.BeginAnimation(OpacityProperty, fadeInAnimation);

            _ = TextBox_ValueEdit.Focus();
        }

        private void HideEditor()
        {
            var fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200));
            fadeOutAnimation.Completed += (sender, e) =>
            {
                IO_Overlay.Visibility = Visibility.Collapsed;
                TextBox_ValueEdit.Visibility = Visibility.Collapsed;
            };

            IO_Overlay.BeginAnimation(OpacityProperty, fadeOutAnimation);
            TextBox_ValueEdit.BeginAnimation(OpacityProperty, fadeOutAnimation);
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            var fadeInAnimation = new DoubleAnimation(1, TimeSpan.FromMilliseconds(200));

            Button_Edit.Visibility = Visibility.Visible;
            Button_Edit.BeginAnimation(OpacityProperty, fadeInAnimation);
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            var fadeOutAnimation = new DoubleAnimation(0, TimeSpan.FromMilliseconds(200));
            fadeOutAnimation.Completed += (sender, e) => Button_Edit.Visibility = Visibility.Hidden;

            Button_Edit.BeginAnimation(OpacityProperty, fadeOutAnimation);
        }

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            ShowEditor();
        }

        private void IO_Main_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ShowEditor();
        }

        private void TextBox_ValueEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            UpdatePropertyFromEditor();
            HideEditor();
        }

        private void TextBox_ValueEdit_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            UpdatePropertyFromEditor();
            HideEditor();
        }

        private void TextBox_ValueEdit_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            var committed = e.Key switch
            {
                Key.Enter => true,
                _ => false
            };

            var escaped = e.Key switch
            {
                Key.Escape => true,
                _ => false
            };

            if (AssociatedProperty != null)
            {
                if (committed)
                {
                    UpdatePropertyFromEditor();
                    HideEditor();
                }
                else if (escaped)
                {
                    UpdateEditorFromProperty();
                    HideEditor();
                }
            }
        }

        private async void UpdateTimer_Tick(object sender, EventArgs e)
        {
            var propertyValue = await Task.Run(() => AssociatedProperty?.RawComputedValue ?? "");
            IO_Main.Value = propertyValue ?? "";
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(0, 333)));

            _valueUpdateTimer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = TimeSpan.FromMilliseconds(333)
            };

            _valueUpdateTimer.Start();
            _valueUpdateTimer.Tick += UpdateTimer_Tick;

            IO_Main.Value = "updating...";
            IO_Main.Value = "";
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _valueUpdateTimer.Stop();
            _valueUpdateTimer.Tick -= UpdateTimer_Tick;
            _valueUpdateTimer = null;
        }
    }
}
