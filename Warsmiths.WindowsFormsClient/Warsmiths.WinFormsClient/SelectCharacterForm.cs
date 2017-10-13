using System.Windows.Forms;
using Warsmiths.Common.Domain;

namespace Warsmiths.WinFormsClient
{
    public partial class SelectCharacterForm : Form
    {
        public string SelectedCharacterName { get; set; }

        public SelectCharacterForm()
        {
            InitializeComponent();
        }

        public void ConfigCharactersSelectionList()
        {
            listBox1.Items.Clear();
            foreach (var character in OnlineForm.Player.Characters)
            {
                listBox1.Items.Add(character.Name);
            }
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            SelectedCharacterName = (string) listBox1.SelectedItem;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
