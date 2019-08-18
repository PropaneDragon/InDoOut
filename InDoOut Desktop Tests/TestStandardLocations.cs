using InDoOut_Desktop.Location;
using InDoOut_Executable_Core.Location;

namespace InDoOut_Desktop_Tests
{
    internal class TestStandardLocations : StandardLocations
    {
        public void PublicForcePathTo(Location location, string path) => ForcePathTo(location, path);
    }
}
