namespace InDoOut_Conversion_Plugins.Storage
{
    public class ConvertFromMBFunction : AbstractStorageConversionFunction
    {
        public override string[] Keywords => new[] { "megabytes", "mebibytes", "mebabyte", "mebibits", "megabites", "megabits", "binary" };

        public override StorageSize.SizeType AssociatedSizeType => StorageSize.SizeType.MiB;
    }
}
