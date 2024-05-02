using System;

namespace KeePassRPC.Models
{
    public interface IGuidService
    {
        Guid NewGuid();
    }
}