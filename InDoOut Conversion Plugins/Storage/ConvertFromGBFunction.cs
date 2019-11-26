namespace InDoOut_Conversion_Plugins.Storage
{
    public class ConvertFromGBFunction : AbstractStorageConversionFunction
    {
        public override string[] Keywords => new[] { "gigabytes", "gibibytes", "gibibits", "gigobytes", "gigabits", "binary" };

        public override StorageSize.SizeType AssociatedSizeType => StorageSize.SizeType.GiB;
    }
}
