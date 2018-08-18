using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Gma.System.MouseKeyHook;

namespace Tracker
{
    public partial class Form1 : Form
    {
        private IKeyboardMouseEvents m_GlobalHook;
        User u = new User();
        LocalData offlineData = new LocalData();
        internal LocalData OfflineData { get => offlineData; set => offlineData = value; }
        public Point Last_mouse_pos { get => last_mouse_pos; set => last_mouse_pos = value; }

        Point last_mouse_pos;

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            offlineData.trackActivity("KEYBOARD", u);
        }

        private void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
        {
            offlineData.trackActivity("KEYBOARD", u);
        }

        private void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
        {
            offlineData.trackActivity("MOUSE", u);
        }

        public Form1()
        {
            InitializeComponent();
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;
            m_GlobalHook.KeyPress += GlobalHookKeyPress;

        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {

            m_GlobalHook.MouseDownExt -= GlobalHookMouseDownExt;
            m_GlobalHook.KeyPress -= GlobalHookKeyPress;

            //It is recommened to dispose it
            m_GlobalHook.Dispose();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                ShowIcon = false;
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(1000, "Hello world!", "My Nice little text", ToolTipIcon.Info);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            notifyIcon1.BalloonTipText = "Hello world!";
            notifyIcon1.BalloonTipTitle = "My Nice Little Title";
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {

            ShowInTaskbar = true;
            notifyIcon1.Visible = false;
            WindowState = FormWindowState.Normal;
        }

        private void statusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {

                Rectangle bounds = Screen.GetBounds(Point.Empty);
                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    }

                    var directory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    var uniqueFileName = string.Format(@"{0}.png", Guid.NewGuid());
                    var path = System.IO.Path.Combine(directory, "Tracker", "Images", uniqueFileName);
                    bitmap.Save(path, ImageFormat.Png);
                    offlineData.trackFile(uniqueFileName, u);
                }

            } catch( Exception ex)
            {
                Console.Write(ex.Message);
                Console.Write("Fatal error :: can not capture screen shot");
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            label5.Text = "Loading ... ";
            bool loggedIn = u.login(textBox1.Text, textBox2.Text);
            if (loggedIn)
            {
                logout.Show();
                panel1.Hide();
                panel2.Show();
                label5.Text = "Wlcome, " + u.Email + " Logged In";
            } else
            {
                label5.Text = "Logged Out";
            }
        }

        private void trackBtn_Click(object sender, EventArgs e)
        {
           if (u.LoggedIn)
           {
                trackInfo.Text = "Processing ....";

                switch (trackBtn.Text)
                {
                    case "START": {
                        timer1.Enabled = true;
                        OfflineData.endTracking(u);
                        trackInfo.Text = "tracking on";
                        trackBtn.Text = "END";
                        }
                    break;
                    case "END": {
                            timer1.Enabled = false;
                            OfflineData.startTracking(u);
                            trackInfo.Text = "tracking off";
                            trackBtn.Text = "START";
                        }
                    break;
                }

            }
        }

        private void logout_Click(object sender, EventArgs e)
        {
            OfflineData.endTracking(u);
            if (OfflineData.sync(u))
            {
                u = new User();
                label5.Text = "You are not logged in.";
                panel2.Hide();
                panel1.Show();
                logout.Hide();
            }

        }

       
    }
}
