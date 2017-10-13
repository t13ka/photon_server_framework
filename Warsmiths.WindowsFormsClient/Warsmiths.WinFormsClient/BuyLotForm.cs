using System;
using System.Windows.Forms;
using Warsmiths.Common.Domain;

namespace Warsmiths.WinFormsClient
{
    public partial class BuyLotForm : Form
    {
        public Lot SelectedToBuy { get; set; }

        public BuyLotForm()
        {
            InitializeComponent();
        }

        private void BuyLotForm_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem == null)
            {
                MessageBox.Show("Please, Select the lot first!", "Warning", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                return;
            }
            SelectedToBuy = (Lot) listBox1.SelectedItem;
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