namespace InDoOut_Executable_Core.Storage
{
    public enum LoadResult
    {
        Unknown = -1,
        OK = 0,
        InvalidLocation,
        InvalidFile,
        InvalidExtension,
        InsufficientPermissions,
        MissingData,
        WrongVersion
    }

    public enum SaveResult
    {
        Unknown = -1,
        OK = 0,
        InvalidLocation,
        InsufficientPermissions,
        InvalidFileName
    }
}
