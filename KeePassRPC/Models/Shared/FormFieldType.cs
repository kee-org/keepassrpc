namespace KeePassRPC.Models.Shared
{
    // This is not used by configV2 so essentially deprecated, although older clients will still expect
    // field types to be described using this enum for many years to come.
    // FFTusername is special type because bultin FF supports with only username and password
    public enum FormFieldType { FFTradio, FFTusername, FFTtext, FFTpassword, FFTselect, FFTcheckbox }
}