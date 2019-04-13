using System;
using System.Drawing;
using System.IO;
using KeePass.Plugins;
using KeePassLib;

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

        private string dbIconToBase64(PwDatabase db)
        {
            string cachedBase64 = DataExchangeModel.IconCache<string>.GetIconEncoding(db.IOConnectionInfo.Path);
            if (string.IsNullOrEmpty(cachedBase64))
            {
                // Don't think this should ever happen but we'll return a null icon if we have to
                return "";
            }
            else
            {
                return cachedBase64;
            }
        }

        private static object iconSavingLock = new object();

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
                string cachedBase64 = DataExchangeModel.IconCache<PwUuid>.GetIconEncoding(customIconUUID);
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
                uuid = new PwUuid(new byte[]{
                    (byte)(iconIdInt & 0xFF), (byte)(iconIdInt & 0xFF),
                    (byte)(iconIdInt & 0xFF), (byte)(iconIdInt & 0xFF),
                    (byte)(iconIdInt >> 8 & 0xFF), (byte)(iconIdInt >> 8 & 0xFF),
                    (byte)(iconIdInt >> 8 & 0xFF), (byte)(iconIdInt >> 8 & 0xFF),
                    (byte)(iconIdInt >> 16 & 0xFF), (byte)(iconIdInt >> 16 & 0xFF),
                    (byte)(iconIdInt >> 16 & 0xFF), (byte)(iconIdInt >> 16 & 0xFF),
                    (byte)(iconIdInt >> 24 & 0xFF), (byte)(iconIdInt >> 24 & 0xFF),
                    (byte)(iconIdInt >> 24 & 0xFF), (byte)(iconIdInt >> 24 & 0xFF)
                });

                string cachedBase64 = DataExchangeModel.IconCache<PwUuid>.GetIconEncoding(uuid);
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
                DataExchangeModel.IconCache<PwUuid>.AddIcon(uuid, imageData);
            }

            return imageData;
        }

        /// <summary>
        /// converts a string to the relevant icon for this entry
        /// </summary>
        /// <param name="imageData">base64 representation of the image</param>
        /// <param name="customIconUUID">UUID of the generated custom icon; may be Zero</param>
        /// <param name="iconId">PwIcon of the matched standard icon; ignore if customIconUUID != Zero</param>
        /// <returns>true if the supplied imageData was converted into a customIcon 
        /// or matched with a standard icon.</returns>
        public bool base64ToIcon(string imageData, ref PwUuid customIconUUID, ref PwIcon iconId)
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

            try
            {
                //MemoryStream id = new MemoryStream();
                //icon.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

                using (Image img = KeePass.UI.UIUtil.LoadImage(Convert.FromBase64String(imageData)))
                using (Image imgNew = new Bitmap(img, new Size(16, 16)))
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
