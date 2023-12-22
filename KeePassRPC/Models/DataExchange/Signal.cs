namespace KeePassRPC.Models.DataExchange
{
    public enum Signal
    {
        /// <summary>
        /// 
        /// </summary>
        PLEASE_AUTHENTICATE = 0,
        /// <summary>
        /// deprecated?
        /// </summary>
        JSCALLBACKS_SETUP = 1,
        /// <summary>
        /// deprecated?
        /// </summary>
        ICECALLBACKS_SETUP = 2,

        DATABASE_OPENING = 3,
        DATABASE_OPEN = 4,
        DATABASE_CLOSING = 5,
        DATABASE_CLOSED = 6,
        DATABASE_SAVING = 7,
        DATABASE_SAVED = 8,
        DATABASE_DELETING = 9,
        DATABASE_DELETED = 10,
        DATABASE_SELECTED = 11,
        EXITING = 12
    }
}