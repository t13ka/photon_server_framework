using System;
using System.Collections.Generic;
using System.Linq;
using Warsmiths.Common.Domain.Enums;
using Warsmiths.Common.Domain.Equipment;
using Warsmiths.Common.Domain.Equipment.Purchase;
using Warsmiths.Common.Utils;

// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable ForCanBeConvertedToForeach

// ReSharper disable SwitchStatementMissingSomeCases

namespace Warsmiths.Common.Domain
{
    public class DomainConfiguration : IEntity
    {
        #region Fields

        public static readonly Random Random = new Random((int)DateTime.Now.Ticks);

        #endregion

        #region Props

        public DateTime CreateDateTime;

        public DateTime UpdateDateTime;

        public bool Current ;

        public List<IEntity> Objects ;

        public List<BaseElement> Elements = new List<BaseElement>();
        public List<BaseReciept> Reciepts = new List<BaseReciept>();
        public List<IEntity> Items = new List<IEntity>();

        /// <summary>
        ///     максимальное количество элементов для заказа на 1 игрока в течении периода
        /// </summary>
        public short MaxElementsAmountForPlayer;

        /// <summary>
        /// Количество генерируемого элемента при заказе у системы.
        /// По умолчанию = 1. Если изменить данный элемент (например = 200), то
        /// когда игрок заказал покупку у сервера элементов, ему будет прибавляться это значение
        /// каждый раз когда происходит генерация элемента 
        /// </summary>
        public int GeneratedElementAmount ;

        /// <summary>
        /// Временые интервалы в секунда до генерации ресурса
        /// </summary>
        public int[] ElementsSpawnPoints ;

        public static float[] EquipmentLevelPercents =
        {
            7.8f,
            7.8f,
            9.3f,
            11.2f,
            13.5f,
            16.2f,
            19.4f,
            23.3f,
            27.9f,
            33.5f,
            40.2f,
            48.2f,
            57.9f,
            69.4f,
            83.3f,
            100f,
            100f, // idle
            100f,
            100f,
        };



        public static List<LevelingSystem.LevelReward> HeroLevelRewards = new List<LevelingSystem.LevelReward>
        {
            new LevelingSystem.LevelReward {Lvl = 0, Reciepts = {"Quest1_1"}, Tasks = new List<string> {"Task1"}},
        };

        public static List<LevelingSystem.LevelReward> CraftLevelRewards = new List<LevelingSystem.LevelReward>
        {

            new LevelingSystem.LevelReward
            {
                Lvl = 0,
                Reciepts = {"Quest0_1"},
            },
            new LevelingSystem.LevelReward {Lvl = 1, Reciepts = {"Quest1_1"}},
            new LevelingSystem.LevelReward {Lvl = 2, Reciepts = {"Quest2_1"}},
            new LevelingSystem.LevelReward {Lvl = 3, Reciepts = {"Quest3_1"}},
            new LevelingSystem.LevelReward {Lvl = 4, Reciepts = {"Quest4_1"}},
            new LevelingSystem.LevelReward
            {
                Lvl = 5,
                Reciepts = {"Quest5_1"},
                LevelFeatures = new List<LvlRewardFeaturesTypes>
                {
                    LvlRewardFeaturesTypes.CraftCustom,
                }
            },
            new LevelingSystem.LevelReward
            {
                Lvl = 10,
                LevelFeatures = new List<LvlRewardFeaturesTypes>
                {
                    LvlRewardFeaturesTypes.CraftMelt,
                }
            },
            new LevelingSystem.LevelReward
            {
                Lvl = 12,
                LevelFeatures = new List<LvlRewardFeaturesTypes>
                {
                    LvlRewardFeaturesTypes.CraftElementSlot5,
                }
            },
            new LevelingSystem.LevelReward
            {
                Lvl = 20,
                LevelFeatures = new List<LvlRewardFeaturesTypes>
                {
                    LvlRewardFeaturesTypes.CraftFiering
                }
            },
            new LevelingSystem.LevelReward
            {
                Lvl = 22,
                LevelFeatures = new List<LvlRewardFeaturesTypes>
                {
                    LvlRewardFeaturesTypes.CraftElementSlot5,

                }
            },
            new LevelingSystem.LevelReward
            {
                Lvl = 27,
                LevelFeatures = new List<LvlRewardFeaturesTypes>
                {
                    LvlRewardFeaturesTypes.CraftElementSlot6,

                }
            },  new LevelingSystem.LevelReward
            {
                Lvl = 22,
                LevelFeatures = new List<LvlRewardFeaturesTypes>
                {
                    LvlRewardFeaturesTypes.CraftPerkSlot5,
                }
            },
            new LevelingSystem.LevelReward
            {
                Lvl = 27,
                LevelFeatures = new List<LvlRewardFeaturesTypes>
                {
                    LvlRewardFeaturesTypes.CraftPerkSlot6,
                }
            },
            new LevelingSystem.LevelReward
            {
                Lvl = 35,
                LevelFeatures = new List<LvlRewardFeaturesTypes>
                {
                    LvlRewardFeaturesTypes.CraftPerkSlot7,
                }
            },
            new LevelingSystem.LevelReward
            {
                Lvl = 40,
                LevelFeatures = new List<LvlRewardFeaturesTypes>
                {
                    LvlRewardFeaturesTypes.CraftPerkSlot8,
                }
            }

        };

        public static int[] CharacterLevelsExperience =
        {
            1000,
            1250,
            1563,
            1953,
            2441,
            3052,
            3815,
            4768,
            5960,
            7451,
            9313,
            11642,
            14552,
            18190,
            22737
        };

        public static float[] CharacterCraftLevelsExperience => new[]
        {
            500, 8000, 10000, 14000, 19000, 27000, 35000, 44000, 54000, 65000,
            7781.227f, 8559.35f, 9415.285f, 10356.81f, 11392.5f, 13670.99f, 16405.19f, 19686.23f, 23623.48f,28348.17f,
            34017.81f, 40821.37f, 48985.64f, 58782.77f, 70539.33f, 84647.19f, 101576.6f, 121892, 146270.3f,175524.4f,
            210629.3f, 252755.2f, 303306.2f, 363967.4f, 436760.9f, 524113.1f, 628935.7f, 754722.9f, 905667.4f,1086801,
            1304161, 1564993, 1877992, 2253590, 2704308, 3245170, 3894204, 4673045, 5607654, 6729185
        };

        #endregion

        #region Ctor

        public DomainConfiguration(bool defaultConf = false)
        {
            if (defaultConf)
            {
                CreateDateTime = DateTime.UtcNow;
                UpdateDateTime = CreateDateTime;
                Current = true;
                _id = Guid.NewGuid().ToString();

                Objects = new List<IEntity>();
                Objects.AddRange(GetArmorsData());
                Objects.AddRange(GetWeaponsData());
                Objects.AddRange(GetShieldsData());
                Objects.AddRange(GetModulesData());
                Elements.AddRange(GetElementsData());
                Items.AddRange(GetItems());
                Reciepts = new List<BaseReciept>();
                var timeDivider = 60;
                ElementsSpawnPoints = new[]
                {
                    60/timeDivider,
                    117/timeDivider,
                    171/timeDivider,
                    223/timeDivider,
                    273/timeDivider,
                    320/timeDivider,
                    364/timeDivider,
                    406/timeDivider,
                    446/timeDivider,
                    482/timeDivider,
                    517/timeDivider,
                    549/timeDivider,
                    578/timeDivider,
                    605/timeDivider,
                    629/timeDivider,
                    651/timeDivider,
                    670/timeDivider,
                    687/timeDivider,
                    701/timeDivider,
                    720/timeDivider
                };

                MaxElementsAmountForPlayer = 20;
                GeneratedElementAmount = 1;
            }
        }

        private static IEnumerable<IEntity> GetWeaponsData()
        {
            return new List<IEntity>
            {
                // far
                new BaseWeapon
                {
                    Name = "Caretaker",
                    Sprite = "Caretaker",
                    WeaponType = WeaponTypes.Ranged,
                    WeaponHands = WeaponHandsTypes.TwoHand,
                    WeaponSize = WeaponSizeTypes.Small,

                    BaseDamage = 15.8f,
                    Chance = 3,
                    Price = 1320,
                    AttackSpeed = 1.43f,
                    Anomality = 12.9f,
                    Weight = 126,
                    AttackArea = 0,
                    DamageRadius = 2,
                    MaxTargets = 0,
                    AttackRange = 15f,
                    CriticalModifier = 0.5f,
                    DisruptionModifier = 1.8f,
                    Holder = 3,
                    Strength = 5,
                    Accuracy = 0.7f,
                    SpecialAttack = SpecialAttackTypes.Крионика,
                    ShotsInLine = 1,
                },
                new BaseWeapon
                {
                    Name = "Chronograph",
                    Sprite = "Chronograph",
                    WeaponType = WeaponTypes.Ranged,
                    WeaponHands = WeaponHandsTypes.TwoHand,
                    WeaponSize = WeaponSizeTypes.Small,

                    BaseDamage = 9.2f,
                    Chance = 4,
                    Price = 1469,
                    AttackSpeed = 0.83f,
                    Anomality = 17.2f,
                    Weight = 51,
                    AttackArea = 0,
                    DamageRadius = 1,
                    MaxTargets = 0,
                    AttackRange = 25f,
                    CriticalModifier = 2f,
                    DisruptionModifier = 1.1f,
                    Holder = 20,
                    Strength = 1,
                    Accuracy = 0.8f,
                    SpecialAttack = SpecialAttackTypes.Кривая,
                    ShotsInLine = 4,
                },
                new BaseWeapon
                {
                    Name = "Cracker",
                    Sprite = "Cracker",
                    WeaponType = WeaponTypes.Ranged,
                    WeaponHands = WeaponHandsTypes.TwoHand,

                    BaseDamage = 23.9f,
                    Chance = 4,
                    Price = 2186,
                    AttackSpeed = 1.67f,
                    Anomality = 17.2f,
                    Weight = 191,
                    AttackArea = 0,
                    DamageRadius = 3,
                    MaxTargets = 0,
                    AttackRange = 20f,
                    CriticalModifier = 0.3f,
                    DisruptionModifier = 1.8f,
                    Holder = 2,
                    Strength = 5,
                    Accuracy = 0.3f,
                    SpecialAttack = SpecialAttackTypes.Катарсис,
                    ShotsInLine = 2,
                },
                new BaseWeapon
                {
                    Name = "Cyber Disk",
                    Sprite = "CyberDisk",
                    WeaponType = WeaponTypes.Ranged,
                    WeaponHands = WeaponHandsTypes.OneHand,
                    WeaponSize = WeaponSizeTypes.Small,

                    BaseDamage = 8.5f,
                    Chance = 3,
                    Price = 1179,
                    AttackSpeed = 0.83f,
                    Anomality = 12.9f,
                    Weight = 41,
                    AttackArea = 0,
                    DamageRadius = 2,
                    MaxTargets = 0,
                    AttackRange = 10f,
                    CriticalModifier = 0.5f,
                    DisruptionModifier = 1.5f,
                    Holder = 2,
                    Strength = 5f,
                    Accuracy = 0.7f,
                    ShotsInLine = 1,
                    SpecialAttack = SpecialAttackTypes.Шок,
                },
                new BaseWeapon
                {
                    Name = "Gladiator",
                    Sprite = "Gladiator",
                    WeaponType = WeaponTypes.Ranged,
                    WeaponHands = WeaponHandsTypes.TwoHand,

                    BaseDamage = 17.9f,
                    Chance = 2,
                    Price = 1143,
                    AttackSpeed = 1.25f,
                    Anomality = 8.6f,
                    Weight =143,
                    AttackArea = 0,
                    DamageRadius = 2,
                    MaxTargets = 0,
                    AttackRange = 25f,
                    CriticalModifier = 1f,
                    DisruptionModifier = 1.4f,
                    Holder = 18,
                    Strength = 1,
                    Accuracy = 0.6f,
                    SpecialAttack = SpecialAttackTypes.Град,
                    ShotsInLine = 3,
                },
                new BaseWeapon
                {
                    Name = "Goliath",
                    Sprite = "Goliath",
                    WeaponType = WeaponTypes.Ranged,
                    WeaponHands = WeaponHandsTypes.OneHand,

                    BaseDamage = 13.8f,
                    Chance = 1,
                    Price = 737,
                    AttackSpeed = 1.04f,
                    Anomality = 4.3f,
                    Weight = 110,
                    AttackArea = 0,
                    DamageRadius = 1,
                    MaxTargets = 0,
                    AttackRange = 20f,
                    CriticalModifier = 0.7f,
                    DisruptionModifier = 1.3f,
                    Holder = 10,
                    Strength = 2,
                    Accuracy = 0.7f,
                    ShotsInLine = 2,
                    SpecialAttack = SpecialAttackTypes.Граната,
                },
                new BaseWeapon
                {
                    Name = "Hook",
                    Sprite = "Hook",
                    WeaponType = WeaponTypes.Ranged,
                    WeaponHands = WeaponHandsTypes.OneHand,

                    BaseDamage = 11f,
                    Chance = 2,
                    Price = 1048,
                    AttackSpeed = 0.83f,
                    Anomality = 8.6f,
                    Weight = 53,
                    AttackArea = 0,
                    DamageRadius = 1,
                    MaxTargets = 0,
                    AttackRange = 15f,
                    CriticalModifier = 0.3f,
                    DisruptionModifier = 1.7f,
                    Holder = 2,
                    Strength = 7,
                    Accuracy = 0.5f,
                    ShotsInLine = 1,
                    SpecialAttack = SpecialAttackTypes.Крюк,
                },
                new BaseWeapon
                {
                    Name = "Kvorsh Sphere",
                    Sprite = "KvorshSphere",
                    WeaponType = WeaponTypes.Ranged,
                    WeaponHands = WeaponHandsTypes.OneHand,
                    WeaponSize = WeaponSizeTypes.Small,

                    BaseDamage = 10.6f,
                    Chance = 2,
                    Price = 842,
                    AttackSpeed = 1.04f,
                    Anomality = 8.6f,
                    Weight = 85,
                    AttackArea = 0,
                    DamageRadius = 2,
                    MaxTargets = 0,
                    AttackRange = 15f,
                    CriticalModifier = 2f,
                    DisruptionModifier = 1.1f,
                    Holder = 42,
                    Strength = 0.5f,
                    Accuracy = 0.4f,
                    ShotsInLine = 7,
                    SpecialAttack = SpecialAttackTypes.Веер,
                },
                new BaseWeapon
                {
                    Name = "Manticore",
                    Sprite = "Manticore",
                    WeaponType = WeaponTypes.Ranged,
                    WeaponHands = WeaponHandsTypes.TwoHand,
                    WeaponSize = WeaponSizeTypes.Small,

                    BaseDamage = 9.2f,
                    Chance = 2,
                    Price = 1040,
                    AttackSpeed = 0.83f,
                    Anomality = 8.6f,
                    Weight = 74,
                    AttackArea = 0,
                    DamageRadius = 1,
                    MaxTargets = 0,
                    AttackRange = 30f,
                    CriticalModifier = 2f,
                    DisruptionModifier = 1.3f,
                    Holder = 6,
                    Strength = 3,
                    Accuracy = 0.7f,
                    SpecialAttack = SpecialAttackTypes.Стабилизация,
                    ShotsInLine = 1,
                },
                new BaseWeapon
                {
                    Name = "Nekroptor",
                    Sprite = "Nekroptor",
                    WeaponType = WeaponTypes.Ranged,
                    WeaponHands = WeaponHandsTypes.OneHand,

                    BaseDamage = 22.1f,
                    Chance = 3,
                    Price = 1254,
                    AttackSpeed = 1.67f,
                    Anomality = 12.9f,
                    Weight = 106,
                    AttackArea = 0,
                    DamageRadius = 2,
                    MaxTargets = 0,
                    AttackRange = 15f,
                    CriticalModifier = 0.5f,
                    DisruptionModifier = 1.8f,
                    Holder = 3,
                    Strength = 2,
                    Accuracy = 0.6f,
                    ShotsInLine = 1,
                    SpecialAttack = SpecialAttackTypes.Рывок
                },
                new BaseWeapon
                {
                    Name = "Pulsar",
                    Sprite = "Pulsar",
                    WeaponType = WeaponTypes.Ranged,
                    WeaponHands = WeaponHandsTypes.OneHand,
                    WeaponSize = WeaponSizeTypes.Small,

                    BaseDamage = 7.1f,
                    Chance = 1,
                    Price = 855,
                    AttackSpeed = 0.69f,
                    Anomality = 4.3f,
                    Weight = 113,
                    AttackArea = 0,
                    DamageRadius = 2,
                    MaxTargets = 0,
                    AttackRange = 10f,
                    CriticalModifier = 1f,
                    DisruptionModifier = 1.2f,
                    Holder = 4,
                    Strength = 5f,
                    Accuracy = 0.5f,
                    ShotsInLine = 1,
                    SpecialAttack = SpecialAttackTypes.Крионика,
                },
                new BaseWeapon
                {
                    Name = "Shotgun Dvach",
                    Sprite = "ShotgunDvach",
                    WeaponType = WeaponTypes.Ranged,
                    WeaponHands = WeaponHandsTypes.TwoHand,

                    BaseDamage = 14.3f,
                    Chance = 1,
                    Price = 742,
                    AttackSpeed = 1f,
                    Anomality = 4.3f,
                    Weight = 115,
                    AttackArea = 0,
                    DamageRadius = 2,
                    MaxTargets = 0,
                    AttackRange = 10f,
                    CriticalModifier = 0.5f,
                    DisruptionModifier = 1.5f,
                    Holder = 6,
                    Strength = 2,
                    Accuracy = 0.6f,
                    SpecialAttack = SpecialAttackTypes.Штык,
                    ShotsInLine = 2,
                },
                // Melee
                new BaseWeapon
                {
                    Name = "Aggregator Dagger",
                    Sprite = "AggregatorDagger",
                    WeaponType = WeaponTypes.Melee,
                    WeaponHands = WeaponHandsTypes.OneHand,
                    WeaponSize = WeaponSizeTypes.Small,

                    BaseDamage = 21.2f,
                    Chance = 3,
                    Price = 1057,
                    AttackSpeed = 0.69f,
                    Anomality = 12.9f,
                    Weight = 59,
                    AttackArea = 1,
                    DamageRadius = 0,
                    MaxTargets = 3,
                    AttackRange = 1.3f,
                    CriticalModifier = 2f,
                    DisruptionModifier = 1.1f,
                    Holder = 0,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Осколок,
                },
                new BaseWeapon
                {
                    Name = "Aggregator Spear",
                    Sprite = "AggregatorSpear",

                    WeaponType = WeaponTypes.Melee,
                    WeaponHands = WeaponHandsTypes.TwoHand,
                    WeaponSize = WeaponSizeTypes.Small,

                    BaseDamage = 27.6f,
                    Chance = 2,
                    Price = 950,
                    AttackSpeed = 0.83f,
                    Anomality = 8.6f,
                    Weight = 96,
                    AttackArea = 1,
                    DamageRadius = 0,
                    MaxTargets = 2,
                    AttackRange = 3f,
                    CriticalModifier = 2.5f,
                    DisruptionModifier = 1.2f,
                    Holder = 0,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Вспышка,
                },
                new BaseWeapon
                {
                    Name = "Chain Sword",
                    Sprite = "ChainSword",
                    WeaponType = WeaponTypes.Melee,
                    WeaponHands = WeaponHandsTypes.OneHand,

                    BaseDamage = 23.6f,
                    Chance = 4,
                    Price = 1638,
                    AttackSpeed = 0.6f,
                    Anomality = 17.2f,
                    Weight = 74,
                    AttackArea = 2,
                    DamageRadius = 0,
                    MaxTargets = 7,
                    AttackRange = 2.0f,
                    CriticalModifier = 0.5f,
                    DisruptionModifier = 1.2f,
                    Holder = 0,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Вихрь,
                },
                new BaseWeapon
                {
                    Name = "Disc Axe",
                    Sprite = "DiscAxe",
                    WeaponType = WeaponTypes.Melee,
                    WeaponHands = WeaponHandsTypes.TwoHand,

                    BaseDamage = 39.1f,
                    Chance = 1,
                    Price = 873,
                    AttackSpeed = 0.91f,
                    Anomality = 4.3f,
                    Weight = 123,
                    AttackArea = 2,
                    DamageRadius = 0,
                    MaxTargets = 3,
                    AttackRange = 1.8f,
                    CriticalModifier = 1.7f,
                    DisruptionModifier = 1.3f,
                    Holder = 0,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Клин,
                },
                new BaseWeapon
                {
                    Name = "Disc Sword",
                    Sprite = "DiscSword",
                    WeaponType = WeaponTypes.Melee,
                    WeaponHands = WeaponHandsTypes.TwoHand,

                    BaseDamage = 43f,
                    Chance = 3,
                    Price = 1400,
                    AttackSpeed = 1f,
                    Anomality = 12.9f,
                    Weight = 226,
                    AttackArea = 2,
                    DamageRadius = 0,
                    MaxTargets = 3,
                    AttackRange = 2.5f,
                    CriticalModifier = 0.7f,
                    DisruptionModifier = 1.8f,
                    Holder = 0,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Рычаг,
                },
                new BaseWeapon
                {
                    Name = "Flamberge",
                    Sprite = "Flamberge",
                    WeaponType = WeaponTypes.Melee,
                    WeaponHands = WeaponHandsTypes.TwoHand,

                    BaseDamage = 53.8f,
                    Chance = 2,
                    Price = 1223,
                    AttackSpeed = 1.25f,
                    Anomality = 8.6f,
                    Weight = 169,
                    AttackArea = 2,
                    DamageRadius = 0,
                    MaxTargets = 4,
                    AttackRange = 2.0f,
                    CriticalModifier = 1.0f,
                    DisruptionModifier = 1.5f,
                    Holder = 0,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Отсечение,
                },
                new BaseWeapon
                {
                    Name = "Ionic Sword",
                    Sprite = "IonicSword",
                    WeaponType = WeaponTypes.Melee,
                    WeaponHands = WeaponHandsTypes.TwoHand,
                    WeaponSize = WeaponSizeTypes.Small,

                    BaseDamage = 47.3f,
                    Chance = 4,
                    Price = 1988,
                    AttackSpeed = 1.43f,
                    Anomality = 17.2f,
                    Weight = 198,
                    AttackArea = 2,
                    DamageRadius = 0,
                    MaxTargets = 3,
                    AttackRange = 1.7f,
                    CriticalModifier = 1.5f,
                    DisruptionModifier = 1.6f,
                    Holder = 0,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Дизентеграция,
                },
                new BaseWeapon
                {
                    Name = "Power Fist",
                    Sprite = "PowerFist",
                    WeaponType = WeaponTypes.Melee,
                    WeaponHands = WeaponHandsTypes.OneHand,
                    WeaponSize = WeaponSizeTypes.Small,

                    BaseDamage = 23.1f,
                    Chance = 1,
                    Price = 777,
                    AttackSpeed = 0.76f,
                    Anomality = 4.3f,
                    Weight = 73,
                    AttackArea = 2,
                    DamageRadius = 0,
                    MaxTargets = 3,
                    AttackRange = 0.8f,
                    CriticalModifier = 1f,
                    DisruptionModifier = 1.5f,
                    Holder = 0,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Волна,
                },
                new BaseWeapon
                {
                    Name = "Resonator Sword",
                    Sprite = "ResonatorSword",
                    WeaponType = WeaponTypes.Melee,
                    WeaponHands = WeaponHandsTypes.OneHand,

                    BaseDamage = 41.3f,
                    Chance = 3,
                    Price = 587,
                    AttackSpeed = 1.04f,
                    Anomality = 4.3f,
                    Weight = 130,
                    AttackArea = 1,
                    DamageRadius = 0,
                    MaxTargets = 5,
                    AttackRange = 1.5f,
                    CriticalModifier = 0.7f,
                    DisruptionModifier = 1.4f,
                    Holder = 0,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Бросок,
                },
                new BaseWeapon
                {
                    Name = "Sonic Dagger",
                    Sprite = "SonicDagger",
                    WeaponType = WeaponTypes.Melee,
                    WeaponHands = WeaponHandsTypes.OneHand,
                    WeaponSize = WeaponSizeTypes.Small,

                    BaseDamage = 19.6f,
                    Chance = 2,
                    Price = 920,
                    AttackSpeed = 0.64f,
                    Anomality = 8.6f,
                    Weight = 48,
                    AttackArea = 1,
                    DamageRadius = 0,
                    MaxTargets = 4,
                    AttackRange = 1f,
                    CriticalModifier = 1.7f,
                    DisruptionModifier = 1.3f,
                    Holder = 0,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Кровотечение,
                },
                new BaseWeapon
                {
                    Name = "Sonic Sword",
                    Sprite = "SonicSword",
                    WeaponType = WeaponTypes.Melee,
                    WeaponHands = WeaponHandsTypes.OneHand,

                    BaseDamage = 33.1f,
                    Chance = 3,
                    Price = 961,
                    AttackSpeed = 0.83f,
                    Anomality = 12.9f,
                    Weight = 81,
                    AttackArea = 1,
                    DamageRadius = 0,
                    MaxTargets = 6,
                    AttackRange = 1.3f,
                    CriticalModifier = 1.5f,
                    DisruptionModifier = 1.3f,
                    Holder = 0,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Острота,
                },
                new BaseWeapon
                {
                    Name = "Wave Hammer",
                    Sprite = "WaveHammer",
                    WeaponType = WeaponTypes.Melee,
                    WeaponHands = WeaponHandsTypes.TwoHand,
                    WeaponSize = WeaponSizeTypes.Small,

                    BaseDamage = 55.1f,
                    Chance = 3,
                    Price = 1728,
                    AttackSpeed = 1.67f,
                    Anomality = 12.9f,
                    Weight = 193,
                    AttackArea = 3,
                    DamageRadius = 0,
                    MaxTargets = 2,
                    AttackRange = 1.8f,
                    CriticalModifier = 0.7f,
                    DisruptionModifier = 1.6f,
                    Holder = 0,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Сокрушение,
                },
                // Magic
                new BaseWeapon
                {
                    Name = "Annihilator",
                    Sprite = "Annihilator",
                    WeaponType = WeaponTypes.Magic,
                    WeaponHands = WeaponHandsTypes.TwoHand,
                    WeaponSize = WeaponSizeTypes.Small,

                    BaseDamage = 23.6f,
                    Chance = 3,
                    Price = 1199,
                    AttackSpeed = 1.43f,
                    Anomality = 12.9f,
                    Weight = 142,
                    AttackArea = 0,
                    DamageRadius = 0,
                    MaxTargets = 1,
                    AttackRange = 0f,
                    CriticalModifier = 0.7f,
                    DisruptionModifier = 1.3f,
                    Holder = 8,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Ноша,
                },
                new BaseWeapon
                {
                    Name = "Haze",
                    Sprite = "Haze",
                    WeaponType = WeaponTypes.Magic,
                    WeaponHands = WeaponHandsTypes.OneHand,
                    WeaponSize = WeaponSizeTypes.Small,

                    BaseDamage = 10.6f,
                    Chance = 1,
                    Price = 854,
                    AttackSpeed = 0.69f,
                    Anomality = 4.3f,
                    Weight = 64,
                    AttackArea = 0,
                    DamageRadius = 0,
                    MaxTargets = 2,
                    AttackRange = 0f,
                    CriticalModifier = 0.5f,
                    DisruptionModifier = 1.5f,
                    Holder = 10,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Глюк,
                },
                new BaseWeapon
                {
                    Name = "Leveler",
                    Sprite = "Leveler",
                    WeaponType = WeaponTypes.Magic,
                    WeaponHands = WeaponHandsTypes.TwoHand,

                    BaseDamage = 26.9f,
                    Chance = 1,
                    Price = 1061,
                    AttackSpeed = 1.25f,
                    Anomality = 4.3f,
                    Weight = 134,
                    AttackArea = 0,
                    DamageRadius = 0,
                    MaxTargets = 1,
                    AttackRange = 0f,
                    CriticalModifier = 1.0f,
                    DisruptionModifier = 1.1f,
                    Holder = 11,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Стазис,
                },
                new BaseWeapon
                {
                    Name = "Lightning",
                    Sprite = "Lightning",
                    WeaponType = WeaponTypes.Magic,
                    WeaponHands = WeaponHandsTypes.OneHand,
                    WeaponSize = WeaponSizeTypes.Small,

                    BaseDamage = 15.9f,
                    Chance = 4,
                    Price = 1416,
                    AttackSpeed = 1.04f,
                    Anomality = 17.2f,
                    Weight = 48,
                    AttackArea = 0,
                    DamageRadius = 0,
                    MaxTargets = 1,
                    AttackRange = 0f,
                    CriticalModifier = 1.5f,
                    DisruptionModifier = 1.5f,
                    Holder = 12,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Судьба,
                },
                new BaseWeapon
                {
                    Name = "Mediator",
                    Sprite = "Mediator",
                    WeaponType = WeaponTypes.Magic,
                    WeaponHands = WeaponHandsTypes.TwoHand,
                    WeaponSize = WeaponSizeTypes.Small,

                    BaseDamage = 27.6f,
                    Chance = 2,
                    Price = 1428,
                    AttackSpeed = 1.67f,
                    Anomality = 8.6f,
                    Weight = 179,
                    AttackArea = 0,
                    DamageRadius = 0,
                    MaxTargets = 3,
                    AttackRange = 0f,
                    CriticalModifier = 0.5f,
                    DisruptionModifier = 1.5f,
                    Holder = 6,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Опусташение,
                },
                new BaseWeapon
                {
                    Name = "Membrane",
                    Sprite = "Membrane",
                    WeaponType = WeaponTypes.Magic,
                    WeaponHands = WeaponHandsTypes.TwoHand,

                    BaseDamage = 30.7f,
                    Chance = 4,
                    Price = 1743,
                    AttackSpeed = 1.43f,
                    Anomality = 17.2f,
                    Weight = 123,
                    AttackArea = 0,
                    DamageRadius = 0,
                    MaxTargets = 3,
                    AttackRange = 0f,
                    CriticalModifier = 0.7f,
                    DisruptionModifier = 1.1f,
                    Holder = 6,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Берст,
                },
                new BaseWeapon
                {
                    Name = "Net",
                    Sprite = "Net",
                    WeaponType = WeaponTypes.Magic,
                    WeaponHands = WeaponHandsTypes.OneHand,

                    BaseDamage = 16.5f,
                    Chance = 2,
                    Price = 1131,
                    AttackSpeed = 0.83f,
                    Anomality = 8.6f,
                    Weight = 83,
                    AttackArea = 0,
                    DamageRadius = 0,
                    MaxTargets = 2,
                    AttackRange = 0f,
                    CriticalModifier = 2.0f,
                    DisruptionModifier = 1.3f,
                    Holder = 8,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Клей,
                },
                new BaseWeapon
                {
                    Name = "Pointer",
                    Sprite = "Pointer",
                    WeaponType = WeaponTypes.Magic,
                    WeaponHands = WeaponHandsTypes.OneHand,
                    WeaponSize = WeaponSizeTypes.Small,

                    BaseDamage = 11.6f,
                    Chance = 2,
                    Price = 816,
                    AttackSpeed = 0.76f,
                    Anomality = 8.6f,
                    Weight = 58,
                    AttackArea = 0,
                    DamageRadius = 0,
                    MaxTargets = 1,
                    AttackRange = 0f,
                    CriticalModifier = 0.5f,
                    DisruptionModifier = 1.3f,
                    Holder = 15,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Травля,
                },
                new BaseWeapon
                {
                    Name = "Prime",
                    Sprite = "Prime",
                    WeaponType = WeaponTypes.Magic,
                    WeaponHands = WeaponHandsTypes.OneHand,

                    BaseDamage = 11.8f,
                    Chance = 3,
                    Price = 1368,
                    AttackSpeed = 0.6f,
                    Anomality = 12.9f,
                    Weight = 59,
                    AttackArea = 0,
                    DamageRadius = 0,
                    MaxTargets = 2,
                    AttackRange = 0f,
                    CriticalModifier = 1.0f,
                    DisruptionModifier = 1.8f,
                    Holder = 10,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Фобос,
                },
                new BaseWeapon
                {
                    Name = "Punisher",
                    Sprite = "Punisher",
                    WeaponType = WeaponTypes.Magic,
                    WeaponHands = WeaponHandsTypes.TwoHand,

                    BaseDamage = 21.5f,
                    Chance = 2,
                    Price = 992,
                    AttackSpeed = 1f,
                    Anomality = 8.6f,
                    Weight = 161,
                    AttackArea = 0,
                    DamageRadius = 0,
                    MaxTargets = 2,
                    AttackRange = 0f,
                    CriticalModifier = 0.5f,
                    DisruptionModifier = 1.8f,
                    Holder = 6,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Бласт,
                },
                new BaseWeapon
                {
                    Name = "Swarm",
                    Sprite = "Swarm",
                    WeaponType = WeaponTypes.Magic,
                    WeaponHands = WeaponHandsTypes.OneHand,

                    BaseDamage = 16.5f,
                    Chance = 3,
                    Price = 1169,
                    AttackSpeed = 0.83f,
                    Anomality = 12.9f,
                    Weight = 99,
                    AttackArea = 0,
                    DamageRadius = 0,
                    MaxTargets = 1,
                    AttackRange = 0f,
                    CriticalModifier = 1.0f,
                    DisruptionModifier = 1.7f,
                    Holder = 18,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Жатва,
                },
                new BaseWeapon
                {
                    Name = "Zero Inverter",
                    Sprite = "ZeroInverter",
                    WeaponType = WeaponTypes.Magic,
                    WeaponHands = WeaponHandsTypes.TwoHand,
                    WeaponSize = WeaponSizeTypes.Small,

                    BaseDamage = 13.8f,
                    Chance = 3,
                    Price = 1067,
                    AttackSpeed = 0.83f,
                    Anomality = 12.9f,
                    Weight = 103,
                    AttackArea = 0,
                    DamageRadius = 0,
                    MaxTargets = 2,
                    AttackRange = 0f,
                    CriticalModifier = 1.0f,
                    DisruptionModifier = 1.6f,
                    Holder = 14,
                    Strength = 0,
                    Accuracy = 0,
                    SpecialAttack = SpecialAttackTypes.Цепь,
                },
            };
        }

        private static IEnumerable<IEntity> GetShieldsData()
        {
            return new List<IEntity>
            {
                // Shields
                new BaseWeapon
                {
                    Name = "Field Generator",
                    Sprite = "ax1",
                    WeaponType = WeaponTypes.Ranged,
                    WeaponHands = WeaponHandsTypes.OneHand,
                    WeaponSize = WeaponSizeTypes.Shield,

                    BaseDamage = 12f,
                    Chance = 2,
                    Price = 1150,

                    Anomality = 8.6f,
                    Weight = 80,
                    SpecialAttack = SpecialAttackTypes.Экран,
                },
                new BaseWeapon
                {
                    Name = "Silence Rod",
                    Sprite = "ax1",
                    WeaponType = WeaponTypes.Magic,
                    WeaponHands = WeaponHandsTypes.OneHand,
                    WeaponSize = WeaponSizeTypes.Shield,

                    BaseDamage = 12f,
                    Chance = 2,
                    Price = 1200,

                    Anomality = 8.6f,
                    Weight = 50,
                    SpecialAttack = SpecialAttackTypes.Эфир,
                },
                new BaseWeapon
                {
                    Name = "Trimmer",
                    Sprite = "ax1",
                    WeaponType = WeaponTypes.Melee,
                    WeaponHands = WeaponHandsTypes.OneHand,
                    WeaponSize = WeaponSizeTypes.Shield,

                    BaseDamage = 12f,
                    Chance = 1,
                    Price = 800,

                    Anomality = 4.3f,
                    Weight = 45,
                    SpecialAttack = SpecialAttackTypes.Поглотитель,
                },
                new BaseWeapon
                {
                    Name = "Void Shield",
                    Sprite = "ax1",
                    WeaponType = WeaponTypes.Melee,
                    WeaponHands = WeaponHandsTypes.OneHand,
                    WeaponSize = WeaponSizeTypes.Shield,

                    BaseDamage = 12f,
                    Chance = 3,
                    Price = 1300,

                    Anomality = 12.9f,
                    Weight = 120,
                    SpecialAttack = SpecialAttackTypes.Поглотитель,
                },
                new BaseWeapon
                {
                    Name = "Wand Of Feed",
                    Sprite = "ax1",
                    WeaponType = WeaponTypes.Magic,
                    WeaponHands = WeaponHandsTypes.OneHand,
                    WeaponSize = WeaponSizeTypes.Shield,

                    BaseDamage = 12f,
                    Chance = 4,
                    Price = 1700,

                    Anomality = 17.2f,
                    Weight = 65,
                    SpecialAttack = SpecialAttackTypes.Восполнение,
                },
                new BaseWeapon
                {
                    Name = "Wave Distorter",
                    Sprite = "ax1",
                    WeaponType = WeaponTypes.Ranged,
                    WeaponHands = WeaponHandsTypes.OneHand,
                    WeaponSize = WeaponSizeTypes.Shield,

                    BaseDamage = 12f,
                    Chance = 2,
                    Price = 800,

                    Anomality = 8.6f,
                    Weight = 100,
                    SpecialAttack = SpecialAttackTypes.Искажение,
                },
            };
        }

        private static IEnumerable<IEntity> GetModulesData()
        {
            var l = new List<IEntity>();

            for (var i = ModulesTypesOfImpacts.MeleeAttackSpeed; i <= ModulesTypesOfImpacts.WisdomValue; ++i)
            {
                var places = BaseModule.AllowedPlaces[i];

                for (var j = 0; j < places.Count; j++)
                {
                    var m = new BaseModule
                    {
                        Name = i.MakeName(),
                        Sprite = "",
                        TypesOfImpact = i,
                        InsertTo = places[j]
                    };
                    l.Add(m);
                }
            }
            return l;
        }
        private static IEnumerable<IEntity> GetArmorsData()
        {
            var armors = new List<IEntity>
            {
                new BaseArmor
                {
                    StructureType = StructureTypes.Metall,
                    ArmorType = ArmorTypes.Heavy,
                    Weight = 900,
                    Name = "Heavy Armor",
                    Price = 6500

                },
                new BaseArmor
                {
                    StructureType = StructureTypes.Metall,
                    Weight = 250,
                    ArmorType = ArmorTypes.Light,
                    Name = "Light Armor",
                    Price = 3600
                },
                new BaseArmor
                {
                    StructureType = StructureTypes.Metall,
                    Weight = 500,
                    ArmorType = ArmorTypes.Medium,
                    Name = "Medium Armor",
                    Price = 5000
                },
            };

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < armors.Count; ++i)
            {
                ((BaseArmor)armors[i]).InitializeDefaultArmorParts();
            }
            return armors;
        }
        public List<IEntity> GetItems()
        {
            var items = new List<IEntity>
            {
                    new BaseCraftHero()
                {
                    Name = "hero1",
                    Health = 1,
                    Price = 20,
                    RequiredLevel = 10
                },
                new BaseCraftHero
                {
                    Name = "hero2",
                    Health = 2,
                    Price = 20,
                    RequiredLevel = 0
                },
              new BaseCraftHero
                {
                    Name = "hero3",
                    Health = 3,
                    Price = 20,
                    RequiredLevel = 20
                },
               new BaseCraftHero
                {
                    Name = "hero4",
                    Health = 4,
                    Price = 35,
                    RequiredLevel = 0
                },
                new BaseCraftHero
                {
                    Name = "hero5",
                    Health = 5,
                    Price = 20,
                    RequiredLevel = 40
                },
                 new BaseCraftHero
                {
                    Name = "hero6",
                    Health = 6,
                    Price = 45,
                    RequiredLevel = 0
                }, new BasePerk { Name = "Add", Number = "A1" },
            new BasePerk { Name = "Move", Number = "B1" },
            new BasePerk { Name = "Chronograph", Number = "C1", },
            new BasePerk { Name = "Chip", Number = "D1" },
            new BasePerk { Name = "BoostsSlots", Number = "E1" },
            new BasePerk { Name = "Activator", Number = "I1" },
            new BasePerk { Name = "Lattice", Number = "T1" },
            new BasePerk { Name = "BuildZone", Number = "Q1" },
            new BasePerk { Name = "AliancePower", Number = "H1" },
            new BasePerk { Name = "Power1_3", Number = "D1" },
            new BasePerk { Name = "Power13_15", Number = "J2" },
            new BasePerk { Name = "Add2", Number = "A2" },
            new BasePerk { Name = "Add2_1", Number = "A2" },
            new BasePerk { Name = "Add3", Number = "A3" },
            new BasePerk { Name = "Cluster", Number = "1J" },
            new BasePerk { Name = "Power", Number = "2J" },
                new BaseCraftMic { Name = "Hand"},
                new BaseCraftMic { Name = "Coins" },
                new BaseCraftMic { Name = "Leg"},
                new BaseCraftMic { Name = "Larec"},
                new BaseItem { Name = "Crystals"}
            };
            return items;
        }
        private static IEnumerable<BaseElement> GetElementsData()
        {
            var elementList = new List<BaseElement>
            {
                // Metal
                new BaseElement
                {
                    Type = ElementTypes.Metall,
                    ColorType = ElementColorTypes.Red,
                    Trigger = TriggerTypes.Reanimator,
                    Durability = 10f,
                    Anomality = 10,
                    Weight = 5f,
                    BasePrice = 100,
                    Price = 100,
                    CategoryType = ElementCategoryTypes.First
                },
                new BaseElement
                {
                    Type = ElementTypes.Metall,
                    ColorType = ElementColorTypes.Blue,
                    Trigger = TriggerTypes.Imbalance,
                    Durability = 10f,
                    Anomality = 16,
                    Weight = 5.25f,
                    BasePrice = 140,
                    Price = 140,
                    CategoryType = ElementCategoryTypes.First
                },
                new BaseElement
                {
                    Type = ElementTypes.Metall,
                    ColorType = ElementColorTypes.Yellow,
                    Trigger = TriggerTypes.Reflector,
                    Durability = 10f,
                    Anomality = 22,
                    Weight = 5.5f,
                    BasePrice = 180,
                    Price = 180,
                    CategoryType = ElementCategoryTypes.First
                },
                new BaseElement
                {
                    Type = ElementTypes.Metall,
                    ColorType = ElementColorTypes.BlueRed,
                    Trigger = TriggerTypes.Scale,
                    Durability = 8f,
                    Anomality = 28,
                    Weight = 5.7f,
                    BasePrice = 220,
                    Price = 220,
                    CategoryType = ElementCategoryTypes.Second

                },    new BaseElement
                {
                    Type = ElementTypes.Metall,
                    ColorType = ElementColorTypes.BlueYellow,
                    Trigger = TriggerTypes.Covenant,
                    Durability = 8f,
                    Anomality = 34,
                    Weight = 6f,
                    BasePrice = 260,
                    Price = 260,
                    CategoryType = ElementCategoryTypes.Third
                },
                new BaseElement
                {
                    Type = ElementTypes.Metall,
                    ColorType = ElementColorTypes.RedYellow,
                    Trigger = TriggerTypes.Focus,
                    Durability = 8f,
                    Anomality = 40,
                    Weight = 6.25f,
                    BasePrice = 300,
                    Price = 300,
                    CategoryType = ElementCategoryTypes.Fourth
                },
                // Plastic
                 new BaseElement
                {
                    Type = ElementTypes.Plastic,
                    ColorType = ElementColorTypes.Red,
                    Trigger = TriggerTypes.Bait,
                    Durability = 10,
                    Anomality = 12,
                    Weight = 5.7f,
                    BasePrice = 120,
                    Price = 120,
                    CategoryType = ElementCategoryTypes.First
                },
                new BaseElement
                {
                    Type = ElementTypes.Plastic,
                    ColorType = ElementColorTypes.Blue,
                    Trigger = TriggerTypes.Tide,
                    Durability = 10,
                    Anomality = 18,
                    Weight = 6,
                    BasePrice = 160,
                    Price = 160,
                       CategoryType = ElementCategoryTypes.First
                },
                new BaseElement
                {
                    Type = ElementTypes.Plastic,
                    ColorType = ElementColorTypes.Yellow,
                    Trigger = TriggerTypes.Lash,
                    Durability = 10f,
                    Anomality = 24,
                    Weight = 6.2f,
                    BasePrice = 200,
                    Price = 200,
                       CategoryType = ElementCategoryTypes.First
                },
                new BaseElement
                {
                    Type = ElementTypes.Plastic,
                    ColorType = ElementColorTypes.BlueRed,
                    Trigger = TriggerTypes.Nanofuel,
                    Durability = 8f,
                    Anomality = 30,
                    Weight = 6.5f,
                    BasePrice = 240,
                    Price = 240,
                       CategoryType = ElementCategoryTypes.Second
                },
                new BaseElement
                {
                    Type = ElementTypes.Plastic,
                    ColorType = ElementColorTypes.BlueYellow,
                    Trigger = TriggerTypes.Duality,
                    Durability = 8f,
                    Anomality = 36,
                    Weight = 6.7f,
                    BasePrice = 280,
                    Price = 280,
                       CategoryType = ElementCategoryTypes.Third
                },
                new BaseElement
                {
                    Type = ElementTypes.Plastic,
                    ColorType = ElementColorTypes.RedYellow,
                    Trigger = TriggerTypes.Consolation,
                    Durability = 8f,
                    Anomality = 42,
                    Weight = 7f,
                    BasePrice = 320,
                    Price = 320,
                       CategoryType = ElementCategoryTypes.Fourth
                },
                // Crystall
                new BaseElement
                {
                    Type = ElementTypes.Crystal,
                    ColorType = ElementColorTypes.Red,
                    Trigger = TriggerTypes.Crystal,
                    Durability = 10,
                    Anomality = 14,
                    Weight = 6.5f,
                    BasePrice = 140,
                    Price = 140,
                       CategoryType = ElementCategoryTypes.First
                },
                new BaseElement
                {
                    Type = ElementTypes.Crystal,
                    ColorType = ElementColorTypes.Blue,
                    Trigger = TriggerTypes.Alembic,
                    Durability = 10f,
                    Anomality = 20,
                    Weight = 6.7f,
                    BasePrice = 180,
                    Price = 180,
                       CategoryType = ElementCategoryTypes.First
                },
                new BaseElement
                {
                    Type = ElementTypes.Crystal,
                    ColorType = ElementColorTypes.Yellow,
                    Trigger = TriggerTypes.Affect,
                    Durability = 10,
                    Anomality = 26,
                    Weight = 7f,
                    BasePrice = 220,
                    Price = 220,
                       CategoryType = ElementCategoryTypes.Second
                },
                new BaseElement
                {
                    Type = ElementTypes.Crystal,
                    ColorType = ElementColorTypes.BlueRed,
                    Trigger = TriggerTypes.Heart,
                    Durability = 8f,
                    Anomality = 32,
                    Weight = 7.2f,
                    BasePrice = 260,
                    Price = 260,
                       CategoryType = ElementCategoryTypes.Second
                },
                new BaseElement
                {
                    Type = ElementTypes.Crystal,
                    ColorType = ElementColorTypes.BlueYellow,
                    Trigger = TriggerTypes.Blocker,
                    Durability = 8,
                    Anomality = 38,
                    Weight = 7.5f,
                    BasePrice = 300,
                    Price = 300,
                       CategoryType = ElementCategoryTypes.Third
                },
                new BaseElement
                {
                    Type = ElementTypes.Crystal,
                    ColorType = ElementColorTypes.RedYellow,
                    Trigger = TriggerTypes.Frenzy,
                    Durability = 8,
                    Anomality = 44,
                    Weight = 7.7f,
                    BasePrice = 340,
                    Price = 340,
                       CategoryType = ElementCategoryTypes.Fourth
                },
                // Metall plastic
                new BaseElement
                {
                    Type = ElementTypes.MetallPlastic,
                    ColorType = ElementColorTypes.Red,
                    Trigger = TriggerTypes.Teleport,
                    Durability = 8f,
                    Anomality = 16,
                    Weight = 7.2f,
                    BasePrice = 160,
                    Price = 160,
                       CategoryType = ElementCategoryTypes.Second
                },
                new BaseElement
                {
                    Type = ElementTypes.MetallPlastic,
                    ColorType = ElementColorTypes.Blue,
                    Trigger = TriggerTypes.Sand,
                    Durability = 8f,
                    Anomality = 22,
                    Weight = 7.5f,
                    BasePrice = 200,
                    Price = 200,
                       CategoryType = ElementCategoryTypes.Second
                },
                new BaseElement
                {
                    Type = ElementTypes.MetallPlastic,
                    ColorType = ElementColorTypes.Yellow,
                    Trigger = TriggerTypes.Sprint,
                    Durability = 8f,
                    Anomality = 28,
                    Weight = 7.7f,
                    BasePrice = 240,
                    Price = 240,
                       CategoryType = ElementCategoryTypes.Second
                },
                new BaseElement
                {
                    Type = ElementTypes.MetallPlastic,
                    ColorType = ElementColorTypes.BlueRed,
                    Trigger = TriggerTypes.Reainforcment,
                    Durability = 6f,
                    Anomality = 34,
                    Weight = 8f,
                    BasePrice = 280,
                    Price = 280,
                       CategoryType = ElementCategoryTypes.Third
                },
                new BaseElement
                {
                    Type = ElementTypes.MetallPlastic,
                    ColorType = ElementColorTypes.BlueYellow,
                    Trigger = TriggerTypes.Nanofuel,
                    Durability = 6f,
                    Anomality = 40,
                    Weight = 8.2f,
                    BasePrice = 320,
                    Price = 320,
                       CategoryType = ElementCategoryTypes.Fourth
                },
                new BaseElement
                {
                    Type = ElementTypes.MetallPlastic,
                    ColorType = ElementColorTypes.RedYellow,
                    Trigger = TriggerTypes.Apathy,
                    Durability = 6f,
                    Anomality = 46,
                    Weight = 8.5f,
                    BasePrice = 360,
                    Price = 360,
                       CategoryType = ElementCategoryTypes.Fifth
                },
                // MetallCrystall
                new BaseElement
                {
                    Type = ElementTypes.MetallCrystal,
                    ColorType = ElementColorTypes.Red,
                    Trigger = TriggerTypes.Cover,
                    Durability = 8f,
                    Anomality = 18,
                    Weight = 8,
                    BasePrice = 180,
                    Price = 180,
                       CategoryType = ElementCategoryTypes.Third
                },
                new BaseElement
                {
                    Type = ElementTypes.MetallCrystal,
                    ColorType = ElementColorTypes.Blue,
                    Trigger = TriggerTypes.Funnel,
                    Durability = 8f,
                    Anomality = 24,
                    Weight = 8.2f,
                    BasePrice = 220,
                    Price = 220,
                       CategoryType = ElementCategoryTypes.Third
                },
                 new BaseElement
                {
                    Type = ElementTypes.MetallCrystal,
                    ColorType = ElementColorTypes.Yellow,
                    Trigger = TriggerTypes.Purity,
                    Durability = 8f,
                    Anomality = 30,
                    Weight = 8.5f,
                    BasePrice = 260,
                    Price = 260,
                       CategoryType = ElementCategoryTypes.Third
                },
                 new BaseElement
                {
                    Type = ElementTypes.MetallCrystal,
                    ColorType = ElementColorTypes.BlueRed,
                    Trigger = TriggerTypes.Foresight,
                    Durability = 6f,
                    Anomality = 36,
                    Weight = 8.7f,
                    BasePrice = 300,
                    Price = 300,
                       CategoryType = ElementCategoryTypes.Fifth
                },
                 new BaseElement
                {
                    Type = ElementTypes.MetallCrystal,
                    ColorType = ElementColorTypes.BlueYellow,
                    Trigger = TriggerTypes.Adamantium,
                    Durability = 6f,
                    Anomality = 42,
                    Weight = 9f,
                    BasePrice = 340,
                    Price = 340,
                       CategoryType = ElementCategoryTypes.Fifth
                },
                new BaseElement
                {
                    Type = ElementTypes.MetallCrystal,
                    ColorType = ElementColorTypes.RedYellow,
                    Trigger = TriggerTypes.Cristrage,
                    Durability = 6f,
                    Anomality = 48,
                    Weight = 9.2f,
                    BasePrice = 380,
                    Price = 380,
                       CategoryType = ElementCategoryTypes.Fifth
                },
                 // Crystal Plastic
                 new BaseElement
                {
                    Type = ElementTypes.CrystalPlastic,
                    ColorType = ElementColorTypes.Red,
                    Trigger = TriggerTypes.Keeper,
                    Durability = 8f,
                    Anomality = 20,
                    Weight = 8.7f,
                    BasePrice = 200,
                    Price = 200,
                       CategoryType = ElementCategoryTypes.Fifth
                },
                new BaseElement
                {
                    Type = ElementTypes.CrystalPlastic,
                    ColorType = ElementColorTypes.Blue,
                    Trigger = TriggerTypes.Exile,
                    Durability = 8f,
                    Anomality = 26,
                    Weight = 9f,
                    BasePrice = 240,
                    Price = 240,
                       CategoryType = ElementCategoryTypes.Fifth
                },
                new BaseElement
                {
                    Type = ElementTypes.CrystalPlastic,
                    ColorType = ElementColorTypes.Yellow,
                    Trigger = TriggerTypes.HalfLife,
                    Durability = 8f,
                    Anomality = 32,
                    Weight = 9.2f,
                    BasePrice = 280,
                    Price = 280,
                       CategoryType = ElementCategoryTypes.Fourth
                },
                new BaseElement
                {
                    Type = ElementTypes.CrystalPlastic,
                    ColorType = ElementColorTypes.BlueRed,
                    Trigger = TriggerTypes.Wrath,
                    Durability = 6f,
                    Anomality = 38,
                    Weight = 9.5f,
                    BasePrice = 320,
                    Price = 320,
                       CategoryType = ElementCategoryTypes.Fifth
                },
                new BaseElement
                {
                    Type = ElementTypes.CrystalPlastic,
                    ColorType = ElementColorTypes.BlueYellow,
                    Trigger = TriggerTypes.Sacrifice,
                    Durability = 6f,
                    Anomality = 44,
                    Weight = 9.7f,
                    BasePrice = 360,
                    Price = 360,
                       CategoryType = ElementCategoryTypes.Fifth
                },
                new BaseElement
                {
                    Type = ElementTypes.CrystalPlastic,
                    ColorType = ElementColorTypes.RedYellow,
                    Trigger = TriggerTypes.PhilosopherStone,
                    Durability = 6f,
                    Anomality = 50,
                    Weight = 10,
                    BasePrice = 400,
                    Price = 400,
                    CategoryType = ElementCategoryTypes.Fifth
                },
            };
            var cel = elementList.Count;
            for (var i = 0; i < cel; ++i)
            {
                elementList[i].SetName();
                switch (elementList[i].ColorType)
                {
                    case ElementColorTypes.Blue:
                        elementList[i].Blue = (int)Math.Round(elementList[i].Durability + elementList[i].Durability * 0.25f);
                        elementList[i].Yellow = (int)Math.Round(elementList[i].Durability - elementList[i].Durability * 0.25f);
                        elementList[i].Red = (int)Math.Round(elementList[i].Durability - elementList[i].Durability * 0.25f);
                        break;
                    case ElementColorTypes.Red:
                        elementList[i].Blue = (int)Math.Round(elementList[i].Durability - elementList[i].Durability * 0.25f);
                        elementList[i].Yellow = (int)Math.Round(elementList[i].Durability - elementList[i].Durability * 0.25f);
                        elementList[i].Red = (int)Math.Round(elementList[i].Durability + elementList[i].Durability * 0.25f);
                        break;
                    case ElementColorTypes.Yellow:
                        elementList[i].Blue = (int)Math.Round(elementList[i].Durability - elementList[i].Durability * 0.25f);
                        elementList[i].Yellow = (int)Math.Round(elementList[i].Durability - elementList[i].Durability * 0.25f);
                        elementList[i].Red = (int)Math.Round(elementList[i].Durability + elementList[i].Durability * 0.25f);
                        break;
                    case ElementColorTypes.BlueRed:
                        elementList[i].Blue = (int)Math.Round(elementList[i].Durability + elementList[i].Durability * 0.25f);
                        elementList[i].Yellow = (int)Math.Round(elementList[i].Durability - elementList[i].Durability * 0.25f);
                        elementList[i].Red = (int)Math.Round(elementList[i].Durability + elementList[i].Durability * 0.25f);
                        break;
                    case ElementColorTypes.BlueYellow:
                        elementList[i].Blue = (int)Math.Round(elementList[i].Durability + elementList[i].Durability * 0.25f);
                        elementList[i].Yellow = (int)Math.Round(elementList[i].Durability + elementList[i].Durability * 0.25f);
                        elementList[i].Red = (int)Math.Round(elementList[i].Durability - elementList[i].Durability * 0.25f);
                        break;
                    case ElementColorTypes.RedYellow:
                        elementList[i].Blue = (int)Math.Round(elementList[i].Durability - elementList[i].Durability * 0.25f);
                        elementList[i].Yellow = (int)Math.Round(elementList[i].Durability + elementList[i].Durability * 0.25f);
                        elementList[i].Red = (int)Math.Round(elementList[i].Durability + elementList[i].Durability * 0.25f);
                        break;
                }
            }

            return elementList;
        }

        #endregion

        #region Methods

        public IEntity GetRandom<T>()
        {
            var query = Objects.Where(t => t.GetType() == typeof(T)).ToArray();
            return query[Random.Next(0, query.Length - 1)];
        }

        public BaseEquipment GetRandomEq<T>(int chanceGroup)
        {
            var query = Objects.Where(t => t.GetType() == typeof(T) && t is BaseEquipment && ((BaseEquipment)t).Chance == chanceGroup).ToArray();
            if (query.Length == 0)
            {
                query = Objects.Where(t => t.GetType() == typeof(T) && t is BaseEquipment).ToArray();
            }
            return (BaseEquipment)query[Random.Next(0, query.Length - 1)];
        }


        public IEnumerable<T> GetAll<T>()
        {
            return Objects.OfType<T>().ToList();
        }

        public IEntity Get(string id)
        {
            return Objects.FirstOrDefault(t => t._id == id);
        }
        #endregion
    }
}