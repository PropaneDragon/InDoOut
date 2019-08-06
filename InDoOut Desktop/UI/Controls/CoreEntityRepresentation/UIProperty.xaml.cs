using InDoOut_Core.Entities.Functions;
using InDoOut_Desktop.UI.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace InDoOut_Desktop.UI.Controls.CoreEntityRepresentation
{
    public partial class UIProperty : UserControl, IUIProperty
    {
        private IProperty _property = null;

        public IProperty AssociatedProperty { get => _property; set => SetProperty(value); }

        public UIProperty() : base()
        {
            InitializeComponent();
        }

        public UIProperty(IProperty property) : this()
        {
            AssociatedProperty = property;
        }

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
            }
        }

        private void UpdateEditorFromProperty()
        {
            if (AssociatedProperty != null)
            {
                TextBox_ValueEdit.Text = AssociatedProperty.RawValue ?? "";
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
            TextBox_ValueEdit.Visibility = Visibility.Visible;

            IO_Overlay.BeginAnimation(OpacityProperty, fadeInAnimation);
            TextBox_ValueEdit.BeginAnimation(OpacityProperty, fadeInAnimation);
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
            fadeOutAnimation.Completed += (sender, e) => Button_Edit.Visibility = System.Windows.Visibility.Hidden;

            Button_Edit.BeginAnimation(OpacityProperty, fadeOutAnimation);
        }

        private void Button_Edit_Click(object sender, RoutedEventArgs e)
        {
            ShowEditor();
        }

        private void TextBox_ValueEdit_LostFocus(object sender, RoutedEventArgs e)
        {
            HideEditor();
        }

        private void TextBox_ValueEdit_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
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
    }
}
