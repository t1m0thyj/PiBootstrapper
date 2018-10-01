using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.IO;

namespace PiBootstrapper
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void UpdateDriveList()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            driveComboBox.Items.Clear();

            foreach (DriveInfo d in allDrives)
            {
                if (d.DriveType != DriveType.Removable)
                {
                    continue;
                }

                try
                {
                    string itemName = d.Name;
                    if (d.VolumeLabel.Length > 0)
                    {
                        itemName += " - " + d.VolumeLabel;
                    }

                    driveComboBox.Items.Add(itemName);

                    if (d.VolumeLabel == "boot")
                    {
                        driveComboBox.SelectedItem = itemName;
                    }
                }
                catch (IOException) { }
            }
        }

        private void UpdateGuiState()
        {
            userTextBox.Enabled = (typeComboBox.SelectedIndex == 1);

            bool isEmpty = (driveComboBox.SelectedIndex == -1
                || nameTextBox.Text.Length == 0
                || (typeComboBox.SelectedIndex == 1 && userTextBox.Text.Length == 0)
                || passwordTextBox.Text.Length == 0
                || passwordTextBox2.Text.Length == 0);

            startButton.Enabled = !isEmpty;
        }

        private void ConfigureImage(string bootDrive, string networkConfig, bool enableSsh)
        {
            string wpaSupplicantConf = Path.Combine(bootDrive, "wpa_supplicant.conf");
            bool writeConfig = true;

            if (File.Exists(wpaSupplicantConf))
            {
                DialogResult result = MessageBox.Show("The file /boot/wpa_supplicant.conf " +
                    "already exists on the SD card. Are you sure you want to overwrite it?",
                    "Question", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                writeConfig = (result == DialogResult.Yes);
            }

            if (writeConfig)
            {
                var regionInfo = new RegionInfo(CultureInfo.CurrentCulture.LCID);
                string country = regionInfo.TwoLetterISORegionName;

                string configText = "ctrl_interface=DIR=/var/run/wpa_supplicant GROUP=netdev\n"
                    + "update_config=1\ncountry=" + country + "\n\n" + networkConfig + "\n";
                File.WriteAllText(wpaSupplicantConf, configText);
            }

            if (enableSsh)
            {
                File.Create(Path.Combine(bootDrive, "ssh"));
            }
            else
            {
                File.Delete(Path.Combine(bootDrive, "ssh"));
            }
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            UpdateDriveList();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            string bootDrive = driveComboBox.GetItemText(
                driveComboBox.SelectedItem).Split(' ')[0].TrimEnd('\\');

            if (!Directory.Exists(bootDrive))
            {
                MessageBox.Show("The drive you have selected is no longer accessible.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!File.Exists(Path.Combine(bootDrive, "kernel.img")))
            {
                MessageBox.Show("The drive you have selected is not a Raspbian SD card.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (passwordTextBox.Text != passwordTextBox2.Text)
            {
                MessageBox.Show("The passwords you have entered do not match.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (typeComboBox.SelectedIndex == 0 && (passwordTextBox.Text.Length < 8
                || passwordTextBox.Text.Length > 63))
            {
                MessageBox.Show("Password must be between 8 and 63 characters long.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else if (typeComboBox.SelectedIndex == 1 && passwordTextBox.Text.Length > 14)
            {
                MessageBox.Show("Password cannot exceed 14 characters in length.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string networkName = nameTextBox.Text;
            string username = userTextBox.Text;
            string password = passwordTextBox.Text;
            string networkConfig;

            if (typeComboBox.SelectedIndex == 0)
            {
                networkConfig = WpaPersonal.GetConfig(networkName, password);
            }
            else
            {
                networkConfig = WpaEnterprise.GetConfig(networkName, username, password);
            }

            ConfigureImage(bootDrive, networkConfig, sshCheckBox.Checked);

            MessageBox.Show("SD card image has been configured successfully.", "Info");
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void formControl_Modified(object sender, EventArgs e)
        {
            UpdateGuiState();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UpdateDriveList();

            typeComboBox.SelectedIndex = 0;

            UpdateGuiState();
        }
    }
}
