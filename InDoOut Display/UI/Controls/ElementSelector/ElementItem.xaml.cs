using InDoOut_Display_Core.Elements;
using InDoOut_Display_Core.Functions;
using System;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_Display.UI.Controls.ElementSelector
{
    public partial class ElementItem : UserControl
    {
        private IDisplayElement _displayElement = null;

        public IDisplayElement DisplayElement { get => _displayElement; set => ChangeDisplayElement(value); }

        public event EventHandler<EventArgs> ElementSelected;

        public ElementItem()
        {
            InitializeComponent();
        }

        public bool LoadElementFromFunction(IElementFunction function)
        {
            var uiElement = function?.CreateAssociatedUIElement();
            if (uiElement != null && uiElement is UIElement)
            {
                DisplayElement = uiElement;

                return true;
            }

            return false;
        }

        public bool LoadElementFromFunction<T>() where T : class, IElementFunction
        {
            var functionBuilder = new ElementFunctionBuilder();
            var functionInstance = functionBuilder.BuildInstance<T>();

            return functionInstance != null && LoadElementFromFunction(functionInstance);
        }

        private void ChangeDisplayElement(IDisplayElement element)
        {
            _displayElement = element;

            if (element is UIElement uiElement)
            {
                Text_Name.Text = element.AssociatedElementFunction?.SafeName ?? "Invalid";
                Border_Container.Child = uiElement;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) => ElementSelected?.Invoke(this, e);
    }
}
