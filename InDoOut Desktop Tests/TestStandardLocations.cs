using InDoOut_Desktop.Location;

namespace InDoOut_Desktop_Tests
{
    internal class TestStandardLocations : StandardLocations
    {
        public void PublicForcePathTo(Location location, string path) => ForcePathTo(location, path);
    }
}
