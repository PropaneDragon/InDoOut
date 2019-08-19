using InDoOut_Executable_Core.Location;

namespace InDoOut_Executable_Core_Tests
{
    public class TestStandardLocations : StandardLocations
    {
        public void PublicForcePathTo(Location location, string path) => ForcePathTo(location, path);
    }
}
