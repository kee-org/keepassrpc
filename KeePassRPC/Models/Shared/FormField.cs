using KeePassRPC.Models.DataExchange;

namespace KeePassRPC.Models.Shared
{
    public class FormField
    {
        public string Name;
        public string DisplayName;
        public string Value;
        public FormFieldType @Type;
        public string Id;
        public int Page;
        public PlaceholderHandling PlaceholderHandling;

        public FormField() { }

        public FormField(string name,
            string displayName,
            string value,
            FormFieldType @type,
            string id,
            int page,
            PlaceholderHandling placeholderHandling)
        {
            Name = name;
            DisplayName = displayName;
            Value = value;
            @Type = @type;
            Id = id;
            Page = page;
            PlaceholderHandling = placeholderHandling;
        }

        // Assumes funky Username type has already been determined so all textual stuff is type text by now
        public static FormFieldType FormFieldTypeFromHtmlTypeOrFieldType(string t, FieldType ft)
        {
            switch (t) {
                case "password":
                    return FormFieldType.FFTpassword;
                case "radio":
                    return FormFieldType.FFTradio;
                case "checkbox":
                    return FormFieldType.FFTcheckbox;
                case "select-one":
                    return FormFieldType.FFTselect;
                default:
                    return Utilities.FieldTypeToFormFieldType(ft);
            }
        }
    }
}