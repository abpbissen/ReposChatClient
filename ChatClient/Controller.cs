using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace ChatClient
{
    internal class Controller
    {
        LinqMailDataContext db = new LinqMailDataContext();

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
        public bool SqlGV(DataGridView gv)
        {
            gv.DataSource = (from chat in db.ctTbls orderby chat.ChatNr descending select chat).Take(10);
            return true;
        }
        public void LinqDelete()
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
        
    }
}
