using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PS3Lib; //Déclaration de la PS3Lib
using System.Threading;

namespace Simple_projet
{
    public partial class Form1 : Form
    {
        static PS3API PS3 = new PS3API();
        public Form1()
        {
            InitializeComponent();
        }
        public class Buttons
        {
            public static UInt32
                //X = 262144,
            X = 4,
            Square = 805306368,
            L1 = 526336,
            L2 = 8388608,
            L3 = 35651584,
            R1 = 16777216,
            R2 = 4194304,
            R3 = 67108868,
            Crouch = 131072,
            Prone = 65536,
            StartButton = 32768;

            public static bool ButtonPressed(UInt32 clientIndex, UInt32 Button)
            {
                if (PS3.Extension.ReadUInt32(0xF44980 + 0x3330 + (0x3700 * (uint)clientIndex)) == Button)
                    return true;
                else return false;
            }
        }
        private void iPrintln(int Client, string text)
        {
            SV_GameSendServerCommand(Client, "f \"" + text + "\"");
        }
        private void iPrintlnBold(int client, string text)
        {
            SV_GameSendServerCommand(client, "c \"" + text + "\"");
        }
        public void SV_GameSendServerCommand(int Client, string Command)
        {
            CallFunction(SV_Gamesendservercommand, new object[] { Client, 0, Command });
        }
        public static uint Add_Ammo = 0x24879C;
        public static uint PlayerState = 0xF44980;
        public static uint G_GivePlayerWeapon = 0x2947FC;
        private void GiveWeapon(int client, int weapon, int ammo, int akimbo)
        {
            CallFunction(G_GivePlayerWeapon, new object[] { PlayerState + ((uint)(client * 0x3700)), weapon, akimbo });
            CallFunction(Add_Ammo, new object[] { PlayerState + (client * 0x3700), weapon, 0, ammo, 1 });
            CallFunction(SV_Gamesendservercommand, new object[] { client, 0, "a \"" + weapon + "\"" });
        }
        public int CallFunction(uint func_address, params object[] parameters)
        {
            int length = parameters.Length;
            uint num2 = 0;
            for (uint i = 0; i < length; i++)
            {
                byte[] buffer;
                if (parameters[i] is int)
                {
                    buffer = BitConverter.GetBytes((int)parameters[i]);
                    Array.Reverse(buffer);
                    PS3.SetMemory(0x10050000 + ((i + num2) * 4), buffer);
                }
                else if (parameters[i] is uint)
                {
                    buffer = BitConverter.GetBytes((uint)parameters[i]);
                    Array.Reverse(buffer);
                    PS3.SetMemory(0x10050000 + ((i + num2) * 4), buffer);
                }
                else if (parameters[i] is string)
                {
                    byte[] buffer2 = Encoding.UTF8.GetBytes(Convert.ToString(parameters[i]) + "\0");
                    PS3.SetMemory(0x10050054 + (i * 0x400), buffer2);
                    uint num4 = 0x10050054 + (i * 0x400);
                    byte[] array = BitConverter.GetBytes(num4);
                    Array.Reverse(array);
                    PS3.SetMemory(0x10050000 + ((i + num2) * 4), array);
                }
                else if (parameters[i] is float)
                {
                    num2++;
                    buffer = BitConverter.GetBytes((float)parameters[i]);
                    Array.Reverse(buffer);
                    PS3.SetMemory(0x10050024 + ((num2 - 1) * 4), buffer);
                }
            }
            byte[] bytes = BitConverter.GetBytes(func_address);
            Array.Reverse(bytes);
            PS3.SetMemory(0x1005004c, bytes);
            Thread.Sleep(20);
            byte[] buffer5 = new byte[4];
            PS3.GetMemory(0x10050050, buffer5);
            Array.Reverse(buffer5);
            return BitConverter.ToInt32(buffer5, 0);
        }
        private uint function_address = 0x4A0700;
        public static uint CBUF = 0x2B1C14;
        public static uint SV_Gamesendservercommand = 0x672444;
        private void WritePPC()
        {
            PS3.SetMemory(function_address, new byte[] { 0x4e, 0x80, 0, 0x20 });
            Thread.Sleep(20);
            byte[] buffer = new byte[] { 
                 0x7c, 8, 2, 0xa6, 0xf8, 1, 0, 0x80, 60, 0x60, 0x10, 5, 0x81, 0x83, 0, 0x4c, 
                 0x2c, 12, 0, 0, 0x41, 130, 0, 100, 0x80, 0x83, 0, 4, 0x80, 0xa3, 0, 8, 
                 0x80, 0xc3, 0, 12, 0x80, 0xe3, 0, 0x10, 0x81, 3, 0, 20, 0x81, 0x23, 0, 0x18, 
                 0x81, 0x43, 0, 0x1c, 0x81, 0x63, 0, 0x20, 0xc0, 0x23, 0, 0x24, 0xc0, 0x43, 0, 40, 
                 0xc0, 0x63, 0, 0x2c, 0xc0, 0x83, 0, 0x30, 0xc0, 0xa3, 0, 0x34, 0xc0, 0xc3, 0, 0x38, 
                 0xc0, 0xe3, 0, 60, 0xc1, 3, 0, 0x40, 0xc1, 0x23, 0, 0x48, 0x80, 0x63, 0, 0, 
                 0x7d, 0x89, 3, 0xa6, 0x4e, 0x80, 4, 0x21, 60, 0x80, 0x10, 5, 0x38, 160, 0, 0, 
                 0x90, 0xa4, 0, 0x4c, 0x80, 100, 0, 80, 0xe8, 1, 0, 0x80, 0x7c, 8, 3, 0xa6, 
                 0x38, 0x21, 0, 0x70, 0x4e, 0x80, 0, 0x20
              };
            PS3.SetMemory(function_address + 4, buffer);
            PS3.SetMemory(0x10050000, new byte[0x2854]);
            PS3.SetMemory(function_address, new byte[] { 0xf8, 0x21, 0xff, 0x91 });
        }
        private void CBuf_AddText(int Client, string Command)
        {
            CallFunction(CBUF, new object[] { Client, Command });
        }
        private static byte[] ReverseBytes(byte[] buffer)
        {
            Array.Reverse(buffer);
            return buffer;
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void elegantRadioButton1_CheckedChanged(object sender)
        {
            PS3.ChangeAPI(SelectAPI.TargetManager);
        }

        private void elegantRadioButton2_CheckedChanged(object sender)
        {
            PS3.ChangeAPI(SelectAPI.ControlConsole);
        }

        private void elegantThemeButton1_Click(object sender, EventArgs e)
        {
            try
            {
                if (PS3.ConnectTarget() && PS3.AttachProcess())
                {
                    toolStripStatusLabel2.Text = "Linked !";
                    toolStripStatusLabel2.ForeColor = Color.Lime;
                    WritePPC();
                }
                else
                {
                    toolStripStatusLabel2.Text = "Not Linked !";
                    toolStripStatusLabel2.ForeColor = Color.Red;
                }
            }
            catch(Exception)
            {
                toolStripStatusLabel2.Text = "Not Linked !";
                toolStripStatusLabel2.ForeColor = Color.Red;
            }
        }

        private void elegantThemeButton2_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel5.Text = "Enable";
            toolStripStatusLabel5.ForeColor = Color.Lime;
            timer3.Start();
        }
        public static string get_player_name(uint client)
        {
            string getnames = PS3.Extension.ReadString(0xF47A1C + client * 0x3700);
            return getnames;
        }
        private void elegantThemeButton11_Click(object sender, EventArgs e)
        {
            Players.Items.Clear();
            for (uint i = 0; i < 0x12; i++)
            {
                Players.Items.Add(get_player_name(i));
            }
        }

        private void unlimitedAmmoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int pedID = (Players.SelectedIndex);
            PS3.SetMemory(0xF44EBA + ((uint)(Players.SelectedIndex * 0x3700)), new byte[] { 0xff, 0xff });
            PS3.SetMemory(0xF44ECA + ((uint)(Players.SelectedIndex * 0x3700)), new byte[] { 0xff, 0xff });
            PS3.SetMemory(0xF44EDA + ((uint)(Players.SelectedIndex * 0x3700)), new byte[] { 0xff, 0xff });
            PS3.SetMemory(0xF44EEA + ((uint)(Players.SelectedIndex * 0x3700)), new byte[] { 0xff, 0xff });
            PS3.SetMemory(0xF44EFA + ((uint)(Players.SelectedIndex * 0x3700)), new byte[] { 0xff, 0xff });
            iPrintln(Players.SelectedIndex, "Unlimited Ammo : [^2ON^7]");
        }

        private void enableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int pedID = (Players.SelectedIndex);
            PS3.SetMemory(0xE04B2A + ((uint)(Players.SelectedIndex * 640)), new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
            iPrintln(Players.SelectedIndex, "God Mode : [^2ON^7]");
        }

        private void disableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int pedID = (Players.SelectedIndex);
            PS3.SetMemory(0xE04B2A + ((uint)(Players.SelectedIndex * 640)), new byte[] { 0, 0x64, 0, 0 });
            iPrintln(Players.SelectedIndex, "God Mode : [^1OFF^7]");
        }

        private void enableToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            timer1.Start();
            iPrintln(Players.SelectedIndex, "NoClip Bind to ^5R3");
        }
        public static bool NoClip = false;
        //Buttons.ButtonPressed(0, Buttons.R3 & Buttons.L3))
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (NoClip == false)
            {
                if (Buttons.ButtonPressed(0, Buttons.R3))
                {
                    PS3.SetMemory(0xF47C9F + ((uint)(Players.SelectedIndex * 0x3700)), new byte[] { 0x02 });
                    NoClip = true;
                }
                if (Buttons.ButtonPressed(1, Buttons.R3))
                {
                    PS3.SetMemory(0xF47C9F + ((uint)(Players.SelectedIndex * 0x3700)), new byte[] { 0x02 });
                    NoClip = true;
                }
                if (Buttons.ButtonPressed(2, Buttons.R3))
                {
                    PS3.SetMemory(0xF47C9F + ((uint)(Players.SelectedIndex * 0x3700)), new byte[] { 0x02 });
                    NoClip = true;
                }
                if (Buttons.ButtonPressed(3, Buttons.R3))
                {
                    PS3.SetMemory(0xF47C9F + ((uint)(Players.SelectedIndex * 0x3700)), new byte[] { 0x02 });
                    NoClip = true;
                }
            }
            else if (NoClip == true)
            {
                if (Buttons.ButtonPressed(0, Buttons.R3))
                {
                    PS3.SetMemory(0xF47C9F + ((uint)(Players.SelectedIndex * 0x3700)), new byte[] { 0x00 });
                    NoClip = false;
                }
                if (Buttons.ButtonPressed(1, Buttons.R3))
                {
                    PS3.SetMemory(0xF47C9F + ((uint)(Players.SelectedIndex * 0x3700)), new byte[] { 0x00 });
                    NoClip = false;
                }
                if (Buttons.ButtonPressed(2, Buttons.R3))
                {
                    PS3.SetMemory(0xF47C9F + ((uint)(Players.SelectedIndex * 0x3700)), new byte[] { 0x00 });
                    NoClip = false;
                }
                if (Buttons.ButtonPressed(3, Buttons.R3))
                {
                    PS3.SetMemory(0xF47C9F + ((uint)(Players.SelectedIndex * 0x3700)), new byte[] { 0x00 });
                    NoClip = false;
                }
            }
        }

        private void disableToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            iPrintln(Players.SelectedIndex, "NoClip Bind to ^5R3 ^7is disable");
        }

        private void enableToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            int pedID = (Players.SelectedIndex);
            PS3.SetMemory(0xF44993 + ((uint)(Players.SelectedIndex * 0x3700)), new byte[] { 7 });
            iPrintln(Players.SelectedIndex, "Third Person : [^2ON^7]");
        }

        private void disableToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            int pedID = (Players.SelectedIndex);
            PS3.SetMemory(0xF44993 + ((uint)(Players.SelectedIndex * 0x3700)), new byte[2]);
            iPrintln(Players.SelectedIndex, "Third Person : [^1OFF^7]");
        }

        private void enableToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            PS3.SetMemory(0xF47C9F + ((uint)(Players.SelectedIndex * 0x3700)), new byte[] { 0x04 });
            iPrintln(Players.SelectedIndex, "Freeze : [^2ON^7]");
        }

        private void disableToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            PS3.SetMemory(0xF47C9F + ((uint)(Players.SelectedIndex * 0x3700)), new byte[] { 0x00 });
            iPrintln(Players.SelectedIndex, "Freeze : [^1OFF^7]");
        }

        private void enableToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            PS3.SetMemory(0xF4791F + ((uint)(Players.SelectedIndex * 0x3700)), new byte[] { 15 });
            iPrintln(Players.SelectedIndex, "Lag : [^2ON^7]");
        }

        private void disableToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            PS3.SetMemory(0xF4791F + ((uint)(Players.SelectedIndex * 0x3700)), new byte[] { 2 });
            iPrintln(Players.SelectedIndex, "Lag : [^1OFF^7]");
        }

        private void elegantThemeButton3_Click(object sender, EventArgs e)
        {
            float num = Convert.ToSingle(numericUpDown1.Value);
            PS3.SetMemory(0xEC708, ReverseBytes(BitConverter.GetBytes(num)));
            iPrintlnBold(-1, "Jump changed to ^2 " + numericUpDown1.Value);
        }

        private void elegantThemeButton4_Click(object sender, EventArgs e)
        {
            byte[] buffer = new byte[4];
            buffer[0] = 0x42;
            buffer[1] = 0x9c;
            PS3.SetMemory(0xEC708, buffer);
            iPrintlnBold(-1, "Jump Reset");
        }

        private void elegantThemeButton6_Click(object sender, EventArgs e)
        {
            int num = (int)numericUpDown2.Value;
            byte num2 = (byte)num;
            byte num3 = (byte)(num >> 8);
            PS3.SetMemory(0x22E01E, new byte[] { num3, num2 });
            iPrintlnBold(-1, "Speed changed to ^2 " + numericUpDown2.Value);
        }

        private void elegantThemeButton5_Click(object sender, EventArgs e)
        {
            PS3.SetMemory(0x22E01E, new byte[] { 0, 190, 0x90, 0x61 });
            iPrintlnBold(-1, "Speed Reset");
        }

        private void elegantThemeButton10_Click(object sender, EventArgs e)
        {
            float num = Convert.ToSingle(numericUpDown4.Value);
            PS3.SetMemory(0xF905C, ReverseBytes(BitConverter.GetBytes(num)));
            iPrintlnBold(-1, "FallDamage changed to ^2 " + numericUpDown4.Value);
        }

        private void elegantThemeButton8_Click(object sender, EventArgs e)
        {
           
        }

        private void elegantThemeButton9_Click(object sender, EventArgs e)
        {
            PS3.SetMemory(0xF905C, new byte[] { 0x7c, 0x86, 40, 0x38, 0x7c });
            iPrintlnBold(-1, "FallDamage Reset");
        }

        private void elegantThemeButton7_Click(object sender, EventArgs e)
        {
        }

        private void elegantThemeCheckBox1_CheckedChanged(object sender)
        {
            if (elegantThemeCheckBox1.Checked)
            {
                PS3.SetMemory(0xE04B2A, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                PS3.SetMemory(0xE04B2A + 640, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                PS3.SetMemory(0xE04B2A + 640 + 640, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                PS3.SetMemory(0xE04B2A + 640 + 640 + 640, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                iPrintlnBold(-1, "God Mode for All : [^2ON^7]");
            }
            else
            {
                PS3.SetMemory(0xE04B2A, new byte[] { 0, 0x64, 0, 0 });
                PS3.SetMemory(0xE04B2A + 640, new byte[] { 0, 0x64, 0, 0 });
                PS3.SetMemory(0xE04B2A + 640 + 640, new byte[] { 0, 0x64, 0, 0 });
                PS3.SetMemory(0xE04B2A + 640 + 640 + 640, new byte[] { 0, 0x64, 0, 0 });
                iPrintlnBold(-1, "God Mode for All : [^1OFF^7]");
            }
        }

        private void elegantThemeCheckBox2_CheckedChanged(object sender)
        {
            if (elegantThemeCheckBox2.Checked)
            {
                PS3.SetMemory(0xF44EBA, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                PS3.SetMemory(0xF44EBA + 0x3700, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                PS3.SetMemory(0xF44EBA + 0x3700 + 0x3700, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                PS3.SetMemory(0xF44EBA + 0x3700 + 0x3700 + 0x3700, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                //
                PS3.SetMemory(0xF44ECA, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                PS3.SetMemory(0xF44ECA + 0x3700, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                PS3.SetMemory(0xF44ECA + 0x3700 + 0x3700, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                PS3.SetMemory(0xF44ECA + 0x3700 + 0x3700 + 0x3700, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                //
                PS3.SetMemory(0xF44EDA, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                PS3.SetMemory(0xF44EDA + 0x3700, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                PS3.SetMemory(0xF44EDA + 0x3700 + 0x3700, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                PS3.SetMemory(0xF44EDA + 0x3700 + 0x3700 + 0x3700, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                //
                PS3.SetMemory(0xF44EEA, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                PS3.SetMemory(0xF44EEA + 0x3700, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                PS3.SetMemory(0xF44EEA + 0x3700 + 0x3700, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                PS3.SetMemory(0xF44EEA + 0x3700 + 0x3700 + 0x3700, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                //
                PS3.SetMemory(0xF44EFA, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                PS3.SetMemory(0xF44EFA + 0x3700, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                PS3.SetMemory(0xF44EFA + 0x3700 + 0x3700, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                PS3.SetMemory(0xF44EFA + 0x3700 + 0x3700 + 0x3700, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
                iPrintlnBold(-1, "Unlimited Ammo for All : [^2ON^7]");
            }
            else
            {
                PS3.SetMemory(0xF44EBA, new byte[] { 0, 0x64, 0, 0 });
                PS3.SetMemory(0xF44EBA + 0x3700, new byte[] { 0, 0x64, 0, 0});
                PS3.SetMemory(0xF44EBA + 0x3700 + 0x3700, new byte[] { 0, 0x64, 0, 0});
                PS3.SetMemory(0xF44EBA + 0x3700 + 0x3700 + 0x3700, new byte[] { 0, 0x64, 0, 0});
                //
                PS3.SetMemory(0xF44ECA, new byte[] { 0, 0x64, 0, 0});
                PS3.SetMemory(0xF44ECA + 0x3700, new byte[] { 0, 0x64, 0, 0});
                PS3.SetMemory(0xF44ECA + 0x3700 + 0x3700, new byte[] { 0, 0x64, 0, 0});
                PS3.SetMemory(0xF44ECA + 0x3700 + 0x3700 + 0x3700, new byte[] { 0, 0x64, 0, 0});
                //
                PS3.SetMemory(0xF44EDA, new byte[] { 0, 0x64, 0, 0});
                PS3.SetMemory(0xF44EDA + 0x3700, new byte[] { 0, 0x64, 0, 0});
                PS3.SetMemory(0xF44EDA + 0x3700 + 0x3700, new byte[] { 0, 0x64, 0, 0});
                PS3.SetMemory(0xF44EDA + 0x3700 + 0x3700 + 0x3700, new byte[] { 0, 0x64, 0, 0});
                //
                PS3.SetMemory(0xF44EEA, new byte[] { 0, 0x64, 0, 0});
                PS3.SetMemory(0xF44EEA + 0x3700, new byte[] { 0, 0x64, 0, 0});
                PS3.SetMemory(0xF44EEA + 0x3700 + 0x3700, new byte[] { 0, 0x64, 0, 0});
                PS3.SetMemory(0xF44EEA + 0x3700 + 0x3700 + 0x3700, new byte[] { 0, 0x64, 0, 0});
                //
                PS3.SetMemory(0xF44EFA, new byte[] { 0, 0x64, 0, 0});
                PS3.SetMemory(0xF44EFA + 0x3700, new byte[] { 0, 0x64, 0, 0});
                PS3.SetMemory(0xF44EFA + 0x3700 + 0x3700, new byte[] { 0, 0x64, 0, 0});
                PS3.SetMemory(0xF44EFA + 0x3700 + 0x3700 + 0x3700, new byte[] { 0, 0x64, 0, 0});
                iPrintlnBold(-1, "Unlimited Ammo for All : [^1OFF^7]");
            }
        }

        private void elegantThemeCheckBox3_CheckedChanged(object sender)
        {
            if (elegantThemeCheckBox3.Checked)
            {
                PS3.SetMemory(0xF44993, new byte[] { 7 });
                PS3.SetMemory(0xF44993 + 0x3700, new byte[] { 7 });
                PS3.SetMemory(0xF44993 + 0x3700 + 0x3700, new byte[] { 7 });
                PS3.SetMemory(0xF44993 + 0x3700 + 0x3700 + 0x3700, new byte[] { 7 });
                iPrintlnBold(-1, "3rd Person for All : [^2ON^7]");
            }
            else
            {
                PS3.SetMemory(0xF44993, new byte[2]);
                PS3.SetMemory(0xF44993 + 0x3700, new byte[2]);
                PS3.SetMemory(0xF44993 + 0x3700 + 0x3700, new byte[2]);
                PS3.SetMemory(0xF44993 + 0x3700 + 0x3700 + 0x3700, new byte[2]);
                iPrintlnBold(-1, "3rd Person for All : [^1OFF^7]");
            }
        }

        private void elegantThemeCheckBox4_CheckedChanged(object sender)
        {
            if (elegantThemeCheckBox4.Checked)
            {
                PS3.SetMemory(0xF47C9F, new byte[] { 0x04 });
                PS3.SetMemory(0xF47C9F + 0x3700, new byte[] { 0x04 });
                PS3.SetMemory(0xF47C9F + 0x3700 + 0x3700, new byte[] { 0x04 });
                PS3.SetMemory(0xF47C9F + 0x3700 + 0x3700 + 0x3700, new byte[] { 0x04 });
                iPrintlnBold(-1, "Freeze for All : [^2ON^7]");
            }
            else
            {
                PS3.SetMemory(0xF47C9F, new byte[] { 0x00 });
                PS3.SetMemory(0xF47C9F + 0x3700, new byte[] { 0x00 }); ;
                PS3.SetMemory(0xF47C9F + 0x3700 + 0x3700, new byte[] { 0x00 });
                PS3.SetMemory(0xF47C9F + 0x3700 + 0x3700 + 0x3700, new byte[] { 0x00 });
                iPrintlnBold(-1, "Freeze for All : [^1OFF^7]");
            }
        }

        private void elegantThemeCheckBox5_CheckedChanged(object sender)
        {
            if (elegantThemeCheckBox5.Checked)
            {
                PS3.SetMemory(0x52DDE7, new byte[] { 0x02 });
                iPrintlnBold(0, "Laser : [^2ON^7]");
            }
            else
            {
                PS3.SetMemory(0x52DDE7, new byte[] { 0x00 });
                iPrintlnBold(0, "Laser : [^1OFF^7]");
            }
        }

        private void elegantThemeCheckBox6_CheckedChanged(object sender)
        {
            if (elegantThemeCheckBox6.Checked)
            {
                PS3.SetMemory(0x478d5b, new byte[] { 2 });
                iPrintlnBold(0, "Chrome Player : [^2ON^7]");
            }
            else
            {
                PS3.SetMemory(0x478d5b, new byte[1]);
                iPrintlnBold(0, "Chrome Player : [^1OFF^7]");
            }
        }

        private void elegantThemeCheckBox7_CheckedChanged(object sender)
        {
            if (elegantThemeCheckBox7.Checked)
            {
                PS3.SetMemory(0x23453d0, new byte[] { 0x42 });
                iPrintlnBold(0, "Wallhack : [^2ON^7]");
            }
            else
            {
                PS3.SetMemory(0x23453d0, new byte[] { 0x40 });
                iPrintlnBold(0, "Wallhack : [^1OFF^7]");
            }
        }

        private void elegantThemeButton7_Click_1(object sender, EventArgs e)
        {
            iPrintlnBold(-1, "^7M^5r^7N^5i^7a^5t^7o^2's ^1Ghost Extinction Tool\n^3Visit : ^7www.^5psxhackingnews.net");
            Thread.Sleep(1000);
            iPrintlnBold(-1, "^6Facebook : ^7G^5u^7i^5l^7l^5a^7u^5m^7e ^7M^5r^7N^5i^7a^5t^7o^2");
        }

        private void elegantThemeButton8_Click_1(object sender, EventArgs e)
        {
        }

        private void elegantThemeButton8_Click_2(object sender, EventArgs e)
        {
            PS3.Extension.WriteString(0x172a5b8 + 0x4FC80, elegantThemeTextBox1.Text);
        }

        private void elegantThemeButton12_Click(object sender, EventArgs e)
        {
        }

        private void elegantThemeButton13_Click(object sender, EventArgs e)
        {
            Random randNum = new Random();
            numericUpDown3.Value = randNum.Next(0, 26);
            teeth.Value = randNum.Next(1, 100000);
            relic.Value = randNum.Next(1, 100000);
            score.Value = randNum.Next(1, 100000);
            downed.Value = randNum.Next(1, 100000);
            kill.Value = randNum.Next(1, 100000);
            cashflow.Value = randNum.Next(1, 100000);
            numericUpDown10.Value = randNum.Next(1, 100000);
            completedchallenges.Value = randNum.Next(1, 100000);
            hives.Value = randNum.Next(1, 100000);
            reviveleader.Value = randNum.Next(1, 100000);
            mission.Value = randNum.Next(1, 100000);
            revive.Value = randNum.Next(1, 100000);
        }

        private void elegantThemeButton14_Click(object sender, EventArgs e)
        {
            
        }

        private void elegantThemeButton15_Click(object sender, EventArgs e)
        {
            PS3.SetMemory(0x175589a + 0x4FC80, BitConverter.GetBytes((int)numericUpDown3.Value));
            PS3.SetMemory(0x17A54FC, BitConverter.GetBytes((int)cashflow.Value));
            PS3.SetMemory(0x17A54F1, BitConverter.GetBytes((int)completedchallenges.Value));
            PS3.SetMemory(0x17A60E6, BitConverter.GetBytes((int)teeth.Value));
            PS3.SetMemory(0x17A550A, BitConverter.GetBytes((int)revive.Value));
            PS3.SetMemory(0x17A5522, BitConverter.GetBytes((int)relic.Value));
            PS3.SetMemory(0x17A5532, BitConverter.GetBytes((int)mission.Value));
            PS3.SetMemory(0x17A5574, BitConverter.GetBytes((int)score.Value));
            PS3.SetMemory(0x17A5470, BitConverter.GetBytes((int)reviveleader.Value));
            PS3.SetMemory(0x17A54C8, BitConverter.GetBytes((int)downed.Value));
            PS3.SetMemory(0x17A54CC, BitConverter.GetBytes((int)hives.Value));
            PS3.SetMemory(0x17A54F1, BitConverter.GetBytes((int)completedchallenges.Value));
            PS3.SetMemory(0x17A558C, BitConverter.GetBytes((int)kill.Value));
        }

        private void elegantThemeCheckBox8_CheckedChanged(object sender)
        {
            PS3.SetMemory(0x1755893 + 0x4FC80, new byte[] { 0xdb, 0x1a });
        }
        public void ForceHostON()
        {
            CBuf_AddText(0, "ds_serverConnectTimeout 1000");
            CBuf_AddText(0, "ds_serverConnectTimeout 1");
            CBuf_AddText(0, "party_minplayers 1");
            CBuf_AddText(0, "party_maxplayers 16");
        }
        private void elegantThemeButton14_Click_1(object sender, EventArgs e)
        {
            timer2.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            byte[] bytes = BitConverter.GetBytes(Convert.ToInt32(numericUpDown5.Value));
            ReverseBytes(bytes);
            PS3.SetMemory(0x2350a58, bytes);
            PS3.SetMemory(0x235108b, new byte[1]);
            byte[] buffer = new byte[2];
            buffer[1] = 1;
            PS3.SetMemory(0x23501ea, buffer);
            buffer = new byte[2];
            buffer[1] = 1;
            PS3.SetMemory(0x235714a, buffer);
            buffer = new byte[2];
            buffer[1] = 1;
            PS3.SetMemory(0x2357192, buffer);
            PS3.SetMemory(0x2357222, new byte[2]);
            buffer = new byte[2];
            buffer[1] = 1;
            PS3.SetMemory(0x23515e2, buffer);
            buffer = new byte[2];
            buffer[1] = 1;
            PS3.SetMemory(0x235162a, buffer);
            PS3.SetMemory(0x23513a0, new byte[1]);
            PS3.SetMemory(0x235015b, new byte[] { 1 });
            PS3.SetMemory(0x23501a3, new byte[] { 1 });
            PS3.SetMemory(0x234fff3, new byte[1]);
            PS3.SetMemory(0x234ffab, new byte[] { 2 });
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            PS3.SetMemory(0x614EAC, new byte[] { 0x60, 0x00, 0x00, 0x00 });
            byte[] buffer = { 0x00 };
            PS3.SetMemory(0x700adf, buffer);
            PS3.SetMemory(0x6ff4db, buffer);
            PS3.SetMemory(0x6ff167, buffer);
            PS3.SetMemory(0x700ad7, buffer);
            PS3.SetMemory(0x700adb, buffer);
            PS3.SetMemory(0x6f41eb, buffer);
            PS3.SetMemory(0x700ADC, new byte[] { 0x30, 0xA5, 0x00, 0x00 });
            PS3.SetMemory(0x6FF4D8, new byte[] { 0x39, 0x00, 0x00, 0x00 });
            PS3.SetMemory(0x6FF164, new byte[] { 0x38, 0x60, 0x00, 0x00 });
            PS3.SetMemory(0x700AD4, new byte[] { 0x38, 0xC0, 0x00, 0x00 });
            PS3.SetMemory(0x700AD8, new byte[] { 0x30, 0xE7, 0x00, 0x00 });
            PS3.SetMemory(0x6F41E8, new byte[] { 0x3B, 0xA0, 0x00, 0x00 });
            timer3.Start();
        }

        private void toolStripStatusLabel6_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://paypal.me/MrNiato/5");
        }
    }
}
