using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Chatten
{
    public partial class Form2 : Form
    {
        //Objekter
        LinqMailDataContext db = new LinqMailDataContext();
        Controller c = new Controller();
        LoginResult r = new LoginResult();
        Process UDPProcess = new Process();
        public Form2()
        {
            InitializeComponent();
            ActiveControl = textBox1;
            //Sætter password boxes til password karakterer
            textBox4.UseSystemPasswordChar = true;
            textBox2.UseSystemPasswordChar = true;
            VisibilityF2(false);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            r = c.LoginGruppe(textBox1.Text, textBox2.Text, label4);
            if (r.Mode)
            {
                Form1 f1 = new Form1(this, textBox1.Text);
                textBox1.Text = r.LoginUser;
                f1.Show();
                Hide();
            }
            else
            {
                label4.Text = "Login failed!";
            }
        }
        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisibilityF2(true);
            label4.Text = "";
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (label4.Text != "Failed to create user!")
            {
                c.EntityLogin(textBox5.Text, textBox4.Text, textBox3.Text, label4);
                VisibilityF2(true);
            }
            else
            {
                c.EntityLogin("", "", "", label4);
                VisibilityF2(false);
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            UDPProcess.StartInfo.FileName = Path.Combine(path, "UDPChat.appref-ms");
            UDPProcess.Start();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
                c.LangChoice(radioButton2.Text);
            label3.Text = $"Current language: {Thread.CurrentThread.CurrentCulture.Name}";
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                c.LangChoice(radioButton1.Text);
            label3.Text = $"Current language: {Thread.CurrentThread.CurrentCulture.Name}";
        }
        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }
        //Synligheds styring til form 2
        public bool VisibilityF2(bool b)
        {
            if (b)
            {
                textBox5.Visible = true;
                textBox4.Visible = true;
                textBox3.Visible = true;
                button2.Visible = true;
                label2.Visible = true;
            }
            else
            {
                textBox5.Visible = false;
                textBox4.Visible = false;
                textBox3.Visible = false;
                button2.Visible = false;
                label2.Visible = false;
            }
            return b;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
    public class Program2 : MarshalByRefObject
    {
        public void Main(string[] args, Label L)
        {
            foreach (var item in args)
                L.Text = $"Your mail is: {item} and is found from this domain: {AppDomain.CurrentDomain.FriendlyName}";
        }
    }
}
