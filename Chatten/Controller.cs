using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Configuration;
using System.Reflection;

namespace Chatten
{
    public class LoginResult:Form
    {
        //Auto properties
        public bool Mode { get; set; }
        public string LoginUser { get; set; }
        public int LoginId { get; set; }
    }
    public class Controller
    {
       public LinqMailDataContext db = new LinqMailDataContext();
        LoginResult r = new LoginResult();
        //Krypteret kode og mail
        public string strPassword = "";
        public string strMail = "";
        // Matematisk udregning af kryptering
        public string strPermutation = "";
        public const int bytePermutation1 = 0x19;
        public const int bytePermutation2 = 0x59;
        public const int bytePermutation3 = 0x17;
        public const int bytePermutation4 = 0x41;

        public async Task Mailorder(string from, string pass, string to, string subject, string mailbody, Label errLbl, bool cb)
        {
            //Mail server
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com", 587);
            SmtpServer.EnableSsl = true;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new NetworkCredential(from, pass);
            try
            {
                using (SmtpServer)
                {
                    await SmtpServer.SendMailAsync(from, to, subject, mailbody);
                }
                if (cb)
                {
                    //Bruges til at finde xml fil, uden brug af systemets stifinder(C:\)
                    string MailFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MailFile.xml");
                    string MailFilePathCrypt = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MailFileCrypt.xml");

                    //Linq to xml
                    XElement newElement = new XElement("Message",
                    new XElement("Mail_Body", mailbody));
                    IEnumerable<XElement> LinqMail = from x in newElement.Descendants() select x;
                    newElement.Save(MailFilePath);

                    strMail = Encrypt(mailbody);

                    XElement newElementCrypt = new XElement("Message", new XElement("Mail_Body", strMail));
                    newElementCrypt.Save(MailFilePathCrypt);
                }
                errLbl.ForeColor = Color.Green;
                errLbl.Text = "Mail send successfully";
            }
            catch (SmtpException)
            {
                errLbl.ForeColor = Color.Red;
                errLbl.Text = "Incorrect Password, or mail!";
            }
        }

        public DataGridView Grid1(DataGridView gv1)
        {
            //Grid performance(anti flicker)
            Type dgvType = gv1.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(gv1, true);
            

            //Binding til grids
            gv1.DataSource = from x in db.ctTbls orderby x.ChatNr descending select new { x.ChatNr, x.Navn, x.Besked };
            return gv1;
        }
        public DataGridView Grid2(DataGridView gv2, string fra)
        {
            Type dgvType2 = gv2.GetType();
            PropertyInfo pi2 = dgvType2.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi2.SetValue(gv2, true);
            gv2.DataSource = (from x in db.MailBeskeds where x.Fra != fra  orderby x.Id descending select new { x.Id, x.Fra, x.Besked }).Take(13);
            return gv2;

        }
        //Linq 
        public void EntityDelete()
        {
            var delTable =
            from x in db.ctTbls
            where x.ChatNr >= 1
            select x;
            foreach (var d in delTable)
            {
                db.ctTbls.DeleteOnSubmit(d);
            }
            try
            {
                db.SubmitChanges();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }
        }
        public ctTbl EntityctTbl(string besked, string navn, string email)
        {
            //Table<TEntity> oprettes
            ctTbl insertTbl = new ctTbl { Besked = besked, Navn = navn, Email = email };
            //Tilføj den nye TEntity til ctTbls kollektionen
            db.ctTbls.InsertOnSubmit(insertTbl);
            //Prøver at tilføje ændringer
            try
            {
                db.SubmitChanges();
            }
            catch (Exception)
            {
                //prøv igen
                db.SubmitChanges();
            }
            return insertTbl;
        }
        public Login EntityLogin(string name, string password, string mail, Label errLbl)
        {

            Login insertLogin = new Login { Name = name, Password = Encrypt(password), Mail = mail };
            db.Logins.InsertOnSubmit(insertLogin);
            try
            {
                db.SubmitChanges();
                errLbl.ForeColor = Color.Green;
                errLbl.Text = "User created";
            }
            catch (SqlException)
            {
                errLbl.ForeColor = Color.Red;
                errLbl.Text = "Failed to create user!";
            }
            return insertLogin;
        }
        public MailBesked EntityMail(string fra, string brugermail, string besked)
        {
            MailBesked insertMail = new MailBesked { Fra = fra, Besked = besked, BrugerMail = brugermail };
            //Tilføj den nye TEntity til ctTbls kollektionen
            db.MailBeskeds.InsertOnSubmit(insertMail);
            //Prøver at tilføje ændringer
            try
            {
                db.SubmitChanges();
            }
            catch (Exception)
            {
                //prøv igen
                db.SubmitChanges();
            }
            return insertMail;
        }
        public LoginResult LoginGruppe(string Name, string password, Label errLbl)
        {
            var logonBruger = (from x in db.Logins where x.Name == Name && x.Password == Encrypt(password) select x).FirstOrDefault();
            try
            {
                if (logonBruger != null && logonBruger.Name != "")
                {
                    r.Mode = true;
                    r.LoginUser = logonBruger.Name;
                    r.LoginId = logonBruger.Id;
                }
                else
                {
                    r.Mode = false;
                    r.LoginUser = "";
                    r.LoginId = 0;
                }
            }
            catch (SqlException)
            {
                errLbl.Text = "Login error";
            }
            return r;
        }

        //string metode til sprogvalg
        public string LangChoice(string sprog)
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(ConfigurationManager.AppSettings[sprog]);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(ConfigurationManager.AppSettings[sprog]);
            return sprog;
        }
        public string TraceOut(string outStr)
        {
            Trace.Listeners.Add(new TextWriterTraceListener("TextWriterOutput.log", "myListener"));
            Trace.TraceInformation(outStr);
            Trace.Flush();
            
            return outStr;
        }
        //Kryptering
        public string Encrypt(string strData)
        {
            return Convert.ToBase64String(ByteEncrypt(Encoding.UTF8.GetBytes(strData)));
        }

        public string Decrypt(string strData)
        {
            return Encoding.UTF8.GetString(ByteDecrypt(Convert.FromBase64String(strData)));
        }

        public byte[] ByteEncrypt(byte[] strData)
        {
            PasswordDeriveBytes passbytes =
            new PasswordDeriveBytes(strPermutation,
            new byte[] { bytePermutation1,
                         bytePermutation2,
                         bytePermutation3,
                         bytePermutation4
            });

            MemoryStream memstream = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = passbytes.GetBytes(aes.KeySize / 8);
            aes.IV = passbytes.GetBytes(aes.BlockSize / 8);
    

            CryptoStream cryptostream = new CryptoStream(memstream,
            aes.CreateEncryptor(), CryptoStreamMode.Write);
            cryptostream.Write(strData, 0, strData.Length);
            cryptostream.Close();
            return memstream.ToArray();
        }

        public byte[] ByteDecrypt(byte[] strData)
        {
            PasswordDeriveBytes passbytes =
            new PasswordDeriveBytes(strPermutation,
            new byte[] { bytePermutation1,
                         bytePermutation2,
                         bytePermutation3,
                         bytePermutation4
            });

            MemoryStream memstream = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = passbytes.GetBytes(aes.KeySize / 8);
            aes.IV = passbytes.GetBytes(aes.BlockSize / 8);

            CryptoStream cryptostream = new CryptoStream(memstream,
            aes.CreateDecryptor(), CryptoStreamMode.Write);
            cryptostream.Write(strData, 0, strData.Length);
            cryptostream.Close();
            return memstream.ToArray();
        }

    }
}
