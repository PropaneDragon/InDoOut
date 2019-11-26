namespace InDoOut_Conversion_Plugins.Storage
{
    public class ConvertFromEBFunction : AbstractStorageConversionFunction
    {
        public override string[] Keywords => new[] { "exabytes", "exbibytes", "exbibits", "exabits", "exibytes", "exobytes", "binary" };

        public override StorageSize.SizeType AssociatedSizeType => StorageSize.SizeType.EiB;
    }
}
