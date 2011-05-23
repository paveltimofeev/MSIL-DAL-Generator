using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using dalCoreSE;
using System.IO;
using dolls;

namespace DProcessorSE
{
    public partial class Form1 : Form
    {
        DFacto facto;
        string filepath;

        public Form1()
        {
            InitializeComponent();
            filepath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "mydll.dll");
            linkLabel1.Text = filepath;
        }

        private void bProcess_Click(object sender, EventArgs e)
        {
            facto = new DFacto(tbConnStr.Text);
            facto.Filter = comboBox1.Text;
            facto.StoredProcedureFilter = comboBox2.Text;
            facto.RefreshDatabaseSchema();
            facto.Save(filepath, Path.GetFileNameWithoutExtension(filepath));

            MessageBox.Show("Ready!");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                filepath = saveFileDialog1.FileName;
                linkLabel1.Text = filepath;
            }
        }

        Hashtable litems = new Hashtable();
        Hashtable ritems = new Hashtable();

        private void bRefresh_Click(object sender, EventArgs e)
        {
            facto = new DFacto(tbConnStr.Text);
            facto.Filter = comboBox1.Text;
            facto.StoredProcedureFilter = comboBox2.Text;
            facto.RefreshDatabaseSchema();

            litems.Clear();
            foreach (EntityData ed in facto.AllEntities)
            {
                litems.Add(ed.ClassName, ed);
            }

            BindLists();
        }

        private void BindLists()
        {
            lwEntities.Items.Clear();
            lwSelectedEntities.Items.Clear();

            foreach (string str in litems.Keys)
            {
                lwEntities.Items.Add(str, !((EntityData)litems[str]).IsStoredProcedure ? 0 : 1);
            }

            foreach (string str in ritems.Keys)
            {
                lwSelectedEntities.Items.Add(str, !((EntityData)ritems[str]).IsStoredProcedure ? 0 : 1);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = lwEntities.SelectedItems[0].Text;
            ritems.Add(name, (EntityData)litems[name]);
            litems.Remove(name);
            BindLists();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string name = lwSelectedEntities.SelectedItems[0].Text;
            litems.Add(name, (EntityData)ritems[name]);
            ritems.Remove(name);
            BindLists();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach(string name in litems.Keys)
            {
                ritems.Add(name, (EntityData)litems[name]);
            }
            litems.Clear();
            BindLists();
        }
    }
}
