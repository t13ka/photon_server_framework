using System;
using System.Windows.Forms;
using Warsmiths.Common.Domain.Enums;

namespace Warsmiths.WinFormsClient
{
    public partial class CreateCharacterForm : Form
    {
        public class CreateCharacterFormResult
        {
            public string Name { get; set; }
            public RaceTypes RaceType { get; set; }
            public HeroTypes HeroType { get; set; }
            public ClassTypes ClassType { get; set; }
            public StartBonusTypes StartBonusType { get; set; }
        }

        private readonly Random _random = new Random();

        public CreateCharacterFormResult CreateCharacterResult { get; set; }

        public CreateCharacterForm()
        {
            InitializeComponent();

            foreach (var value in Enum.GetValues(typeof(RaceTypes)))
            {
                comboBox_race_type.Items.Add(value);
            }

            foreach (var value in Enum.GetValues(typeof(HeroTypes)))
            {
                comboBox_hero_type.Items.Add(value);
            }

            foreach (var value in Enum.GetValues(typeof(ClassTypes)))
            {
                comboBox_class_type.Items.Add(value);
            }

            foreach (var value in Enum.GetValues(typeof(StartBonusTypes)))
            {
                comboBox_start_bonus_types.Items.Add(value);
            }

            comboBox_race_type.SelectedIndex = 0;
            comboBox_hero_type.SelectedIndex = 0;
            comboBox_class_type.SelectedIndex = 0;
            comboBox_start_bonus_types.SelectedIndex = 0;

            textBox1.Text = @"character";
        }

        private void RandomizeCharacter()
        {
            textBox1.Text = @"character_" + Guid.NewGuid();

            var raceTypesMaxValue = Enum.GetValues(typeof(RaceTypes)).Length;
            var heroTypesMaxValue = Enum.GetValues(typeof(HeroTypes)).Length;
            var classTypesMaxValue = Enum.GetValues(typeof(ClassTypes)).Length;
            var startBonusMaxValue = Enum.GetValues(typeof(StartBonusTypes)).Length;

            comboBox_race_type.SelectedIndex = _random.Next(0, raceTypesMaxValue);
            comboBox_hero_type.SelectedIndex = _random.Next(0, heroTypesMaxValue);
            comboBox_class_type.SelectedIndex = _random.Next(0, classTypesMaxValue);
            comboBox_start_bonus_types.SelectedIndex = _random.Next(0, startBonusMaxValue);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RandomizeCharacter();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var result = new CreateCharacterFormResult
            {
                Name = textBox1.Text,
                RaceType = (RaceTypes) comboBox_race_type.SelectedIndex,
                HeroType = (HeroTypes) comboBox_hero_type.SelectedIndex,
                ClassType = (ClassTypes) comboBox_class_type.SelectedIndex,
                StartBonusType = (StartBonusTypes) comboBox_start_bonus_types.SelectedIndex
            };
            CreateCharacterResult = result;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}