using System;
using System.Drawing;
using System.IO;
using System.Linq;
using KeePass.Plugins;
using KeePassLib;
using KeePassLib.Utility;
using KeePassRPC.JsonRpc;
using KeePassRPC.Models.DataExchange;
using KeePassRPC.Models.DataExchange.V2;
using Icon = KeePassRPC.Models.DataExchange.V2.Icon;

namespace KeePassRPC
{
    public class IconConverter
    {
        IPluginHost host;
        KeePassRPCExt KeePassRPCPlugin;
        string[] _standardIconsBase64;

        public IconConverter(IPluginHost host, KeePassRPCExt plugin, string[] _standardIconsBase64)
        {
            this.host = host;
            this.KeePassRPCPlugin = plugin;
            this._standardIconsBase64 = _standardIconsBase64;
        }


        private static object iconSavingLock = new object();

        public IconData[] Base64StandardIconsUnknownToClient(ClientMetadata clientMetadata)
        {
            var highestIndex = HighestKnownStandardIconIndex(clientMetadata);

            return _standardIconsBase64.Skip(highestIndex).Select((data, index) => new IconData()
            {
                Id = index.ToString(),
                Icon = data
            }).ToArray();
        }

        private int HighestKnownStandardIconIndex(ClientMetadata clientMetadata)
        {
            if (clientMetadata != null && clientMetadata.Features != null)
            {
                if (clientMetadata.Features.Contains("KPRPC_FEATURE_ICON_SET_1"))
                {
                    return 68;
                }
            }

            return 0;
        }

        /// <summary>
        /// extract the current icon information for this entry
        /// </summary>
        /// <param name="customIconUUID"></param>
        /// <param name="iconId"></param>
        /// <returns></returns>
        public Icon iconToDto(ClientMetadata clientMetadata, PwUuid customIconUUID, PwIcon iconId)
        {
            if ((clientMetadata != null && clientMetadata.Features != null &&
                 clientMetadata.Features.Contains("KPRPC_FEATURE_ICON_REFERENCES")))
            {
                if (customIconUUID != PwUuid.Zero)
                {
                    return new Icon()
                    {
                        RefId = customIconUUID.ToHexString()
                    };
                }
                return new Icon()
                {
                    Index = ((int)iconId).ToString()
                };
            }
            return new Icon()
            {
                Base64 = iconToBase64(customIconUUID, iconId)
            };
        }


        /// <summary>
        /// extract the current icon information for this entry
        /// </summary>
        /// <param name="customIconUUID"></param>
        /// <param name="iconId"></param>
        /// <returns></returns>
        public string iconToBase64(PwUuid customIconUUID, PwIcon iconId)
        {
            Image icon = null;
            PwUuid uuid = null;

            string imageData = "";
            if (customIconUUID != PwUuid.Zero)
            {
                string cachedBase64 = IconCache<PwUuid>.GetIconEncoding(customIconUUID);
                if (string.IsNullOrEmpty(cachedBase64))
                {
                    object[] delParams = { customIconUUID };
                    object invokeResult = host.MainWindow.Invoke(
                        new KeePassRPCExt.GetCustomIconDelegate(
                            KeePassRPCPlugin.GetCustomIcon), delParams);
                    if (invokeResult != null)
                    {
                        icon = (Image)invokeResult;
                    }

                    if (icon != null)
                    {
                        uuid = customIconUUID;
                    }
                }
                else
                {
                    return cachedBase64;
                }
            }

            // this happens if we didn't want to or couldn't find a custom icon
            if (icon == null)
            {
                int iconIdInt = (int)iconId;
                uuid = new PwUuid(new byte[]
                {
                    (byte)(iconIdInt & 0xFF), (byte)(iconIdInt & 0xFF),
                    (byte)(iconIdInt & 0xFF), (byte)(iconIdInt & 0xFF),
                    (byte)(iconIdInt >> 8 & 0xFF), (byte)(iconIdInt >> 8 & 0xFF),
                    (byte)(iconIdInt >> 8 & 0xFF), (byte)(iconIdInt >> 8 & 0xFF),
                    (byte)(iconIdInt >> 16 & 0xFF), (byte)(iconIdInt >> 16 & 0xFF),
                    (byte)(iconIdInt >> 16 & 0xFF), (byte)(iconIdInt >> 16 & 0xFF),
                    (byte)(iconIdInt >> 24 & 0xFF), (byte)(iconIdInt >> 24 & 0xFF),
                    (byte)(iconIdInt >> 24 & 0xFF), (byte)(iconIdInt >> 24 & 0xFF)
                });

                string cachedBase64 = IconCache<PwUuid>.GetIconEncoding(uuid);
                if (string.IsNullOrEmpty(cachedBase64))
                {
                    object[] delParams = { (int)iconId };
                    object invokeResult = host.MainWindow.Invoke(
                        new KeePassRPCExt.GetIconDelegate(
                            KeePassRPCPlugin.GetIcon), delParams);
                    if (invokeResult != null)
                    {
                        icon = (Image)invokeResult;
                    }
                }
                else
                {
                    return cachedBase64;
                }
            }


            if (icon != null)
            {
                // we found an icon but it wasn't in the cache so lets
                // calculate its base64 encoding and then add it to the cache
                using (MemoryStream ms = new MemoryStream())
                {
                    lock (iconSavingLock)
                    {
                        icon.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    }

                    imageData = Convert.ToBase64String(ms.ToArray());
                }

                IconCache<PwUuid>.AddIcon(uuid, imageData);
            }

            return imageData;
        }

        /// <summary>
        /// converts a DTO Icon to the relevant icon for this entry
        /// </summary>
        /// <param name="icon">DTO representation of the icon</param>
        /// <param name="customIconUUID">UUID of the generated custom icon; may be Zero</param>
        /// <param name="iconId">PwIcon of the matched standard icon; ignore if customIconUUID != Zero</param>
        /// <returns>true if the supplied Icon was converted into a customIcon 
        /// or matched with a standard icon.</returns>
        public bool dtoToIcon(ClientMetadata clientMetadata, Icon icon, ref PwUuid customIconUUID, ref PwIcon iconId)
        {
            iconId = PwIcon.Key;
            customIconUUID = PwUuid.Zero;
            if (!String.IsNullOrEmpty(icon.Index))
            {
                try
                {
                    iconId = (PwIcon)int.Parse(icon.Index);
                    return true;
                }
                catch
                {
                    // ignore
                }
            }
            if (!String.IsNullOrEmpty(icon.RefId))
            {
                try
                {
                    customIconUUID = new PwUuid(MemUtil.HexStringToByteArray(icon.RefId));
                    return true;
                }
                catch
                {
                    // ignore
                }
            }
            return base64ToIcon(clientMetadata, icon.Base64, ref customIconUUID, ref iconId);
        }

        /// <summary>
        /// converts a string to the relevant icon for this entry
        /// </summary>
        /// <param name="imageData">base64 representation of the image</param>
        /// <param name="customIconUUID">UUID of the generated custom icon; may be Zero</param>
        /// <param name="iconId">PwIcon of the matched standard icon; ignore if customIconUUID != Zero</param>
        /// <returns>true if the supplied imageData was converted into a customIcon 
        /// or matched with a standard icon.</returns>
        public bool base64ToIcon(ClientMetadata clientMetadata, string imageData, ref PwUuid customIconUUID,
            ref PwIcon iconId)
        {
            iconId = PwIcon.Key;
            customIconUUID = PwUuid.Zero;

            for (int i = 0; i < _standardIconsBase64.Length; i++)
            {
                string item = _standardIconsBase64[i];
                if (item == imageData)
                {
                    iconId = (PwIcon)i;
                    return true;
                }
            }

            // Although other clients could connect and suffer performance issues due to the larger image
            // sizes being potentially sent more than once, it's not a critical backwards incompatible
            // change so we use the current client's capabilities as a rough indicator of whether we can
            // safely store and deliver higher quality icons to all clients in future, at least for the
            // object we are currently handling.
            var maxImageSize = (clientMetadata != null && clientMetadata.Features != null &&
                                clientMetadata.Features.Contains("KPRPC_FEATURE_ICON_REFERENCES"))
                ? 64
                : 16;

            try
            {
                //MemoryStream id = new MemoryStream();
                //icon.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                using (Image img = GfxUtil.LoadImage(Convert.FromBase64String(imageData)))
                    // Should already be a Bitmap but we clone it anyway
                using (Image imgNew = new Bitmap(img,
                           new Size(Math.Min(maxImageSize, img.Width), Math.Min(maxImageSize, img.Height))))
                using (MemoryStream ms = new MemoryStream())
                {
                    // No need to lock here because we've created a new Bitmap 
                    // (KeePass.UI.UIUtil.LoadImage has no caching or fancy stuff)
                    imgNew.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                    byte[] msByteArray = ms.ToArray();

                    foreach (PwCustomIcon item in host.Database.CustomIcons)
                    {
                        // re-use existing custom icon if it's already in the database
                        // (This will probably fail if database is used on 
                        // both 32 bit and 64 bit machines - not sure why...)
                        if (KeePassLib.Utility.MemUtil.ArraysEqual(msByteArray, item.ImageDataPng))
                        {
                            customIconUUID = item.Uuid;
                            host.Database.UINeedsIconUpdate = true;
                            return true;
                        }
                    }

                    PwCustomIcon pwci = new PwCustomIcon(new PwUuid(true), msByteArray);
                    host.Database.CustomIcons.Add(pwci);

                    customIconUUID = pwci.Uuid;
                    host.Database.UINeedsIconUpdate = true;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}