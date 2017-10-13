using System;
using System.Windows.Forms;
using Warsmiths.Common.Domain;

namespace Warsmiths.WinFormsClient
{
    public partial class UnpublishLotForm : Form
    {
        public Lot SelectedItem { get; set; }

        public UnpublishLotForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SelectedItem = (Lot)listBox1.SelectedItem;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
