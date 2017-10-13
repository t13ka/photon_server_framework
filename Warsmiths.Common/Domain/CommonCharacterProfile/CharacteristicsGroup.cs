namespace Warsmiths.Common.Domain.CommonCharacterProfile
{
    public class CharacteristicsGroup
    {
        public Characteristic Power;
        public Characteristic Speed;
        public Characteristic Intellect;
        public Characteristic Endurance;
        public Characteristic Dexterity;
        public Characteristic Wisdom;

        public CharacteristicsGroup()
        {
            Power = new Characteristic
            {
                Value = 1,
                Percent = 0,
                Strenght = 0,
                Name = "Power",
                Description =
                    "<color=#00FFFF>Классы, принадлежащие характеристике Мощи сочетаются с <b>магическим оружием и средней броней</b>.</color>\n\nСильно влияет на <b>магическую атаку</b>, средне на <b>ближнюю атаку</b> и слабо на <b>дальнюю защиту</b>.\n\n<color=#00FFFF>Увеличивает <b>Обзор</b> и <b>Адаптивность</b>, что позволяет устанавливать больше одного модуля с одинаковым эффектом в доспех.</color>\n\nНавык в Мощи увеличивает шанс снизить <b>запас жизненных сил</b> любого противника, впервые попавшего в зону обзора игрока.  <b>Слабость</b> может перейти в <b>Горение</b> и энергетические щиты противника перестанут восстанавливаться."
            };

            Speed = new Characteristic
            {
                Value = 1,
                Percent = 0,
                Strenght = 0,
                Name = "Speed",
                Description =
                    "<color=#00FFFF>Классы, принадлежащие характеристике Скорости сочетаются с оружием <b> ближнего боя</b> и <b>тяжелой броней</b>.</color>\n\nСильно влияет на <b>ближнюю атаку</b>, средне на <b>дальнюю атаку</b> и на <b>магическую атаку</b>.\n\n<color=#00FFFF>Увеличивает <b>Скорость передвижения</b> и <b>Помехи</b> от оружия, что позволяет эффективней  подавлять противника огнем, сбивать ему применение приемов и  снижать его атакующий и защитный потенциал.</color>\n\nНавык в мощи увеличивает шанс нанести <b>критический удар</b> оружием.  Критический удар также может повлечь за собой нанесение <b>раны</b> противнику, что временно лишит его возможности восстанавливать прочность своего доспеха."
            };

            Intellect = new Characteristic
            {
                Value = 1,
                Percent = 0,
                Strenght = 0,
                Name = "Intelect",
                Description =
                    "<color=#00FFFF>Классы, принадлежащие характеристике Интеллекта сочетаются с <b>оружием дальнего боя и легкой броней</b>.</color>\n\nСильно влияет на <b>дальнюю атаку</b>, слабо на <b>ближнюю атаку</b> и на <b>магическую защиту</b>.\n\n<color=#00FFFF>Увеличивает <b>Инициативу</b> и <b>Управление</b>, что позволяет использовать <b>апгрейды</b> для доспеха которые обеспечивают новые тактические и аналитические возможности.</color>\n\nНавык в интеллекте увеличивает шанс того, что наложенный вами баф будет длиться дольше.  Такой <b>расширенный баф</b> может повлечь за собой <b>дезориентацию</b> противника, что устранит какое-либо его преимущество в атаке или защите перед вами."
            };

            Endurance = new Characteristic
            {
                Value = 1,
                Percent = 0,
                Strenght = 0,
                Name = "Endurance",
                Description =
                    "<color=#00FFFF>Классы, принадлежащие характеристике телосложения сочетаются с оружием <b>дальнего боя</b> и <b>легкой броней</b>.</color>\n\nСильно влияет на <b>дальнюю защиту</b>, слабо на <b>ближнюю защиту</b> и на <b>магическую атаку</b>.\n\n<color=#00FFFF>Увеличивает <b>Заряд</b> и <b>Выносливость</b>, что позволяет длительное время выживать во враждебной среде и без отдыха сражаться на не захваченной территории.</color>\n\nНавык в телосложении увеличивает шанс того, что активируемый вами прием обойдется в двое дешевле по затратам энергии.  <b>Прием со скидкой</b> по энергии может лишиться своей уязвимости и стать <b>невосприимчивым к контрприемам</b>."
            };

            Dexterity = new Characteristic
            {
                Value = 1,
                Percent = 0,
                Strenght = 0,
                Name = "Dexterity",
                Description =
                    "<color=#00FFFF>Классы, принадлежащие характеристике ловкости сочетаются с оружием <b>ближнего боя</b> и <b>тяжелой броней</b>.</color>\n\nСильно влияет на <b>ближнюю защиту</b>, средне на <b>дальнюю защиту</b> и на <b>магическую защиту</b>.\n\n<color=#00FFFF>Увеличивает <b>Реакцию</b> и <b>Сопротивление</b>, что позволяет эффективней противостоять подавлению огнем, сбиву применяемых приемов и снижению своего атакующего и защитного потенциала.</color>\n\nНавык в Ловкости увеличивает шанс <b>реверсала</b>, что усилит ваше восстановление доспеха в успешной защиты против атак противника. Реверсал в свою очередь может также повлечь за собой состояние <b>сверхмобильности</b> с молниеносным реагирований на боевые события."
            };

            Wisdom = new Characteristic
            {
                Value = 1,
                Percent = 0,
                Strenght = 0,
                Name = "Wisdom",
                Description =
                    "<color=#00FFFF>Классы, принадлежащие характеристике Мудрости сочетаются с <b>магическим оружием</b> и <b>средней броней</b>.</color>\n\nСильно влияет на <b>магическую защиту</b>, средне на <b>ближнюю защиту</b> и слабо на <b>дальнюю атаку</b>.\n\n<color=#00FFFF>Увеличивает <b>Живучесть</b> и <b>Волю</b>, что позволяет противостоять аномальности вашего снаряжения, подавляя его собственную волю и вынуждая служить вам и защищать вас.</color>\n\nНавык в Мудрости увеличивает шанс пополнить ряды павших наемников быстрее во время сражения. <b>Пополнение</b> может повлечь за собой особое состояние <b>компромисса</b>, в котором угол атаки вашего доспеха станет на одну ступень выше."
            };
        }

        private static int GetValueByPercent(int skillPercent)
        {
            if (skillPercent <= 6)
            {
                return 1;
            }
            if (skillPercent > 6 && skillPercent <= 12)
            {
                return 2;
            }
            if (skillPercent > 12 && skillPercent <= 18)
            {
                return 3;
            }
            if (skillPercent > 18 && skillPercent <= 27)
            {
                return 4;
            }
            if (skillPercent > 27 && skillPercent <= 36)
            {
                return 5;
            }
            if (skillPercent > 36 && skillPercent <= 45)
            {
                return 6;
            }
            return 7;
        }

        public void Calculate(Character character)
        {
            Endurance.Value = GetValueByPercent(Endurance.Percent);// * character.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.EnduranceValue));
            Dexterity.Value = GetValueByPercent(Dexterity.Percent);// * character.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.DexteretyValue));
            Intellect.Value = GetValueByPercent(Intellect.Percent);// * character.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.IntellectValue));
            Power.Value = GetValueByPercent(Power.Percent);// * character.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.PowerValue));
            Speed.Value = GetValueByPercent(Speed.Percent);// * character.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.SpeedValue));
            Wisdom.Value = GetValueByPercent(Wisdom.Percent);// * character.CommonProfile.GetModuleMultiplier(ModulesTypesOfImpacts.WisdomValue));
        }
    }
}
