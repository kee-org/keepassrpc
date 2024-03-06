namespace KeePassRPC.Models.DataExchange
{
    public class Configuration
    {
        //bool allowUnencryptedMetaData; // doesn't affect encryption of passwords themselves
        //KPDatabaseList knownDatabases; // the MRU list (to expand this in v1+, maybe Firefox preferences can be used?)
        public string[] KnownDatabases;
        public bool AutoCommit; // whether KeePass should save the active database after every change

        public Configuration() { }
        public Configuration(string[] mruList, bool autoCommit)
        {
            KnownDatabases = mruList;
            AutoCommit = autoCommit;
        }
    }
}