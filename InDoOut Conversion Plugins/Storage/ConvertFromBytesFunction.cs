namespace InDoOut_Conversion_Plugins.Storage
{
    public class ConvertFromBytesFunction : AbstractStorageConversionFunction
    {
        public override string[] Keywords => new[] { "bytes", "bits", "binary" };

        public override StorageSize.SizeType AssociatedSizeType => StorageSize.SizeType.Byte;
    }
}
