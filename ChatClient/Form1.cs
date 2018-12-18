using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace ChatClient
{
  
    public partial class Form1 : Form
    {
        Controller c = new Controller();
        public Form1()
        {
            InitializeComponent();
            dataGridView1.Dock = DockStyle.None;
            dataGridView1.DataSource = bindingSource1;
        }
        private async void Timer1_Tick(object sender, EventArgs e)
        {
            await Task.Run(() => c.SqlGV(dataGridView1));
            //await c.SqlGV(c.cnStr, c.ctStr, dataGridView1, bindingSource1);
        }
        private async void TextBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
                if (e.KeyCode == Keys.Enter)
                {
                    if (textBox1.Text == "swipe /all")
                    {
                        await Task.Run(() => c.LinqDelete());
                        dataGridView1.DataSource = "";
                    }
                    else
                    {

                    await Task.Run(() => c.EntityctTbl(textBox1.Text, "Andreas", "abpbissen@gmail.com"));
                    }
                }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            label2.Text = CultureInfo.CurrentUICulture.ToString();
        }

    }
}
