using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ExitGames.Client.Photon;
using Warsmiths.Common.Domain;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.Domain.Equipment.Armors;
using Warsmiths.Common.ListContainer;

namespace Warsmiths.WinFormsClient
{
    public partial class OnlineForm : Form
    {
        public bool Logged { get; set; }
        public static Player Player { get; set; }
        public static Character Character { get; set; }

        private readonly BuyLotForm _buyLotForm = new BuyLotForm();
        private readonly UnpublishLotForm _unpublishLotForm = new UnpublishLotForm();
        private readonly CreateCharacterForm _createCharacterForm = new CreateCharacterForm();
        private readonly SelectCharacterForm _selectCharacterForm = new SelectCharacterForm();
        private readonly CharacterEquipmentForm _characterEquipmentForm = new CharacterEquipmentForm();
        private readonly DomainConfiguration _domainConfiguration = new DomainConfiguration(true);

        public LotsListContainer LotsListContainer { get; set; }

        private delegate void LoginTooglingPanelDelegate(bool showLogInPanel);

        private delegate void CharacterEquipmentUpdateDelegate(string equimentId);

        private delegate void LotsListContainerDelegate(LotsListContainer lotsContainer);

        private delegate void DebugReturnUpdateDelegate(DebugLevel level, string debugMessage);

        private delegate void CharacterSelectedDelegate(string characterName);

        private delegate void ElementsPricesContainerDelegate(ElementsPricesContainer container);


        public OnlineForm()
        {
            InitializeComponent();
        }

        public void SubscribeEvents()
        {
            // events subscribe
            var c = Form1.Client;
            c.OnDebugReturn =
                (level, s) =>
                {
                    //Invoke(new DebugReturnUpdateDelegate(OnDebugReturnEvent), level, s);
                };

            c.OnLogged = () => { Invoke(new LoginTooglingPanelDelegate(OnLogIn), false); };

            c.OnLogOut = () => { Invoke(new LoginTooglingPanelDelegate(OnLogOut), false); };

            c.OnPlayerProfileEvent = profile =>
            {
                Player = profile;
                Invoke(new LoginTooglingPanelDelegate(OnGetPlayerProfile), false);
            };

            c.OnUpdateAuctionEvent = OnUpdateAuctionEvent;
            c.OnUpdateAuctionEvent =
                container => { Invoke(new LotsListContainerDelegate(OnUpdateAuctionEvent), container); };
            c.OnCharacterSelected = s => { Invoke(new CharacterSelectedDelegate(OnCharacterSelected), s); };

            c.OnEquipmentWearSuccess =
                (equipmentId) =>
                {
                    Invoke(new CharacterEquipmentUpdateDelegate(OnPutOnCharacterEquipment), equipmentId);
                };

            c.OnEquipmentUnWearSuccess =
                equipmentId =>
                {
                    Invoke(new CharacterEquipmentUpdateDelegate(OnPutOffCharacterEquipment), equipmentId);
                };

            c.OnUpdateElementPricesEvent =
                data => { Invoke(new ElementsPricesContainerDelegate(OnUpdateElementsPrices), data); };
        }

        private void OnUpdateElementsPrices(ElementsPricesContainer container)
        {
            propertyGrid2.SelectedObject = container;
        }

        private void OnCharacterSelected(string characterName)
        {
            var character = Player.Characters.FirstOrDefault(t => t.Name == characterName);
            if (character != null)
            {
                Character = character;
            }

            if (Character != null)
            {
                UpdateTitleInfo(true);
            }
        }

        private void UpdateTitleInfo(bool payloaded)
        {
            Text = @"Online form";

            if (payloaded)
            {
                if (Player != null && Character != null)
                {
                    Text = $@"Log in as:{Player.FirstName}, Selected character:{Character.Name}";
                }

                if (Player != null && Character == null)
                {
                    Text = $@"Log in as:{Player.FirstName}";
                }
            }
            else
            {
                Text = @"Online form";
            }
        }

        private void OnDebugReturnEvent(DebugLevel level, string debugmessage)
        {
            richTextBox_sever_debug.AppendText(
                $"dt:{DateTime.Now}; l:{level}; message:{debugmessage};{Environment.NewLine}");
        }

        #region On events hendlers

        private void OnPutOnCharacterEquipment(string equimentId)
        {
            _characterEquipmentForm.AddToEquipment(equimentId);
        }

        private void OnPutOffCharacterEquipment(string equimentId)
        {
            _characterEquipmentForm.RemoveFromEquipment(equimentId);
        }

        private void OnUpdateAuctionEvent(LotsListContainer lotsListContainer)
        {
            LotsListContainer = lotsListContainer;

            _buyLotForm.listBox1.Items.Clear();
            foreach (var lot in LotsListContainer.Lots)
            {
                _buyLotForm.listBox1.Items.Add(lot);
            }

            _unpublishLotForm.listBox1.Items.Clear();
            foreach (var lot in LotsListContainer.Lots)
            {
                _unpublishLotForm.listBox1.Items.Add(lot);
            }
        }

        private void OnLogIn(bool show)
        {
            button_logIn.Visible = false;
            button_logOut.Visible = true;
            Logged = true;

            textBox1_login_nickname.ReadOnly = true;
            textBox_login_password.ReadOnly = true;

            Form1.Client.SendGetProfile();
        }

        private void OnLogOut(bool show)
        {
            button_logIn.Visible = true;
            button_logOut.Visible = false;
            Logged = false;
            textBox1_login_nickname.ReadOnly = false;
            textBox_login_password.ReadOnly = false;
            propertyGrid1.SelectedObject = null;
            UpdateTitleInfo(false);
        }

        private void OnGetPlayerProfile(bool get)
        {
            propertyGrid1.SelectedObject = Player;
            UpdateTitleInfo(true);
        }

        #endregion

        private void OnlineForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Form1.Client.Disconnect();
            Form1.Instance.Close();
        }

        private void button_perform_registration_Click(object sender, System.EventArgs e)
        {
            Form1.Client.SendRegistration(textBox_firstname.Text, textBox_lastname.Text, textBox_login.Text,
                textBox_password.Text, textBox_email.Text);
        }

        private async void button1_Click(object sender, System.EventArgs e)
        {
            Form1.Client.SendLogin(textBox1_login_nickname.Text, textBox_login_password.Text);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            Form1.Client.SendLogout();
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            if (_createCharacterForm.ShowDialog() == DialogResult.OK)
            {
                var r = _createCharacterForm.CreateCharacterResult;
                Form1.Client.SendCreateCharacter(r.Name, r.RaceType, r.HeroType, r.ClassType, r.StartBonusType);

                Form1.Client.SendGetProfile();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Player == null)
            {
                MessageBox.Show("Get player profile first!", "Warning", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
            else
            {
                var form = new PublishLotForm(Player);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var item = form.SelectedItem;
                    Form1.Client.SendPublishLot(item, 1000);
                    Form1.Client.SendGetProfile();
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form1.Client.SendGetAuction();
            if (_unpublishLotForm.ShowDialog() == DialogResult.OK)
            {
                var item = _unpublishLotForm.SelectedItem;
                Form1.Client.SendUnpublishLot(item);
                Form1.Client.SendGetProfile();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form1.Client.SendGetAuction();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            //Form1.Client.SendBuyElement(_domainConfiguration.Get<RedCrystal>()._id, 4);
            //Form1.Client.SendBuyElement(_domainConfiguration.Get<BlueCrystal>()._id, 3);
            //Form1.Client.SendBuyElement(ElementsUtils.Get<YellowCrystal>()._id, 5);
            //Form1.Client.SendBuyElement(_domainConfiguration_domainConfiguration.Get<BlueCrystalMetal>()._id, 6);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            //Form1.Client.SendSellElement(_domainConfiguration.Get<RedCrystal>()._id, 1);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            Form1.Client.SendGetElementsOrder();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Form1.Client.SendGetElementsPriceList();
        }

        private void button12_Click(object sender, EventArgs e)
        {
           // Form1.Client.SendEnchanceEquipment(_domainConfiguration.Get<Caretaker>()._id, 1, "element_RedCrystal");
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Form1.Client.SendGetAuction();

            if (_buyLotForm.ShowDialog() == DialogResult.OK)
            {
                var lot = _buyLotForm.SelectedToBuy;
                Form1.Client.SendBuyLot(lot);
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            //Form1.Client.SendDestroyEquipment(_domainConfiguration.Get<Caretaker>()._id);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            var l1 = new List<string> {"test1", "test1", "test1", "test1", "test1",};
            var l2 = new List<string> {"test11", "test111", "test1", "test1", "test1",};
            var l3 = new List<string> {"test11", "test111", "test1", "test1", "test1",};
            var l4 = new List<string> {"test11", "test1111", "test1", "test1", "test1",};
            var l5 = new List<string> {"test11", "test111", "test1", "test1", "test1",};

            Form1.Client.SendSaveReservedFields(l1, l2, l3, l4, l5);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form1.Client.SendGetProfile();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Form1.Client.SendGetInventory();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox_sever_debug.Clear();
        }

        private void button7_Click_1(object sender, EventArgs e)
        {
            if (Player == null)
            {
                MessageBox.Show("Get profile first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            _selectCharacterForm.ConfigCharactersSelectionList();
            if (_selectCharacterForm.ShowDialog() == DialogResult.OK)
            {
                Form1.Client.SendSelectCharacter(_selectCharacterForm.SelectedCharacterName);
            }
        }

        private void button16_Click_1(object sender, EventArgs e)
        {
            if (Character != null)
            {
                _characterEquipmentForm.groupBox_character_equipment.Text = $@"{Character.Name} equipment";

                if (_characterEquipmentForm.ShowDialog() == DialogResult.OK)
                {
                    Form1.Client.SendGetProfile();
                }
            }
            else
            {
                MessageBox.Show("You are not selected the character!", "Warning", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            var armPart = new List<ArmorPart>
            {
                new ArmorPart(ArmorPartTypes.Chest, new List<Common.Domain.Craft.Common.Position>()) {Durability = 12},
                new ArmorPart(ArmorPartTypes.LeftLeg, new List<Common.Domain.Craft.Common.Position>()){Durability = 13},
                new ArmorPart(ArmorPartTypes.RightHand, new List<Common.Domain.Craft.Common.Position>()){Durability = 13},
                new ArmorPart(ArmorPartTypes.LeftHand, new List<Common.Domain.Craft.Common.Position>()){Durability = 13},
                new ArmorPart(ArmorPartTypes.Back, new List<Common.Domain.Craft.Common.Position>()){Durability = 13},
                new ArmorPart(ArmorPartTypes.RightLeg, new List<Common.Domain.Craft.Common.Position>()){Durability = 12},
            };

            var reciept = new BaseReciept
            {
                Rarity = RaretyTypes.Epic,
                PartsPosition = armPart,
                ArmorType = ArmorTypes.Heavy,
                Anomality = 22,
                Blue = 1,
                Casing = 12,
                Discription = "qwe",
                Elements = new List<BaseElement>(),
                ExDisctiontion = "???",
                LatticeForm = 255,
                LevelRequire = 32,
                Price = 2323,
                Red = 23,
                Durability = 33,
                Weight = 323
            };
            Form1.Client.SendCreateArmor(reciept);
        }

        private string _recieptLastId;
        private void button18_Click(object sender, EventArgs e)
        {
            var reciept = new BaseReciept();
            _recieptLastId = reciept._id;
            Form1.Client.SendSaveReciept(reciept);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            Form1.Client.SendGetReciept(_recieptLastId);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            Form1.Client.SendInsertModule("armor_HeavyArmor", "module_CritModule", ArmorPartTypes.RightHand);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            Form1.Client.SendRemoveModule("armor_HeavyArmor", "module_CritModule");
        }

        private void button22_Click(object sender, EventArgs e)
        {
            Form1.Client.SendReplaceModule("armor_HeavyArmor", "module_CritModule", "module_CritModule");
        }

        private void button23_Click(object sender, EventArgs e)
        {
            Form1.Client.SendAddClassToCharacter(ClassTypes.Chosen);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            Form1.Client.SendRemoveClassFromCharacter(ClassTypes.Chosen);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            var abiName = "super";
            var position = 1;
            Form1.Client.SendAddAbilityToCharacter(abiName, position);
        }

        private void button26_Click(object sender, EventArgs e)
        {
            var abiName = "super";
            Form1.Client.SendRemoveAbilityFromCharacter(abiName);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            var experienceValue = 232;
            Form1.Client.SendSetCraftExperience(experienceValue);
        }

        private void button28_Click(object sender, EventArgs e)
        {
            var experienceValue = 13131;
            Form1.Client.SendSetExperience(experienceValue);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            var max = Enum.GetValues(typeof(CharacteristicE)).Length;
            var random = new Random();
            var randomCharacteristic =(CharacteristicE) random.Next(0, max);

            var randomSkillPercent = random.Next(0, 25);
            Form1.Client.SendSetExperience(randomCharacteristic, randomSkillPercent);
        }
    }
}