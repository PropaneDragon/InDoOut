using InDoOut_Core.Entities.Functions;
using System;
using System.Collections.Generic;

namespace InDoOut_Conversion_Plugins.Storage
{
    public abstract class AbstractStorageConversionFunction : Function
    {
        private readonly IOutput _output, _failed;
        private readonly IProperty<double> _value;
        private Dictionary<StorageSize.SizeType, IResult> _results;

        public override string Description => $"Converts from {StorageSize.SizeTypeToString(AssociatedSizeType, 2)} to other sizes.";

        public override string Name => $"Convert from {StorageSize.SizeTypeToString(AssociatedSizeType, 2)}";

        public override string Group => "Conversion - Storage";

        public override IOutput TriggerOnFailure => _failed;

        public abstract StorageSize.SizeType AssociatedSizeType { get; }

        public AbstractStorageConversionFunction()
        {
            _ = CreateInput();

            _output = CreateOutput();
            _failed = CreateOutput("Failed to convert", OutputType.Negative);

            _value = AddProperty(new Property<double>(StorageSize.SizeTypeToString(AssociatedSizeType, 2), "The value to convert from.", true, 0));

            CreateStorageOutputs(AssociatedSizeType);
        }

        protected override IOutput Started(IInput triggeredBy)
        {
            foreach (var result in _results)
            {
                var storageType = result.Key;
                var resultEntity = result.Value;

                _ = resultEntity.ValueFrom(StorageSize.ConvertBetween(_value.FullValue, AssociatedSizeType, storageType));
            }

            return _output;
        }

        private void CreateStorageOutputs(StorageSize.SizeType type)
        {
            _results = new Dictionary<StorageSize.SizeType, IResult>();

            var enumValues = Enum.GetValues(typeof(StorageSize.SizeType));

            foreach (StorageSize.SizeType storageType in enumValues)
            {
                if (storageType != type)
                {
                    var result = AddResult(new Result(StorageSize.SizeTypeToString(storageType, 2), $"The {StorageSize.SizeTypeToString(storageType, 1)} equivalent of the input value", "0"));
                    _results.Add(storageType, result);
                }
            }
        }
    }
}
