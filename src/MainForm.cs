using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
            comboBox1.Items.Clear();

            foreach (DriveInfo d in allDrives)
            {
                if (d.DriveType != DriveType.Removable)
                {
                    continue;
                }

                string itemName;
                if (d.VolumeLabel.Length > 0)
                {
                    itemName = d.Name.TrimEnd('\\') + " (" + d.VolumeLabel + ")";
                }
                else
                {
                    itemName = d.Name.TrimEnd('\\');
                }

                comboBox1.Items.Add(itemName);

                if (d.VolumeLabel == "boot")
                {
                    comboBox1.SelectedItem = itemName;
                }
            }
        }

        private void EnsureNonEmpty()
        {
            bool isEmpty = (comboBox1.SelectedIndex == -1
                || textBox1.Text.Length == 0
                || textBox2.Text.Length == 0
                || textBox3.Text.Length == 0
                || textBox4.Text.Length == 0);

            startButton.Enabled = !isEmpty;
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            UpdateDriveList();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            string drive = comboBox1.GetItemText(comboBox1.SelectedItem).Substring(0, 2);
            if (!Directory.Exists(Path.Combine(drive, "aardvarks")))
            {
                MessageBox.Show("aardvarks", "Error");
                return;
            }

            if (textBox3.Text != textBox4.Text)
            {
                MessageBox.Show("The passwords you have entered do not match.", "Error");
                return;
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnsureNonEmpty();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            EnsureNonEmpty();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            EnsureNonEmpty();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            EnsureNonEmpty();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            EnsureNonEmpty();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UpdateDriveList();

            comboBox2.SelectedIndex = 0;

            EnsureNonEmpty();
        }
    }
}
