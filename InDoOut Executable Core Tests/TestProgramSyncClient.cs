using InDoOut_Executable_Core.Networking;

namespace InDoOut_Executable_Core_Tests
{
    internal class TestProgramSyncClient : ProgramSyncClient
    {
        public static string SplitIdentifier => SPLIT_IDENTIFIER_STRING;

        public TestProgramSyncClient() : base()
        {
        }
    }
}
