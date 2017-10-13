using System;
using System.Windows.Forms;
using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.WinFormsClient
{
    public partial class SelectEquipmentPlaceForm : Form
    {
        public EquipmentPlaceTypes CurrentEquipmentPlace { get; set; }
        public SelectEquipmentPlaceForm()
        {
            InitializeComponent();

            var types = Enum.GetValues(typeof(EquipmentPlaceTypes));
            foreach (var type in types)
            {
                comboBox1.Items.Add(type);
            }
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            CurrentEquipmentPlace = (EquipmentPlaceTypes)comboBox1.SelectedItem;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
