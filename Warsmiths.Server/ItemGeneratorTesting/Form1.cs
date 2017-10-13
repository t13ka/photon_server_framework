using System;
using System.Windows.Forms;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Enums.ItemGeneration;
using Warsmiths.DatabaseService;

namespace ItemGeneratorTesting
{
    public partial class Form1 : Form
    {
        private int _index = 0;
        private readonly DomainConfiguration _domainConfig = new DomainConfiguration(true);

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var limit = numericUpDown4.Value;
            var top = limit * limit;
            if (top == 0)
            {
                top = 1;
            } else if (top == 1)
            {
                top = top + 1;
            }

            for (var i = 0; i < top; i++)
            {
                Generate();
            }
        }

       private void Generate()
        {

            var range = (int) numericUpDown4.Value;
            var mastery = numericUpDown1.Value;
            var luck = numericUpDown2.Value;
            var aid = numericUpDown3.Value;

            var argMastery = ItemGenerationMasteryTypes.Regular;
            var argLuck = ItemGenerationLuckTypes.Regular;
            var argAid = ItemGenerationMutualAidTypes.Regular;

            // mastery
            if (mastery >= 0 && mastery < 0.2m)
            {
                argMastery = ItemGenerationMasteryTypes.Regular;
            }
            if (mastery >= 0.2m && mastery < 0.3m)
            {
                argMastery = ItemGenerationMasteryTypes.Rare;
            }
            if (mastery >= 0.3m && mastery < 0.5m)
            {
                argMastery = ItemGenerationMasteryTypes.Epic;
            }
            if (mastery >= 0.5m)
            {
                argMastery = ItemGenerationMasteryTypes.Legend;
            }

            // luck
            argLuck = (ItemGenerationLuckTypes) luck;

            // aid
            if (aid >= 0 && aid < 2)
            {
                argAid = ItemGenerationMutualAidTypes.Regular;
            }
            if (aid >= 2 && aid < 4)
            {
                argAid = ItemGenerationMutualAidTypes.Rare;
            }
            if (aid >= 4 && aid < 6)
            {
                argAid = ItemGenerationMutualAidTypes.Epic;
            }
            if (aid >= 6)
            {
                argAid = ItemGenerationMutualAidTypes.Legend;
            }
            /*var item = _domainConfig.GenerateEquipment(argMastery, argLuck, argAid);

            richTextBox1.AppendText($"## step:{++_index}## " + item + Environment.NewLine);
            richTextBox1.AppendText(Environment.NewLine);*/
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _index = 0;
            richTextBox1.Clear();
        }
    }
}