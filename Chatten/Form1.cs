using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
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
        Form opener;
        Form2 f2 = new Form2();
        AppDomain SecondaryDomain;
        public string[] args;
        public string mail;
        string userName = "";
        IEnumerable<string> q;
        
        public Form1(Form ParentForm, string u)
        {
           InitializeComponent();

           VisibilityF1(false);
           textBox3.UseSystemPasswordChar = true;
           userName = u;
           opener = ParentForm;
            //Initiering af sekundær app domain
           SecondaryDomain = AppDomain.CreateDomain("Vamos");
           var otherType = typeof(Program2);
           var obj = SecondaryDomain.CreateInstanceAndUnwrap(otherType.Assembly.FullName, otherType.FullName) as Program2;
            //IEnumerable string type
           q = from x in db.Logins where x.Name == u select x.Mail;
           args = new[] { q.FirstOrDefault() };
          
           obj.Main(args, label2);
           ActiveControl = textBox1;
           label1.Text = $"Main domain is: {AppDomain.CurrentDomain.FriendlyName}";
        }

        private async void TextBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

                if (e.KeyCode == Keys.Enter)
                {
                    //chat
                    if (!checkBox1.Checked)
                    {
                        await Task.Run(() => ParentForm);
                        c.EntityctTbl(textBox1.Text, userName, args.FirstOrDefault());
                        c.TraceOut(textBox1.Text);
                        textBox1.Text = "";
                    }
                    //mail
                    else if (checkBox1.Checked && textBox2.Text != "")
                    {
                        label5.ForeColor = Color.Orange;
                        label5.Text = "Attempting to send email...";
                        label5.Visible = true;
                        await c.Mailorder(args.FirstOrDefault(), textBox3.Text, textBox2.Text, "Mail besked", textBox1.Text, label5, checkBox2.Checked);

                        if (label5.Text == "Mail send successfully")
                        {
                            linkLabel1.Visible = true;
                
                            //Venter på ParentForm er kørt færdig
                            await Task.Run(() => ParentForm);

                        c.EntityMail(args.FirstOrDefault(), args.FirstOrDefault(), textBox1.Text);
                        textBox1.Text = "";

                        }
                        
                    }
                }
        }
        //Timer opdaterer gridviews 
        private async void timer1_Tick_1(object sender, EventArgs e)
        {
            c.Grid1(dataGridView1);
            await Task.Run(() => ParentForm);
            //Når der er flueben i checkbox1, er checkbox2 synlig
            VisibilityF1(true);


            if (checkBox3.Checked)
            {
                textBox2.AppendText(args.FirstOrDefault());
                //checkbox.checked sættes til false, så email kun indsættes 1 gang
                checkBox3.Checked = false;
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //Lukker Form2, når form1 lukkes
            Application.Exit();
        }
        //Synligheds styring til form 1
        public bool VisibilityF1(bool b)
        {
            if (b)
            {
                //viser det som skal bruges til chat
                checkBox2.Visible = checkBox1.Checked;
                checkBox3.Visible = checkBox1.Checked;
                textBox2.Visible = checkBox1.Checked;
                textBox3.Visible = checkBox1.Checked;
                label3.Visible = checkBox1.Checked;
                label4.Visible = checkBox1.Checked;
            }
            else
            {
                //skjuler alt undtagen det som skal bruges til chat
                textBox2.Visible = false;
                textBox3.Visible = false;
                label3.Visible = false;
                label4.Visible = false;
                dataGridView2.Visible = false;
                label5.Visible = false;
                linkLabel1.Visible = false;
                checkBox2.Visible = false;
                checkBox3.Visible = false;
            }
            return b;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            f2.Show();
            Dispose();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {

            dataGridView2.Visible = true;
            c.Grid2(dataGridView2, args.FirstOrDefault());

           
        }
    }
}