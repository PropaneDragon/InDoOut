namespace InDoOut_Conversion_Plugins.Storage
{
    public class ConvertFromKBFunction : AbstractStorageConversionFunction
    {
        public override string[] Keywords => new[] { "kilobytes", "kibibytes", "kibibits", "kilobits", "kilabytes", "kilobites", "kilabytes", "binary" };

        public override StorageSize.SizeType AssociatedSizeType => StorageSize.SizeType.KiB;
    }
}
