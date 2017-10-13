using System;
using System.Linq;
using System.Windows.Forms;
using Warsmiths.Common.Domain;
using Warsmiths.DatabaseService.Repositories;

namespace DomainConfigurationEditor
{
    public partial class Form1 : Form
    {
        readonly DomainConfigRepository _configurationRepository = new DomainConfigRepository();

        public Form1()
        {
            InitializeComponent();
            var current = _configurationRepository.SearchFor(t => t.Current).ToList().FirstOrDefault();
            propertyGrid1.SelectedObject = current;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (propertyGrid1.SelectedObject != null)
            {
                var rep = (DomainConfiguration)propertyGrid1.SelectedObject;
                _configurationRepository.Update(rep);
            }
        }
    }
}
