using System;

namespace KeePassRPC.Models.Persistent
{
    public interface IGuidService
    {
        Guid NewGuid();
    }
}