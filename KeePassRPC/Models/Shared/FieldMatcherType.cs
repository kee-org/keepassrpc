namespace KeePassRPC.Models.Shared
{
    // For standard KeePass entries with no KPRPC-specific config, we can save storage space (and one day data-exchange bytes) by just recording that the client should use a typical locator to work out which field is the best match, because we have no additional information to help with this task.
    
    // We could extend this to very common additional heuristics in future (e.g. if many sites and entries end up with a custom locator with Id and Name == "password"). That would be pretty complex though so probably won't be worthwhile. 
    public enum FieldMatcherType
    {
        Custom = 0,
        UsernameDefaultHeuristic = 1,
        PasswordDefaultHeuristic = 2
    }
}