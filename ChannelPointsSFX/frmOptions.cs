using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChannelPointsSFX
{
    public delegate void ReenableSettingsButton();
    public partial class frmOptions : Form
    {
        public event ReenableSettingsButton enSetBut = null;

        public frmOptions()
        {
            InitializeComponent();
        }

        private void frmOptions_Load(object sender, EventArgs e)
        {
            cbTrayMini.Checked = Properties.Settings.Default.minimizeToTray;
            serverBox.Checked = Properties.Settings.Default.serverStatus;
            alertTimer.Value = Properties.Settings.Default.rewardTimer;
            httpPort.Value = Properties.Settings.Default.httpPort;
            string ver = typeof(frmOptions).Assembly.GetName().Version.ToString();
            lblVersioning.Text = "Channel Points SFX v" + (ver.Substring(0,ver.Length-2)) + " Options";
        }

        private void btnResetID_Click(object sender, EventArgs e)
        {
            string userInput = Prompt.ShowDialog("Please enter your Twitch ChannelID (THIS IS NOT YOUR USERNAME)\r\nGo to https://dude22072.com/twitchchannelid.php if you don't know what this is.", "Enter Channel ID");
            if (userInput == "") return;
            Properties.Settings.Default.savedChannelID = userInput;
            Properties.Settings.Default.Save();
            MessageBox.Show("Please restart the program to connect to the new ChannelID.", "Dude22072's Channel Points SFX Program", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (enSetBut != null)
            {
                enSetBut();
            }
            this.Close();
            this.Dispose();
        }

        private void cbTrayMini_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.minimizeToTray = cbTrayMini.Checked;
            Properties.Settings.Default.Save();
        }

        private void alertTimer_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.rewardTimer = ((System.Windows.Forms.NumericUpDown)(sender)).Value;
            Properties.Settings.Default.Save();
        }
        private void httpPort_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.httpPort = ((System.Windows.Forms.NumericUpDown)(sender)).Value;
            Properties.Settings.Default.Save();
        }
    }
}
