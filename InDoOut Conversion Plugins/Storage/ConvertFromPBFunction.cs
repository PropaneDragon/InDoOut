namespace InDoOut_Conversion_Plugins.Storage
{
    public class ConvertFromPBFunction : AbstractStorageConversionFunction
    {
        public override string[] Keywords => new[] { "petabytes", "pebibytes", "pebibits", "pettabytes", "petabits", "pettabits", "petabites", "pettabites", "binary" };

        public override StorageSize.SizeType AssociatedSizeType => StorageSize.SizeType.PiB;
    }
}
