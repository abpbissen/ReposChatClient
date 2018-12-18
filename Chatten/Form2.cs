using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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

        public Form2()
        {
            InitializeComponent();
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

        public bool Visibility(bool b)
        {
            if(b)
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
