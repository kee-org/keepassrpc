using DomainPublicSuffix;

namespace KeePassRPC
{
    public class URLSummary
    {
        public string HostnameAndPort;
        public string Port;
        public DomainName Domain;

        private URLSummary(string hostnameAndPort, string port, DomainName domain)
        {
            HostnameAndPort = hostnameAndPort;
            Port = port;
            Domain = domain;
        }

        public static URLSummary FromURL(string URL)
        {
            if (URL.StartsWith("data:"))
            {
                return new URLSummary("", "", null);
            }

            bool isFile = false;
            int protocolIndex = URL.IndexOf("://");
            string hostAndPort = "";
            if (URL.StartsWith("file://"))
            {
                isFile = true;
                // the "host and port" of a file is the actual file name
                // (i.e. everything except the query string)
                int qsIndex = URL.IndexOf("?");
                if (qsIndex > -1)
                    hostAndPort = URL.Substring(7, qsIndex - 7);
                else if (URL.Length > 7)
                    hostAndPort = URL.Substring(7);
            }
            else if (protocolIndex > -1)
            {
                string URLExcludingProt = URL.Substring(protocolIndex + 3);
                int pathStart = URLExcludingProt.IndexOf("/", 0);

                if (pathStart > -1 && URLExcludingProt.Length > pathStart)
                {
                    hostAndPort = URL.Substring(protocolIndex + 3, pathStart);
                }
                else if (pathStart == -1) // it's already just a hostname and optional port
                {
                    hostAndPort = URLExcludingProt;
                }
            }
            else
            {
                // we haven't received a protocol

                string URLExcludingProt = URL;
                int pathStart = URLExcludingProt.IndexOf("/", 0);

                if (pathStart > -1 && URLExcludingProt.Length > pathStart)
                {
                    hostAndPort = URL.Substring(0, pathStart);
                }
                else if (pathStart == -1) // it's already just a hostname and optional port
                {
                    hostAndPort = URLExcludingProt;
                }
            }
            int portIndex = -1;
            DomainName domain = null;

            if (!isFile)
            {
                portIndex = hostAndPort.IndexOf(":");
                
                // Protect against common malformed URL (Windows file path without file protocol)
                if (portIndex <= 1)
                    portIndex = -1;

                DomainName.TryParse(
                    hostAndPort.Substring(0, portIndex > 0 ? portIndex : hostAndPort.Length),
                    out domain);
            }
            return new URLSummary(hostAndPort, 
                portIndex > 0 && portIndex+1 < hostAndPort.Length ? hostAndPort.Substring(portIndex+1) : "", 
                domain);
        }
    }
}
