using InDoOut_Plugins.Options;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_Desktop.UI.Controls.Options
{
    public partial class OptionsDisplay : UserControl
    {
        public OptionsDisplay()
        {
            InitializeComponent();
        }

        public void PopulateForOptions(List<IOption> options)
        {
            Wrap_Options.Children.Clear();

            var optionInterfaceFactory = new OptionInterfaceFactory();

            foreach (var option in options)
            {
                var interfaceOption = optionInterfaceFactory.GetInterfaceOptionFor(option.GetType());
                if (interfaceOption != null && interfaceOption is UIElement element)
                {
                    interfaceOption.UpdateFromOption(option);

                    _ = Wrap_Options.Children.Add(element);
                }
            }
        }
    }
}
