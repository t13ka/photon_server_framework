using System;
using System.Linq;
using System.Windows.Forms;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.Domain.Equipment;

namespace Warsmiths.WinFormsClient
{
    public partial class CharacterEquipmentForm : Form
    {
        public CharacterEquipmentForm()
        {
            InitializeComponent();
        }

        private void UpdateInfo()
        {
            groupBox_player_inventory.Text = $@"Player inventory ({listBox1.Items.Count})";
            groupBox_character_equipment.Text = $@"Character equipment ({listBox2.Items.Count})";
        }

        public void UpdateEquipmentLists()
        {
            listBox1.Items.Clear();
            listBox2.Items.Clear();

            /*foreach (var item in OnlineForm.Player.Inventory)
            {
                listBox1.Items.Add(item.Value);
            }

            foreach (var item in OnlineForm.Character.Equipment)
            {
                listBox2.Items.Add(item);
            }*/
            UpdateInfo();
        }

        public void AddToEquipment(string eqId)
        {
            var inventoryItem = listBox1.Items[listBox1.SelectedIndex];
            listBox2.Items.Add(inventoryItem);
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);

            UpdateInfo();
        }
        public void RemoveFromEquipment(string equimentId)
        {
            var equipmentItem = listBox2.Items[listBox2.SelectedIndex];
            listBox1.Items.Add(equipmentItem);
            listBox2.Items.RemoveAt(listBox2.SelectedIndex);

            UpdateInfo();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var form = new SelectEquipmentPlaceForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                var selectedPlace = form.CurrentEquipmentPlace;
                var selectedEquipment = (BaseEquipment)listBox1.SelectedItem;
                Form1.Client.WearEquipment(selectedEquipment._id, selectedPlace);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var item = (IEntity) listBox2.SelectedItem;
           /* var equipmetnToRemove = OnlineForm.Character.Equipment.FirstOrDefault(t => t._id == item._id);
            //if (string.IsNullOrEmpty(equipmetnToRemove.Place) == false)
            {
                Form1.Client.SendUnWearEquipment(equipmetnToRemove._id, ((BaseEquipment)equipmetnToRemove).Place);
            }*/
        }

        private void CharacterEquipmentForm_Shown(object sender, EventArgs e)
        {
            UpdateEquipmentLists();

            if (string.IsNullOrEmpty(OnlineForm.Character.Name) == false)
            {

                label_selected_character.Text = $@"Character:{OnlineForm.Character.Name}";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
