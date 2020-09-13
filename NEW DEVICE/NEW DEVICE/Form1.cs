using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;
namespace NEW_DEVICE
{
    public partial class Form1 : Form
    {
        public const int MAXICONS = 1000;
        public const int MOUSETOOLBARY = 50;
        [Serializable]
        public class c
        {
            public String Icon;
            public String Program;
            public String Tip;
            public Point Location;
        }
        public static Form1 mf = null;
        public static Form2 f2 = null;
        public PictureBox[] pbs = new PictureBox[MAXICONS];
        public static Point rightmousepos;
        public static Point leftmousepos;
        ProcessStartInfo si;
        
        [Serializable]
        public class cNEWDEVICE
        {
            public String Background;
            public c[] cs = new c[MAXICONS];
        }

        cNEWDEVICE NEWDEVICE = new cNEWDEVICE();
        public Point pbClickLocation = new Point(0, 0);
        public PictureBox cpb;
        public Point cpbLocation;
        public Form1()
        {
            InitializeComponent();
            mf = this;
        }

        private void minimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void turnOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LoadDesktop(object sender, EventArgs e)
        {
            try
            {
                NEWDEVICE = (cNEWDEVICE)ByteArrayToObject(File.ReadAllBytes("NEW DEVICE.BIN"));
            }
            catch
            {}
        }

        private void SaveDesktop(object sender, EventArgs e)
        {
            try
            {
                File.WriteAllBytes("NEW DEVICE.BIN", ObjectToByteArray(NEWDEVICE));
            }
            catch
            { }
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int FirstEmpty = -1;
            bool overlap = false;
            // Check if overlap, pictureboxes can not overlap
            for (int i = 0; i < MAXICONS; i++)
            {
                if (NEWDEVICE != null)
                {
                    if (NEWDEVICE.cs != null)
                    {
                        if (NEWDEVICE.cs[i] != null)
                        {
                            if (NEWDEVICE.cs[i].Location != null)
                            {
                                if (((rightmousepos.X >= NEWDEVICE.cs[i].Location.X) & (rightmousepos.X <= (NEWDEVICE.cs[i].Location.X + 100)) &
                                    ((rightmousepos.Y >= (NEWDEVICE.cs[i].Location.Y) & (rightmousepos.Y <= NEWDEVICE.cs[i].Location.Y + 100)))))
                                {
                                    overlap = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (overlap)
            {
                return;
            }
            if (f2 == null)
            {
                f2 = new Form2(this);
            }
            DialogResult dr = f2.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                // find first free PictureBox
                for (int i = 0; i < MAXICONS; i++)
                {
                    if (NEWDEVICE.cs[i] == null)
                    {
                        FirstEmpty = i;
                        break;
                    }
                }
                if (FirstEmpty != -1)
                {
                    // We found first empty
                    NEWDEVICE.cs[FirstEmpty] = new c();
                    NEWDEVICE.cs[FirstEmpty].Icon = f2.textBox1.Text;
                    NEWDEVICE.cs[FirstEmpty].Program = f2.textBox2.Text;
                    NEWDEVICE.cs[FirstEmpty].Tip = f2.textBox3.Text;
                    NEWDEVICE.cs[FirstEmpty].Location = rightmousepos;
                    pbs[FirstEmpty] = new PictureBox();
                    pbs[FirstEmpty].Location = NEWDEVICE.cs[FirstEmpty].Location;
                    if ((NEWDEVICE.cs[FirstEmpty].Icon != "") & (NEWDEVICE.cs[FirstEmpty].Icon != null))
                    {
                        try
                        {
                            pbs[FirstEmpty].Image = Image.FromFile(NEWDEVICE.cs[FirstEmpty].Icon);
                        }
                        catch
                        {
                            pbs[FirstEmpty].Image = Image.FromFile("20200723_113654.jpg");
                        }
                    }
                    pbs[FirstEmpty].Cursor = Cursors.Hand;
                    pbs[FirstEmpty].Size = new Size(100, 100);
                    pbs[FirstEmpty].SizeMode = PictureBoxSizeMode.StretchImage;
                    pbs[FirstEmpty].Click += new System.EventHandler(this.pictureBox_Click);
                    pbs[FirstEmpty].MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
                    Controls.Add(pbs[FirstEmpty]);
                }
            }
            f2 = null;
        }

        private void modifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Find clicked control PictureBox, if any
            int FirstEmpty = -1;
            if (f2 == null)
            {
                f2 = new Form2(this);
            }
            // Find Selected PictureBox
            for (int i = 0; i < MAXICONS; i++)
            {
                if (NEWDEVICE != null)
                {
                    if (NEWDEVICE.cs != null)
                    {
                        if (NEWDEVICE.cs[i] != null)
                        {
                            if (NEWDEVICE.cs[i].Location != null)
                            {
                                if (((cpbLocation.X >= NEWDEVICE.cs[i].Location.X) &  (cpbLocation.X <= (NEWDEVICE.cs[i].Location.X + 100)) &
                                    ((cpbLocation.Y >= (NEWDEVICE.cs[i].Location.Y) & (cpbLocation.Y <= NEWDEVICE.cs[i].Location.Y + 100)))))
                                {
                                    FirstEmpty = i;
                                    break;
                                }
                            }
                        }
                    }
                }
            }            
            if (FirstEmpty == -1)
            {
                return;
            }
            // Update Form2 with selected properties
            f2.textBox1.Text = NEWDEVICE.cs[FirstEmpty].Icon;
            f2.textBox2.Text = NEWDEVICE.cs[FirstEmpty].Program;
            f2.textBox3.Text = NEWDEVICE.cs[FirstEmpty].Tip;
            DialogResult dr = f2.ShowDialog(this);
            if (dr == DialogResult.OK)
            {
                // We found first empty
                NEWDEVICE.cs[FirstEmpty].Icon = f2.textBox1.Text;
                NEWDEVICE.cs[FirstEmpty].Program = f2.textBox2.Text;
                NEWDEVICE.cs[FirstEmpty].Tip = f2.textBox3.Text;
                if ((NEWDEVICE.cs[FirstEmpty].Icon != "") & (NEWDEVICE.cs[FirstEmpty].Icon != null))
                {
                    try
                    {
                        pbs[FirstEmpty].Image = Image.FromFile(NEWDEVICE.cs[FirstEmpty].Icon);
                    }
                    catch
                    {
                        pbs[FirstEmpty].Image = Image.FromFile("20200723_113654.jpg");                        
                    }
                }
            }
            f2 = null;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Find clicked control PictureBox, if any
            int FirstEmpty = -1;
            // Find Selected PictureBox
            for (int i = 0; i < MAXICONS; i++)
            {
                if (NEWDEVICE != null)
                {
                    if (NEWDEVICE.cs != null)
                    {
                        if (NEWDEVICE.cs[i] != null)
                        {
                            if (NEWDEVICE.cs[i].Location != null)
                            {
                                if (((cpbLocation.X >= NEWDEVICE.cs[i].Location.X) & (cpbLocation.X <= (NEWDEVICE.cs[i].Location.X + 100)) &
                                    ((cpbLocation.Y >= (NEWDEVICE.cs[i].Location.Y) & (cpbLocation.Y <= NEWDEVICE.cs[i].Location.Y + 100)))))
                                {
                                    FirstEmpty = i;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (FirstEmpty == -1)
            {
                return;
            }
            // Delete control, PictureBox
            NEWDEVICE.cs[FirstEmpty] = null;
            pbs[FirstEmpty].Dispose();
            pbs[FirstEmpty] = null;
        }

        private void toolbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolbarToolStripMenuItem.Checked = !toolbarToolStripMenuItem.Checked;
            if (toolbarToolStripMenuItem.Checked)
            {
                panel1.Visible = true;
            }
            else
            {
                panel1.Visible = false;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (toolbarToolStripMenuItem.Checked)
            {
                if (e.Y <= MOUSETOOLBARY)
                {
                    panel1.Visible = true;
                }
                else
                {
                    panel1.Visible = false;
                }
            }
        }

        public static Control FindControlAtPoint(Control container, Point pos)
        {
            Control child;
            foreach (Control c in container.Controls)
            {
                if (c.Visible && c.Bounds.Contains(pos))
                {
                    child = FindControlAtPoint(c, new Point(pos.X - c.Left, pos.Y - c.Top));
                    if (child == null) return c;
                    else return child;
                }
            }
            return null;
        }

        public static Control FindControlAtCursor(Form form)
        {
            Point pos = Cursor.Position;
            if (form.Bounds.Contains(pos))
                return FindControlAtPoint(form, form.PointToClient(pos));
            return null;
        }

        // Convert an object to a byte array
        private byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }

        // Convert a byte array to an Object
        private Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);
            return obj;
        }

        private void backgroundToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            ofd.Filter = "Image files(*.jpg, *.jpeg, *.jpe, *.jfif, *.png)|*.jpg; *.jpeg; *.jpe; *.jfif; *.png|All files (*.*)|*.*";
            ofd.FilterIndex = 1;
            ofd.RestoreDirectory = true;
            DialogResult dr = ofd.ShowDialog();
            if (dr == DialogResult.OK)
            {
                if ((ofd.FileName != "") & (ofd.FileName != null))
                {
                    NEWDEVICE.Background = ofd.FileName;
                    BackgroundImage = Image.FromFile(NEWDEVICE.Background);
                }
                else
                {
                    NEWDEVICE.Background = null;
                    BackgroundImage = null;
                }
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                rightmousepos.X = e.X;
                rightmousepos.Y = e.Y;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadDesktop(sender, e);
            LoadCreateIcons(sender, e);
        }

        private void LoadCreateIcons(object sender, EventArgs e)
        {
            // Load general setting
            try
            {
                BackgroundImage = Image.FromFile(NEWDEVICE.Background);
            }
            catch
            {
                BackgroundImage = null;
            }
            // Create dekstop controls from loaded components file
            for (int i = 0; i < MAXICONS; i++)
            {
                if (NEWDEVICE.cs[i] != null)
                {
                    pbs[i] = new PictureBox();
                    pbs[i].Location = NEWDEVICE.cs[i].Location;
                    if ((NEWDEVICE.cs[i].Icon != "") & (NEWDEVICE.cs[i].Icon != null))
                    {
                        try
                        {
                            pbs[i].Image = Image.FromFile(NEWDEVICE.cs[i].Icon);
                        }
                        catch
                        {
                            pbs[i].Image = Image.FromFile("20200723_113654.jpg");
                        }
                    }
                    pbs[i].Cursor = Cursors.Hand;
                    pbs[i].Size = new Size(100, 100);
                    pbs[i].SizeMode = PictureBoxSizeMode.StretchImage;
                    pbs[i].Click += new System.EventHandler(this.pictureBox_Click);
                    pbs[i].MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
                    Controls.Add(pbs[i]);                    
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveDesktop(sender, e);
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            // Find clicked PictureBox
            int FirstEmpty = -1;
            Point clickmousepos = new Point(0, 0);
            PictureBox pb = (PictureBox)FindControlAtCursor(mf);
            if (pb == null)
            {
                return;
            }
            clickmousepos.X = pb.Location.X;
            clickmousepos.Y = pb.Location.Y;
            // Find Selected PictureBox
            for (int i = 0; i < MAXICONS; i++)
            {
                if (NEWDEVICE != null)
                {
                    if (NEWDEVICE.cs != null)
                    {
                        if (NEWDEVICE.cs[i] != null)
                        {
                            if (NEWDEVICE.cs[i].Location != null)
                            {
                                if (((clickmousepos.X >= NEWDEVICE.cs[i].Location.X) & (clickmousepos.X <= (NEWDEVICE.cs[i].Location.X + 100)) &
                                    ((clickmousepos.Y >= (NEWDEVICE.cs[i].Location.Y) & (clickmousepos.Y <= NEWDEVICE.cs[i].Location.Y + 100)))))
                                {
                                    FirstEmpty = i;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (FirstEmpty == -1)
            {
                return;
            }
            // Execute link
            try
            {
                si = new ProcessStartInfo(NEWDEVICE.cs[FirstEmpty].Program);
                Process.Start(si);
            }
            catch
            { }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                leftmousepos.X = e.X;
                leftmousepos.Y = e.Y;
            }
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                cpb = (PictureBox)sender;
                cpbLocation = cpb.Location;
            }
        }
    }
}
