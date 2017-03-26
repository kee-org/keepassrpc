// **********************************************************************
//
// Copyright (c) 2003-2008 ZeroC, Inc. All rights reserved.
//
// This copy of Ice is licensed to you under the terms described in the
// ICE_LICENSE file included in this distribution.
//
// **********************************************************************

// Ice version 3.3.0
// Generated from file `KeeICE.ice'

#if __MonoCS__

using _System = System;
using _Microsoft = Microsoft;
#else

using _System = global::System;
using _Microsoft = global::Microsoft;
#endif

namespace KeeICE
{
    namespace KPlib
    {
        public enum loginSearchType
        {
            LSTall,
            LSTnoForms,
            LSTnoRealms
        }

        public enum formFieldType
        {
            FFTradio,
            FFTusername,
            FFTtext,
            FFTpassword,
            FFTselect,
            FFTcheckbox
        }

        public class KPFormField : _System.ICloneable
        {
            #region Slice data members

            public string name;

            public string displayName;

            public string value;

            public KeeICE.KPlib.formFieldType type;

            public string id;

            public int page;

            #endregion

            #region Constructors

            public KPFormField()
            {
            }

            public KPFormField(string name, string displayName, string value, KeeICE.KPlib.formFieldType type, string id, int page)
            {
                this.name = name;
                this.displayName = displayName;
                this.value = value;
                this.type = type;
                this.id = id;
                this.page = page;
            }

            #endregion

            #region ICloneable members

            public object Clone()
            {
                return MemberwiseClone();
            }

            #endregion

            #region Object members

            public override int GetHashCode()
            {
                int h__ = 0;
                if(name != null)
                {
                    h__ = 5 * h__ + name.GetHashCode();
                }
                if(displayName != null)
                {
                    h__ = 5 * h__ + displayName.GetHashCode();
                }
                if(value != null)
                {
                    h__ = 5 * h__ + value.GetHashCode();
                }
                h__ = 5 * h__ + type.GetHashCode();
                if(id != null)
                {
                    h__ = 5 * h__ + id.GetHashCode();
                }
                h__ = 5 * h__ + page.GetHashCode();
                return h__;
            }

            public override bool Equals(object other__)
            {
                if(object.ReferenceEquals(this, other__))
                {
                    return true;
                }
                if(other__ == null)
                {
                    return false;
                }
                if(GetType() != other__.GetType())
                {
                    return false;
                }
                KPFormField o__ = (KPFormField)other__;
                if(name == null)
                {
                    if(o__.name != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if(!name.Equals(o__.name))
                    {
                        return false;
                    }
                }
                if(displayName == null)
                {
                    if(o__.displayName != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if(!displayName.Equals(o__.displayName))
                    {
                        return false;
                    }
                }
                if(value == null)
                {
                    if(o__.value != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if(!value.Equals(o__.value))
                    {
                        return false;
                    }
                }
                if(!type.Equals(o__.type))
                {
                    return false;
                }
                if(id == null)
                {
                    if(o__.id != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if(!id.Equals(o__.id))
                    {
                        return false;
                    }
                }
                if(!page.Equals(o__.page))
                {
                    return false;
                }
                return true;
            }

            #endregion

            #region Comparison members

            public static bool operator==(KPFormField lhs__, KPFormField rhs__)
            {
                return Equals(lhs__, rhs__);
            }

            public static bool operator!=(KPFormField lhs__, KPFormField rhs__)
            {
                return !Equals(lhs__, rhs__);
            }

            #endregion

            #region Marshalling support

            public void write__(IceInternal.BasicStream os__)
            {
                os__.writeString(name);
                os__.writeString(displayName);
                os__.writeString(value);
                os__.writeByte((byte)type, 6);
                os__.writeString(id);
                os__.writeInt(page);
            }

            public void read__(IceInternal.BasicStream is__)
            {
                name = is__.readString();
                displayName = is__.readString();
                value = is__.readString();
                type = (KeeICE.KPlib.formFieldType)is__.readByte(6);
                id = is__.readString();
                page = is__.readInt();
            }

            #endregion
        }

        public class KPGroup : _System.ICloneable
        {
            #region Slice data members

            public string title;

            public string uniqueID;

            public string iconImageData;

            #endregion

            #region Constructors

            public KPGroup()
            {
            }

            public KPGroup(string title, string uniqueID, string iconImageData)
            {
                this.title = title;
                this.uniqueID = uniqueID;
                this.iconImageData = iconImageData;
            }

            #endregion

            #region ICloneable members

            public object Clone()
            {
                return MemberwiseClone();
            }

            #endregion

            #region Object members

            public override int GetHashCode()
            {
                int h__ = 0;
                if(title != null)
                {
                    h__ = 5 * h__ + title.GetHashCode();
                }
                if(uniqueID != null)
                {
                    h__ = 5 * h__ + uniqueID.GetHashCode();
                }
                if(iconImageData != null)
                {
                    h__ = 5 * h__ + iconImageData.GetHashCode();
                }
                return h__;
            }

            public override bool Equals(object other__)
            {
                if(object.ReferenceEquals(this, other__))
                {
                    return true;
                }
                if(other__ == null)
                {
                    return false;
                }
                if(GetType() != other__.GetType())
                {
                    return false;
                }
                KPGroup o__ = (KPGroup)other__;
                if(title == null)
                {
                    if(o__.title != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if(!title.Equals(o__.title))
                    {
                        return false;
                    }
                }
                if(uniqueID == null)
                {
                    if(o__.uniqueID != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if(!uniqueID.Equals(o__.uniqueID))
                    {
                        return false;
                    }
                }
                if(iconImageData == null)
                {
                    if(o__.iconImageData != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if(!iconImageData.Equals(o__.iconImageData))
                    {
                        return false;
                    }
                }
                return true;
            }

            #endregion

            #region Comparison members

            public static bool operator==(KPGroup lhs__, KPGroup rhs__)
            {
                return Equals(lhs__, rhs__);
            }

            public static bool operator!=(KPGroup lhs__, KPGroup rhs__)
            {
                return !Equals(lhs__, rhs__);
            }

            #endregion

            #region Marshalling support

            public void write__(IceInternal.BasicStream os__)
            {
                os__.writeString(title);
                os__.writeString(uniqueID);
                os__.writeString(iconImageData);
            }

            public void read__(IceInternal.BasicStream is__)
            {
                title = is__.readString();
                uniqueID = is__.readString();
                iconImageData = is__.readString();
            }

            #endregion
        }

        public class KPEntry : _System.ICloneable
        {
            #region Slice data members

            public string[] URLs;

            public string formActionURL;

            public string HTTPRealm;

            public string title;

            public KeeICE.KPlib.KPFormField[] formFieldList;

            public bool exactMatch;

            public string uniqueID;

            public bool alwaysAutoFill;

            public bool neverAutoFill;

            public bool alwaysAutoSubmit;

            public bool neverAutoSubmit;

            public int priority;

            public string parentGroupName;

            public string parentGroupUUID;

            public string parentGroupPath;

            public string iconImageData;

            #endregion

            #region Constructors

            public KPEntry()
            {
            }

            public KPEntry(string[] URLs, string formActionURL, string HTTPRealm, string title, KeeICE.KPlib.KPFormField[] formFieldList, bool exactMatch, string uniqueID, bool alwaysAutoFill, bool neverAutoFill, bool alwaysAutoSubmit, bool neverAutoSubmit, int priority, string parentGroupName, string parentGroupUUID, string parentGroupPath, string iconImageData)
            {
                this.URLs = URLs;
                this.formActionURL = formActionURL;
                this.HTTPRealm = HTTPRealm;
                this.title = title;
                this.formFieldList = formFieldList;
                this.exactMatch = exactMatch;
                this.uniqueID = uniqueID;
                this.alwaysAutoFill = alwaysAutoFill;
                this.neverAutoFill = neverAutoFill;
                this.alwaysAutoSubmit = alwaysAutoSubmit;
                this.neverAutoSubmit = neverAutoSubmit;
                this.priority = priority;
                this.parentGroupName = parentGroupName;
                this.parentGroupUUID = parentGroupUUID;
                this.parentGroupPath = parentGroupPath;
                this.iconImageData = iconImageData;
            }

            #endregion

            #region ICloneable members

            public object Clone()
            {
                return MemberwiseClone();
            }

            #endregion

            #region Object members

            public override int GetHashCode()
            {
                int h__ = 0;
                if(URLs != null)
                {
                    h__ = 5 * h__ + IceUtilInternal.Arrays.GetHashCode(URLs);
                }
                if(formActionURL != null)
                {
                    h__ = 5 * h__ + formActionURL.GetHashCode();
                }
                if(HTTPRealm != null)
                {
                    h__ = 5 * h__ + HTTPRealm.GetHashCode();
                }
                if(title != null)
                {
                    h__ = 5 * h__ + title.GetHashCode();
                }
                if(formFieldList != null)
                {
                    h__ = 5 * h__ + IceUtilInternal.Arrays.GetHashCode(formFieldList);
                }
                h__ = 5 * h__ + exactMatch.GetHashCode();
                if(uniqueID != null)
                {
                    h__ = 5 * h__ + uniqueID.GetHashCode();
                }
                h__ = 5 * h__ + alwaysAutoFill.GetHashCode();
                h__ = 5 * h__ + neverAutoFill.GetHashCode();
                h__ = 5 * h__ + alwaysAutoSubmit.GetHashCode();
                h__ = 5 * h__ + neverAutoSubmit.GetHashCode();
                h__ = 5 * h__ + priority.GetHashCode();
                if(parentGroupName != null)
                {
                    h__ = 5 * h__ + parentGroupName.GetHashCode();
                }
                if(parentGroupUUID != null)
                {
                    h__ = 5 * h__ + parentGroupUUID.GetHashCode();
                }
                if(parentGroupPath != null)
                {
                    h__ = 5 * h__ + parentGroupPath.GetHashCode();
                }
                if(iconImageData != null)
                {
                    h__ = 5 * h__ + iconImageData.GetHashCode();
                }
                return h__;
            }

            public override bool Equals(object other__)
            {
                if(object.ReferenceEquals(this, other__))
                {
                    return true;
                }
                if(other__ == null)
                {
                    return false;
                }
                if(GetType() != other__.GetType())
                {
                    return false;
                }
                KPEntry o__ = (KPEntry)other__;
                if(URLs == null)
                {
                    if(o__.URLs != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if(!IceUtilInternal.Arrays.Equals(URLs, o__.URLs))
                    {
                        return false;
                    }
                }
                if(formActionURL == null)
                {
                    if(o__.formActionURL != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if(!formActionURL.Equals(o__.formActionURL))
                    {
                        return false;
                    }
                }
                if(HTTPRealm == null)
                {
                    if(o__.HTTPRealm != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if(!HTTPRealm.Equals(o__.HTTPRealm))
                    {
                        return false;
                    }
                }
                if(title == null)
                {
                    if(o__.title != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if(!title.Equals(o__.title))
                    {
                        return false;
                    }
                }
                if(formFieldList == null)
                {
                    if(o__.formFieldList != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if(!IceUtilInternal.Arrays.Equals(formFieldList, o__.formFieldList))
                    {
                        return false;
                    }
                }
                if(!exactMatch.Equals(o__.exactMatch))
                {
                    return false;
                }
                if(uniqueID == null)
                {
                    if(o__.uniqueID != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if(!uniqueID.Equals(o__.uniqueID))
                    {
                        return false;
                    }
                }
                if(!alwaysAutoFill.Equals(o__.alwaysAutoFill))
                {
                    return false;
                }
                if(!neverAutoFill.Equals(o__.neverAutoFill))
                {
                    return false;
                }
                if(!alwaysAutoSubmit.Equals(o__.alwaysAutoSubmit))
                {
                    return false;
                }
                if(!neverAutoSubmit.Equals(o__.neverAutoSubmit))
                {
                    return false;
                }
                if(!priority.Equals(o__.priority))
                {
                    return false;
                }
                if(parentGroupName == null)
                {
                    if(o__.parentGroupName != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if(!parentGroupName.Equals(o__.parentGroupName))
                    {
                        return false;
                    }
                }
                if(parentGroupUUID == null)
                {
                    if(o__.parentGroupUUID != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if(!parentGroupUUID.Equals(o__.parentGroupUUID))
                    {
                        return false;
                    }
                }
                if(parentGroupPath == null)
                {
                    if(o__.parentGroupPath != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if(!parentGroupPath.Equals(o__.parentGroupPath))
                    {
                        return false;
                    }
                }
                if(iconImageData == null)
                {
                    if(o__.iconImageData != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if(!iconImageData.Equals(o__.iconImageData))
                    {
                        return false;
                    }
                }
                return true;
            }

            #endregion

            #region Comparison members

            public static bool operator==(KPEntry lhs__, KPEntry rhs__)
            {
                return Equals(lhs__, rhs__);
            }

            public static bool operator!=(KPEntry lhs__, KPEntry rhs__)
            {
                return !Equals(lhs__, rhs__);
            }

            #endregion

            #region Marshalling support

            public void write__(IceInternal.BasicStream os__)
            {
                os__.writeStringSeq(URLs);
                os__.writeString(formActionURL);
                os__.writeString(HTTPRealm);
                os__.writeString(title);
                if(formFieldList == null)
                {
                    os__.writeSize(0);
                }
                else
                {
                    os__.writeSize(formFieldList.Length);
                    for(int ix__ = 0; ix__ < formFieldList.Length; ++ix__)
                    {
                        (formFieldList == null ? new KeeICE.KPlib.KPFormField() : formFieldList[ix__]).write__(os__);
                    }
                }
                os__.writeBool(exactMatch);
                os__.writeString(uniqueID);
                os__.writeBool(alwaysAutoFill);
                os__.writeBool(neverAutoFill);
                os__.writeBool(alwaysAutoSubmit);
                os__.writeBool(neverAutoSubmit);
                os__.writeInt(priority);
                os__.writeString(parentGroupName);
                os__.writeString(parentGroupUUID);
                os__.writeString(parentGroupPath);
                os__.writeString(iconImageData);
            }

            public void read__(IceInternal.BasicStream is__)
            {
                URLs = is__.readStringSeq();
                formActionURL = is__.readString();
                HTTPRealm = is__.readString();
                title = is__.readString();
                {
                    int szx__ = is__.readSize();
                    is__.startSeq(szx__, 9);
                    formFieldList = new KeeICE.KPlib.KPFormField[szx__];
                    for(int ix__ = 0; ix__ < szx__; ++ix__)
                    {
                        formFieldList[ix__] = new KeeICE.KPlib.KPFormField();
                        formFieldList[ix__].read__(is__);
                        is__.checkSeq();
                        is__.endElement();
                    }
                    is__.endSeq(szx__);
                }
                exactMatch = is__.readBool();
                uniqueID = is__.readString();
                alwaysAutoFill = is__.readBool();
                neverAutoFill = is__.readBool();
                alwaysAutoSubmit = is__.readBool();
                neverAutoSubmit = is__.readBool();
                priority = is__.readInt();
                parentGroupName = is__.readString();
                parentGroupUUID = is__.readString();
                parentGroupPath = is__.readString();
                iconImageData = is__.readString();
            }

            #endregion
        }

        public class KeeICEException : Ice.UserException
        {
            #region Slice data members

            public string reason;

            #endregion

            #region Constructors

            public KeeICEException()
            {
            }

            public KeeICEException(_System.Exception ex__) : base(ex__)
            {
            }

            private void initDM__(string reason)
            {
                this.reason = reason;
            }

            public KeeICEException(string reason)
            {
                initDM__(reason);
            }

            public KeeICEException(string reason, _System.Exception ex__) : base(ex__)
            {
                initDM__(reason);
            }

            #endregion

            public override string ice_name()
            {
                return "KeeICE::KPlib::KeeICEException";
            }

            #region Object members

            public override int GetHashCode()
            {
                int h__ = 0;
                if(reason != null)
                {
                    h__ = 5 * h__ + reason.GetHashCode();
                }
                return h__;
            }

            public override bool Equals(object other__)
            {
                if(other__ == null)
                {
                    return false;
                }
                if(object.ReferenceEquals(this, other__))
                {
                    return true;
                }
                if(!(other__ is KeeICEException))
                {
                    return false;
                }
                KeeICEException o__ = (KeeICEException)other__;
                if(reason == null)
                {
                    if(o__.reason != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if(!reason.Equals(o__.reason))
                    {
                        return false;
                    }
                }
                return true;
            }

            #endregion

            #region Comparison members

            public static bool operator==(KeeICEException lhs__, KeeICEException rhs__)
            {
                return Equals(lhs__, rhs__);
            }

            public static bool operator!=(KeeICEException lhs__, KeeICEException rhs__)
            {
                return !Equals(lhs__, rhs__);
            }

            #endregion

            #region Marshaling support

            public override void write__(IceInternal.BasicStream os__)
            {
                os__.writeString("::KeeICE::KPlib::KeeICEException");
                os__.startWriteSlice();
                os__.writeString(reason);
                os__.endWriteSlice();
            }

            public override void read__(IceInternal.BasicStream is__, bool rid__)
            {
                if(rid__)
                {
                    /* string myId = */ is__.readString();
                }
                is__.startReadSlice();
                reason = is__.readString();
                is__.endReadSlice();
            }

            public override void write__(Ice.OutputStream outS__)
            {
                Ice.MarshalException ex = new Ice.MarshalException();
                ex.reason = "exception KeeICE::KPlib::KeeICEException was not generated with stream support";
                throw ex;
            }

            public override void read__(Ice.InputStream inS__, bool rid__)
            {
                Ice.MarshalException ex = new Ice.MarshalException();
                ex.reason = "exception KeeICE::KPlib::KeeICEException was not generated with stream support";
                throw ex;
            }

            public override bool usesClasses__()
            {
                return true;
            }

            #endregion
        }

        public class KFConfiguration : _System.ICloneable
        {
            #region Slice data members

            public string[] knownDatabases;

            public bool autoCommit;

            #endregion

            #region Constructors

            public KFConfiguration()
            {
            }

            public KFConfiguration(string[] knownDatabases, bool autoCommit)
            {
                this.knownDatabases = knownDatabases;
                this.autoCommit = autoCommit;
            }

            #endregion

            #region ICloneable members

            public object Clone()
            {
                return MemberwiseClone();
            }

            #endregion

            #region Object members

            public override int GetHashCode()
            {
                int h__ = 0;
                if(knownDatabases != null)
                {
                    h__ = 5 * h__ + IceUtilInternal.Arrays.GetHashCode(knownDatabases);
                }
                h__ = 5 * h__ + autoCommit.GetHashCode();
                return h__;
            }

            public override bool Equals(object other__)
            {
                if(object.ReferenceEquals(this, other__))
                {
                    return true;
                }
                if(other__ == null)
                {
                    return false;
                }
                if(GetType() != other__.GetType())
                {
                    return false;
                }
                KFConfiguration o__ = (KFConfiguration)other__;
                if(knownDatabases == null)
                {
                    if(o__.knownDatabases != null)
                    {
                        return false;
                    }
                }
                else
                {
                    if(!IceUtilInternal.Arrays.Equals(knownDatabases, o__.knownDatabases))
                    {
                        return false;
                    }
                }
                if(!autoCommit.Equals(o__.autoCommit))
                {
                    return false;
                }
                return true;
            }

            #endregion

            #region Comparison members

            public static bool operator==(KFConfiguration lhs__, KFConfiguration rhs__)
            {
                return Equals(lhs__, rhs__);
            }

            public static bool operator!=(KFConfiguration lhs__, KFConfiguration rhs__)
            {
                return !Equals(lhs__, rhs__);
            }

            #endregion

            #region Marshalling support

            public void write__(IceInternal.BasicStream os__)
            {
                os__.writeStringSeq(knownDatabases);
                os__.writeBool(autoCommit);
            }

            public void read__(IceInternal.BasicStream is__)
            {
                knownDatabases = is__.readStringSeq();
                autoCommit = is__.readBool();
            }

            #endregion
        }

        public interface KP : Ice.Object, KPOperations_, KPOperationsNC_
        {
        }

        public interface CallbackReceiver : Ice.Object, CallbackReceiverOperations_, CallbackReceiverOperationsNC_
        {
        }
    }
}

namespace KeeICE
{
    namespace KPlib
    {
        public interface KPPrx : Ice.ObjectPrx
        {
            bool checkVersion(float keeFoxVersion, float keeICEVersion, out int result);
            bool checkVersion(float keeFoxVersion, float keeICEVersion, out int result, _System.Collections.Generic.Dictionary<string, string> context__);

            string getDatabaseName();
            string getDatabaseName(_System.Collections.Generic.Dictionary<string, string> context__);

            string getDatabaseFileName();
            string getDatabaseFileName(_System.Collections.Generic.Dictionary<string, string> context__);

            void changeDatabase(string fileName, bool closeCurrent);
            void changeDatabase(string fileName, bool closeCurrent, _System.Collections.Generic.Dictionary<string, string> context__);

            KeeICE.KPlib.KPEntry AddLogin(KeeICE.KPlib.KPEntry login, string parentUUID);
            KeeICE.KPlib.KPEntry AddLogin(KeeICE.KPlib.KPEntry login, string parentUUID, _System.Collections.Generic.Dictionary<string, string> context__);

            void ModifyLogin(KeeICE.KPlib.KPEntry oldLogin, KeeICE.KPlib.KPEntry newLogin);
            void ModifyLogin(KeeICE.KPlib.KPEntry oldLogin, KeeICE.KPlib.KPEntry newLogin, _System.Collections.Generic.Dictionary<string, string> context__);

            int getAllLogins(out KeeICE.KPlib.KPEntry[] logins);
            int getAllLogins(out KeeICE.KPlib.KPEntry[] logins, _System.Collections.Generic.Dictionary<string, string> context__);

            int findLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches, string uniqueID, out KeeICE.KPlib.KPEntry[] logins);
            int findLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches, string uniqueID, out KeeICE.KPlib.KPEntry[] logins, _System.Collections.Generic.Dictionary<string, string> context__);

            int countLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches);
            int countLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches, _System.Collections.Generic.Dictionary<string, string> context__);

            void addClient(Ice.Identity ident);
            void addClient(Ice.Identity ident, _System.Collections.Generic.Dictionary<string, string> context__);

            int findGroups(string name, string uuid, out KeeICE.KPlib.KPGroup[] groups);
            int findGroups(string name, string uuid, out KeeICE.KPlib.KPGroup[] groups, _System.Collections.Generic.Dictionary<string, string> context__);

            KeeICE.KPlib.KPGroup getRoot();
            KeeICE.KPlib.KPGroup getRoot(_System.Collections.Generic.Dictionary<string, string> context__);

            KeeICE.KPlib.KPGroup getParent(string uuid);
            KeeICE.KPlib.KPGroup getParent(string uuid, _System.Collections.Generic.Dictionary<string, string> context__);

            KeeICE.KPlib.KPGroup[] getChildGroups(string uuid);
            KeeICE.KPlib.KPGroup[] getChildGroups(string uuid, _System.Collections.Generic.Dictionary<string, string> context__);

            KeeICE.KPlib.KPEntry[] getChildEntries(string uuid);
            KeeICE.KPlib.KPEntry[] getChildEntries(string uuid, _System.Collections.Generic.Dictionary<string, string> context__);

            KeeICE.KPlib.KPGroup addGroup(string name, string parentUuid);
            KeeICE.KPlib.KPGroup addGroup(string name, string parentUuid, _System.Collections.Generic.Dictionary<string, string> context__);

            bool removeGroup(string uuid);
            bool removeGroup(string uuid, _System.Collections.Generic.Dictionary<string, string> context__);

            bool removeEntry(string uuid);
            bool removeEntry(string uuid, _System.Collections.Generic.Dictionary<string, string> context__);

            void LaunchGroupEditor(string uuid);
            void LaunchGroupEditor(string uuid, _System.Collections.Generic.Dictionary<string, string> context__);

            void LaunchLoginEditor(string uuid);
            void LaunchLoginEditor(string uuid, _System.Collections.Generic.Dictionary<string, string> context__);

            KeeICE.KPlib.KFConfiguration getCurrentKFConfig();
            KeeICE.KPlib.KFConfiguration getCurrentKFConfig(_System.Collections.Generic.Dictionary<string, string> context__);

            bool setCurrentKFConfig(KeeICE.KPlib.KFConfiguration config);
            bool setCurrentKFConfig(KeeICE.KPlib.KFConfiguration config, _System.Collections.Generic.Dictionary<string, string> context__);

            bool setCurrentDBRootGroup(string uuid);
            bool setCurrentDBRootGroup(string uuid, _System.Collections.Generic.Dictionary<string, string> context__);
        }

        public interface CallbackReceiverPrx : Ice.ObjectPrx
        {
            void callback(int num);
            void callback(int num, _System.Collections.Generic.Dictionary<string, string> context__);
        }
    }
}

namespace KeeICE
{
    namespace KPlib
    {
        public interface KPOperations_
        {
            bool checkVersion(float keeFoxVersion, float keeICEVersion, out int result, Ice.Current current__);

            string getDatabaseName(Ice.Current current__);

            string getDatabaseFileName(Ice.Current current__);

            void changeDatabase(string fileName, bool closeCurrent, Ice.Current current__);

            KeeICE.KPlib.KPEntry AddLogin(KeeICE.KPlib.KPEntry login, string parentUUID, Ice.Current current__);

            void ModifyLogin(KeeICE.KPlib.KPEntry oldLogin, KeeICE.KPlib.KPEntry newLogin, Ice.Current current__);

            int getAllLogins(out KeeICE.KPlib.KPEntry[] logins, Ice.Current current__);

            int findLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches, string uniqueID, out KeeICE.KPlib.KPEntry[] logins, Ice.Current current__);

            int countLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches, Ice.Current current__);

            void addClient(Ice.Identity ident, Ice.Current current__);

            int findGroups(string name, string uuid, out KeeICE.KPlib.KPGroup[] groups, Ice.Current current__);

            KeeICE.KPlib.KPGroup getRoot(Ice.Current current__);

            KeeICE.KPlib.KPGroup getParent(string uuid, Ice.Current current__);

            KeeICE.KPlib.KPGroup[] getChildGroups(string uuid, Ice.Current current__);

            KeeICE.KPlib.KPEntry[] getChildEntries(string uuid, Ice.Current current__);

            KeeICE.KPlib.KPGroup addGroup(string name, string parentUuid, Ice.Current current__);

            bool removeGroup(string uuid, Ice.Current current__);

            bool removeEntry(string uuid, Ice.Current current__);

            void LaunchGroupEditor(string uuid, Ice.Current current__);

            void LaunchLoginEditor(string uuid, Ice.Current current__);

            KeeICE.KPlib.KFConfiguration getCurrentKFConfig(Ice.Current current__);

            bool setCurrentKFConfig(KeeICE.KPlib.KFConfiguration config, Ice.Current current__);

            bool setCurrentDBRootGroup(string uuid, Ice.Current current__);
        }

        public interface KPOperationsNC_
        {
            bool checkVersion(float keeFoxVersion, float keeICEVersion, out int result);

            string getDatabaseName();

            string getDatabaseFileName();

            void changeDatabase(string fileName, bool closeCurrent);

            KeeICE.KPlib.KPEntry AddLogin(KeeICE.KPlib.KPEntry login, string parentUUID);

            void ModifyLogin(KeeICE.KPlib.KPEntry oldLogin, KeeICE.KPlib.KPEntry newLogin);

            int getAllLogins(out KeeICE.KPlib.KPEntry[] logins);

            int findLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches, string uniqueID, out KeeICE.KPlib.KPEntry[] logins);

            int countLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches);

            void addClient(Ice.Identity ident);

            int findGroups(string name, string uuid, out KeeICE.KPlib.KPGroup[] groups);

            KeeICE.KPlib.KPGroup getRoot();

            KeeICE.KPlib.KPGroup getParent(string uuid);

            KeeICE.KPlib.KPGroup[] getChildGroups(string uuid);

            KeeICE.KPlib.KPEntry[] getChildEntries(string uuid);

            KeeICE.KPlib.KPGroup addGroup(string name, string parentUuid);

            bool removeGroup(string uuid);

            bool removeEntry(string uuid);

            void LaunchGroupEditor(string uuid);

            void LaunchLoginEditor(string uuid);

            KeeICE.KPlib.KFConfiguration getCurrentKFConfig();

            bool setCurrentKFConfig(KeeICE.KPlib.KFConfiguration config);

            bool setCurrentDBRootGroup(string uuid);
        }

        public interface CallbackReceiverOperations_
        {
            void callback(int num, Ice.Current current__);
        }

        public interface CallbackReceiverOperationsNC_
        {
            void callback(int num);
        }
    }
}

namespace KeeICE
{
    namespace KPlib
    {
        public sealed class KPFormFieldListHelper
        {
            public static void write(IceInternal.BasicStream os__, KeeICE.KPlib.KPFormField[] v__)
            {
                if(v__ == null)
                {
                    os__.writeSize(0);
                }
                else
                {
                    os__.writeSize(v__.Length);
                    for(int ix__ = 0; ix__ < v__.Length; ++ix__)
                    {
                        (v__ == null ? new KeeICE.KPlib.KPFormField() : v__[ix__]).write__(os__);
                    }
                }
            }

            public static KeeICE.KPlib.KPFormField[] read(IceInternal.BasicStream is__)
            {
                KeeICE.KPlib.KPFormField[] v__;
                {
                    int szx__ = is__.readSize();
                    is__.startSeq(szx__, 9);
                    v__ = new KeeICE.KPlib.KPFormField[szx__];
                    for(int ix__ = 0; ix__ < szx__; ++ix__)
                    {
                        v__[ix__] = new KeeICE.KPlib.KPFormField();
                        v__[ix__].read__(is__);
                        is__.checkSeq();
                        is__.endElement();
                    }
                    is__.endSeq(szx__);
                }
                return v__;
            }
        }

        public sealed class KPGroupListHelper
        {
            public static void write(IceInternal.BasicStream os__, KeeICE.KPlib.KPGroup[] v__)
            {
                if(v__ == null)
                {
                    os__.writeSize(0);
                }
                else
                {
                    os__.writeSize(v__.Length);
                    for(int ix__ = 0; ix__ < v__.Length; ++ix__)
                    {
                        (v__ == null ? new KeeICE.KPlib.KPGroup() : v__[ix__]).write__(os__);
                    }
                }
            }

            public static KeeICE.KPlib.KPGroup[] read(IceInternal.BasicStream is__)
            {
                KeeICE.KPlib.KPGroup[] v__;
                {
                    int szx__ = is__.readSize();
                    is__.startSeq(szx__, 3);
                    v__ = new KeeICE.KPlib.KPGroup[szx__];
                    for(int ix__ = 0; ix__ < szx__; ++ix__)
                    {
                        v__[ix__] = new KeeICE.KPlib.KPGroup();
                        v__[ix__].read__(is__);
                        is__.checkSeq();
                        is__.endElement();
                    }
                    is__.endSeq(szx__);
                }
                return v__;
            }
        }

        public sealed class KPURLsHelper
        {
            public static void write(IceInternal.BasicStream os__, string[] v__)
            {
                os__.writeStringSeq(v__);
            }

            public static string[] read(IceInternal.BasicStream is__)
            {
                string[] v__;
                v__ = is__.readStringSeq();
                return v__;
            }
        }

        public sealed class KPEntryListHelper
        {
            public static void write(IceInternal.BasicStream os__, KeeICE.KPlib.KPEntry[] v__)
            {
                if(v__ == null)
                {
                    os__.writeSize(0);
                }
                else
                {
                    os__.writeSize(v__.Length);
                    for(int ix__ = 0; ix__ < v__.Length; ++ix__)
                    {
                        (v__ == null ? new KeeICE.KPlib.KPEntry() : v__[ix__]).write__(os__);
                    }
                }
            }

            public static KeeICE.KPlib.KPEntry[] read(IceInternal.BasicStream is__)
            {
                KeeICE.KPlib.KPEntry[] v__;
                {
                    int szx__ = is__.readSize();
                    is__.startSeq(szx__, 19);
                    v__ = new KeeICE.KPlib.KPEntry[szx__];
                    for(int ix__ = 0; ix__ < szx__; ++ix__)
                    {
                        v__[ix__] = new KeeICE.KPlib.KPEntry();
                        v__[ix__].read__(is__);
                        is__.checkSeq();
                        is__.endElement();
                    }
                    is__.endSeq(szx__);
                }
                return v__;
            }
        }

        public sealed class KPDatabaseFileNameListHelper
        {
            public static void write(IceInternal.BasicStream os__, string[] v__)
            {
                os__.writeStringSeq(v__);
            }

            public static string[] read(IceInternal.BasicStream is__)
            {
                string[] v__;
                v__ = is__.readStringSeq();
                return v__;
            }
        }

        public sealed class KPPrxHelper : Ice.ObjectPrxHelperBase, KPPrx
        {
            #region Synchronous operations

            public KeeICE.KPlib.KPEntry AddLogin(KeeICE.KPlib.KPEntry login, string parentUUID)
            {
                return AddLogin(login, parentUUID, null, false);
            }

            public KeeICE.KPlib.KPEntry AddLogin(KeeICE.KPlib.KPEntry login, string parentUUID, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                return AddLogin(login, parentUUID, context__, true);
            }

            private KeeICE.KPlib.KPEntry AddLogin(KeeICE.KPlib.KPEntry login, string parentUUID, _System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        checkTwowayOnly__("AddLogin");
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        return del__.AddLogin(login, parentUUID, context__);
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public void LaunchGroupEditor(string uuid)
            {
                LaunchGroupEditor(uuid, null, false);
            }

            public void LaunchGroupEditor(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                LaunchGroupEditor(uuid, context__, true);
            }

            private void LaunchGroupEditor(string uuid, _System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        del__.LaunchGroupEditor(uuid, context__);
                        return;
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public void LaunchLoginEditor(string uuid)
            {
                LaunchLoginEditor(uuid, null, false);
            }

            public void LaunchLoginEditor(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                LaunchLoginEditor(uuid, context__, true);
            }

            private void LaunchLoginEditor(string uuid, _System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        del__.LaunchLoginEditor(uuid, context__);
                        return;
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public void ModifyLogin(KeeICE.KPlib.KPEntry oldLogin, KeeICE.KPlib.KPEntry newLogin)
            {
                ModifyLogin(oldLogin, newLogin, null, false);
            }

            public void ModifyLogin(KeeICE.KPlib.KPEntry oldLogin, KeeICE.KPlib.KPEntry newLogin, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                ModifyLogin(oldLogin, newLogin, context__, true);
            }

            private void ModifyLogin(KeeICE.KPlib.KPEntry oldLogin, KeeICE.KPlib.KPEntry newLogin, _System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        checkTwowayOnly__("ModifyLogin");
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        del__.ModifyLogin(oldLogin, newLogin, context__);
                        return;
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public void addClient(Ice.Identity ident)
            {
                addClient(ident, null, false);
            }

            public void addClient(Ice.Identity ident, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                addClient(ident, context__, true);
            }

            private void addClient(Ice.Identity ident, _System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        del__.addClient(ident, context__);
                        return;
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public KeeICE.KPlib.KPGroup addGroup(string name, string parentUuid)
            {
                return addGroup(name, parentUuid, null, false);
            }

            public KeeICE.KPlib.KPGroup addGroup(string name, string parentUuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                return addGroup(name, parentUuid, context__, true);
            }

            private KeeICE.KPlib.KPGroup addGroup(string name, string parentUuid, _System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        checkTwowayOnly__("addGroup");
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        return del__.addGroup(name, parentUuid, context__);
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public void changeDatabase(string fileName, bool closeCurrent)
            {
                changeDatabase(fileName, closeCurrent, null, false);
            }

            public void changeDatabase(string fileName, bool closeCurrent, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                changeDatabase(fileName, closeCurrent, context__, true);
            }

            private void changeDatabase(string fileName, bool closeCurrent, _System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        del__.changeDatabase(fileName, closeCurrent, context__);
                        return;
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public bool checkVersion(float keeFoxVersion, float keeICEVersion, out int result)
            {
                return checkVersion(keeFoxVersion, keeICEVersion, out result, null, false);
            }

            public bool checkVersion(float keeFoxVersion, float keeICEVersion, out int result, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                return checkVersion(keeFoxVersion, keeICEVersion, out result, context__, true);
            }

            private bool checkVersion(float keeFoxVersion, float keeICEVersion, out int result, _System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        checkTwowayOnly__("checkVersion");
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        return del__.checkVersion(keeFoxVersion, keeICEVersion, out result, context__);
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public int countLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches)
            {
                return countLogins(hostname, actionURL, httpRealm, lst, requireFullURLMatches, null, false);
            }

            public int countLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                return countLogins(hostname, actionURL, httpRealm, lst, requireFullURLMatches, context__, true);
            }

            private int countLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches, _System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        checkTwowayOnly__("countLogins");
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        return del__.countLogins(hostname, actionURL, httpRealm, lst, requireFullURLMatches, context__);
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public int findGroups(string name, string uuid, out KeeICE.KPlib.KPGroup[] groups)
            {
                return findGroups(name, uuid, out groups, null, false);
            }

            public int findGroups(string name, string uuid, out KeeICE.KPlib.KPGroup[] groups, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                return findGroups(name, uuid, out groups, context__, true);
            }

            private int findGroups(string name, string uuid, out KeeICE.KPlib.KPGroup[] groups, _System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        checkTwowayOnly__("findGroups");
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        return del__.findGroups(name, uuid, out groups, context__);
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public int findLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches, string uniqueID, out KeeICE.KPlib.KPEntry[] logins)
            {
                return findLogins(hostname, actionURL, httpRealm, lst, requireFullURLMatches, uniqueID, out logins, null, false);
            }

            public int findLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches, string uniqueID, out KeeICE.KPlib.KPEntry[] logins, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                return findLogins(hostname, actionURL, httpRealm, lst, requireFullURLMatches, uniqueID, out logins, context__, true);
            }

            private int findLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches, string uniqueID, out KeeICE.KPlib.KPEntry[] logins, _System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        checkTwowayOnly__("findLogins");
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        return del__.findLogins(hostname, actionURL, httpRealm, lst, requireFullURLMatches, uniqueID, out logins, context__);
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public int getAllLogins(out KeeICE.KPlib.KPEntry[] logins)
            {
                return getAllLogins(out logins, null, false);
            }

            public int getAllLogins(out KeeICE.KPlib.KPEntry[] logins, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                return getAllLogins(out logins, context__, true);
            }

            private int getAllLogins(out KeeICE.KPlib.KPEntry[] logins, _System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        checkTwowayOnly__("getAllLogins");
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        return del__.getAllLogins(out logins, context__);
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public KeeICE.KPlib.KPEntry[] getChildEntries(string uuid)
            {
                return getChildEntries(uuid, null, false);
            }

            public KeeICE.KPlib.KPEntry[] getChildEntries(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                return getChildEntries(uuid, context__, true);
            }

            private KeeICE.KPlib.KPEntry[] getChildEntries(string uuid, _System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        checkTwowayOnly__("getChildEntries");
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        return del__.getChildEntries(uuid, context__);
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public KeeICE.KPlib.KPGroup[] getChildGroups(string uuid)
            {
                return getChildGroups(uuid, null, false);
            }

            public KeeICE.KPlib.KPGroup[] getChildGroups(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                return getChildGroups(uuid, context__, true);
            }

            private KeeICE.KPlib.KPGroup[] getChildGroups(string uuid, _System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        checkTwowayOnly__("getChildGroups");
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        return del__.getChildGroups(uuid, context__);
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public KeeICE.KPlib.KFConfiguration getCurrentKFConfig()
            {
                return getCurrentKFConfig(null, false);
            }

            public KeeICE.KPlib.KFConfiguration getCurrentKFConfig(_System.Collections.Generic.Dictionary<string, string> context__)
            {
                return getCurrentKFConfig(context__, true);
            }

            private KeeICE.KPlib.KFConfiguration getCurrentKFConfig(_System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        checkTwowayOnly__("getCurrentKFConfig");
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        return del__.getCurrentKFConfig(context__);
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public string getDatabaseFileName()
            {
                return getDatabaseFileName(null, false);
            }

            public string getDatabaseFileName(_System.Collections.Generic.Dictionary<string, string> context__)
            {
                return getDatabaseFileName(context__, true);
            }

            private string getDatabaseFileName(_System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        checkTwowayOnly__("getDatabaseFileName");
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        return del__.getDatabaseFileName(context__);
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public string getDatabaseName()
            {
                return getDatabaseName(null, false);
            }

            public string getDatabaseName(_System.Collections.Generic.Dictionary<string, string> context__)
            {
                return getDatabaseName(context__, true);
            }

            private string getDatabaseName(_System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        checkTwowayOnly__("getDatabaseName");
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        return del__.getDatabaseName(context__);
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public KeeICE.KPlib.KPGroup getParent(string uuid)
            {
                return getParent(uuid, null, false);
            }

            public KeeICE.KPlib.KPGroup getParent(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                return getParent(uuid, context__, true);
            }

            private KeeICE.KPlib.KPGroup getParent(string uuid, _System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        checkTwowayOnly__("getParent");
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        return del__.getParent(uuid, context__);
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public KeeICE.KPlib.KPGroup getRoot()
            {
                return getRoot(null, false);
            }

            public KeeICE.KPlib.KPGroup getRoot(_System.Collections.Generic.Dictionary<string, string> context__)
            {
                return getRoot(context__, true);
            }

            private KeeICE.KPlib.KPGroup getRoot(_System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        checkTwowayOnly__("getRoot");
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        return del__.getRoot(context__);
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public bool removeEntry(string uuid)
            {
                return removeEntry(uuid, null, false);
            }

            public bool removeEntry(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                return removeEntry(uuid, context__, true);
            }

            private bool removeEntry(string uuid, _System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        checkTwowayOnly__("removeEntry");
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        return del__.removeEntry(uuid, context__);
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public bool removeGroup(string uuid)
            {
                return removeGroup(uuid, null, false);
            }

            public bool removeGroup(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                return removeGroup(uuid, context__, true);
            }

            private bool removeGroup(string uuid, _System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        checkTwowayOnly__("removeGroup");
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        return del__.removeGroup(uuid, context__);
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public bool setCurrentDBRootGroup(string uuid)
            {
                return setCurrentDBRootGroup(uuid, null, false);
            }

            public bool setCurrentDBRootGroup(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                return setCurrentDBRootGroup(uuid, context__, true);
            }

            private bool setCurrentDBRootGroup(string uuid, _System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        checkTwowayOnly__("setCurrentDBRootGroup");
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        return del__.setCurrentDBRootGroup(uuid, context__);
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            public bool setCurrentKFConfig(KeeICE.KPlib.KFConfiguration config)
            {
                return setCurrentKFConfig(config, null, false);
            }

            public bool setCurrentKFConfig(KeeICE.KPlib.KFConfiguration config, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                return setCurrentKFConfig(config, context__, true);
            }

            private bool setCurrentKFConfig(KeeICE.KPlib.KFConfiguration config, _System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        checkTwowayOnly__("setCurrentKFConfig");
                        delBase__ = getDelegate__(false);
                        KPDel_ del__ = (KPDel_)delBase__;
                        return del__.setCurrentKFConfig(config, context__);
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            #endregion

            #region Checked and unchecked cast operations

            public static KPPrx checkedCast(Ice.ObjectPrx b)
            {
                if(b == null)
                {
                    return null;
                }
                KPPrx r = b as KPPrx;
                if((r == null) && b.ice_isA("::KeeICE::KPlib::KP"))
                {
                    KPPrxHelper h = new KPPrxHelper();
                    h.copyFrom__(b);
                    r = h;
                }
                return r;
            }

            public static KPPrx checkedCast(Ice.ObjectPrx b, _System.Collections.Generic.Dictionary<string, string> ctx)
            {
                if(b == null)
                {
                    return null;
                }
                KPPrx r = b as KPPrx;
                if((r == null) && b.ice_isA("::KeeICE::KPlib::KP", ctx))
                {
                    KPPrxHelper h = new KPPrxHelper();
                    h.copyFrom__(b);
                    r = h;
                }
                return r;
            }

            public static KPPrx checkedCast(Ice.ObjectPrx b, string f)
            {
                if(b == null)
                {
                    return null;
                }
                Ice.ObjectPrx bb = b.ice_facet(f);
                try
                {
                    if(bb.ice_isA("::KeeICE::KPlib::KP"))
                    {
                        KPPrxHelper h = new KPPrxHelper();
                        h.copyFrom__(bb);
                        return h;
                    }
                }
                catch(Ice.FacetNotExistException)
                {
                }
                return null;
            }

            public static KPPrx checkedCast(Ice.ObjectPrx b, string f, _System.Collections.Generic.Dictionary<string, string> ctx)
            {
                if(b == null)
                {
                    return null;
                }
                Ice.ObjectPrx bb = b.ice_facet(f);
                try
                {
                    if(bb.ice_isA("::KeeICE::KPlib::KP", ctx))
                    {
                        KPPrxHelper h = new KPPrxHelper();
                        h.copyFrom__(bb);
                        return h;
                    }
                }
                catch(Ice.FacetNotExistException)
                {
                }
                return null;
            }

            public static KPPrx uncheckedCast(Ice.ObjectPrx b)
            {
                if(b == null)
                {
                    return null;
                }
                KPPrx r = b as KPPrx;
                if(r == null)
                {
                    KPPrxHelper h = new KPPrxHelper();
                    h.copyFrom__(b);
                    r = h;
                }
                return r;
            }

            public static KPPrx uncheckedCast(Ice.ObjectPrx b, string f)
            {
                if(b == null)
                {
                    return null;
                }
                Ice.ObjectPrx bb = b.ice_facet(f);
                KPPrxHelper h = new KPPrxHelper();
                h.copyFrom__(bb);
                return h;
            }

            #endregion

            #region Marshaling support

            protected override Ice.ObjectDelM_ createDelegateM__()
            {
                return new KPDelM_();
            }

            protected override Ice.ObjectDelD_ createDelegateD__()
            {
                return new KPDelD_();
            }

            public static void write__(IceInternal.BasicStream os__, KPPrx v__)
            {
                os__.writeProxy(v__);
            }

            public static KPPrx read__(IceInternal.BasicStream is__)
            {
                Ice.ObjectPrx proxy = is__.readProxy();
                if(proxy != null)
                {
                    KPPrxHelper result = new KPPrxHelper();
                    result.copyFrom__(proxy);
                    return result;
                }
                return null;
            }

            #endregion
        }

        public sealed class CallbackReceiverPrxHelper : Ice.ObjectPrxHelperBase, CallbackReceiverPrx
        {
            #region Synchronous operations

            public void callback(int num)
            {
                callback(num, null, false);
            }

            public void callback(int num, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                callback(num, context__, true);
            }

            private void callback(int num, _System.Collections.Generic.Dictionary<string, string> context__, bool explicitContext__)
            {
                if(explicitContext__ && context__ == null)
                {
                    context__ = emptyContext_;
                }
                int cnt__ = 0;
                while(true)
                {
                    Ice.ObjectDel_ delBase__ = null;
                    try
                    {
                        delBase__ = getDelegate__(false);
                        CallbackReceiverDel_ del__ = (CallbackReceiverDel_)delBase__;
                        del__.callback(num, context__);
                        return;
                    }
                    catch(IceInternal.LocalExceptionWrapper ex__)
                    {
                        handleExceptionWrapper__(delBase__, ex__, null);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        handleException__(delBase__, ex__, null, ref cnt__);
                    }
                }
            }

            #endregion

            #region Checked and unchecked cast operations

            public static CallbackReceiverPrx checkedCast(Ice.ObjectPrx b)
            {
                if(b == null)
                {
                    return null;
                }
                CallbackReceiverPrx r = b as CallbackReceiverPrx;
                if((r == null) && b.ice_isA("::KeeICE::KPlib::CallbackReceiver"))
                {
                    CallbackReceiverPrxHelper h = new CallbackReceiverPrxHelper();
                    h.copyFrom__(b);
                    r = h;
                }
                return r;
            }

            public static CallbackReceiverPrx checkedCast(Ice.ObjectPrx b, _System.Collections.Generic.Dictionary<string, string> ctx)
            {
                if(b == null)
                {
                    return null;
                }
                CallbackReceiverPrx r = b as CallbackReceiverPrx;
                if((r == null) && b.ice_isA("::KeeICE::KPlib::CallbackReceiver", ctx))
                {
                    CallbackReceiverPrxHelper h = new CallbackReceiverPrxHelper();
                    h.copyFrom__(b);
                    r = h;
                }
                return r;
            }

            public static CallbackReceiverPrx checkedCast(Ice.ObjectPrx b, string f)
            {
                if(b == null)
                {
                    return null;
                }
                Ice.ObjectPrx bb = b.ice_facet(f);
                try
                {
                    if(bb.ice_isA("::KeeICE::KPlib::CallbackReceiver"))
                    {
                        CallbackReceiverPrxHelper h = new CallbackReceiverPrxHelper();
                        h.copyFrom__(bb);
                        return h;
                    }
                }
                catch(Ice.FacetNotExistException)
                {
                }
                return null;
            }

            public static CallbackReceiverPrx checkedCast(Ice.ObjectPrx b, string f, _System.Collections.Generic.Dictionary<string, string> ctx)
            {
                if(b == null)
                {
                    return null;
                }
                Ice.ObjectPrx bb = b.ice_facet(f);
                try
                {
                    if(bb.ice_isA("::KeeICE::KPlib::CallbackReceiver", ctx))
                    {
                        CallbackReceiverPrxHelper h = new CallbackReceiverPrxHelper();
                        h.copyFrom__(bb);
                        return h;
                    }
                }
                catch(Ice.FacetNotExistException)
                {
                }
                return null;
            }

            public static CallbackReceiverPrx uncheckedCast(Ice.ObjectPrx b)
            {
                if(b == null)
                {
                    return null;
                }
                CallbackReceiverPrx r = b as CallbackReceiverPrx;
                if(r == null)
                {
                    CallbackReceiverPrxHelper h = new CallbackReceiverPrxHelper();
                    h.copyFrom__(b);
                    r = h;
                }
                return r;
            }

            public static CallbackReceiverPrx uncheckedCast(Ice.ObjectPrx b, string f)
            {
                if(b == null)
                {
                    return null;
                }
                Ice.ObjectPrx bb = b.ice_facet(f);
                CallbackReceiverPrxHelper h = new CallbackReceiverPrxHelper();
                h.copyFrom__(bb);
                return h;
            }

            #endregion

            #region Marshaling support

            protected override Ice.ObjectDelM_ createDelegateM__()
            {
                return new CallbackReceiverDelM_();
            }

            protected override Ice.ObjectDelD_ createDelegateD__()
            {
                return new CallbackReceiverDelD_();
            }

            public static void write__(IceInternal.BasicStream os__, CallbackReceiverPrx v__)
            {
                os__.writeProxy(v__);
            }

            public static CallbackReceiverPrx read__(IceInternal.BasicStream is__)
            {
                Ice.ObjectPrx proxy = is__.readProxy();
                if(proxy != null)
                {
                    CallbackReceiverPrxHelper result = new CallbackReceiverPrxHelper();
                    result.copyFrom__(proxy);
                    return result;
                }
                return null;
            }

            #endregion
        }
    }
}

namespace KeeICE
{
    namespace KPlib
    {
        public interface KPDel_ : Ice.ObjectDel_
        {
            bool checkVersion(float keeFoxVersion, float keeICEVersion, out int result, _System.Collections.Generic.Dictionary<string, string> context__);

            string getDatabaseName(_System.Collections.Generic.Dictionary<string, string> context__);

            string getDatabaseFileName(_System.Collections.Generic.Dictionary<string, string> context__);

            void changeDatabase(string fileName, bool closeCurrent, _System.Collections.Generic.Dictionary<string, string> context__);

            KeeICE.KPlib.KPEntry AddLogin(KeeICE.KPlib.KPEntry login, string parentUUID, _System.Collections.Generic.Dictionary<string, string> context__);

            void ModifyLogin(KeeICE.KPlib.KPEntry oldLogin, KeeICE.KPlib.KPEntry newLogin, _System.Collections.Generic.Dictionary<string, string> context__);

            int getAllLogins(out KeeICE.KPlib.KPEntry[] logins, _System.Collections.Generic.Dictionary<string, string> context__);

            int findLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches, string uniqueID, out KeeICE.KPlib.KPEntry[] logins, _System.Collections.Generic.Dictionary<string, string> context__);

            int countLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches, _System.Collections.Generic.Dictionary<string, string> context__);

            void addClient(Ice.Identity ident, _System.Collections.Generic.Dictionary<string, string> context__);

            int findGroups(string name, string uuid, out KeeICE.KPlib.KPGroup[] groups, _System.Collections.Generic.Dictionary<string, string> context__);

            KeeICE.KPlib.KPGroup getRoot(_System.Collections.Generic.Dictionary<string, string> context__);

            KeeICE.KPlib.KPGroup getParent(string uuid, _System.Collections.Generic.Dictionary<string, string> context__);

            KeeICE.KPlib.KPGroup[] getChildGroups(string uuid, _System.Collections.Generic.Dictionary<string, string> context__);

            KeeICE.KPlib.KPEntry[] getChildEntries(string uuid, _System.Collections.Generic.Dictionary<string, string> context__);

            KeeICE.KPlib.KPGroup addGroup(string name, string parentUuid, _System.Collections.Generic.Dictionary<string, string> context__);

            bool removeGroup(string uuid, _System.Collections.Generic.Dictionary<string, string> context__);

            bool removeEntry(string uuid, _System.Collections.Generic.Dictionary<string, string> context__);

            void LaunchGroupEditor(string uuid, _System.Collections.Generic.Dictionary<string, string> context__);

            void LaunchLoginEditor(string uuid, _System.Collections.Generic.Dictionary<string, string> context__);

            KeeICE.KPlib.KFConfiguration getCurrentKFConfig(_System.Collections.Generic.Dictionary<string, string> context__);

            bool setCurrentKFConfig(KeeICE.KPlib.KFConfiguration config, _System.Collections.Generic.Dictionary<string, string> context__);

            bool setCurrentDBRootGroup(string uuid, _System.Collections.Generic.Dictionary<string, string> context__);
        }

        public interface CallbackReceiverDel_ : Ice.ObjectDel_
        {
            void callback(int num, _System.Collections.Generic.Dictionary<string, string> context__);
        }
    }
}

namespace KeeICE
{
    namespace KPlib
    {
        public sealed class KPDelM_ : Ice.ObjectDelM_, KPDel_
        {
            public KeeICE.KPlib.KPEntry AddLogin(KeeICE.KPlib.KPEntry login, string parentUUID, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("AddLogin", Ice.OperationMode.Normal, context__);
                try
                {
                    try
                    {
                        IceInternal.BasicStream os__ = og__.ostr();
                        if(login == null)
                        {
                            KeeICE.KPlib.KPEntry tmp__ = new KeeICE.KPlib.KPEntry();
                            tmp__.write__(os__);
                        }
                        else
                        {
                            login.write__(os__);
                        }
                        os__.writeString(parentUUID);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        og__.abort(ex__);
                    }
                    bool ok__ = og__.invoke();
                    try
                    {
                        if(!ok__)
                        {
                            try
                            {
                                og__.throwUserException();
                            }
                            catch(KeeICE.KPlib.KeeICEException)
                            {
                                throw;
                            }
                            catch(Ice.UserException ex)
                            {
                                throw new Ice.UnknownUserException(ex.ice_name(), ex);
                            }
                        }
                        IceInternal.BasicStream is__ = og__.istr();
                        is__.startReadEncaps();
                        KeeICE.KPlib.KPEntry ret__;
                        ret__ = null;
                        if(ret__ == null)
                        {
                            ret__ = new KeeICE.KPlib.KPEntry();
                        }
                        ret__.read__(is__);
                        is__.endReadEncaps();
                        return ret__;
                    }
                    catch(Ice.LocalException ex__)
                    {
                        throw new IceInternal.LocalExceptionWrapper(ex__, false);
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public void LaunchGroupEditor(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("LaunchGroupEditor", Ice.OperationMode.Normal, context__);
                try
                {
                    try
                    {
                        IceInternal.BasicStream os__ = og__.ostr();
                        os__.writeString(uuid);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        og__.abort(ex__);
                    }
                    bool ok__ = og__.invoke();
                    if(!og__.istr().isEmpty())
                    {
                        try
                        {
                            if(!ok__)
                            {
                                try
                                {
                                    og__.throwUserException();
                                }
                                catch(Ice.UserException ex)
                                {
                                    throw new Ice.UnknownUserException(ex.ice_name(), ex);
                                }
                            }
                            og__.istr().skipEmptyEncaps();
                        }
                        catch(Ice.LocalException ex__)
                        {
                            throw new IceInternal.LocalExceptionWrapper(ex__, false);
                        }
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public void LaunchLoginEditor(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("LaunchLoginEditor", Ice.OperationMode.Normal, context__);
                try
                {
                    try
                    {
                        IceInternal.BasicStream os__ = og__.ostr();
                        os__.writeString(uuid);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        og__.abort(ex__);
                    }
                    bool ok__ = og__.invoke();
                    if(!og__.istr().isEmpty())
                    {
                        try
                        {
                            if(!ok__)
                            {
                                try
                                {
                                    og__.throwUserException();
                                }
                                catch(Ice.UserException ex)
                                {
                                    throw new Ice.UnknownUserException(ex.ice_name(), ex);
                                }
                            }
                            og__.istr().skipEmptyEncaps();
                        }
                        catch(Ice.LocalException ex__)
                        {
                            throw new IceInternal.LocalExceptionWrapper(ex__, false);
                        }
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public void ModifyLogin(KeeICE.KPlib.KPEntry oldLogin, KeeICE.KPlib.KPEntry newLogin, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("ModifyLogin", Ice.OperationMode.Normal, context__);
                try
                {
                    try
                    {
                        IceInternal.BasicStream os__ = og__.ostr();
                        if(oldLogin == null)
                        {
                            KeeICE.KPlib.KPEntry tmp__ = new KeeICE.KPlib.KPEntry();
                            tmp__.write__(os__);
                        }
                        else
                        {
                            oldLogin.write__(os__);
                        }
                        if(newLogin == null)
                        {
                            KeeICE.KPlib.KPEntry tmp__ = new KeeICE.KPlib.KPEntry();
                            tmp__.write__(os__);
                        }
                        else
                        {
                            newLogin.write__(os__);
                        }
                    }
                    catch(Ice.LocalException ex__)
                    {
                        og__.abort(ex__);
                    }
                    bool ok__ = og__.invoke();
                    try
                    {
                        if(!ok__)
                        {
                            try
                            {
                                og__.throwUserException();
                            }
                            catch(KeeICE.KPlib.KeeICEException)
                            {
                                throw;
                            }
                            catch(Ice.UserException ex)
                            {
                                throw new Ice.UnknownUserException(ex.ice_name(), ex);
                            }
                        }
                        IceInternal.BasicStream is__ = og__.istr();
                        is__.startReadEncaps();
                        is__.endReadEncaps();
                    }
                    catch(Ice.LocalException ex__)
                    {
                        throw new IceInternal.LocalExceptionWrapper(ex__, false);
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public void addClient(Ice.Identity ident, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("addClient", Ice.OperationMode.Normal, context__);
                try
                {
                    try
                    {
                        IceInternal.BasicStream os__ = og__.ostr();
                        if(ident == null)
                        {
                            Ice.Identity tmp__ = new Ice.Identity();
                            tmp__.write__(os__);
                        }
                        else
                        {
                            ident.write__(os__);
                        }
                    }
                    catch(Ice.LocalException ex__)
                    {
                        og__.abort(ex__);
                    }
                    bool ok__ = og__.invoke();
                    if(!og__.istr().isEmpty())
                    {
                        try
                        {
                            if(!ok__)
                            {
                                try
                                {
                                    og__.throwUserException();
                                }
                                catch(Ice.UserException ex)
                                {
                                    throw new Ice.UnknownUserException(ex.ice_name(), ex);
                                }
                            }
                            og__.istr().skipEmptyEncaps();
                        }
                        catch(Ice.LocalException ex__)
                        {
                            throw new IceInternal.LocalExceptionWrapper(ex__, false);
                        }
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public KeeICE.KPlib.KPGroup addGroup(string name, string parentUuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("addGroup", Ice.OperationMode.Normal, context__);
                try
                {
                    try
                    {
                        IceInternal.BasicStream os__ = og__.ostr();
                        os__.writeString(name);
                        os__.writeString(parentUuid);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        og__.abort(ex__);
                    }
                    bool ok__ = og__.invoke();
                    try
                    {
                        if(!ok__)
                        {
                            try
                            {
                                og__.throwUserException();
                            }
                            catch(Ice.UserException ex)
                            {
                                throw new Ice.UnknownUserException(ex.ice_name(), ex);
                            }
                        }
                        IceInternal.BasicStream is__ = og__.istr();
                        is__.startReadEncaps();
                        KeeICE.KPlib.KPGroup ret__;
                        ret__ = null;
                        if(ret__ == null)
                        {
                            ret__ = new KeeICE.KPlib.KPGroup();
                        }
                        ret__.read__(is__);
                        is__.endReadEncaps();
                        return ret__;
                    }
                    catch(Ice.LocalException ex__)
                    {
                        throw new IceInternal.LocalExceptionWrapper(ex__, false);
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public void changeDatabase(string fileName, bool closeCurrent, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("changeDatabase", Ice.OperationMode.Normal, context__);
                try
                {
                    try
                    {
                        IceInternal.BasicStream os__ = og__.ostr();
                        os__.writeString(fileName);
                        os__.writeBool(closeCurrent);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        og__.abort(ex__);
                    }
                    bool ok__ = og__.invoke();
                    if(!og__.istr().isEmpty())
                    {
                        try
                        {
                            if(!ok__)
                            {
                                try
                                {
                                    og__.throwUserException();
                                }
                                catch(Ice.UserException ex)
                                {
                                    throw new Ice.UnknownUserException(ex.ice_name(), ex);
                                }
                            }
                            og__.istr().skipEmptyEncaps();
                        }
                        catch(Ice.LocalException ex__)
                        {
                            throw new IceInternal.LocalExceptionWrapper(ex__, false);
                        }
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public bool checkVersion(float keeFoxVersion, float keeICEVersion, out int result, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("checkVersion", Ice.OperationMode.Normal, context__);
                try
                {
                    try
                    {
                        IceInternal.BasicStream os__ = og__.ostr();
                        os__.writeFloat(keeFoxVersion);
                        os__.writeFloat(keeICEVersion);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        og__.abort(ex__);
                    }
                    bool ok__ = og__.invoke();
                    try
                    {
                        if(!ok__)
                        {
                            try
                            {
                                og__.throwUserException();
                            }
                            catch(Ice.UserException ex)
                            {
                                throw new Ice.UnknownUserException(ex.ice_name(), ex);
                            }
                        }
                        IceInternal.BasicStream is__ = og__.istr();
                        is__.startReadEncaps();
                        result = is__.readInt();
                        bool ret__;
                        ret__ = is__.readBool();
                        is__.endReadEncaps();
                        return ret__;
                    }
                    catch(Ice.LocalException ex__)
                    {
                        throw new IceInternal.LocalExceptionWrapper(ex__, false);
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public int countLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("countLogins", Ice.OperationMode.Normal, context__);
                try
                {
                    try
                    {
                        IceInternal.BasicStream os__ = og__.ostr();
                        os__.writeString(hostname);
                        os__.writeString(actionURL);
                        os__.writeString(httpRealm);
                        os__.writeByte((byte)lst, 3);
                        os__.writeBool(requireFullURLMatches);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        og__.abort(ex__);
                    }
                    bool ok__ = og__.invoke();
                    try
                    {
                        if(!ok__)
                        {
                            try
                            {
                                og__.throwUserException();
                            }
                            catch(KeeICE.KPlib.KeeICEException)
                            {
                                throw;
                            }
                            catch(Ice.UserException ex)
                            {
                                throw new Ice.UnknownUserException(ex.ice_name(), ex);
                            }
                        }
                        IceInternal.BasicStream is__ = og__.istr();
                        is__.startReadEncaps();
                        int ret__;
                        ret__ = is__.readInt();
                        is__.endReadEncaps();
                        return ret__;
                    }
                    catch(Ice.LocalException ex__)
                    {
                        throw new IceInternal.LocalExceptionWrapper(ex__, false);
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public int findGroups(string name, string uuid, out KeeICE.KPlib.KPGroup[] groups, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("findGroups", Ice.OperationMode.Normal, context__);
                try
                {
                    try
                    {
                        IceInternal.BasicStream os__ = og__.ostr();
                        os__.writeString(name);
                        os__.writeString(uuid);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        og__.abort(ex__);
                    }
                    bool ok__ = og__.invoke();
                    try
                    {
                        if(!ok__)
                        {
                            try
                            {
                                og__.throwUserException();
                            }
                            catch(Ice.UserException ex)
                            {
                                throw new Ice.UnknownUserException(ex.ice_name(), ex);
                            }
                        }
                        IceInternal.BasicStream is__ = og__.istr();
                        is__.startReadEncaps();
                        {
                            int szx__ = is__.readSize();
                            is__.startSeq(szx__, 3);
                            groups = new KeeICE.KPlib.KPGroup[szx__];
                            for(int ix__ = 0; ix__ < szx__; ++ix__)
                            {
                                groups[ix__] = new KeeICE.KPlib.KPGroup();
                                groups[ix__].read__(is__);
                                is__.checkSeq();
                                is__.endElement();
                            }
                            is__.endSeq(szx__);
                        }
                        int ret__;
                        ret__ = is__.readInt();
                        is__.endReadEncaps();
                        return ret__;
                    }
                    catch(Ice.LocalException ex__)
                    {
                        throw new IceInternal.LocalExceptionWrapper(ex__, false);
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public int findLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches, string uniqueID, out KeeICE.KPlib.KPEntry[] logins, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("findLogins", Ice.OperationMode.Normal, context__);
                try
                {
                    try
                    {
                        IceInternal.BasicStream os__ = og__.ostr();
                        os__.writeString(hostname);
                        os__.writeString(actionURL);
                        os__.writeString(httpRealm);
                        os__.writeByte((byte)lst, 3);
                        os__.writeBool(requireFullURLMatches);
                        os__.writeString(uniqueID);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        og__.abort(ex__);
                    }
                    bool ok__ = og__.invoke();
                    try
                    {
                        if(!ok__)
                        {
                            try
                            {
                                og__.throwUserException();
                            }
                            catch(KeeICE.KPlib.KeeICEException)
                            {
                                throw;
                            }
                            catch(Ice.UserException ex)
                            {
                                throw new Ice.UnknownUserException(ex.ice_name(), ex);
                            }
                        }
                        IceInternal.BasicStream is__ = og__.istr();
                        is__.startReadEncaps();
                        {
                            int szx__ = is__.readSize();
                            is__.startSeq(szx__, 19);
                            logins = new KeeICE.KPlib.KPEntry[szx__];
                            for(int ix__ = 0; ix__ < szx__; ++ix__)
                            {
                                logins[ix__] = new KeeICE.KPlib.KPEntry();
                                logins[ix__].read__(is__);
                                is__.checkSeq();
                                is__.endElement();
                            }
                            is__.endSeq(szx__);
                        }
                        int ret__;
                        ret__ = is__.readInt();
                        is__.endReadEncaps();
                        return ret__;
                    }
                    catch(Ice.LocalException ex__)
                    {
                        throw new IceInternal.LocalExceptionWrapper(ex__, false);
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public int getAllLogins(out KeeICE.KPlib.KPEntry[] logins, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("getAllLogins", Ice.OperationMode.Normal, context__);
                try
                {
                    bool ok__ = og__.invoke();
                    try
                    {
                        if(!ok__)
                        {
                            try
                            {
                                og__.throwUserException();
                            }
                            catch(KeeICE.KPlib.KeeICEException)
                            {
                                throw;
                            }
                            catch(Ice.UserException ex)
                            {
                                throw new Ice.UnknownUserException(ex.ice_name(), ex);
                            }
                        }
                        IceInternal.BasicStream is__ = og__.istr();
                        is__.startReadEncaps();
                        {
                            int szx__ = is__.readSize();
                            is__.startSeq(szx__, 19);
                            logins = new KeeICE.KPlib.KPEntry[szx__];
                            for(int ix__ = 0; ix__ < szx__; ++ix__)
                            {
                                logins[ix__] = new KeeICE.KPlib.KPEntry();
                                logins[ix__].read__(is__);
                                is__.checkSeq();
                                is__.endElement();
                            }
                            is__.endSeq(szx__);
                        }
                        int ret__;
                        ret__ = is__.readInt();
                        is__.endReadEncaps();
                        return ret__;
                    }
                    catch(Ice.LocalException ex__)
                    {
                        throw new IceInternal.LocalExceptionWrapper(ex__, false);
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public KeeICE.KPlib.KPEntry[] getChildEntries(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("getChildEntries", Ice.OperationMode.Normal, context__);
                try
                {
                    try
                    {
                        IceInternal.BasicStream os__ = og__.ostr();
                        os__.writeString(uuid);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        og__.abort(ex__);
                    }
                    bool ok__ = og__.invoke();
                    try
                    {
                        if(!ok__)
                        {
                            try
                            {
                                og__.throwUserException();
                            }
                            catch(Ice.UserException ex)
                            {
                                throw new Ice.UnknownUserException(ex.ice_name(), ex);
                            }
                        }
                        IceInternal.BasicStream is__ = og__.istr();
                        is__.startReadEncaps();
                        KeeICE.KPlib.KPEntry[] ret__;
                        {
                            int szx__ = is__.readSize();
                            is__.startSeq(szx__, 19);
                            ret__ = new KeeICE.KPlib.KPEntry[szx__];
                            for(int ix__ = 0; ix__ < szx__; ++ix__)
                            {
                                ret__[ix__] = new KeeICE.KPlib.KPEntry();
                                ret__[ix__].read__(is__);
                                is__.checkSeq();
                                is__.endElement();
                            }
                            is__.endSeq(szx__);
                        }
                        is__.endReadEncaps();
                        return ret__;
                    }
                    catch(Ice.LocalException ex__)
                    {
                        throw new IceInternal.LocalExceptionWrapper(ex__, false);
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public KeeICE.KPlib.KPGroup[] getChildGroups(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("getChildGroups", Ice.OperationMode.Normal, context__);
                try
                {
                    try
                    {
                        IceInternal.BasicStream os__ = og__.ostr();
                        os__.writeString(uuid);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        og__.abort(ex__);
                    }
                    bool ok__ = og__.invoke();
                    try
                    {
                        if(!ok__)
                        {
                            try
                            {
                                og__.throwUserException();
                            }
                            catch(Ice.UserException ex)
                            {
                                throw new Ice.UnknownUserException(ex.ice_name(), ex);
                            }
                        }
                        IceInternal.BasicStream is__ = og__.istr();
                        is__.startReadEncaps();
                        KeeICE.KPlib.KPGroup[] ret__;
                        {
                            int szx__ = is__.readSize();
                            is__.startSeq(szx__, 3);
                            ret__ = new KeeICE.KPlib.KPGroup[szx__];
                            for(int ix__ = 0; ix__ < szx__; ++ix__)
                            {
                                ret__[ix__] = new KeeICE.KPlib.KPGroup();
                                ret__[ix__].read__(is__);
                                is__.checkSeq();
                                is__.endElement();
                            }
                            is__.endSeq(szx__);
                        }
                        is__.endReadEncaps();
                        return ret__;
                    }
                    catch(Ice.LocalException ex__)
                    {
                        throw new IceInternal.LocalExceptionWrapper(ex__, false);
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public KeeICE.KPlib.KFConfiguration getCurrentKFConfig(_System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("getCurrentKFConfig", Ice.OperationMode.Normal, context__);
                try
                {
                    bool ok__ = og__.invoke();
                    try
                    {
                        if(!ok__)
                        {
                            try
                            {
                                og__.throwUserException();
                            }
                            catch(Ice.UserException ex)
                            {
                                throw new Ice.UnknownUserException(ex.ice_name(), ex);
                            }
                        }
                        IceInternal.BasicStream is__ = og__.istr();
                        is__.startReadEncaps();
                        KeeICE.KPlib.KFConfiguration ret__;
                        ret__ = null;
                        if(ret__ == null)
                        {
                            ret__ = new KeeICE.KPlib.KFConfiguration();
                        }
                        ret__.read__(is__);
                        is__.endReadEncaps();
                        return ret__;
                    }
                    catch(Ice.LocalException ex__)
                    {
                        throw new IceInternal.LocalExceptionWrapper(ex__, false);
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public string getDatabaseFileName(_System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("getDatabaseFileName", Ice.OperationMode.Normal, context__);
                try
                {
                    bool ok__ = og__.invoke();
                    try
                    {
                        if(!ok__)
                        {
                            try
                            {
                                og__.throwUserException();
                            }
                            catch(Ice.UserException ex)
                            {
                                throw new Ice.UnknownUserException(ex.ice_name(), ex);
                            }
                        }
                        IceInternal.BasicStream is__ = og__.istr();
                        is__.startReadEncaps();
                        string ret__;
                        ret__ = is__.readString();
                        is__.endReadEncaps();
                        return ret__;
                    }
                    catch(Ice.LocalException ex__)
                    {
                        throw new IceInternal.LocalExceptionWrapper(ex__, false);
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public string getDatabaseName(_System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("getDatabaseName", Ice.OperationMode.Normal, context__);
                try
                {
                    bool ok__ = og__.invoke();
                    try
                    {
                        if(!ok__)
                        {
                            try
                            {
                                og__.throwUserException();
                            }
                            catch(Ice.UserException ex)
                            {
                                throw new Ice.UnknownUserException(ex.ice_name(), ex);
                            }
                        }
                        IceInternal.BasicStream is__ = og__.istr();
                        is__.startReadEncaps();
                        string ret__;
                        ret__ = is__.readString();
                        is__.endReadEncaps();
                        return ret__;
                    }
                    catch(Ice.LocalException ex__)
                    {
                        throw new IceInternal.LocalExceptionWrapper(ex__, false);
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public KeeICE.KPlib.KPGroup getParent(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("getParent", Ice.OperationMode.Normal, context__);
                try
                {
                    try
                    {
                        IceInternal.BasicStream os__ = og__.ostr();
                        os__.writeString(uuid);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        og__.abort(ex__);
                    }
                    bool ok__ = og__.invoke();
                    try
                    {
                        if(!ok__)
                        {
                            try
                            {
                                og__.throwUserException();
                            }
                            catch(Ice.UserException ex)
                            {
                                throw new Ice.UnknownUserException(ex.ice_name(), ex);
                            }
                        }
                        IceInternal.BasicStream is__ = og__.istr();
                        is__.startReadEncaps();
                        KeeICE.KPlib.KPGroup ret__;
                        ret__ = null;
                        if(ret__ == null)
                        {
                            ret__ = new KeeICE.KPlib.KPGroup();
                        }
                        ret__.read__(is__);
                        is__.endReadEncaps();
                        return ret__;
                    }
                    catch(Ice.LocalException ex__)
                    {
                        throw new IceInternal.LocalExceptionWrapper(ex__, false);
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public KeeICE.KPlib.KPGroup getRoot(_System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("getRoot", Ice.OperationMode.Normal, context__);
                try
                {
                    bool ok__ = og__.invoke();
                    try
                    {
                        if(!ok__)
                        {
                            try
                            {
                                og__.throwUserException();
                            }
                            catch(Ice.UserException ex)
                            {
                                throw new Ice.UnknownUserException(ex.ice_name(), ex);
                            }
                        }
                        IceInternal.BasicStream is__ = og__.istr();
                        is__.startReadEncaps();
                        KeeICE.KPlib.KPGroup ret__;
                        ret__ = null;
                        if(ret__ == null)
                        {
                            ret__ = new KeeICE.KPlib.KPGroup();
                        }
                        ret__.read__(is__);
                        is__.endReadEncaps();
                        return ret__;
                    }
                    catch(Ice.LocalException ex__)
                    {
                        throw new IceInternal.LocalExceptionWrapper(ex__, false);
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public bool removeEntry(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("removeEntry", Ice.OperationMode.Normal, context__);
                try
                {
                    try
                    {
                        IceInternal.BasicStream os__ = og__.ostr();
                        os__.writeString(uuid);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        og__.abort(ex__);
                    }
                    bool ok__ = og__.invoke();
                    try
                    {
                        if(!ok__)
                        {
                            try
                            {
                                og__.throwUserException();
                            }
                            catch(Ice.UserException ex)
                            {
                                throw new Ice.UnknownUserException(ex.ice_name(), ex);
                            }
                        }
                        IceInternal.BasicStream is__ = og__.istr();
                        is__.startReadEncaps();
                        bool ret__;
                        ret__ = is__.readBool();
                        is__.endReadEncaps();
                        return ret__;
                    }
                    catch(Ice.LocalException ex__)
                    {
                        throw new IceInternal.LocalExceptionWrapper(ex__, false);
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public bool removeGroup(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("removeGroup", Ice.OperationMode.Normal, context__);
                try
                {
                    try
                    {
                        IceInternal.BasicStream os__ = og__.ostr();
                        os__.writeString(uuid);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        og__.abort(ex__);
                    }
                    bool ok__ = og__.invoke();
                    try
                    {
                        if(!ok__)
                        {
                            try
                            {
                                og__.throwUserException();
                            }
                            catch(Ice.UserException ex)
                            {
                                throw new Ice.UnknownUserException(ex.ice_name(), ex);
                            }
                        }
                        IceInternal.BasicStream is__ = og__.istr();
                        is__.startReadEncaps();
                        bool ret__;
                        ret__ = is__.readBool();
                        is__.endReadEncaps();
                        return ret__;
                    }
                    catch(Ice.LocalException ex__)
                    {
                        throw new IceInternal.LocalExceptionWrapper(ex__, false);
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public bool setCurrentDBRootGroup(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("setCurrentDBRootGroup", Ice.OperationMode.Normal, context__);
                try
                {
                    try
                    {
                        IceInternal.BasicStream os__ = og__.ostr();
                        os__.writeString(uuid);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        og__.abort(ex__);
                    }
                    bool ok__ = og__.invoke();
                    try
                    {
                        if(!ok__)
                        {
                            try
                            {
                                og__.throwUserException();
                            }
                            catch(Ice.UserException ex)
                            {
                                throw new Ice.UnknownUserException(ex.ice_name(), ex);
                            }
                        }
                        IceInternal.BasicStream is__ = og__.istr();
                        is__.startReadEncaps();
                        bool ret__;
                        ret__ = is__.readBool();
                        is__.endReadEncaps();
                        return ret__;
                    }
                    catch(Ice.LocalException ex__)
                    {
                        throw new IceInternal.LocalExceptionWrapper(ex__, false);
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }

            public bool setCurrentKFConfig(KeeICE.KPlib.KFConfiguration config, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("setCurrentKFConfig", Ice.OperationMode.Normal, context__);
                try
                {
                    try
                    {
                        IceInternal.BasicStream os__ = og__.ostr();
                        if(config == null)
                        {
                            KeeICE.KPlib.KFConfiguration tmp__ = new KeeICE.KPlib.KFConfiguration();
                            tmp__.write__(os__);
                        }
                        else
                        {
                            config.write__(os__);
                        }
                    }
                    catch(Ice.LocalException ex__)
                    {
                        og__.abort(ex__);
                    }
                    bool ok__ = og__.invoke();
                    try
                    {
                        if(!ok__)
                        {
                            try
                            {
                                og__.throwUserException();
                            }
                            catch(Ice.UserException ex)
                            {
                                throw new Ice.UnknownUserException(ex.ice_name(), ex);
                            }
                        }
                        IceInternal.BasicStream is__ = og__.istr();
                        is__.startReadEncaps();
                        bool ret__;
                        ret__ = is__.readBool();
                        is__.endReadEncaps();
                        return ret__;
                    }
                    catch(Ice.LocalException ex__)
                    {
                        throw new IceInternal.LocalExceptionWrapper(ex__, false);
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }
        }

        public sealed class CallbackReceiverDelM_ : Ice.ObjectDelM_, CallbackReceiverDel_
        {
            public void callback(int num, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                IceInternal.Outgoing og__ = handler__.getOutgoing("callback", Ice.OperationMode.Normal, context__);
                try
                {
                    try
                    {
                        IceInternal.BasicStream os__ = og__.ostr();
                        os__.writeInt(num);
                    }
                    catch(Ice.LocalException ex__)
                    {
                        og__.abort(ex__);
                    }
                    bool ok__ = og__.invoke();
                    if(!og__.istr().isEmpty())
                    {
                        try
                        {
                            if(!ok__)
                            {
                                try
                                {
                                    og__.throwUserException();
                                }
                                catch(Ice.UserException ex)
                                {
                                    throw new Ice.UnknownUserException(ex.ice_name(), ex);
                                }
                            }
                            og__.istr().skipEmptyEncaps();
                        }
                        catch(Ice.LocalException ex__)
                        {
                            throw new IceInternal.LocalExceptionWrapper(ex__, false);
                        }
                    }
                }
                finally
                {
                    handler__.reclaimOutgoing(og__);
                }
            }
        }
    }
}

namespace KeeICE
{
    namespace KPlib
    {
        public sealed class KPDelD_ : Ice.ObjectDelD_, KPDel_
        {
            public KeeICE.KPlib.KPEntry AddLogin(KeeICE.KPlib.KPEntry login, string parentUUID, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "AddLogin", Ice.OperationMode.Normal, context__);
                KeeICE.KPlib.KPEntry result__ = new KeeICE.KPlib.KPEntry();
                Ice.UserException userException__ = null;
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    try
                    {
                        result__ = servant__.AddLogin(login, parentUUID, current__);
                        return Ice.DispatchStatus.DispatchOK;
                    }
                    catch(Ice.UserException ex__)
                    {
                        userException__ = ex__;
                        return Ice.DispatchStatus.DispatchUserException;
                    }
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        if(status__ == Ice.DispatchStatus.DispatchUserException)
                        {
                            throw userException__;
                        }
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(KeeICE.KPlib.KeeICEException)
                {
                    throw;
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
                return result__;
            }

            public void LaunchGroupEditor(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "LaunchGroupEditor", Ice.OperationMode.Normal, context__);
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    servant__.LaunchGroupEditor(uuid, current__);
                    return Ice.DispatchStatus.DispatchOK;
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
            }

            public void LaunchLoginEditor(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "LaunchLoginEditor", Ice.OperationMode.Normal, context__);
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    servant__.LaunchLoginEditor(uuid, current__);
                    return Ice.DispatchStatus.DispatchOK;
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
            }

            public void ModifyLogin(KeeICE.KPlib.KPEntry oldLogin, KeeICE.KPlib.KPEntry newLogin, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "ModifyLogin", Ice.OperationMode.Normal, context__);
                Ice.UserException userException__ = null;
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    try
                    {
                        servant__.ModifyLogin(oldLogin, newLogin, current__);
                        return Ice.DispatchStatus.DispatchOK;
                    }
                    catch(Ice.UserException ex__)
                    {
                        userException__ = ex__;
                        return Ice.DispatchStatus.DispatchUserException;
                    }
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        if(status__ == Ice.DispatchStatus.DispatchUserException)
                        {
                            throw userException__;
                        }
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(KeeICE.KPlib.KeeICEException)
                {
                    throw;
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
            }

            public void addClient(Ice.Identity ident, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "addClient", Ice.OperationMode.Normal, context__);
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    servant__.addClient(ident, current__);
                    return Ice.DispatchStatus.DispatchOK;
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
            }

            public KeeICE.KPlib.KPGroup addGroup(string name, string parentUuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "addGroup", Ice.OperationMode.Normal, context__);
                KeeICE.KPlib.KPGroup result__ = new KeeICE.KPlib.KPGroup();
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    result__ = servant__.addGroup(name, parentUuid, current__);
                    return Ice.DispatchStatus.DispatchOK;
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
                return result__;
            }

            public void changeDatabase(string fileName, bool closeCurrent, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "changeDatabase", Ice.OperationMode.Normal, context__);
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    servant__.changeDatabase(fileName, closeCurrent, current__);
                    return Ice.DispatchStatus.DispatchOK;
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
            }

            public bool checkVersion(float keeFoxVersion, float keeICEVersion, out int result, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "checkVersion", Ice.OperationMode.Normal, context__);
                int resultHolder__ = 0;
                bool result__ = false;
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    result__ = servant__.checkVersion(keeFoxVersion, keeICEVersion, out resultHolder__, current__);
                    return Ice.DispatchStatus.DispatchOK;
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
                result = resultHolder__;
                return result__;
            }

            public int countLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "countLogins", Ice.OperationMode.Normal, context__);
                int result__ = 0;
                Ice.UserException userException__ = null;
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    try
                    {
                        result__ = servant__.countLogins(hostname, actionURL, httpRealm, lst, requireFullURLMatches, current__);
                        return Ice.DispatchStatus.DispatchOK;
                    }
                    catch(Ice.UserException ex__)
                    {
                        userException__ = ex__;
                        return Ice.DispatchStatus.DispatchUserException;
                    }
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        if(status__ == Ice.DispatchStatus.DispatchUserException)
                        {
                            throw userException__;
                        }
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(KeeICE.KPlib.KeeICEException)
                {
                    throw;
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
                return result__;
            }

            public int findGroups(string name, string uuid, out KeeICE.KPlib.KPGroup[] groups, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "findGroups", Ice.OperationMode.Normal, context__);
                KeeICE.KPlib.KPGroup[] groupsHolder__ = null;
                int result__ = 0;
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    result__ = servant__.findGroups(name, uuid, out groupsHolder__, current__);
                    return Ice.DispatchStatus.DispatchOK;
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
                groups = groupsHolder__;
                return result__;
            }

            public int findLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches, string uniqueID, out KeeICE.KPlib.KPEntry[] logins, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "findLogins", Ice.OperationMode.Normal, context__);
                KeeICE.KPlib.KPEntry[] loginsHolder__ = null;
                int result__ = 0;
                Ice.UserException userException__ = null;
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    try
                    {
                        result__ = servant__.findLogins(hostname, actionURL, httpRealm, lst, requireFullURLMatches, uniqueID, out loginsHolder__, current__);
                        return Ice.DispatchStatus.DispatchOK;
                    }
                    catch(Ice.UserException ex__)
                    {
                        userException__ = ex__;
                        return Ice.DispatchStatus.DispatchUserException;
                    }
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        if(status__ == Ice.DispatchStatus.DispatchUserException)
                        {
                            throw userException__;
                        }
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(KeeICE.KPlib.KeeICEException)
                {
                    throw;
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
                logins = loginsHolder__;
                return result__;
            }

            public int getAllLogins(out KeeICE.KPlib.KPEntry[] logins, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "getAllLogins", Ice.OperationMode.Normal, context__);
                KeeICE.KPlib.KPEntry[] loginsHolder__ = null;
                int result__ = 0;
                Ice.UserException userException__ = null;
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    try
                    {
                        result__ = servant__.getAllLogins(out loginsHolder__, current__);
                        return Ice.DispatchStatus.DispatchOK;
                    }
                    catch(Ice.UserException ex__)
                    {
                        userException__ = ex__;
                        return Ice.DispatchStatus.DispatchUserException;
                    }
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        if(status__ == Ice.DispatchStatus.DispatchUserException)
                        {
                            throw userException__;
                        }
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(KeeICE.KPlib.KeeICEException)
                {
                    throw;
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
                logins = loginsHolder__;
                return result__;
            }

            public KeeICE.KPlib.KPEntry[] getChildEntries(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "getChildEntries", Ice.OperationMode.Normal, context__);
                KeeICE.KPlib.KPEntry[] result__ = null;
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    result__ = servant__.getChildEntries(uuid, current__);
                    return Ice.DispatchStatus.DispatchOK;
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
                return result__;
            }

            public KeeICE.KPlib.KPGroup[] getChildGroups(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "getChildGroups", Ice.OperationMode.Normal, context__);
                KeeICE.KPlib.KPGroup[] result__ = null;
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    result__ = servant__.getChildGroups(uuid, current__);
                    return Ice.DispatchStatus.DispatchOK;
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
                return result__;
            }

            public KeeICE.KPlib.KFConfiguration getCurrentKFConfig(_System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "getCurrentKFConfig", Ice.OperationMode.Normal, context__);
                KeeICE.KPlib.KFConfiguration result__ = new KeeICE.KPlib.KFConfiguration();
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    result__ = servant__.getCurrentKFConfig(current__);
                    return Ice.DispatchStatus.DispatchOK;
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
                return result__;
            }

            public string getDatabaseFileName(_System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "getDatabaseFileName", Ice.OperationMode.Normal, context__);
                string result__ = null;
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    result__ = servant__.getDatabaseFileName(current__);
                    return Ice.DispatchStatus.DispatchOK;
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
                return result__;
            }

            public string getDatabaseName(_System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "getDatabaseName", Ice.OperationMode.Normal, context__);
                string result__ = null;
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    result__ = servant__.getDatabaseName(current__);
                    return Ice.DispatchStatus.DispatchOK;
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
                return result__;
            }

            public KeeICE.KPlib.KPGroup getParent(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "getParent", Ice.OperationMode.Normal, context__);
                KeeICE.KPlib.KPGroup result__ = new KeeICE.KPlib.KPGroup();
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    result__ = servant__.getParent(uuid, current__);
                    return Ice.DispatchStatus.DispatchOK;
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
                return result__;
            }

            public KeeICE.KPlib.KPGroup getRoot(_System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "getRoot", Ice.OperationMode.Normal, context__);
                KeeICE.KPlib.KPGroup result__ = new KeeICE.KPlib.KPGroup();
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    result__ = servant__.getRoot(current__);
                    return Ice.DispatchStatus.DispatchOK;
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
                return result__;
            }

            public bool removeEntry(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "removeEntry", Ice.OperationMode.Normal, context__);
                bool result__ = false;
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    result__ = servant__.removeEntry(uuid, current__);
                    return Ice.DispatchStatus.DispatchOK;
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
                return result__;
            }

            public bool removeGroup(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "removeGroup", Ice.OperationMode.Normal, context__);
                bool result__ = false;
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    result__ = servant__.removeGroup(uuid, current__);
                    return Ice.DispatchStatus.DispatchOK;
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
                return result__;
            }

            public bool setCurrentDBRootGroup(string uuid, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "setCurrentDBRootGroup", Ice.OperationMode.Normal, context__);
                bool result__ = false;
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    result__ = servant__.setCurrentDBRootGroup(uuid, current__);
                    return Ice.DispatchStatus.DispatchOK;
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
                return result__;
            }

            public bool setCurrentKFConfig(KeeICE.KPlib.KFConfiguration config, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "setCurrentKFConfig", Ice.OperationMode.Normal, context__);
                bool result__ = false;
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    KP servant__ = null;
                    try
                    {
                        servant__ = (KP)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    result__ = servant__.setCurrentKFConfig(config, current__);
                    return Ice.DispatchStatus.DispatchOK;
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
                return result__;
            }
        }

        public sealed class CallbackReceiverDelD_ : Ice.ObjectDelD_, CallbackReceiverDel_
        {
            public void callback(int num, _System.Collections.Generic.Dictionary<string, string> context__)
            {
                Ice.Current current__ = new Ice.Current();
                initCurrent__(ref current__, "callback", Ice.OperationMode.Normal, context__);
                IceInternal.Direct.RunDelegate run__ = delegate(Ice.Object obj__)
                {
                    CallbackReceiver servant__ = null;
                    try
                    {
                        servant__ = (CallbackReceiver)obj__;
                    }
                    catch(_System.InvalidCastException)
                    {
                        throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                    }
                    servant__.callback(num, current__);
                    return Ice.DispatchStatus.DispatchOK;
                };
                IceInternal.Direct direct__ = null;
                try
                {
                    direct__ = new IceInternal.Direct(current__, run__);
                    try
                    {
                        Ice.DispatchStatus status__ = direct__.servant().collocDispatch__(direct__);
                        _System.Diagnostics.Debug.Assert(status__ == Ice.DispatchStatus.DispatchOK);
                    }
                    finally
                    {
                        direct__.destroy();
                    }
                }
                catch(Ice.SystemException)
                {
                    throw;
                }
                catch(System.Exception ex__)
                {
                    IceInternal.LocalExceptionWrapper.throwWrapper(ex__);
                }
            }
        }
    }
}

namespace KeeICE
{
    namespace KPlib
    {
        public abstract class KPDisp_ : Ice.ObjectImpl, KP
        {
            #region Slice operations

            public bool checkVersion(float keeFoxVersion, float keeICEVersion, out int result)
            {
                return checkVersion(keeFoxVersion, keeICEVersion, out result, Ice.ObjectImpl.defaultCurrent);
            }

            public abstract bool checkVersion(float keeFoxVersion, float keeICEVersion, out int result, Ice.Current current__);

            public string getDatabaseName()
            {
                return getDatabaseName(Ice.ObjectImpl.defaultCurrent);
            }

            public abstract string getDatabaseName(Ice.Current current__);

            public string getDatabaseFileName()
            {
                return getDatabaseFileName(Ice.ObjectImpl.defaultCurrent);
            }

            public abstract string getDatabaseFileName(Ice.Current current__);

            public void changeDatabase(string fileName, bool closeCurrent)
            {
                changeDatabase(fileName, closeCurrent, Ice.ObjectImpl.defaultCurrent);
            }

            public abstract void changeDatabase(string fileName, bool closeCurrent, Ice.Current current__);

            public KeeICE.KPlib.KPEntry AddLogin(KeeICE.KPlib.KPEntry login, string parentUUID)
            {
                return AddLogin(login, parentUUID, Ice.ObjectImpl.defaultCurrent);
            }

            public abstract KeeICE.KPlib.KPEntry AddLogin(KeeICE.KPlib.KPEntry login, string parentUUID, Ice.Current current__);

            public void ModifyLogin(KeeICE.KPlib.KPEntry oldLogin, KeeICE.KPlib.KPEntry newLogin)
            {
                ModifyLogin(oldLogin, newLogin, Ice.ObjectImpl.defaultCurrent);
            }

            public abstract void ModifyLogin(KeeICE.KPlib.KPEntry oldLogin, KeeICE.KPlib.KPEntry newLogin, Ice.Current current__);

            public int getAllLogins(out KeeICE.KPlib.KPEntry[] logins)
            {
                return getAllLogins(out logins, Ice.ObjectImpl.defaultCurrent);
            }

            public abstract int getAllLogins(out KeeICE.KPlib.KPEntry[] logins, Ice.Current current__);

            public int findLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches, string uniqueID, out KeeICE.KPlib.KPEntry[] logins)
            {
                return findLogins(hostname, actionURL, httpRealm, lst, requireFullURLMatches, uniqueID, out logins, Ice.ObjectImpl.defaultCurrent);
            }

            public abstract int findLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches, string uniqueID, out KeeICE.KPlib.KPEntry[] logins, Ice.Current current__);

            public int countLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches)
            {
                return countLogins(hostname, actionURL, httpRealm, lst, requireFullURLMatches, Ice.ObjectImpl.defaultCurrent);
            }

            public abstract int countLogins(string hostname, string actionURL, string httpRealm, KeeICE.KPlib.loginSearchType lst, bool requireFullURLMatches, Ice.Current current__);

            public void addClient(Ice.Identity ident)
            {
                addClient(ident, Ice.ObjectImpl.defaultCurrent);
            }

            public abstract void addClient(Ice.Identity ident, Ice.Current current__);

            public int findGroups(string name, string uuid, out KeeICE.KPlib.KPGroup[] groups)
            {
                return findGroups(name, uuid, out groups, Ice.ObjectImpl.defaultCurrent);
            }

            public abstract int findGroups(string name, string uuid, out KeeICE.KPlib.KPGroup[] groups, Ice.Current current__);

            public KeeICE.KPlib.KPGroup getRoot()
            {
                return getRoot(Ice.ObjectImpl.defaultCurrent);
            }

            public abstract KeeICE.KPlib.KPGroup getRoot(Ice.Current current__);

            public KeeICE.KPlib.KPGroup getParent(string uuid)
            {
                return getParent(uuid, Ice.ObjectImpl.defaultCurrent);
            }

            public abstract KeeICE.KPlib.KPGroup getParent(string uuid, Ice.Current current__);

            public KeeICE.KPlib.KPGroup[] getChildGroups(string uuid)
            {
                return getChildGroups(uuid, Ice.ObjectImpl.defaultCurrent);
            }

            public abstract KeeICE.KPlib.KPGroup[] getChildGroups(string uuid, Ice.Current current__);

            public KeeICE.KPlib.KPEntry[] getChildEntries(string uuid)
            {
                return getChildEntries(uuid, Ice.ObjectImpl.defaultCurrent);
            }

            public abstract KeeICE.KPlib.KPEntry[] getChildEntries(string uuid, Ice.Current current__);

            public KeeICE.KPlib.KPGroup addGroup(string name, string parentUuid)
            {
                return addGroup(name, parentUuid, Ice.ObjectImpl.defaultCurrent);
            }

            public abstract KeeICE.KPlib.KPGroup addGroup(string name, string parentUuid, Ice.Current current__);

            public bool removeGroup(string uuid)
            {
                return removeGroup(uuid, Ice.ObjectImpl.defaultCurrent);
            }

            public abstract bool removeGroup(string uuid, Ice.Current current__);

            public bool removeEntry(string uuid)
            {
                return removeEntry(uuid, Ice.ObjectImpl.defaultCurrent);
            }

            public abstract bool removeEntry(string uuid, Ice.Current current__);

            public void LaunchGroupEditor(string uuid)
            {
                LaunchGroupEditor(uuid, Ice.ObjectImpl.defaultCurrent);
            }

            public abstract void LaunchGroupEditor(string uuid, Ice.Current current__);

            public void LaunchLoginEditor(string uuid)
            {
                LaunchLoginEditor(uuid, Ice.ObjectImpl.defaultCurrent);
            }

            public abstract void LaunchLoginEditor(string uuid, Ice.Current current__);

            public KeeICE.KPlib.KFConfiguration getCurrentKFConfig()
            {
                return getCurrentKFConfig(Ice.ObjectImpl.defaultCurrent);
            }

            public abstract KeeICE.KPlib.KFConfiguration getCurrentKFConfig(Ice.Current current__);

            public bool setCurrentKFConfig(KeeICE.KPlib.KFConfiguration config)
            {
                return setCurrentKFConfig(config, Ice.ObjectImpl.defaultCurrent);
            }

            public abstract bool setCurrentKFConfig(KeeICE.KPlib.KFConfiguration config, Ice.Current current__);

            public bool setCurrentDBRootGroup(string uuid)
            {
                return setCurrentDBRootGroup(uuid, Ice.ObjectImpl.defaultCurrent);
            }

            public abstract bool setCurrentDBRootGroup(string uuid, Ice.Current current__);

            #endregion

            #region Slice type-related members

            public static new string[] ids__ = 
            {
                "::Ice::Object",
                "::KeeICE::KPlib::KP"
            };

            public override bool ice_isA(string s)
            {
                return _System.Array.BinarySearch(ids__, s, IceUtilInternal.StringUtil.OrdinalStringComparer) >= 0;
            }

            public override bool ice_isA(string s, Ice.Current current__)
            {
                return _System.Array.BinarySearch(ids__, s, IceUtilInternal.StringUtil.OrdinalStringComparer) >= 0;
            }

            public override string[] ice_ids()
            {
                return ids__;
            }

            public override string[] ice_ids(Ice.Current current__)
            {
                return ids__;
            }

            public override string ice_id()
            {
                return ids__[1];
            }

            public override string ice_id(Ice.Current current__)
            {
                return ids__[1];
            }

            public static new string ice_staticId()
            {
                return ids__[1];
            }

            #endregion

            #region Operation dispatch

            public static Ice.DispatchStatus checkVersion___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                IceInternal.BasicStream is__ = inS__.istr();
                is__.startReadEncaps();
                float keeFoxVersion;
                keeFoxVersion = is__.readFloat();
                float keeICEVersion;
                keeICEVersion = is__.readFloat();
                is__.endReadEncaps();
                int result;
                IceInternal.BasicStream os__ = inS__.ostr();
                bool ret__ = obj__.checkVersion(keeFoxVersion, keeICEVersion, out result, current__);
                os__.writeInt(result);
                os__.writeBool(ret__);
                return Ice.DispatchStatus.DispatchOK;
            }

            public static Ice.DispatchStatus getDatabaseName___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                inS__.istr().skipEmptyEncaps();
                IceInternal.BasicStream os__ = inS__.ostr();
                string ret__ = obj__.getDatabaseName(current__);
                os__.writeString(ret__);
                return Ice.DispatchStatus.DispatchOK;
            }

            public static Ice.DispatchStatus getDatabaseFileName___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                inS__.istr().skipEmptyEncaps();
                IceInternal.BasicStream os__ = inS__.ostr();
                string ret__ = obj__.getDatabaseFileName(current__);
                os__.writeString(ret__);
                return Ice.DispatchStatus.DispatchOK;
            }

            public static Ice.DispatchStatus changeDatabase___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                IceInternal.BasicStream is__ = inS__.istr();
                is__.startReadEncaps();
                string fileName;
                fileName = is__.readString();
                bool closeCurrent;
                closeCurrent = is__.readBool();
                is__.endReadEncaps();
                obj__.changeDatabase(fileName, closeCurrent, current__);
                return Ice.DispatchStatus.DispatchOK;
            }

            public static Ice.DispatchStatus AddLogin___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                IceInternal.BasicStream is__ = inS__.istr();
                is__.startReadEncaps();
                KeeICE.KPlib.KPEntry login;
                login = null;
                if(login == null)
                {
                    login = new KeeICE.KPlib.KPEntry();
                }
                login.read__(is__);
                string parentUUID;
                parentUUID = is__.readString();
                is__.endReadEncaps();
                IceInternal.BasicStream os__ = inS__.ostr();
                try
                {
                    KeeICE.KPlib.KPEntry ret__ = obj__.AddLogin(login, parentUUID, current__);
                    if(ret__ == null)
                    {
                        KeeICE.KPlib.KPEntry tmp__ = new KeeICE.KPlib.KPEntry();
                        tmp__.write__(os__);
                    }
                    else
                    {
                        ret__.write__(os__);
                    }
                    return Ice.DispatchStatus.DispatchOK;
                }
                catch(KeeICE.KPlib.KeeICEException ex)
                {
                    os__.writeUserException(ex);
                    return Ice.DispatchStatus.DispatchUserException;
                }
            }

            public static Ice.DispatchStatus ModifyLogin___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                IceInternal.BasicStream is__ = inS__.istr();
                is__.startReadEncaps();
                KeeICE.KPlib.KPEntry oldLogin;
                oldLogin = null;
                if(oldLogin == null)
                {
                    oldLogin = new KeeICE.KPlib.KPEntry();
                }
                oldLogin.read__(is__);
                KeeICE.KPlib.KPEntry newLogin;
                newLogin = null;
                if(newLogin == null)
                {
                    newLogin = new KeeICE.KPlib.KPEntry();
                }
                newLogin.read__(is__);
                is__.endReadEncaps();
                IceInternal.BasicStream os__ = inS__.ostr();
                try
                {
                    obj__.ModifyLogin(oldLogin, newLogin, current__);
                    return Ice.DispatchStatus.DispatchOK;
                }
                catch(KeeICE.KPlib.KeeICEException ex)
                {
                    os__.writeUserException(ex);
                    return Ice.DispatchStatus.DispatchUserException;
                }
            }

            public static Ice.DispatchStatus getAllLogins___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                inS__.istr().skipEmptyEncaps();
                KeeICE.KPlib.KPEntry[] logins;
                IceInternal.BasicStream os__ = inS__.ostr();
                try
                {
                    int ret__ = obj__.getAllLogins(out logins, current__);
                    if(logins == null)
                    {
                        os__.writeSize(0);
                    }
                    else
                    {
                        os__.writeSize(logins.Length);
                        for(int ix__ = 0; ix__ < logins.Length; ++ix__)
                        {
                            (logins == null ? new KeeICE.KPlib.KPEntry() : logins[ix__]).write__(os__);
                        }
                    }
                    os__.writeInt(ret__);
                    return Ice.DispatchStatus.DispatchOK;
                }
                catch(KeeICE.KPlib.KeeICEException ex)
                {
                    os__.writeUserException(ex);
                    return Ice.DispatchStatus.DispatchUserException;
                }
            }

            public static Ice.DispatchStatus findLogins___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                IceInternal.BasicStream is__ = inS__.istr();
                is__.startReadEncaps();
                string hostname;
                hostname = is__.readString();
                string actionURL;
                actionURL = is__.readString();
                string httpRealm;
                httpRealm = is__.readString();
                KeeICE.KPlib.loginSearchType lst;
                lst = (KeeICE.KPlib.loginSearchType)is__.readByte(3);
                bool requireFullURLMatches;
                requireFullURLMatches = is__.readBool();
                string uniqueID;
                uniqueID = is__.readString();
                is__.endReadEncaps();
                KeeICE.KPlib.KPEntry[] logins;
                IceInternal.BasicStream os__ = inS__.ostr();
                try
                {
                    int ret__ = obj__.findLogins(hostname, actionURL, httpRealm, lst, requireFullURLMatches, uniqueID, out logins, current__);
                    if(logins == null)
                    {
                        os__.writeSize(0);
                    }
                    else
                    {
                        os__.writeSize(logins.Length);
                        for(int ix__ = 0; ix__ < logins.Length; ++ix__)
                        {
                            (logins == null ? new KeeICE.KPlib.KPEntry() : logins[ix__]).write__(os__);
                        }
                    }
                    os__.writeInt(ret__);
                    return Ice.DispatchStatus.DispatchOK;
                }
                catch(KeeICE.KPlib.KeeICEException ex)
                {
                    os__.writeUserException(ex);
                    return Ice.DispatchStatus.DispatchUserException;
                }
            }

            public static Ice.DispatchStatus countLogins___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                IceInternal.BasicStream is__ = inS__.istr();
                is__.startReadEncaps();
                string hostname;
                hostname = is__.readString();
                string actionURL;
                actionURL = is__.readString();
                string httpRealm;
                httpRealm = is__.readString();
                KeeICE.KPlib.loginSearchType lst;
                lst = (KeeICE.KPlib.loginSearchType)is__.readByte(3);
                bool requireFullURLMatches;
                requireFullURLMatches = is__.readBool();
                is__.endReadEncaps();
                IceInternal.BasicStream os__ = inS__.ostr();
                try
                {
                    int ret__ = obj__.countLogins(hostname, actionURL, httpRealm, lst, requireFullURLMatches, current__);
                    os__.writeInt(ret__);
                    return Ice.DispatchStatus.DispatchOK;
                }
                catch(KeeICE.KPlib.KeeICEException ex)
                {
                    os__.writeUserException(ex);
                    return Ice.DispatchStatus.DispatchUserException;
                }
            }

            public static Ice.DispatchStatus addClient___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                IceInternal.BasicStream is__ = inS__.istr();
                is__.startReadEncaps();
                Ice.Identity ident;
                ident = null;
                if(ident == null)
                {
                    ident = new Ice.Identity();
                }
                ident.read__(is__);
                is__.endReadEncaps();
                obj__.addClient(ident, current__);
                return Ice.DispatchStatus.DispatchOK;
            }

            public static Ice.DispatchStatus findGroups___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                IceInternal.BasicStream is__ = inS__.istr();
                is__.startReadEncaps();
                string name;
                name = is__.readString();
                string uuid;
                uuid = is__.readString();
                is__.endReadEncaps();
                KeeICE.KPlib.KPGroup[] groups;
                IceInternal.BasicStream os__ = inS__.ostr();
                int ret__ = obj__.findGroups(name, uuid, out groups, current__);
                if(groups == null)
                {
                    os__.writeSize(0);
                }
                else
                {
                    os__.writeSize(groups.Length);
                    for(int ix__ = 0; ix__ < groups.Length; ++ix__)
                    {
                        (groups == null ? new KeeICE.KPlib.KPGroup() : groups[ix__]).write__(os__);
                    }
                }
                os__.writeInt(ret__);
                return Ice.DispatchStatus.DispatchOK;
            }

            public static Ice.DispatchStatus getRoot___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                inS__.istr().skipEmptyEncaps();
                IceInternal.BasicStream os__ = inS__.ostr();
                KeeICE.KPlib.KPGroup ret__ = obj__.getRoot(current__);
                if(ret__ == null)
                {
                    KeeICE.KPlib.KPGroup tmp__ = new KeeICE.KPlib.KPGroup();
                    tmp__.write__(os__);
                }
                else
                {
                    ret__.write__(os__);
                }
                return Ice.DispatchStatus.DispatchOK;
            }

            public static Ice.DispatchStatus getParent___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                IceInternal.BasicStream is__ = inS__.istr();
                is__.startReadEncaps();
                string uuid;
                uuid = is__.readString();
                is__.endReadEncaps();
                IceInternal.BasicStream os__ = inS__.ostr();
                KeeICE.KPlib.KPGroup ret__ = obj__.getParent(uuid, current__);
                if(ret__ == null)
                {
                    KeeICE.KPlib.KPGroup tmp__ = new KeeICE.KPlib.KPGroup();
                    tmp__.write__(os__);
                }
                else
                {
                    ret__.write__(os__);
                }
                return Ice.DispatchStatus.DispatchOK;
            }

            public static Ice.DispatchStatus getChildGroups___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                IceInternal.BasicStream is__ = inS__.istr();
                is__.startReadEncaps();
                string uuid;
                uuid = is__.readString();
                is__.endReadEncaps();
                IceInternal.BasicStream os__ = inS__.ostr();
                KeeICE.KPlib.KPGroup[] ret__ = obj__.getChildGroups(uuid, current__);
                if(ret__ == null)
                {
                    os__.writeSize(0);
                }
                else
                {
                    os__.writeSize(ret__.Length);
                    for(int ix__ = 0; ix__ < ret__.Length; ++ix__)
                    {
                        (ret__ == null ? new KeeICE.KPlib.KPGroup() : ret__[ix__]).write__(os__);
                    }
                }
                return Ice.DispatchStatus.DispatchOK;
            }

            public static Ice.DispatchStatus getChildEntries___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                IceInternal.BasicStream is__ = inS__.istr();
                is__.startReadEncaps();
                string uuid;
                uuid = is__.readString();
                is__.endReadEncaps();
                IceInternal.BasicStream os__ = inS__.ostr();
                KeeICE.KPlib.KPEntry[] ret__ = obj__.getChildEntries(uuid, current__);
                if(ret__ == null)
                {
                    os__.writeSize(0);
                }
                else
                {
                    os__.writeSize(ret__.Length);
                    for(int ix__ = 0; ix__ < ret__.Length; ++ix__)
                    {
                        (ret__ == null ? new KeeICE.KPlib.KPEntry() : ret__[ix__]).write__(os__);
                    }
                }
                return Ice.DispatchStatus.DispatchOK;
            }

            public static Ice.DispatchStatus addGroup___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                IceInternal.BasicStream is__ = inS__.istr();
                is__.startReadEncaps();
                string name;
                name = is__.readString();
                string parentUuid;
                parentUuid = is__.readString();
                is__.endReadEncaps();
                IceInternal.BasicStream os__ = inS__.ostr();
                KeeICE.KPlib.KPGroup ret__ = obj__.addGroup(name, parentUuid, current__);
                if(ret__ == null)
                {
                    KeeICE.KPlib.KPGroup tmp__ = new KeeICE.KPlib.KPGroup();
                    tmp__.write__(os__);
                }
                else
                {
                    ret__.write__(os__);
                }
                return Ice.DispatchStatus.DispatchOK;
            }

            public static Ice.DispatchStatus removeGroup___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                IceInternal.BasicStream is__ = inS__.istr();
                is__.startReadEncaps();
                string uuid;
                uuid = is__.readString();
                is__.endReadEncaps();
                IceInternal.BasicStream os__ = inS__.ostr();
                bool ret__ = obj__.removeGroup(uuid, current__);
                os__.writeBool(ret__);
                return Ice.DispatchStatus.DispatchOK;
            }

            public static Ice.DispatchStatus removeEntry___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                IceInternal.BasicStream is__ = inS__.istr();
                is__.startReadEncaps();
                string uuid;
                uuid = is__.readString();
                is__.endReadEncaps();
                IceInternal.BasicStream os__ = inS__.ostr();
                bool ret__ = obj__.removeEntry(uuid, current__);
                os__.writeBool(ret__);
                return Ice.DispatchStatus.DispatchOK;
            }

            public static Ice.DispatchStatus LaunchGroupEditor___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                IceInternal.BasicStream is__ = inS__.istr();
                is__.startReadEncaps();
                string uuid;
                uuid = is__.readString();
                is__.endReadEncaps();
                obj__.LaunchGroupEditor(uuid, current__);
                return Ice.DispatchStatus.DispatchOK;
            }

            public static Ice.DispatchStatus LaunchLoginEditor___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                IceInternal.BasicStream is__ = inS__.istr();
                is__.startReadEncaps();
                string uuid;
                uuid = is__.readString();
                is__.endReadEncaps();
                obj__.LaunchLoginEditor(uuid, current__);
                return Ice.DispatchStatus.DispatchOK;
            }

            public static Ice.DispatchStatus getCurrentKFConfig___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                inS__.istr().skipEmptyEncaps();
                IceInternal.BasicStream os__ = inS__.ostr();
                KeeICE.KPlib.KFConfiguration ret__ = obj__.getCurrentKFConfig(current__);
                if(ret__ == null)
                {
                    KeeICE.KPlib.KFConfiguration tmp__ = new KeeICE.KPlib.KFConfiguration();
                    tmp__.write__(os__);
                }
                else
                {
                    ret__.write__(os__);
                }
                return Ice.DispatchStatus.DispatchOK;
            }

            public static Ice.DispatchStatus setCurrentKFConfig___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                IceInternal.BasicStream is__ = inS__.istr();
                is__.startReadEncaps();
                KeeICE.KPlib.KFConfiguration config;
                config = null;
                if(config == null)
                {
                    config = new KeeICE.KPlib.KFConfiguration();
                }
                config.read__(is__);
                is__.endReadEncaps();
                IceInternal.BasicStream os__ = inS__.ostr();
                bool ret__ = obj__.setCurrentKFConfig(config, current__);
                os__.writeBool(ret__);
                return Ice.DispatchStatus.DispatchOK;
            }

            public static Ice.DispatchStatus setCurrentDBRootGroup___(KP obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                IceInternal.BasicStream is__ = inS__.istr();
                is__.startReadEncaps();
                string uuid;
                uuid = is__.readString();
                is__.endReadEncaps();
                IceInternal.BasicStream os__ = inS__.ostr();
                bool ret__ = obj__.setCurrentDBRootGroup(uuid, current__);
                os__.writeBool(ret__);
                return Ice.DispatchStatus.DispatchOK;
            }

            private static string[] all__ =
            {
                "AddLogin",
                "LaunchGroupEditor",
                "LaunchLoginEditor",
                "ModifyLogin",
                "addClient",
                "addGroup",
                "changeDatabase",
                "checkVersion",
                "countLogins",
                "findGroups",
                "findLogins",
                "getAllLogins",
                "getChildEntries",
                "getChildGroups",
                "getCurrentKFConfig",
                "getDatabaseFileName",
                "getDatabaseName",
                "getParent",
                "getRoot",
                "ice_id",
                "ice_ids",
                "ice_isA",
                "ice_ping",
                "removeEntry",
                "removeGroup",
                "setCurrentDBRootGroup",
                "setCurrentKFConfig"
            };

            public override Ice.DispatchStatus dispatch__(IceInternal.Incoming inS__, Ice.Current current__)
            {
                int pos = _System.Array.BinarySearch(all__, current__.operation, IceUtilInternal.StringUtil.OrdinalStringComparer);
                if(pos < 0)
                {
                    throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                }

                switch(pos)
                {
                    case 0:
                    {
                        return AddLogin___(this, inS__, current__);
                    }
                    case 1:
                    {
                        return LaunchGroupEditor___(this, inS__, current__);
                    }
                    case 2:
                    {
                        return LaunchLoginEditor___(this, inS__, current__);
                    }
                    case 3:
                    {
                        return ModifyLogin___(this, inS__, current__);
                    }
                    case 4:
                    {
                        return addClient___(this, inS__, current__);
                    }
                    case 5:
                    {
                        return addGroup___(this, inS__, current__);
                    }
                    case 6:
                    {
                        return changeDatabase___(this, inS__, current__);
                    }
                    case 7:
                    {
                        return checkVersion___(this, inS__, current__);
                    }
                    case 8:
                    {
                        return countLogins___(this, inS__, current__);
                    }
                    case 9:
                    {
                        return findGroups___(this, inS__, current__);
                    }
                    case 10:
                    {
                        return findLogins___(this, inS__, current__);
                    }
                    case 11:
                    {
                        return getAllLogins___(this, inS__, current__);
                    }
                    case 12:
                    {
                        return getChildEntries___(this, inS__, current__);
                    }
                    case 13:
                    {
                        return getChildGroups___(this, inS__, current__);
                    }
                    case 14:
                    {
                        return getCurrentKFConfig___(this, inS__, current__);
                    }
                    case 15:
                    {
                        return getDatabaseFileName___(this, inS__, current__);
                    }
                    case 16:
                    {
                        return getDatabaseName___(this, inS__, current__);
                    }
                    case 17:
                    {
                        return getParent___(this, inS__, current__);
                    }
                    case 18:
                    {
                        return getRoot___(this, inS__, current__);
                    }
                    case 19:
                    {
                        return ice_id___(this, inS__, current__);
                    }
                    case 20:
                    {
                        return ice_ids___(this, inS__, current__);
                    }
                    case 21:
                    {
                        return ice_isA___(this, inS__, current__);
                    }
                    case 22:
                    {
                        return ice_ping___(this, inS__, current__);
                    }
                    case 23:
                    {
                        return removeEntry___(this, inS__, current__);
                    }
                    case 24:
                    {
                        return removeGroup___(this, inS__, current__);
                    }
                    case 25:
                    {
                        return setCurrentDBRootGroup___(this, inS__, current__);
                    }
                    case 26:
                    {
                        return setCurrentKFConfig___(this, inS__, current__);
                    }
                }

                _System.Diagnostics.Debug.Assert(false);
                throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
            }

            #endregion

            #region Marshaling support

            public override void write__(IceInternal.BasicStream os__)
            {
                os__.writeTypeId(ice_staticId());
                os__.startWriteSlice();
                os__.endWriteSlice();
                base.write__(os__);
            }

            public override void read__(IceInternal.BasicStream is__, bool rid__)
            {
                if(rid__)
                {
                    /* string myId = */ is__.readTypeId();
                }
                is__.startReadSlice();
                is__.endReadSlice();
                base.read__(is__, true);
            }

            public override void write__(Ice.OutputStream outS__)
            {
                Ice.MarshalException ex = new Ice.MarshalException();
                ex.reason = "type KeeICE::KPlib::KP was not generated with stream support";
                throw ex;
            }

            public override void read__(Ice.InputStream inS__, bool rid__)
            {
                Ice.MarshalException ex = new Ice.MarshalException();
                ex.reason = "type KeeICE::KPlib::KP was not generated with stream support";
                throw ex;
            }

            #endregion
        }

        public abstract class CallbackReceiverDisp_ : Ice.ObjectImpl, CallbackReceiver
        {
            #region Slice operations

            public void callback(int num)
            {
                callback(num, Ice.ObjectImpl.defaultCurrent);
            }

            public abstract void callback(int num, Ice.Current current__);

            #endregion

            #region Slice type-related members

            public static new string[] ids__ = 
            {
                "::Ice::Object",
                "::KeeICE::KPlib::CallbackReceiver"
            };

            public override bool ice_isA(string s)
            {
                return _System.Array.BinarySearch(ids__, s, IceUtilInternal.StringUtil.OrdinalStringComparer) >= 0;
            }

            public override bool ice_isA(string s, Ice.Current current__)
            {
                return _System.Array.BinarySearch(ids__, s, IceUtilInternal.StringUtil.OrdinalStringComparer) >= 0;
            }

            public override string[] ice_ids()
            {
                return ids__;
            }

            public override string[] ice_ids(Ice.Current current__)
            {
                return ids__;
            }

            public override string ice_id()
            {
                return ids__[1];
            }

            public override string ice_id(Ice.Current current__)
            {
                return ids__[1];
            }

            public static new string ice_staticId()
            {
                return ids__[1];
            }

            #endregion

            #region Operation dispatch

            public static Ice.DispatchStatus callback___(CallbackReceiver obj__, IceInternal.Incoming inS__, Ice.Current current__)
            {
                checkMode__(Ice.OperationMode.Normal, current__.mode);
                IceInternal.BasicStream is__ = inS__.istr();
                is__.startReadEncaps();
                int num;
                num = is__.readInt();
                is__.endReadEncaps();
                obj__.callback(num, current__);
                return Ice.DispatchStatus.DispatchOK;
            }

            private static string[] all__ =
            {
                "callback",
                "ice_id",
                "ice_ids",
                "ice_isA",
                "ice_ping"
            };

            public override Ice.DispatchStatus dispatch__(IceInternal.Incoming inS__, Ice.Current current__)
            {
                int pos = _System.Array.BinarySearch(all__, current__.operation, IceUtilInternal.StringUtil.OrdinalStringComparer);
                if(pos < 0)
                {
                    throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
                }

                switch(pos)
                {
                    case 0:
                    {
                        return callback___(this, inS__, current__);
                    }
                    case 1:
                    {
                        return ice_id___(this, inS__, current__);
                    }
                    case 2:
                    {
                        return ice_ids___(this, inS__, current__);
                    }
                    case 3:
                    {
                        return ice_isA___(this, inS__, current__);
                    }
                    case 4:
                    {
                        return ice_ping___(this, inS__, current__);
                    }
                }

                _System.Diagnostics.Debug.Assert(false);
                throw new Ice.OperationNotExistException(current__.id, current__.facet, current__.operation);
            }

            #endregion

            #region Marshaling support

            public override void write__(IceInternal.BasicStream os__)
            {
                os__.writeTypeId(ice_staticId());
                os__.startWriteSlice();
                os__.endWriteSlice();
                base.write__(os__);
            }

            public override void read__(IceInternal.BasicStream is__, bool rid__)
            {
                if(rid__)
                {
                    /* string myId = */ is__.readTypeId();
                }
                is__.startReadSlice();
                is__.endReadSlice();
                base.read__(is__, true);
            }

            public override void write__(Ice.OutputStream outS__)
            {
                Ice.MarshalException ex = new Ice.MarshalException();
                ex.reason = "type KeeICE::KPlib::CallbackReceiver was not generated with stream support";
                throw ex;
            }

            public override void read__(Ice.InputStream inS__, bool rid__)
            {
                Ice.MarshalException ex = new Ice.MarshalException();
                ex.reason = "type KeeICE::KPlib::CallbackReceiver was not generated with stream support";
                throw ex;
            }

            #endregion
        }
    }
}
