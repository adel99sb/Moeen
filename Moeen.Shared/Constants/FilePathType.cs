namespace Moeen.Shared.Constants
{
    public enum FilePathType
    {
        UserProfiles = 0,
        LibraryBooks = 1
    }

    public static class FilePathConstants
    {
        public static readonly Dictionary<FilePathType, string> PathMappings = new()
        {
            { FilePathType.UserProfiles, $"uploads/UserProfiles/" },
            { FilePathType.LibraryBooks, $"uploads/LibraryBooks/" }
        };
    }
}
