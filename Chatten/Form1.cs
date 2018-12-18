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
        IEnumerable<string> q;

        public Form1(Form ParentForm, string u)
        {
           InitializeComponent();
           textBox3.UseSystemPasswordChar = true;
           dataGridView2.Visible = false;
           label5.Visible = false;
           checkBox2.Visible = false;
           checkBox3.Visible = false;
           userName = u;
           opener = ParentForm;
            //Initiering af sekundær app domain
           SecondaryDomain = AppDomain.CreateDomain("Mail.exe");
           var otherType = typeof(Program2);
           var obj = SecondaryDomain.CreateInstanceAndUnwrap(otherType.Assembly.FullName, otherType.FullName) as Program2;
            //IEnumerable string type
           q = from x in db.Logins where x.Name == u select x.Mail;
           args = new[] { "Mail is: " + q.FirstOrDefault() };

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
                    if (!checkBox1.Checked)
                    {
                        await Task.Run(() => ParentForm);
                        c.EntityctTbl(textBox1.Text, userName, q.FirstOrDefault());
                        textBox1.Text = "";
                    }
                    else
                    {
                        await c.Mailorder(q.FirstOrDefault(), textBox3.Text, textBox2.Text, "Mail besked", textBox1.Text, label5, checkBox2.Checked);
                        label5.ForeColor = Color.Green;
                        label5.Text = "Mail send successfully";
                        label5.Visible = true;
                        //Venter på ParentForm er kørt færdig
                        await Task.Run(() => ParentForm);
                        c.EntityMail(textBox2.Text, q.FirstOrDefault(), textBox1.Text);
                        textBox1.Text = "";
                        dataGridView2.Visible = true;
                    }
                }
            }
        }

        private async void timer1_Tick_1(object sender, EventArgs e)
        {
            dataGridView1.DataSource = from x in db.ctTbls orderby x.ChatNr descending select new { x.ChatNr, x.Navn, x.Besked };
            dataGridView2.DataSource = (from x in db.MailBeskeds orderby x.Id descending select new { x.Id, x.Fra, x.Besked }).Take(10);
            await Task.Run(() => ParentForm);
            //Når der er flueben i checkbox1, er checkbox2 synlig
            checkBox2.Visible = checkBox1.Checked;
            checkBox3.Visible = checkBox1.Checked;
            if (checkBox3.Checked)
            {
                textBox2.AppendText("abpbissen@gmail.com");
                checkBox3.Checked = false;
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Lukker Form2, når form1 lukkes
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

}