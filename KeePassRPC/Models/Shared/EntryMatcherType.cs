namespace KeePassRPC.Models.Shared
{
    public enum EntryMatcherType
    {
        Custom = 0,
        Hide = 1,
        Url = 2, // magic type that uses primary URL + the 4 URL data arrays and current urlmatchconfig to determine a match
    }
}