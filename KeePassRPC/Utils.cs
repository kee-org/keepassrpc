using System;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace KeePassRPC
{
    static class Utils
    {

        internal static byte[] Hash(string data)
        {
            byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(data);
            return Hash(dataBytes);
        }

        internal static byte[] Hash(byte[] data)
        {
            using (SHA256 shaM = new SHA256Managed())
            {
                return shaM.ComputeHash(data);
            }
        }

        internal static byte[] GetRandomBytes(int quantity)
        {
            byte[] randomNumber = new byte[quantity];
            //TODO2: We don't need many random numbers very often but maybe could improve
            // performance a tad by re-using the same generator object
            using (RNGCryptoServiceProvider Gen = new RNGCryptoServiceProvider())
            {
                Gen.GetNonZeroBytes(randomNumber);
                return randomNumber;
            }
        }

        internal const string Base32Characters = "ybndrfg8ejkmcpqxot1vwisza345h769";

        internal static string EncodeToBase32(uint input)
        {
            string result = "";

            if (input == 0)
                result += Base32Characters[0];
            else
                while (input > 0)
                {
                    result = Base32Characters[(int)(input % Base32Characters.Length)] + result;
                    input /= (uint)Base32Characters.Length;
                }

            return result;
        }

        internal static string GetTypeablePassword(byte[] password)
        {
            return EncodeToBase32(BitConverter.ToUInt32(password, 0));
        }

        private static Form GetTopForm()
        {
            FormCollection fc = Application.OpenForms;
            if (fc == null || fc.Count == 0) return null;
            return fc[fc.Count - 1];
        }

        internal static void ShowMonoSafeMessageBox(string description, string title = "KeePassRPC",
            MessageBoxButtons mb =  MessageBoxButtons.OK, MessageBoxIcon mi = MessageBoxIcon.Information, MessageBoxDefaultButton mdb = MessageBoxDefaultButton.Button1)
        {
            IWin32Window win = null;
            try
            {
                Form f = GetTopForm();
                if (f != null && f.InvokeRequired)
                {
                    f.Invoke(new Action(() => MessageBox.Show(description, title, mb, mi, mdb)));
                    return;
                } else
                {
                    win = f;
                }
            } catch (Exception)
            {
            }

            if (win == null)
            {
                MessageBox.Show(description, title, mb, mi, mdb);
                return;
            }

            try
            {
                MessageBox.Show(win, description, title, mb, mi, mdb);
                return;
            }
            catch (Exception)
            {
            }
            MessageBox.Show(description, title, mb, mi, mdb);
        }

    }
}
