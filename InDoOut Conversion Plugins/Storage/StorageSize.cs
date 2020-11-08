using System;
using System.Linq;

namespace InDoOut_Conversion_Plugins.Storage
{
    public class StorageSize
    {
        public enum SizeType
        {
            Byte,
            KiB,
            MiB,
            GiB,
            TiB,
            PiB,
            EiB,
            ZiB,
            YiB
        }

        private static readonly bool _allowNegative = false;

        /// <summary>
        /// Represents a storage size with convenience methods to handle basic arithmetic of sizes and
        /// conversions between different size types (MB -> KB) for example.
        /// </summary>
        /// <param name="bytes">Initial bytes to create the storage with</param>
        public StorageSize(double bytes)
        {
            Bytes = Math.Round(bytes);
        }

        public double Bytes { get; } = 0;
        public double KiB => ConvertBetween(Bytes, SizeType.Byte, SizeType.KiB);
        public double MiB => ConvertBetween(Bytes, SizeType.Byte, SizeType.MiB);
        public double GiB => ConvertBetween(Bytes, SizeType.Byte, SizeType.GiB);
        public double TiB => ConvertBetween(Bytes, SizeType.Byte, SizeType.TiB);
        public double PiB => ConvertBetween(Bytes, SizeType.Byte, SizeType.PiB);
        public double EiB => ConvertBetween(Bytes, SizeType.Byte, SizeType.EiB);
        public double ZiB => ConvertBetween(Bytes, SizeType.Byte, SizeType.ZiB);
        public double YiB => ConvertBetween(Bytes, SizeType.Byte, SizeType.YiB);

        public static StorageSize FromBytes(double bytes) => new StorageSize(bytes);
        public static StorageSize FromKiB(double size) => FromSize(SizeType.KiB, size);
        public static StorageSize FromMiB(double size) => FromSize(SizeType.MiB, size);
        public static StorageSize FromGiB(double size) => FromSize(SizeType.GiB, size);
        public static StorageSize FromTiB(double size) => FromSize(SizeType.TiB, size);
        public static StorageSize FromPiB(double size) => FromSize(SizeType.PiB, size);
        public static StorageSize FromEiB(double size) => FromSize(SizeType.EiB, size);
        public static StorageSize FromZiB(double size) => FromSize(SizeType.ZiB, size);
        public static StorageSize FromYiB(double size) => FromSize(SizeType.YiB, size);
        public static StorageSize FromSize(SizeType size, double amount) => new StorageSize(ConvertBetween(amount, size, SizeType.Byte));

        /// <summary>
        /// Converts a file size from one type to another
        /// </summary>
        /// <param name="sizeIn">The file size in the storage type given by <paramref name="sizeTypeIn"/></param>
        /// <param name="sizeTypeIn">The storage size type of the given <paramref name="sizeIn"/></param>
        /// <param name="sizeTypeOut">The storage size to get out</param>
        /// <returns></returns>
        public static double ConvertBetween(double sizeIn, SizeType sizeTypeIn, SizeType sizeTypeOut)
        {
            if (sizeTypeIn < sizeTypeOut)
            {
                sizeTypeIn += 1;
                sizeIn /= 1024d;

                return ConvertBetween(sizeIn, sizeTypeIn, sizeTypeOut);
            }
            else if (sizeTypeIn > sizeTypeOut)
            {
                sizeTypeIn -= 1;
                sizeIn *= 1024d;

                return ConvertBetween(sizeIn, sizeTypeIn, sizeTypeOut);
            }

            return sizeIn;
        }

        /// <summary>
        /// Returns a formatted string of a file size to the nearest <see cref="SizeType"/> to 2 decimal places.
        /// </summary>
        /// <returns>A formatted size string</returns>
        public override string ToString() => ToString(RecommendedSizeType());

        /// <summary>
        /// Returns a formatted string of a file size to the nearest <see cref="SizeType"/> to the given number of decimal places.
        /// </summary>
        /// <param name="decimals">The number of decimal places to round to</param>
        /// <returns>A formatted size string</returns>
        public string ToString(int decimals) => ToString(RecommendedSizeType(), decimals);

        /// <summary>
        /// Returns a formatted string of a file size of the given <see cref="SizeType"/> to the given number of decimal places. 
        /// </summary>
        /// <param name="size">The size to output</param>
        /// <param name="decimals">The number of decimal places to round to</param>
        /// <returns>A formatted size string</returns>
        public string ToString(SizeType size, int decimals = 2) => $"{Math.Round(ConvertBetween(Bytes, SizeType.Byte, size), decimals)} {SizeTypeToString(size, Bytes)}";

        public override bool Equals(object obj)
        {
            if (obj is StorageSize)
            {
                var storageSize = obj as StorageSize;
                return this == storageSize;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode() => base.GetHashCode();

        private SizeType RecommendedSizeType()
        {
            var values = Enum.GetValues(typeof(SizeType)).Cast<SizeType>();
            var maxSize = false;

            foreach (var value in values)
            {
                var nextSize = values.FirstOrDefault(item => item > value);
                if (FromSize(nextSize, 1) > this)
                {
                    return value;
                }
                else
                {
                    maxSize = true;
                }
            }

            return maxSize ? values.Last() : values.First();
        }

        public static string SizeTypeToString(SizeType sizeType, double value = 1)
        {
            return sizeType switch
            {
                SizeType.Byte => $"{sizeType}{(value == 1 ? "" : "s")}",
                _ => sizeType.ToString(),
            };
        }

        public static StorageSize operator -(StorageSize left, StorageSize right)
        {
            var result = left.Bytes - right.Bytes;

            if (!_allowNegative && result < 0)
            {
                result = 0;
            }

            return FromBytes(result);
        }

        public static StorageSize operator +(StorageSize left, StorageSize right)
        {
            var result = left.Bytes + right.Bytes;

            if (!_allowNegative && result < 0)
            {
                result = 0;
            }

            return FromBytes(result);
        }

        public static bool operator ==(StorageSize left, StorageSize right) => left.Bytes == right.Bytes;
        public static bool operator !=(StorageSize left, StorageSize right) => !(left == right);
        public static bool operator <(StorageSize left, StorageSize right) => left.Bytes < right.Bytes;
        public static bool operator >(StorageSize left, StorageSize right) => left.Bytes > right.Bytes;
        public static bool operator <=(StorageSize left, StorageSize right) => left.Bytes <= right.Bytes;
        public static bool operator >=(StorageSize left, StorageSize right) => left.Bytes >= right.Bytes;
    }
}
