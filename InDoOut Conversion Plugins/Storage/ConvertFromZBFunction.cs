namespace InDoOut_Conversion_Plugins.Storage
{
    public class ConvertFromZBFunction : AbstractStorageConversionFunction
    {
        public override string[] Keywords => new[] { "zettabytes", "zebibytes", "zebibits", "zetabytes", "zettabites", "zettabits", "binary" };

        public override StorageSize.SizeType AssociatedSizeType => StorageSize.SizeType.ZiB;
    }
}
