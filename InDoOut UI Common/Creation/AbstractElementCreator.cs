using InDoOut_Core.Basic;
using System.Windows;

namespace InDoOut_UI_Common.Creation
{
    public abstract class AbstractElementCreator : IElementCreator
    {
        protected bool ExtractPointFromMetadata(IStored stored, out Point location)
        {
            location = new Point();

            if (stored != null && ExtractMetadataValue(stored, "x", out var x) && ExtractMetadataValue(stored, "y", out var y))
            {
                location = new Point(x, y);

                return true;
            }

            return false;
        }

        protected bool ExtractRectFromMetadata(IStored stored, out Rect rect)
        {
            rect = new Rect();

            if (stored != null && ExtractPointFromMetadata(stored, out var location) && ExtractMetadataValue(stored, "w", out var width) && ExtractMetadataValue(stored, "h", out var height))
            {
                rect = new Rect(location, new Size(width, height));

                return true;
            }

            return false;
        }

        protected bool ExtractThicknessFomMetadata(IStored stored, out Thickness thickness)
        {
            thickness = new Thickness();

            if (stored != null && ExtractMetadataValue(stored, "l", out var left) && ExtractMetadataValue(stored, "t", out var top) && ExtractMetadataValue(stored, "r", out var right) && ExtractMetadataValue(stored, "b", out var bottom))
            {
                thickness = new Thickness(left, top, right, bottom);

                return true;
            }

            return false;
        }

        protected bool ExtractMetadataValue(IStored stored, string key, out double value)
        {
            value = 0;
            return !string.IsNullOrEmpty(key) && stored != null && (stored?.Metadata?.ContainsKey(key) ?? false) && double.TryParse(stored.Metadata[key], out value);
        }
    }
}
