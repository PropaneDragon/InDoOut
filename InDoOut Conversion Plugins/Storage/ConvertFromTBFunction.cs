namespace InDoOut_Conversion_Plugins.Storage
{
    public class ConvertFromTBFunction : AbstractStorageConversionFunction
    {
        public override string[] Keywords => new[] { "terabytes", "tebibytes", "tebibits", "terrabytes", "terabits", "terabites", "terrabites", "terrabits", "binary" };

        public override StorageSize.SizeType AssociatedSizeType => StorageSize.SizeType.TiB;
    }
}
