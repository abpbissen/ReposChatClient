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
    public partial class Form1 : Form
    {
        //Objekter
        LinqMailDataContext db = new LinqMailDataContext();
        Controller c = new Controller();
        LoginResult r = new LoginResult();
        AppDomain SecondaryDomain;
        public string[] args;
        public string mail;
        Form opener;
        Form2 f2 = new Form2();
        string userName = "";
        public Form1(Form ParentForm, string k)
        {
            
            InitializeComponent();
            userName = k;
            opener = ParentForm;
            SecondaryDomain = AppDomain.CreateDomain("Mail.exe");
            var otherType = typeof(OtherProgram);
            var obj = SecondaryDomain.CreateInstanceAndUnwrap(
                                     otherType.Assembly.FullName,
                                     otherType.FullName) as OtherProgram;

            foreach (var n in from x in db.Logins where x.Name == k select x.Mail)
            args = new[] { "mail is: " + n };

            obj.Main(args, label2);


            ActiveControl = textBox1;
           label1.Text = $"Main domain is: {AppDomain.CurrentDomain.FriendlyName}";
        }
        private async void TextBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (textBox1.Text == "swipe /all")
                {
                    await Task.Run(() => c.EntityDelete());
                    dataGridView1.DataSource = "";
                }
                else
                {
                    mail = "abpbissen@gmail.com";
                    await Task.Run(() => c.EntityctTbl(textBox1.Text, userName, mail));
                    textBox1.Text = "";
                }
            }
        }
        private async void timer1_Tick_1(object sender, EventArgs e)
        {
            await Task.Run(() => ParentForm);
            dataGridView1.DataSource = from x in db.ctTbls orderby x.ChatNr descending select new { x.ChatNr, x.Navn, x.Besked };
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Lukker Form2, når form1 lukkes
            Application.Exit();
        }
    }

}