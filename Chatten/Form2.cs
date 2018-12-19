using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
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
         
            label1.Text = Thread.CurrentThread.CurrentCulture.Name;
            //Thread.CurrentThread.CurrentUICulture = label1.Text;
            //Sætter password boxes til password karakterer
            textBox4.UseSystemPasswordChar = true;
            textBox2.UseSystemPasswordChar = true;
            Visibility(false);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            r = c.LoginGruppe(textBox1.Text, textBox2.Text);
            if (r.Mode)
            {
                Form1 f1 = new Form1(this, textBox1.Text);
                textBox1.Text = r.LoginUser;
                f1.Show();
                Hide();
               
            }
            else
            {
                label1.Text = "error";
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Visibility(true);
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            c.EntityLogin(textBox5.Text, textBox4.Text, textBox3.Text);
            label1.Text = "Bruger oprettet";
            Visibility(false);
            
        }


        private void button3_Click(object sender, EventArgs e)
        {
            UDPProcess.StartInfo.FileName = @"C:\Users\ABP\Desktop\UDPChat.appref-ms";
            UDPProcess.Start();
        }
        public bool Visibility(bool b)
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

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
                c.LangChoice(radioButton2.Text);
            label1.Text = Thread.CurrentThread.CurrentCulture.Name;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                c.LangChoice(radioButton1.Text);
            label1.Text = Thread.CurrentThread.CurrentCulture.Name;
        }
    }

    public class Program2 : MarshalByRefObject
    {
        public void Main(string[] args, Label L)
        {
            foreach (var item in args)
                L.Text = $"{item} and is executed from this domain: {AppDomain.CurrentDomain.FriendlyName}";
        }
    }
}
