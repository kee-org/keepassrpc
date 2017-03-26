namespace JsonOutputDemo
{
    #region Imports

    using System.Xml.Serialization;

    #endregion

    //
    // See RSS 0.91 specification at http://backend.userland.com/rss091 for
    // explanation of the XML vocabulary represented by the classes in this
    // file.
    //

    [ XmlRoot("rss", Namespace = "", IsNullable = false) ]
    public class RichSiteSummary 
    {
        public Channel channel;
        [ XmlAttribute ]
        public string version;
    }
    
    public class Channel 
    {
        public string title;
        [ XmlElement(DataType = "anyURI") ]
        public string link;
        public string description;
        [ XmlElement(DataType = "language") ]
        public string language;
        public string rating;
        public Image image;
        public TextInput textInput;
        public string copyright;
        [ XmlElement(DataType = "anyURI") ]
        public string docs;
        public string managingEditor;
        public string webMaster;
        public string pubDate;
        public string lastBuildDate;
        [ XmlArrayItem("hour", IsNullable = false) ]
        public int[] skipHours;
        [ XmlArrayItem("day", IsNullable = false) ]
        public Day[] skipDays;
        [ XmlElement("item") ]
        public Item[] item;
    }
    
    public class Image 
    {
        public string title;
        [ XmlElement(DataType = "anyURI") ]
        public string url;
        [ XmlElement(DataType = "anyURI") ]
        public string link;
        public int width;
        [ XmlIgnore() ]
        public bool widthSpecified;
        public int height;
        [ XmlIgnore() ]
        public bool heightSpecified;
        public string description;
    }
    
    public class Item 
    {
        public string title;
        public string description;
        public string pubDate;
        [ XmlElement(DataType = "anyURI") ]
        public string link;
    }
    
    public class TextInput 
    {
        public string title;
        public string description;
        public string name;
        [ XmlElement(DataType = "anyURI") ]
        public string link;
    }

    public enum Day 
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday,
    }
}
