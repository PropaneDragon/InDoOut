using InDoOut_Core.Instancing;
using InDoOut_Core.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using InDoOut_UI_Common.Controls.Options.Types;

namespace InDoOut_UI_Common.Controls.Options
{
    public class OptionInterfaceFactory : Singleton<OptionInterfaceFactory>, IOptionInterfaceFactory
    {
        private readonly Dictionary<Type, Type> _optionToInterfaceAssociations = new Dictionary<Type, Type>();

        public OptionInterfaceFactory()
        {
        }

        public ILinkedInterfaceOption GetInterfaceOptionFor<OptionType>() where OptionType : IOption
        {
            return GetInterfaceOptionFor(typeof(OptionType));
        }

        public ILinkedInterfaceOption GetInterfaceOptionFor(Type type)
        {
            //Todo: Check possible security issues with this.

            if (type != null && typeof(IOption).IsAssignableFrom(type))
            {
                if (_optionToInterfaceAssociations.ContainsKey(type))
                {
                    return TryBuildInterfaceType(_optionToInterfaceAssociations[type]);
                }
                else
                {
                    var typeName = type.Name;
                    if (!string.IsNullOrEmpty(typeName))
                    {
                        var proposedInterfaceName = $"{typeName}Interface";
                        var proposedInterfaceType = Assembly.GetExecutingAssembly().DefinedTypes.FirstOrDefault(type => type.Name == proposedInterfaceName);
                        if (proposedInterfaceType != null)
                        {
                            _optionToInterfaceAssociations[type] = proposedInterfaceType;

                            return TryBuildInterfaceType(proposedInterfaceType);
                        }
                    }
                }
            }

            return null;
        }

        private ILinkedInterfaceOption TryBuildInterfaceType(Type type)
        {
            if (type != null)
            {
                var instanceBuilder = new InstanceBuilder<ILinkedInterfaceOption>();
                var interfaceOptionInstance = instanceBuilder.BuildInstance(type);
                if (interfaceOptionInstance != null)
                {
                    return interfaceOptionInstance;
                }
            }

            return null;
        }
    }
}
