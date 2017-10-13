using System;
using System.Windows.Forms;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Equipment;

namespace Warsmiths.WinFormsClient
{
    public partial class PublishLotForm : Form
    {
        private Player _player;
        public BaseEquipment SelectedItem { get; set; }

        public PublishLotForm()
        {
            InitializeComponent();
        }

        public PublishLotForm(Player player)
        {
            _player = player;
            
            InitializeComponent();

            foreach (var item in _player.Inventory)
            {
                listBox1.Items.Add(item.Value);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            SelectedItem = (BaseEquipment)listBox1.SelectedItem;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
