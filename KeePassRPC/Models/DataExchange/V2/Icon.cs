namespace KeePassRPC.Models.DataExchange.V2
{
    public class Icon
    {
        public string Index; // Requires feature KPRPC_FEATURE_ICON_REFERENCES
        public string RefId; // Requires feature KPRPC_FEATURE_ICON_REFERENCES
        public string Base64; // format assumed to be PNG
        
        public Icon() { }

        public Icon(string index,
            string refId,
            string base64)
        {
            Index = index;
            RefId = refId;
            Base64 = base64;
        }
    }
}