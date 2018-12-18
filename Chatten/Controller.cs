using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chatten
{
    public class LoginResult:Form
    {
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

        //Linq 
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
        public Login EntityLogin(string name, string password, string mail)
        {

            Login insertLogin = new Login { Name = name, Password = Encrypt(password), Mail = mail };
            db.Logins.InsertOnSubmit(insertLogin);
            try
            {
                db.SubmitChanges();
            }
            catch (Exception)
            {
                db.SubmitChanges();
            }
            return insertLogin;
        }
        public LoginResult LoginGruppe(string Name, string password)
        {
            var logonBruger =
            (from x in db.Logins where x.Name == Name && x.Password == Encrypt(password) select x).FirstOrDefault();
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
            return r;
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
