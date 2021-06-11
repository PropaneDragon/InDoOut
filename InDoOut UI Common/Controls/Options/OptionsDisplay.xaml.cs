using InDoOut_Core.Options;
using InDoOut_UI_Common.Controls.Options.Types;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace InDoOut_UI_Common.Controls.Options
{
    public partial class OptionsDisplay : UserControl, IOptionsDisplay
    {
        private readonly Dictionary<ILinkedInterfaceOption, IOption> _optionAssociations = new Dictionary<ILinkedInterfaceOption, IOption>();

        public string Title { get => Text_Title.Text; set => Text_Title.Text = value; }

        public List<IOption> AssociatedOptions { get => _optionAssociations.Values.ToList(); set => PopulateForOptions(value); }

        public OptionsDisplay()
        {
            InitializeComponent();

            Title = "";
        }

        public OptionsDisplay(string title, List<IOption> options) : this()
        {
            Title = title;
            AssociatedOptions = options;
        }

        public void PopulateForOptions(List<IOption> options)
        {
            Wrap_Options.Children.Clear();
            _optionAssociations.Clear();

            var optionInterfaceFactory = new OptionInterfaceFactory();

            foreach (var option in options)
            {
                if (option.Visible)
                {
                    var interfaceOption = optionInterfaceFactory.GetInterfaceOptionFor(option.GetType());
                    if (interfaceOption != null && interfaceOption is UIElement element && interfaceOption.UpdateFromOption(option))
                    {
                        _ = Wrap_Options.Children.Add(element);

                        _optionAssociations[interfaceOption] = option;
                    }
                }
            }
        }

        public void CommitChanges()
        {
            foreach (var association in _optionAssociations)
            {
                if (association.Key.UpdateOptionValue(association.Value))
                {
                    _ = association.Key.UpdateFromOption(association.Value);
                }
            }
        }
    }
}
