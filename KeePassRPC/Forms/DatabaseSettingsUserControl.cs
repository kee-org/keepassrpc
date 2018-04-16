using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KeePassLib;

namespace KeePassRPC.Forms
{
    public partial class DatabaseSettingsUserControl : UserControl
    {
        private PwDatabase database;
        private DatabaseConfig config;

        public DatabaseSettingsUserControl(PwDatabase db)
        {
            database = db;
            config = db.GetKPRPCConfig();
            InitializeComponent();
            PresentDefaultMatchAccuracy();
        }
        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (ParentForm != null)
            {
                ParentForm.FormClosing +=
                  delegate (object aSender, FormClosingEventArgs aEventArgs)
                  {
                      if (ParentForm.DialogResult == DialogResult.OK)
                      {
                          config.DefaultMatchAccuracy = DetermineDefaultMatchAccuracy();
                          database.SetKPRPCConfig(config);
                      }
                  };
            }
        }

        private void PresentDefaultMatchAccuracy()
        {
            switch (config.DefaultMatchAccuracy)
            {
                case MatchAccuracyMethod.Exact: radioButton3.Checked = true; break;
                case MatchAccuracyMethod.Hostname: radioButton2.Checked = true; break;
                case MatchAccuracyMethod.Domain: radioButton1.Checked = true; break;
            }
        }

        private MatchAccuracyMethod DetermineDefaultMatchAccuracy()
        {
            if (radioButton3.Checked) return MatchAccuracyMethod.Exact;
            else if (radioButton2.Checked) return MatchAccuracyMethod.Hostname;
            else return MatchAccuracyMethod.Domain;
        }
    }
}
