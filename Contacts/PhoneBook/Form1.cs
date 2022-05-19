using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhoneBook
{
    public partial class Form1 : System.Windows.Forms.Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            try
            {
                panel1.Enabled = true;
                //Add a new row
                App.PhoneBook.AddPhoneBookRow(App.PhoneBook.NewPhoneBookRow());
                phoneBookBindingSource.MoveLast();
                txtPhoneNumber.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                App.PhoneBook.RejectChanges();
            }
        }

        private void btnRemove_Click_1(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure want to delete this record?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                phoneBookBindingSource.RemoveCurrent();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            panel1.Enabled = true;
            txtPhoneNumber.Focus();
        }

        private void dataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (MessageBox.Show("Are you sure want to delete this record?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    phoneBookBindingSource.RemoveCurrent();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                //End edit, save dataset to file
                phoneBookBindingSource.EndEdit();
                App.PhoneBook.AcceptChanges();
                App.PhoneBook.WriteXml(string.Format("{0}//data.dat", Application.StartupPath));
                panel1.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                App.PhoneBook.RejectChanges();
            }
        }

        static AppData db;
        protected static AppData App
        {
            get
            {
                if (db == null)
                    db = new AppData();
                return db;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Read file, then load data to dataset
            string fileName = string.Format("{0}//data.dat", Application.StartupPath);
            if (File.Exists(fileName))
                App.PhoneBook.ReadXml(fileName);
            phoneBookBindingSource.DataSource = App.PhoneBook;
            panel1.Enabled = false;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                //you can use linq to query data
                var query = from o in App.PhoneBook
                            where o.PhoneNumber.ToLowerInvariant().Contains(txtSearch.Text.ToLowerInvariant()) || o.FullName.ToLowerInvariant().Contains(txtSearch.Text.ToLowerInvariant()) || o.Email.ToLowerInvariant().Contains(txtSearch.Text.ToLowerInvariant())
                            select o;
                dataGridView.DataSource = query.ToList();
            }
            else
                dataGridView.DataSource = phoneBookBindingSource;
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    //you can use linq to query data
                    var query = from o in App.PhoneBook
                                where o.PhoneNumber.ToLowerInvariant().Contains(txtSearch.Text.ToLowerInvariant()) 
                                || o.FullName.ToLowerInvariant().Contains(txtSearch.Text.ToLowerInvariant()) 
                                || o.Email.ToLowerInvariant().Contains(txtSearch.Text.ToLowerInvariant()) 
                                || o.Birthday.ToLowerInvariant().Contains(txtSearch.Text.ToLowerInvariant())
                                select o;
                    dataGridView.DataSource = query.ToList();
                }
                else
                    dataGridView.DataSource = phoneBookBindingSource;
            }
        }
    }
}
