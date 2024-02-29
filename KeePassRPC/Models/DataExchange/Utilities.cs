using KeePassRPC.Models.Shared;

namespace KeePassRPC.Models.DataExchange
{
    public class Utilities
    {
        public static string FormFieldTypeToHtmlType(FormFieldType fft)
        {
            if (fft == FormFieldType.FFTpassword)
                return "password";
            if (fft == FormFieldType.FFTselect)
                return "select-one";
            if (fft == FormFieldType.FFTradio)
                return "radio";
            if (fft == FormFieldType.FFTcheckbox)
                return "checkbox";
            return "text";
        }
        
        public static FieldType FormFieldTypeToFieldType(FormFieldType fft)
        {
            FieldType type = FieldType.Text;
            if (fft == FormFieldType.FFTpassword)
                type = FieldType.Password;
            else if (fft == FormFieldType.FFTselect)
                type = FieldType.Existing;
            else if (fft == FormFieldType.FFTradio)
                type = FieldType.Existing;
            else if (fft == FormFieldType.FFTusername)
                type = FieldType.Text;
            else if (fft == FormFieldType.FFTcheckbox)
                type = FieldType.Toggle;
            return type;
        }

        public static string FieldTypeToDisplay(FieldType type, bool titleCase)
        {
            string typeD = "Text";
            if (type == FieldType.Password)
                typeD = "Password";
            else if (type == FieldType.Existing)
                typeD = "Existing";
            else if (type == FieldType.Text)
                typeD = "Text";
            else if (type == FieldType.Toggle)
                typeD = "Toggle";
            if (!titleCase)
                return typeD.ToLower();
            return typeD;
        }
        
        
        public static string FieldTypeToHtmlType(FieldType ft)
        {
            //Note loss of precision converting from Existing to Radio/Select. Thus, this method should only
            //be used as long as is necessary for the transition from config v1 to v2.
            switch (ft)
            {
                case FieldType.Password:
                    return "password";
                case FieldType.Existing:
                    return "radio";
                case FieldType.Toggle:
                    return "checkbox";
                default:
                    return "text";
            }
        }
        
        public static FormFieldType FieldTypeToFormFieldType(FieldType ft, string htmlType = null)
        {
            //Note potential loss of precision converting from Existing to Radio/Select. Thus, this method should only
            //be used as long as is necessary for the transition from config v1 to v2.
            switch (ft)
            {
                case FieldType.Password:
                    return FormFieldType.FFTpassword;
                case FieldType.Existing:
                    return htmlType == "select-one" ? FormFieldType.FFTselect : FormFieldType.FFTradio;
                case FieldType.Toggle:
                    return FormFieldType.FFTcheckbox;
                default:
                    return FormFieldType.FFTtext;
            }
        }
    }
}