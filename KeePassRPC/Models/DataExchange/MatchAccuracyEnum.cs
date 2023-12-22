namespace KeePassRPC.Models.DataExchange
{
    public enum MatchAccuracyEnum
    {
        // Best = Non-URL match (i.e. we matched by UUID instead)
        // Best = Regex match (it is impossible for us to infer how
        // accurate a regex match is in comparison with other classes
        // of match so we always treat it as the best possible match
        // even if the regex itself is very loose)
        // Best = Same URL including query string

        // Close = Same URL excluding query string

        // HostnameAndPort = Same hostname and port

        // HostnameExcludingPort = Same hostname (domain + subdomains)

        // Domain = Same domain

        // None = No match (e.g. when we are being asked to return all entries)

        Best = 50,
        Close = 40,
        HostnameAndPort = 30,
        HostnameExcludingPort = 20,
        Domain = 10,
        None = 0
    }
}