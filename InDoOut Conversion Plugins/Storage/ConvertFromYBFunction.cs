namespace InDoOut_Conversion_Plugins.Storage
{
    public class ConvertFromYBFunction : AbstractStorageConversionFunction
    {
        public override string[] Keywords => new[] { "yottabytes", "yobibyte", "yobibits", "yotabytes", "yottabites", "yotabites", "binary" };

        public override StorageSize.SizeType AssociatedSizeType => StorageSize.SizeType.YiB;
    }
}
