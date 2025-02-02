﻿#pragma warning disable IDE1006 // Naming Styles
using System;
using System.Windows.Forms;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;
using System.Collections.Generic;
using System.Windows.Media;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Text;

#if DEBUG
using System.Diagnostics;
#endif

namespace ChannelPointsSFX
{
    public partial class frmMain : Form
    {
        private static TwitchPubSub client;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "Needs to be modified at runtime.")]
        private static Dictionary<string, string> bindings = new Dictionary<string, string>();
        private static MediaPlayer thePlayer;
        private static List<MediaPlayer> allPlayers = new List<MediaPlayer>();
        private int volumeLevel, savedVolumeLevel; 
        private int boxSelection = 0;
        private static HttpListener listener;
        private object mutex = new object();
        private static bool killServer = false;

        public frmMain()
        {
            InitializeComponent();
        }

        private void HandleConnections(object state)
        {
            btnServer.ForeColor = System.Drawing.Color.Green;
            try
            {
                listener = new HttpListener();
                listener.Prefixes.Add("http://localhost:" + Decimal.ToInt32(Properties.Settings.Default.httpPort) + "/");
                listener.Start();

                Task listenTask = HandleIncomingConnections();
                listenTask.GetAwaiter().GetResult();

                listener.Close();
                killServer = false;
                btnServer.ForeColor = System.Drawing.Color.Red;
            }
            finally
            {
                lock (mutex)
                {
                    Properties.Settings.Default.serverStatus = false;
                }
            }
        }

        private static async Task HandleIncomingConnections()
        {
            string alertSchema = File.ReadAllText("alert.html");
            while (!killServer)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();
                HttpListenerRequest req = ctx.Request;

                string headStr = "", bodyStr = "";
                if (req.Url.AbsolutePath == "/")
                {
                    if (File.Exists("alert.txt"))
                    {
                        String[] alertData = File.ReadAllLines("alert.txt");
                        if (alertData.Length == 3)
                        {
                            bodyStr = alertSchema.Replace("|DISPLAY_NAME|", alertData[0]).Replace("|TITLE|", alertData[1]).Replace("|COST|", alertData[2]);
                        }
                    }
                    else
                    {
                        headStr = "<meta http-equiv=\"refresh\" content=\"1\">";
                    }
                }

                byte[] data = Encoding.UTF8.GetBytes("");
                if (headStr.Length > 0)
                {
                    data = Encoding.UTF8.GetBytes("<!DOCTYPE html>" +
                    "<html lang=\"en\">" +
                        "<head>" +
                        "<meta charset=\"utf-8\">" +
                        headStr +
                        "</head>" +
                    "</html>");
                }
                else if (bodyStr.Length > 0)
                {
                    data = Encoding.UTF8.GetBytes(bodyStr);
                }

                HttpListenerResponse resp = ctx.Response;
                resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            Properties.Settings.Default.serverStatus = false;
            volumeLevel = savedVolumeLevel = Convert.ToInt32(Properties.Settings.Default.savedVolumeLevel);
            
            client = new TwitchPubSub();

            client.OnPubSubServiceConnected += onPubSubServiceConnected;
            client.OnListenResponse += onListenResponse;
            client.OnRewardRedeemed += OnRewardRedeemed;

            if (Properties.Settings.Default.savedChannelID == "")
            {
                Properties.Settings.Default.savedChannelID = Prompt.ShowDialog("Please enter your Twitch ChannelID. (THIS IS NOT YOUR USERNAME)\r\nGo to https://dude22072.com/twitchchannelid.php if you don't know what this is.", "Enter Channel ID");
                if (Properties.Settings.Default.savedChannelID == "") { this.Close(); return; }
                Properties.Settings.Default.Save();
            }

            client.ListenToRewards(Properties.Settings.Default.savedChannelID);
            client.Connect();

            if (File.Exists("alert.txt"))
            {
                File.Delete("alert.txt");
            }

            if (File.Exists("settings.txt"))
            {
                String[] loadSettings = File.ReadAllLines("settings.txt");
                foreach(string line in loadSettings)
                {
                    string[] split = line.Split('|');
                    bindings.Add(split[0], split[1]);
                }
            }

            reloadListItems();

            txtVolume.Text = volumeLevel.ToString();
            trkVolume.Value = volumeLevel;

            trayIcon.BalloonTipText = "Running in tray. Double click tray icon to maximize.";
            trayIcon.BalloonTipTitle = "Dude22072\'s Channel Points SFX Program";
            trayIcon.BalloonTipIcon = ToolTipIcon.Info;
            trayIcon.Icon = this.Icon;

            using (System.Net.WebClient client = new System.Net.WebClient())
            {
#if DEBUG
                string urlPrefix = "https://dude22072.com/testing/";
#else
                string urlPrefix = "https://dude22072.com/";
#endif
                if (typeof(frmMain).Assembly.GetName().Version.CompareTo(new Version(client.DownloadString(urlPrefix + "ChannelPointsSFX-version.txt"))) < 0)
                {
                    lblNOTICE.Visible = true;
                    toolTip1.SetToolTip(lblNOTICE, client.DownloadString(urlPrefix + "ChannelPointsSFX-changelog.txt"));
                }
            }
        }

        private void frmMain_OnClosing(object sender, FormClosingEventArgs e)
        {
            trayIcon.Visible = false;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, uint Msg);

        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            ShowWindow(this.Handle, 0x09);
        }

        /// <summary>
        /// Keeps the layout consistent on resize.
        /// </summary>
        private void frmMain_SizeChanged(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                if (Properties.Settings.Default.minimizeToTray == true)
                {
                    trayIcon.Visible = true;
                    trayIcon.ShowBalloonTip(250);
                    this.Hide();
                }
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                trayIcon.Visible = false;
            }

            btnUp.Left = this.Width - 45;
            btnDown.Left = this.Width - 45;
            txtVolume.Left = this.Width - 52;
            trkVolume.Width = txtVolume.Left - 18;
            lblLine.Width = this.Width - 84;
            int listBoxArea = btnUp.Left - lstbxSoundsRewards.Left;
            lstbxSoundsRewards.Width = listBoxArea / 2 - 6;
            lstbxSoundsPaths.Left = (lstbxSoundsRewards.Left + lstbxSoundsRewards.Width);
            lstbxSoundsPaths.Width = listBoxArea / 2 - 3;
            btnRemove.Left = (lstbxSoundsPaths.Left + lstbxSoundsPaths.Width) - btnRemove.Width;
            btnTest.Left = lstbxSoundsPaths.Left - 38;
            btnStopAll.Width = btnRemove.Left + btnRemove.Width - btnStopAll.Left;
            lblNOTICE.Width = this.Width - 36;
            btnSettings.Left = this.Width - 45;
            btnEditSound.Left = btnRemove.Left + btnRemove.Width - btnEditSound.Width;
        }

        private static void onPubSubServiceConnected(object sender, EventArgs e)
        {
            client.SendTopics();
        }

        /// <summary>
        /// Throws an exception if we failed to connect to the PubSub.
        /// </summary>
        private static void onListenResponse(object sender, OnListenResponseArgs e)
        {
            if (e.Successful)
            {
#if DEBUG
                Debug.WriteLine($"Successfully verified listening to topic: {e.Topic}");
#endif
            }
            else
                throw new Exception($"Failed to listen! Response: {e.Response}");
        }

        private void BuildAlert(string Title, string DisplayName, int RewardCost, string Sound)
        {
            if (File.Exists("alert.txt"))
            {
                Task.Delay(500).ContinueWith((task) => {
                    BuildAlert(Title, DisplayName, RewardCost, Sound);
                });
            }
            else
            {
                File.WriteAllText("alert.txt", DisplayName + "\n" + Title + "\n" + RewardCost);
                Task.Delay(500).ContinueWith((task) => {
                    PlaySound(Sound);
                });

                Task.Delay(Decimal.ToInt32(Properties.Settings.Default.rewardTimer) * 1000).ContinueWith((task) => {
                    if (File.Exists("alert.txt"))
                    {
                        File.Delete("alert.txt");
                    }
                });
            }
        }

        /// <summary>
        /// Checks the bindings array and plays the appropriate sound file when a Channel Points reward is redeemed.
        /// </summary>
        private void OnRewardRedeemed(object sender, OnRewardRedeemedArgs e)
        {
#if DEBUG
            Debug.WriteLine("Reward Redeemed:");
            Debug.WriteLine("\tTitle:" + e.RewardTitle + "|");
            Debug.WriteLine("\tPrompt:" + e.RewardPrompt + "|");
            Debug.WriteLine("\tUser:" + e.DisplayName + "|");
            Debug.WriteLine("\tStatus:" + e.Status + "|");
            Debug.WriteLine("----------------");
#endif
            if(bindings.ContainsKey(e.RewardTitle) && e.Status != "ACTION_TAKEN")
            {
                bindings.TryGetValue(e.RewardTitle, out string output);
#if DEBUG
                Debug.WriteLine(output);
#endif
                BuildAlert(e.RewardTitle, e.DisplayName, e.RewardCost, output);
            }
        }

        /// <summary>
        /// Updates the volume level as a user moves the trackbar.
        /// </summary>
        private void trkVolume_Scroll(object sender, EventArgs e)
        {
            this.volumeLevel = trkVolume.Value;
            txtVolume.Text = this.volumeLevel.ToString();
        }

        /// <summary>
        /// Saves the current volume level once the user unclicks the Trackbar.
        /// </summary>
        private void TrkVolume_MouseUp(object sender, MouseEventArgs e)
        {
            if (savedVolumeLevel != volumeLevel)
            {
                Properties.Settings.Default.savedVolumeLevel = volumeLevel.ToString();
                Properties.Settings.Default.Save();
                savedVolumeLevel = Convert.ToInt32(Properties.Settings.Default.savedVolumeLevel);
            }
        }

        /// <summary>
        /// Sets the volume level for the MediaPlayer object.
        /// </summary>
        /// <param name="volume">Assumed to be between 0 and 100.</param>
        public void SetVolume(int volume)
        {
            // MediaPlayer volume is a float value between 0 and 1.
            thePlayer.Volume = volume / 100.0f;
        }

        /// <summary>
        /// Plays a sound file from a path.
        /// </summary>
        /// <param name="filename">The path to the sound file to play.</param>
        public void PlaySound(string filename)
        {
#if DEBUG
            Debug.WriteLine("Playing sound from \"" + filename + "\".");
#endif
            thePlayer = new MediaPlayer();
            thePlayer.Open(new Uri(filename));
            this.SetVolume(this.volumeLevel);
            thePlayer.MediaEnded += ThePlayer_MediaEnded;
            btnStopAll.Enabled = true;
            allPlayers.Add(thePlayer);
            thePlayer.Play();
        }

        /// <summary>
        /// Closes the media file once it's done playing.
        /// </summary>
        private void ThePlayer_MediaEnded(object sender, EventArgs e)
        {
#if DEBUG
            Debug.WriteLine("Sound playing ended.");
#endif
            thePlayer.Close();
            allPlayers.Remove((MediaPlayer)sender);
        }

        /// <summary>
        /// Reloads the bindings array into the ListBoxes and saves the bindings to file if nessecary.
        /// </summary>
        private void reloadListItems()
        {
            lstbxSoundsRewards.Items.Clear();
            lstbxSoundsPaths.Items.Clear();

            foreach (KeyValuePair<string, string> kvp in bindings)
            {
                lstbxSoundsRewards.Items.Add(kvp.Key);
                string[] val = kvp.Value.Split('\\');
                lstbxSoundsPaths.Items.Add(val[val.Length-1]);
            }

            if (boxSelection > (bindings.Count - 1)) boxSelection = (bindings.Count - 1);

            if(bindings.Count == 0)
            {
                btnUp.Enabled = false;
                btnDown.Enabled = false;
                btnRemove.Enabled = false;
                btnTest.Enabled = false;
                btnEditSound.Enabled = false;
                btnEditTitle.Enabled = false;
            } 
            else
            {
                btnUp.Enabled = true;
                btnDown.Enabled = true;
                btnRemove.Enabled = true;
                btnTest.Enabled = true;
                btnEditSound.Enabled = true;
                btnEditTitle.Enabled = true;
                lstbxSoundsRewards.SelectedIndex = boxSelection;
                lstbxSoundsPaths.SelectedIndex = boxSelection;
                saveBindings();
            }
        }

        /// <summary>
        /// Saves the bindings array to the settings.txt file.
        /// </summary>
        private void saveBindings()
        {
            String buildString = "";
            foreach (KeyValuePair<string, string> kvp in bindings)
            {
                buildString += kvp.Key + "|" + kvp.Value + "\n";
            }
            File.WriteAllText("settings.txt", buildString);
        }

        /// <summary>
        /// Decreases the boxSelection by one and highlights the appropriate items in the list boxes.
        /// </summary>
        private void btnUp_Click(object sender, EventArgs e)
        {
            boxSelection--;
            if (boxSelection < 0) boxSelection = 0;
            lstbxSoundsRewards.SelectedIndex = boxSelection;
            lstbxSoundsPaths.SelectedIndex = boxSelection;
        }

        /// <summary>
        /// Increases the boxSelection by one and highlights the appropriate items in the list boxes.
        /// </summary>
        private void btnDown_Click(object sender, EventArgs e)
        {
            boxSelection++;
            if (boxSelection > (bindings.Count-1)) boxSelection = (bindings.Count - 1);
            lstbxSoundsRewards.SelectedIndex = boxSelection;
            lstbxSoundsPaths.SelectedIndex = boxSelection;
        }

        /// <summary>
        /// Prevents users from clicking on a listbox item.
        /// </summary>
        private void lstbxSoundsRewards_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstbxSoundsRewards.SelectedIndex = boxSelection;
        }

        int hoveredIndexRewards = -1;
        private void lstbxSoundsRewards_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // See which row is currently under the mouse:
            int newHoveredIndex = lstbxSoundsRewards.IndexFromPoint(e.Location);

            // If the row has changed since last moving the mouse:
            if (hoveredIndexRewards != newHoveredIndex)
            {
                // Change the variable for the next time we move the mouse:
                hoveredIndexRewards = newHoveredIndex;

                // If over a row showing data (rather than blank space):
                if (hoveredIndexRewards > -1)
                {
                    //Set tooltip text for the row now under the mouse:
                    toolTip1.Active = false;
                    toolTip1.SetToolTip(lstbxSoundsRewards, lstbxSoundsRewards.Items[hoveredIndexRewards].ToString());
                    toolTip1.Active = true;
                }
            }
        }

        /// <summary>
        /// Prevents users from clicking on a listbox item.
        /// </summary>
        private void lstbxSoundsPaths_SelectedIndexChanged(object sender, EventArgs e)
        {
            lstbxSoundsPaths.SelectedIndex = boxSelection;
        }

        int hoveredIndexPaths = -1;
        private void lstbxSoundsPaths_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // See which row is currently under the mouse:
            int newHoveredIndex = lstbxSoundsPaths.IndexFromPoint(e.Location);

            // If the row has changed since last moving the mouse:
            if (hoveredIndexPaths != newHoveredIndex)
            {
                // Change the variable for the next time we move the mouse:
                hoveredIndexPaths = newHoveredIndex;

                // If over a row showing data (rather than blank space):
                if (hoveredIndexPaths > -1)
                {
                    //Set tooltip text for the row now under the mouse:
                    toolTip1.Active = false;
                    bindings.TryGetValue(lstbxSoundsRewards.Items[hoveredIndexPaths].ToString(), out string fullPath);
                    toolTip1.SetToolTip(lstbxSoundsPaths, fullPath);
                    toolTip1.Active = true;
                }
            }
        }

        /// <summary>
        /// Removes the selected item from the bingings array and reloads the listboxes.
        /// </summary>
        private void btnRemove_Click(object sender, EventArgs e)
        {
            bindings.Remove(lstbxSoundsRewards.SelectedItem.ToString());
            reloadListItems();
        }

        /// <summary>
        /// Tests the currently selected listbox item as if a Reward had been redeemed.
        /// </summary>
        private void btnTest_Click(object sender, EventArgs e)
        {
            if(lstbxSoundsRewards.SelectedItem == null) return;
            bindings.TryGetValue(lstbxSoundsRewards.SelectedItem.ToString(), out string output);
            PlaySound(output);
        }

        private void btnStopAll_Click(object sender, EventArgs e)
        {
            foreach(MediaPlayer player in allPlayers)
            {
                player.Stop();
                player.Close();
            }
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            frmOptions optionsForm = new frmOptions();
            optionsForm.enSetBut += new ReenableSettingsButton(oFrmOptions_enSetBut);
            optionsForm.StartPosition = FormStartPosition.Manual;
            optionsForm.Location = this.Location;
            optionsForm.Icon = this.Icon;
            optionsForm.Show();
            btnSettings.Enabled = false;
        }

        private void btnServer_Click(object sender, EventArgs e)
        {
            lock (mutex)
            {
                if (Properties.Settings.Default.serverStatus == true)
                {
                    killServer = true;
                    btnServer.ForeColor = System.Drawing.Color.Orange;
                }
                else
                {
                    killServer = false;
                    Properties.Settings.Default.serverStatus = true;
                    btnServer.ForeColor = System.Drawing.Color.Orange;
                    ThreadPool.QueueUserWorkItem(HandleConnections);
                }
            }
        }

        void oFrmOptions_enSetBut()
        {
            btnSettings.Enabled = true;
        }

        private void btnEditTitle_Click(object sender, EventArgs e)
        {
            if (lstbxSoundsRewards.SelectedItem == null) return;
            bindings.TryGetValue(lstbxSoundsRewards.SelectedItem.ToString(), out string output);
            if (output.Length > 0)
            {
                string rewardName = Prompt.ShowDialog("Enter the twitch Channel Points reward name EXACTLY as it is on twitch.", "Channel Reward Name");
                bindings.Remove(lstbxSoundsRewards.SelectedItem.ToString());
                bindings[rewardName] = output;
                reloadListItems();
            }
        }

        private void btnEditSound_Click(object sender, EventArgs e)
        {
            if (lstbxSoundsRewards.SelectedItem == null) return;

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    string filePath = openFileDialog.FileName;
                    bindings[lstbxSoundsRewards.SelectedItem.ToString()] = filePath;
                    reloadListItems();
                }
            }
        }

        /// <summary>
        /// Prompts user for the Channel Points Reward name and then opens an OpenFileDialog to select the file.
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string rewardName = Prompt.ShowDialog("Enter the twitch Channel Points reward name EXACTLY as it is on twitch.", "Channel Reward Name");

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    string filePath = openFileDialog.FileName;
                    bindings.Add(rewardName, filePath);
                    reloadListItems();
                }
            }
        }
    }

    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label textLabel = new Label() { Left = 50, Top = 20, AutoSize = true, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            if (prompt.ShowDialog() == DialogResult.OK) {
                prompt.Dispose();
                return textBox.Text;
            } 

            prompt.Dispose();
            return "";
        }
    }
}
