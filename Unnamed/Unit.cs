using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Unnamed
{
    [Magic]
    public class Unit : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public string Name { get; set; }
        public string Race { get; set; }
        public string Bio { get; set; }
        public string Habitat { get; set; }
        public string Background { get; set; }
        public string Gender { get; set; }
        public string Avatar { get; set; }
        public string Class { get; set; }
        public string Subclass { get; set; }
        public string ElementType { get; set; } = "Normal";
        public string ElementEffect { get; set; }
        public int LVL { get; set; } = 1;
        public int EXP { get; set; } = 0;
        public int EXP4NextLVL => 75 + (LVL * 25);
        public int atPoints { get; set; } = 0;
        public int skPoints1 { get; set; } = 0;
        private static readonly List<string> DefaultRaces = new List<string>
        {
            "Floran", "Elf", "Human", "Half-Orc",
            "Dark Elf", "Tiefling", "Dragonborn", "Void Elf", "Demonborn", "Ascended", "Metalborn"
        };

        public List<StatData> Attributes { get; set; } = new List<StatData> { };

        public int HP_MOD { get; set; } = 0;
        public int maxHP => Attributes[2].Value * 4 + HP_MOD;
        public int HP { get; set; }
        public int MP_MOD { get; set; } = 0;
        public int maxMP => Attributes[3].Value * 4 + MP_MOD;
        public int MP { get; set; }
        public int SP_MOD { get; set; } = 0;
        public int maxSP => Attributes[1].Value + Attributes[2].Value + SP_MOD;
        public int SP { get; set; } = 0;
        public int WP_MOD { get; set; } = 0;
        public int maxWP => Attributes[4].Value * 4 + WP_MOD;
        public int WP { get; set; }
        public double carryWeight_MOD()
        {
            int value = 0;
            foreach (Item i in Inventory)
            {
                string s = i.Description;
                string[] itemProps = s.Split('\n');
                for (int j = 0; j < itemProps.Length; j++)
                {
                    string[] prop = itemProps[j].Split(' ');
                    if (prop[0] == "CW") value += Int32.Parse(prop[1]);
                }
            }
            return value;
        }
        public double maxCarryWeight => Attributes[0].Value + (Attributes[0].Value/5) + carryWeight_MOD();
        public double carryWeight => Inventory.Sum(x => x.TotalWeight);
        public double WeightMod => carryWeight / maxCarryWeight;

        public List<StatData> CombatSkills { get; set; } = new List<StatData> { };
        //public List<StatData> CivilSkills { get; set; } = new List<StatData> { };

        public ObservableCollection<StatData> CharTraits { get; set; } = new ObservableCollection<StatData> { };
        public ObservableCollection<Move> Passives { get; set; } = new ObservableCollection<Move> { };
        public ObservableCollection<StatData> Mutations { get; set; } = new ObservableCollection<StatData> { };

        public ObservableCollection<Move> MoveList { get; set; } = new ObservableCollection<Move> { };
        public ObservableCollection<Item> Inventory { get; set; } = new ObservableCollection<Item> { };
        public int Coins { get; set; } = 0;
        public int Cost => 10+((Rating)/10)*10;
        public int Rating => Attributes.Sum(x => x.Value)/2 + CombatSkills.Sum(x => x.Value)/2;

        Random r = new Random();

        public Unit()
        {
            PropertyChanged += new PropertyChangedEventHandler(SubPropertyChanged);
        }

        public Unit(bool IsCharacter)
        {
            PropertyChanged += new PropertyChangedEventHandler(SubPropertyChanged);

            generateUnitData();
        }

        public Unit(string name)
        {
            PropertyChanged += new PropertyChangedEventHandler(SubPropertyChanged);

            switch (name)
            {
                /*case "Wolf":
                    Attributes.Add(new StatData("STR", r.Next(40, 61)));
                    Attributes.Add(new StatData("DEX", r.Next(50, 76)));
                    Attributes.Add(new StatData("END", r.Next(10, 61)));
                    Attributes.Add(new StatData("INT", 15));
                    Attributes.Add(new StatData("WIS", r.Next(30, 61)));
                    Attributes.Add(new StatData("CHR", r.Next(0, 31)));

                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("STR")/2, Stat("STR"))));

                    Name = name; Class = "Beast"; Race = name;
                    if (r.Next(0, 2) == 0) Gender = "F"; else Gender = "M";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    Inventory.Add(new Item("Wolf pelt", 20, 1, "PA +2"));
                    if (r.Next(0, 100) < 25) Coins = r.Next(0, 25);

                    MoveList.Add(new Move("Bite", "STR\nBrawl\ncSP -6\ntHP -3\ntSP -2"));
                    MoveList.Add(new Move("Scratch", "STR\nBrawl\ncSP -3\ntHP -3"));
                    MoveList.Add(new Move("Grip", "STR\nBrawl\ncSP -11\ntHP -4\ntSP -6"));
                    break;
                case "Dire wolf":
                    Attributes.Add(new StatData("STR", r.Next(65, 86)));
                    Attributes.Add(new StatData("DEX", r.Next(50, 76)));
                    Attributes.Add(new StatData("END", r.Next(25, 76)));
                    Attributes.Add(new StatData("INT", 15));
                    Attributes.Add(new StatData("WIS", r.Next(30, 61)));
                    Attributes.Add(new StatData("CHR", r.Next(10, 41)));

                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("STR")/2, Stat("STR"))));

                    Name = name; Class = "Beast"; Race = name;
                    if (r.Next(0, 2) == 0) Gender = "F"; else Gender = "M";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    Inventory.Add(new Item("Wolf pelt", 20, 1, "PA +2"));
                    if (r.Next(0, 100) < 25) Coins = r.Next(0, 25);

                    MoveList.Add(new Move("Bite", "STR\nBrawl\ncSP -6\ntHP -3\ntSP -2"));
                    MoveList.Add(new Move("Scratch", "STR\nBrawl\ncSP -3\ntHP -3"));
                    MoveList.Add(new Move("Grip", "STR\nBrawl\ncSP -11\ntHP -4\ntSP -6"));
                    break;
                case "Ice wolf":
                    Attributes.Add(new StatData("STR", r.Next(70, 91)));
                    Attributes.Add(new StatData("DEX", r.Next(50, 76)));
                    Attributes.Add(new StatData("END", r.Next(15, 66)));
                    Attributes.Add(new StatData("INT", 35));
                    Attributes.Add(new StatData("WIS", r.Next(30, 61)));
                    Attributes.Add(new StatData("CHR", r.Next(20, 51)));

                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("STR") / 2, Stat("STR"))));

                    Name = name; Class = "Beast"; Race = name; ElementType = "Ice";
                    if (r.Next(0, 2) == 0) Gender = "F"; else Gender = "M";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    Inventory.Add(new Item("Ice Wolf pelt", 15, 1, "PA +2"));

                    MoveList.Add(new Move("Frost Bite", "STR\nBrawl\ncSP -6\ntHP -3\ntSP -2\nIce"));
                    MoveList.Add(new Move("Scratch", "STR\nBrawl\ncSP -3\ntHP -3"));
                    MoveList.Add(new Move("Cold breath", "END\nBrawl\ncSP -11\ntHP -4\ntSP -6\nIce"));
                    break;
                case "Werewolf":
                    Attributes.Add(new StatData("STR", r.Next(55, 76)));
                    Attributes.Add(new StatData("DEX", r.Next(40, 66)));
                    Attributes.Add(new StatData("END", r.Next(20, 71)));
                    Attributes.Add(new StatData("INT", r.Next(20, 51)));
                    Attributes.Add(new StatData("WIS", r.Next(25, 56)));
                    Attributes.Add(new StatData("CHR", r.Next(20, 51)));

                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("STR") / 2, Stat("STR"))));
                    CombatSkills.Add(new StatData("Persuasion", r.Next(Stat("CHR")/2, Stat("CHR"))));

                    Name = name; Class = "Beast"; Race = name;
                    if (r.Next(0, 2) == 0) Gender = "F"; else Gender = "M";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    Inventory.Add(new Item("Wolf pelt", 20, 1, "PA +2"));
                    if (r.Next(0, 100) < 25) Coins = r.Next(0, 25);

                    MoveList.Add(new Move("Bite", "STR\nBrawl\ncSP -6\ntHP -3\ntSP -2"));
                    MoveList.Add(new Move("Claws", "STR\nBrawl\ncSP -4\ntHP -4"));
                    MoveList.Add(new Move("Roar", "CHR\nPersuasion\ncSP -9\ntaWP -3"));
                    break;
                case "Bear":
                    Attributes.Add(new StatData("STR", r.Next(75, 96)));
                    Attributes.Add(new StatData("DEX", r.Next(30, 51)));
                    Attributes.Add(new StatData("END", r.Next(60, 81)));
                    Attributes.Add(new StatData("INT", 5));
                    Attributes.Add(new StatData("WIS", r.Next(45, 66)));
                    Attributes.Add(new StatData("CHR", r.Next(15, 36)));

                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("STR") / 2, Stat("STR"))));

                    Name = name; Class = "Beast"; Race = name;
                    if (r.Next(0, 2) == 0) Gender = "F"; else Gender = "M";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    Inventory.Add(new Item("Bear pelt", 50, 3, "PA +4"));
                    Inventory.Add(new Item("Bear claws", 2, 0.1, "Ingridient"));
                    if (r.Next(0, 100) < 15) Coins = r.Next(0, 25);

                    MoveList.Add(new Move("Bite", "STR\nBrawl\ncSP -6\ntHP -3\ntSP -2"));
                    MoveList.Add(new Move("Claws", "STR\nBrawl\ncSP -4\ntHP -4"));
                    break;
                case "Sabre cat":
                    Attributes.Add(new StatData("STR", r.Next(70, 91)));
                    Attributes.Add(new StatData("DEX", r.Next(50, 71)));
                    Attributes.Add(new StatData("END", r.Next(55, 76)));
                    Attributes.Add(new StatData("INT", 15));
                    Attributes.Add(new StatData("WIS", r.Next(40, 61)));
                    Attributes.Add(new StatData("CHR", r.Next(20, 41)));

                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("STR") / 2, Stat("STR"))));

                    Name = name; Class = "Beast"; Race = name;
                    if (r.Next(0, 2) == 0) Gender = "F"; else Gender = "M";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    Inventory.Add(new Item("Sabre cat pelt", 25, 2, "PA +3"));
                    if (r.Next(0, 2) == 0) Inventory.Add(new Item("Eye of Sabre cat", 2, 0.1, "Ingridient"));
                    Inventory.Add(new Item("Sabre cat tooth", 2, 0.1, "Ingridient"));
                    if (r.Next(0, 100) < 15) Coins = r.Next(0, 25);

                    MoveList.Add(new Move("Bite", "STR\nBrawl\ncSP -6\ntHP -3\ntSP -2"));
                    MoveList.Add(new Move("Claws", "STR\nBrawl\ncSP -4\ntHP -4"));
                    break;
                case "Raven":
                    Attributes.Add(new StatData("STR", 10));
                    Attributes.Add(new StatData("DEX", r.Next(35, 71)));
                    Attributes.Add(new StatData("END", 8));
                    Attributes.Add(new StatData("INT", 10));
                    Attributes.Add(new StatData("WIS", r.Next(30, 61)));
                    Attributes.Add(new StatData("CHR", r.Next(0, 31)));

                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("STR"), Stat("STR") * 2)));

                    Name = name; Class = "Bird"; Race = name;
                    if (r.Next(0, 2) == 0) Gender = "F"; else Gender = "M";
                    HP = maxHP; MP = maxMP; WP = maxWP;

                    MoveList.Add(new Move("Beak", "STR\nBrawl\ncSP -6\ntHP -2\ntSP -3"));
                    MoveList.Add(new Move("Scratch", "STR\nBrawl\ncSP -2\ntHP -2"));
                    MoveList.Add(new Move("Grip", "STR\nBrawl\ncSP -9\ntHP -2\ntSP -6"));
                    break;
                case "Insect swarm":
                    Attributes.Add(new StatData("STR", r.Next(5, 16)));
                    Attributes.Add(new StatData("DEX", r.Next(65, 86)));
                    Attributes.Add(new StatData("END", r.Next(20, 51)));
                    Attributes.Add(new StatData("INT", 5));
                    Attributes.Add(new StatData("WIS", r.Next(5, 36)));
                    Attributes.Add(new StatData("CHR", 5));

                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("STR"), Stat("STR") * 2)));

                    Name = name; Class = "Swarm"; Race = name; ElementType = "Bug";
                    HP = maxHP; MP = maxMP; WP = maxWP;

                    MoveList.Add(new Move("Bite", "STR\nBrawl\ncSP -6\ntHP -1\ntSP -4\nBug"));
                    break;
                case "Giant spider":
                    Attributes.Add(new StatData("STR", r.Next(50, 71)));
                    Attributes.Add(new StatData("DEX", r.Next(60, 81)));
                    Attributes.Add(new StatData("END", r.Next(40, 61)));
                    Attributes.Add(new StatData("INT", 10));
                    Attributes.Add(new StatData("WIS", r.Next(35, 56)));
                    Attributes.Add(new StatData("CHR", 20));

                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("STR")/2, Stat("STR"))));
                    CombatSkills.Add(new StatData("Throwing", r.Next(Stat("DEX")/2, Stat("DEX"))));

                    Name = name; Class = "Spider"; Race = name; ElementType = "Bug";
                    if (r.Next(0, 2) == 0) Gender = "F"; else Gender = "M";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    Inventory.Add(new Item("Spider's Venom", 21, 0.3, "DEX\nThrowing\ncSP -13\ntHP -6\ntSP -6\nPoison\nConsumable"));

                    MoveList.Add(new Move("Bite", "STR\nBrawl\ncSP -7\ntHP -4\ntSP -2\nBug"));
                    MoveList.Add(new Move("Web", "DEX\nThrowing\ncSP -10\ntSP -10\nBug"));
                    MoveList.Add(new Move("Poison Shot", "DEX\nThrowing\ncSP -12\ntHP -3\ntSP -9\nPoison"));
                    break;
                case "Mushroom":
                    Attributes.Add(new StatData("STR", 0));
                    Attributes.Add(new StatData("DEX", 0));
                    Attributes.Add(new StatData("END", 10));
                    Attributes.Add(new StatData("INT", r.Next(25, 51)));
                    Attributes.Add(new StatData("WIS", r.Next(15, 36)));
                    Attributes.Add(new StatData("CHR", 0));

                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("STR"), Stat("STR") * 2)));
                    CombatSkills.Add(new StatData("Throwing", r.Next(Stat("DEX"), Stat("DEX") * 2)));

                    switch (r.Next(0, 10))
                    {
                        case 0:
                            CombatSkills.Add(new StatData("Pyrokinetic", r.Next(Stat("INT"), Stat("INT") * 2)));
                            MoveList.Add(new Move("Fire", "INT\nPyrokinetic\ncSP -8\ncaHP -3\ntaHP -5\nFire"));
                            Name = "Red Mushroom";
                            break;
                        case 1:
                            CombatSkills.Add(new StatData("Aerotheurge", r.Next(Stat("INT"), Stat("INT") * 2)));
                            MoveList.Add(new Move("Charge", "INT\nAerotheurge\ncSP -8\ncaMP -3\ntaHP -2\ntaMP -3\nStorm"));
                            Name = "Sparky Mushroom";
                            break;
                        case 2:
                            CombatSkills.Add(new StatData("Hydrosophist", r.Next(Stat("INT"), Stat("INT") * 2)));
                            MoveList.Add(new Move("Healing steam", "INT\nHydrosophist\ncSP -8\ncaHP +2\ntaHP +2\ntaSP -2\nWater"));
                            Name = "Blue Mushroom";
                            break;
                        case 3:
                            CombatSkills.Add(new StatData("Hydrosophist", r.Next(Stat("INT"), Stat("INT") * 2)));
                            MoveList.Add(new Move("Freeze", "INT\nHydrosophist\ncSP -8\ncaSP -2\ntaSP -4\nIce"));
                            Name = "Frozen Mushroom";
                            break;
                        case 4:
                            CombatSkills.Add(new StatData("Geomagnetic", r.Next(Stat("INT"), Stat("INT") * 2)));
                            MoveList.Add(new Move("Mud Splash", "INT\nGeomagnetic\ncSP -8\ncaSP -2\ntaSP -4\nEarth"));
                            Name = "Mud Mushroom";
                            break;
                        case 5:
                            CombatSkills.Add(new StatData("ForceOfNature", r.Next(Stat("WIS"), Stat("WIS") * 2)));
                            MoveList.Add(new Move("Poison Spores", "WIS\nForceOfNature\ncSP -8\ncaSP -4\ncaHP -1\ntaHP -3\ntaSP -4\nPoison"));
                            Name = "Poison Mushroom";
                            break;
                        case 6:
                            CombatSkills.Add(new StatData("PowerOfLight", r.Next(Stat("WIS"), Stat("WIS") * 2)));
                            MoveList.Add(new Move("Restoration", "WIS\nPowerOfLight\ncSP -8\ncaHP +2\ntaSP -2\ntaHP +2\nLight"));
                            Name = "White Mushroom";
                            break;
                        case 7:
                            CombatSkills.Add(new StatData("Psionics", r.Next((Stat("WIS") + Stat("CHR")) / 2, (Stat("WIS") + Stat("CHR")))));
                            MoveList.Add(new Move("Psycho noise", "WIS\nPsionics\ncSP -8\ncaWP -2\ntaSP -2\ntaWP -2\nPsy"));
                            Name = "Ringing Mushroom";
                            break;
                        case 8:
                            CombatSkills.Add(new StatData("Entropy", r.Next((Stat("WIS") + Stat("INT")) / 2, (Stat("WIS") + Stat("INT")))));
                            MoveList.Add(new Move("Slow time", "INT\nEntropy\ncaSP -10\ntaSP -10\nVoid"));
                            Name = "Slow Mushroom";
                            break;
                        case 9:
                            CombatSkills.Add(new StatData("ForceOfNature", r.Next(Stat("WIS"), Stat("WIS") * 2)));
                            MoveList.Add(new Move("Pheromone cloud", "WIS\nPsionics\ncSP -8\ncaWP -2\ntaSP -2\ntaWP -2\nPoison"));
                            Name = "Fragrant Mushroom";
                            break;
                        default: break;
                    }

                    Class = "Mushroom"; Race = name; ElementType = "Plant";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    break;
                case "Planetar":
                    Attributes.Add(new StatData("STR", 120));
                    Attributes.Add(new StatData("DEX", 100));
                    Attributes.Add(new StatData("END", 120));
                    Attributes.Add(new StatData("INT", 95));
                    Attributes.Add(new StatData("WIS", 110));
                    Attributes.Add(new StatData("CHR", 125));

                    CombatSkills.Add(new StatData("PowerOfLight", Stat("WIS")));
                    CombatSkills.Add(new StatData("LongBlade", r.Next(Stat("STR") / 2, Stat("STR"))));

                    Name = name; Class = "Celestial"; Race = "Celestial";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    SetPersonalityTraits(); LVL = 0;

                    Inventory.Add(new Item("Planetar core", 10000000, 4.2, "PA +81\nMA +81\nMPR +8\nSPR +8\nHPR +8\nWPR +42"));
                    Inventory.Add(new Item("Greatsword", 140, 17, "STR\nLongBlade\ncSP -17\ntHP -17"));
                    MoveList.Add(new Move("Beam of Light", "CHR\nPowerOfLight\ncMP -10\ntHP -8\ntSP -2\nLight"));
                    MoveList.Add(new Move("Mass Heal", "CHR\nPowerOfLight\ncMP -15\ncaHP +5\nLight"));
                    break;
                case "Pit Fiend":
                    Attributes.Add(new StatData("STR", 130));
                    Attributes.Add(new StatData("DEX", 70));
                    Attributes.Add(new StatData("END", 120));
                    Attributes.Add(new StatData("INT", 110));
                    Attributes.Add(new StatData("WIS", 90));
                    Attributes.Add(new StatData("CHR", 120));

                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("STR") / 2, Stat("STR"))));
                    CombatSkills.Add(new StatData("BluntWeapon", r.Next(Stat("STR") / 2, Stat("STR"))));

                    Name = name; Class = "Fiend"; Race = "Fiend"; ElementType = "Void";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    LVL = 0;

                    Inventory.Add(new Item("Hellfire core", 66666, 6.6, "PA +66\nMPR +6\nSPR +6\nHPR +6"));
                    Inventory.Add(new Item("Mace", 90, 14, "STR\nBluntWeapon\ncSP -14\ntHP -14"));
                    MoveList.Add(new Move("Bite", "STR\nBrawl\ncSP -6\ntHP -4\ntSP -2\nFire"));
                    MoveList.Add(new Move("Claws", "STR\nBrawl\ncSP -6\ntHP -6\nSteel"));
                    MoveList.Add(new Move("Tail", "STR\nBrawl\ncSP -8\ntHP -4\ntSP -4"));
                    break;
                case "Fire Elemental":
                    Attributes.Add(new StatData("STR", r.Next(30, 51)));
                    Attributes.Add(new StatData("DEX", r.Next(65, 86)));
                    Attributes.Add(new StatData("END", r.Next(60, 81)));
                    Attributes.Add(new StatData("INT", r.Next(15, 31)));
                    Attributes.Add(new StatData("WIS", r.Next(30, 51)));
                    Attributes.Add(new StatData("CHR", r.Next(15, 36)));

                    CombatSkills.Add(new StatData("Pyrokinetic", r.Next(Stat("DEX")/2, Stat("DEX"))));

                    Name = name; Class = "Elemental"; Race = "Elemental";
                    Gender = "F"; ElementType = "Fire";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    Inventory.Add(new Item("Fire core", 50, 0.3, "MA +4"));

                    MoveList.Add(new Move("Fire Blast", "DEX\nPyrokinetic\ncSP -10\ntHP -10\nFire"));
                    MoveList.Add(new Move("Supernova", "END\nPyrokinetic\ncaHP -20\ntaHP -10\ntaWP -10\nFire"));
                    MoveList.Add(new Move("Infernal Embrace", "DEX\nPyrokinetic\ncHP -30\ncSP -10\ntHP -30\ntSP -10\nFire"));
                    break;
                case "Storm Elemental":
                    Attributes.Add(new StatData("STR", r.Next(50, 71)));
                    Attributes.Add(new StatData("DEX", 100));
                    Attributes.Add(new StatData("END", r.Next(57, 71)));
                    Attributes.Add(new StatData("INT", r.Next(15, 36)));
                    Attributes.Add(new StatData("WIS", r.Next(30, 51)));
                    Attributes.Add(new StatData("CHR", r.Next(15, 36)));

                    CombatSkills.Add(new StatData("Aerotheurge", r.Next(Stat("DEX")/2, Stat("DEX"))));

                    Name = name; Class = "Elemental"; Race = "Elemental";
                    if (r.Next(0, 2) == 0) Gender = "F"; else Gender = "M";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    ElementType = "Storm";
                    Inventory.Add(new Item("Storm core", 60, 0.3, "MA +5"));

                    MoveList.Add(new Move("Spark", "DEX\nAerotheurge\ncSP -12\ntHP -8\ntMP -4\nStorm"));
                    MoveList.Add(new Move("Superconductor", "END\nAerotheurge\ncaHP -5\ncaMP -10\ntaHP -10\ntaMP -5\nStorm"));
                    MoveList.Add(new Move("Chain Lightning", "DEX\nAerotheurge\ncSP -26\ntHP -6\ntMP -4\ntaHP -5\ntaMP -3\nStorm"));
                    break;
                case "Wisp":
                    Attributes.Add(new StatData("STR", 5));
                    Attributes.Add(new StatData("DEX", 140));
                    Attributes.Add(new StatData("END", r.Next(30, 51)));
                    Attributes.Add(new StatData("INT", r.Next(45, 66)));
                    Attributes.Add(new StatData("WIS", r.Next(50, 71)));
                    Attributes.Add(new StatData("CHR", r.Next(35, 56)));

                    CombatSkills.Add(new StatData("Aerotheurge", r.Next(Stat("INT")/2, Stat("INT"))));

                    Name = name; Class = "Elemental"; Race = "Wisp";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    ElementType = "Light";

                    MoveList.Add(new Move("Spark", "INT\nAerotheurge\ncMP -6\ntHP -4\ntMP -2\nStorm"));
                    break;
                case "Water Elemental":
                    Attributes.Add(new StatData("STR", r.Next(70, 91)));
                    Attributes.Add(new StatData("DEX", r.Next(50, 71)));
                    Attributes.Add(new StatData("END", r.Next(70, 91)));
                    Attributes.Add(new StatData("INT", 26));
                    Attributes.Add(new StatData("WIS", r.Next(31, 51)));
                    Attributes.Add(new StatData("CHR", r.Next(20, 41)));

                    CombatSkills.Add(new StatData("Hydrosophist", Stat("END")));
                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("STR")/2, Stat("STR"))));

                    Name = name; Class = "Elemental"; Race = "Elemental";
                    if (r.Next(0, 2) == 0) Gender = "F"; else Gender = "M";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    ElementType = "Water";
                    Inventory.Add(new Item("Water core", 70, 0.3, "MA +6"));

                    MoveList.Add(new Move("Slam", "STR\nBrawl\ncSP -10\ntSP -8\ntWP -2\nWater"));
                    MoveList.Add(new Move("Healing Steam", "END\nHydrosophist\ncMP -30\ncaHP +15\ncaSP -5\ntaHP +5\ntaSP -5\nWater"));
                    MoveList.Add(new Move("Airless Embrace", "DEX\nBrawl\ncHP -30\ncSP -10\ntHP -14\ntSP -26\nWater"));
                    break;
                case "Ice Golem":
                    Attributes.Add(new StatData("STR", 100));
                    Attributes.Add(new StatData("DEX", r.Next(15, 36)));
                    Attributes.Add(new StatData("END", 95));
                    Attributes.Add(new StatData("INT", 15));
                    Attributes.Add(new StatData("WIS", r.Next(35, 56)));
                    Attributes.Add(new StatData("CHR", 10));

                    CombatSkills.Add(new StatData("Hydrosophist", r.Next((int)(Stat("END")*0.75), Stat("END"))));
                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("STR")/2, Stat("STR"))));

                    Name = name; Class = "Golem"; Race = "Golem";
                    if (r.Next(0, 2) == 0) Gender = "F"; else Gender = "M";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    ElementType = "Ice";
                    Inventory.Add(new Item("Ice core", 100, 0.5, "PA +4"));

                    MoveList.Add(new Move("Ice Punch", "STR\nBrawl\ncSP -14\ntHP -8\ntSP -6\nIce"));
                    MoveList.Add(new Move("Ice Blade", "STR\nBrawl\ncSP -12\ntHP -10\ntSP -2\nIce"));
                    MoveList.Add(new Move("Blizzard", "END\nHydrosophist\ncMP -30\ncaSP -10\ntaSP -20\nIce"));
                    break;
                case "Flesh Golem":
                    Attributes.Add(new StatData("STR", 95));
                    Attributes.Add(new StatData("DEX", r.Next(25, 46)));
                    Attributes.Add(new StatData("END", 90));
                    Attributes.Add(new StatData("INT", 30));
                    Attributes.Add(new StatData("WIS", r.Next(35, 51)));
                    Attributes.Add(new StatData("CHR", 25));

                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("STR") / 2, Stat("STR"))));
                    CombatSkills.Add(new StatData("Throwing", r.Next(Stat("STR") / 2, Stat("STR"))));

                    Name = name; Class = "Golem"; Race = "Golem";
                    if (r.Next(0, 2) == 0) Gender = "F"; else Gender = "M";
                    HP = maxHP; MP = maxMP; WP = maxWP;

                    MoveList.Add(new Move("Slam", "STR\nBrawl\ncSP -12\ntHP -8\ntSP -4"));
                    break;
                case "Rock Golem":
                    Attributes.Add(new StatData("STR", 110));
                    Attributes.Add(new StatData("DEX", r.Next(15, 46)));
                    Attributes.Add(new StatData("END", 100));
                    Attributes.Add(new StatData("INT", 15));
                    Attributes.Add(new StatData("WIS", r.Next(35, 56)));
                    Attributes.Add(new StatData("CHR", 5));

                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("STR")/2, Stat("STR"))));
                    CombatSkills.Add(new StatData("Throwing", r.Next(Stat("STR")/2, Stat("STR"))));
                    ElementType = "Rock";

                    Name = name; Class = "Golem"; Race = "Golem";
                    if (r.Next(0, 2) == 0) Gender = "F"; else Gender = "M";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    Inventory.Add(new Item("Rock core", 150, 1, "SPR +2\nPA +5"));

                    MoveList.Add(new Move("Rock Punch", "STR\nBrawl\ncSP -12\ntHP -8\ntSP -4\nRock"));
                    MoveList.Add(new Move("Rock Throw", "STR\nThrowing\ncSP -12\ntHP -8\ntSP -4\nRock"));
                    MoveList.Add(new Move("Earthquake", "STR\nBrawl\ncSP -30\ncaSP -10\ntaSP -20\nEarth"));
                    break;
                case "Gargoyle":
                    Attributes.Add(new StatData("STR", 75));
                    Attributes.Add(new StatData("DEX", r.Next(35, 56)));
                    Attributes.Add(new StatData("END", 80));
                    Attributes.Add(new StatData("INT", 30));
                    Attributes.Add(new StatData("WIS", r.Next(35, 56)));
                    Attributes.Add(new StatData("CHR", 35));

                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("STR") / 2, Stat("STR"))));
                    ElementType = "Rock";

                    Name = name; Class = "Elemental"; Race = "Gargoyle";
                    if (r.Next(0, 2) == 0) Gender = "F"; else Gender = "M";
                    HP = maxHP; MP = maxMP; WP = maxWP;

                    MoveList.Add(new Move("Bite", "STR\nBrawl\ncSP -6\ntHP -3\ntSP -2\nRock"));
                    MoveList.Add(new Move("Claws", "STR\nBrawl\ncSP -6\ntHP -6\nRock"));
                    break;
                case "Iron Golem":
                    Attributes.Add(new StatData("STR", 120));
                    Attributes.Add(new StatData("DEX", r.Next(15, 46)));
                    Attributes.Add(new StatData("END", 100));
                    Attributes.Add(new StatData("INT", 15));
                    Attributes.Add(new StatData("WIS", r.Next(35, 56)));
                    Attributes.Add(new StatData("CHR", 5));

                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("STR") / 2, Stat("STR"))));
                    ElementType = "Steel";

                    Name = name; Class = "Golem"; Race = "Golem";
                    if (r.Next(0, 2) == 0) Gender = "F"; else Gender = "M";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    Inventory.Add(new Item("Magnetic core", 300, 2, "SPR +4\nPA +10"));
                    Inventory.Add(new Item("Iron ore", 2, 1, r.Next(5,25), "Material"));

                    MoveList.Add(new Move("Slam", "STR\nBrawl\ncSP -12\ntHP -8\ntSP -4\nSteel"));
                    MoveList.Add(new Move("Blade", "STR\nBrawl\ncSP -12\ntHP -12\nSteel"));
                    break;
                case "Animated Armor":
                    Attributes.Add(new StatData("STR", r.Next(50, 71)));
                    Attributes.Add(new StatData("DEX", r.Next(35, 56)));
                    Attributes.Add(new StatData("END", r.Next(45, 66)));
                    Attributes.Add(new StatData("INT", 5));
                    Attributes.Add(new StatData("WIS", 15));
                    Attributes.Add(new StatData("CHR", 5));

                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("STR") / 2, Stat("STR"))));
                    ElementType = "Steel";

                    Name = name; Class = "Armor"; Race = "Armor";
                    if (r.Next(0, 2) == 0) Gender = "F"; else Gender = "M";
                    HP = maxHP; MP = maxMP; WP = maxWP;

                    MoveList.Add(new Move("Slam", "STR\nBrawl\ncSP -12\ntHP -8\ntSP -4\nSteel"));
                    MoveList.Add(new Move("Blade", "STR\nBrawl\ncSP -12\ntHP -12\nSteel"));
                    break;
                case "Flying Sword":
                    Attributes.Add(new StatData("STR", r.Next(40, 61)));
                    Attributes.Add(new StatData("DEX", r.Next(55, 76)));
                    Attributes.Add(new StatData("END", r.Next(35, 56)));
                    Attributes.Add(new StatData("INT", 5));
                    Attributes.Add(new StatData("WIS", 25));
                    Attributes.Add(new StatData("CHR", 5));

                    CombatSkills.Add(new StatData("LongBlade", r.Next(Stat("STR"), Stat("STR")*2)));
                    ElementType = "Steel";

                    Name = name; Class = "Sword"; Race = "Sword";
                    HP = maxHP; MP = maxMP; WP = maxWP;

                    MoveList.Add(new Move("Strike", "STR\nLongBlade\ncSP -17\ntHP -17\nSteel"));
                    break;
                case "Treant":
                    Attributes.Add(new StatData("STR", 115));
                    Attributes.Add(new StatData("DEX", r.Next(15, 41)));
                    Attributes.Add(new StatData("END", 105));
                    Attributes.Add(new StatData("INT", r.Next(40, 61)));
                    Attributes.Add(new StatData("WIS", r.Next(60, 81)));
                    Attributes.Add(new StatData("CHR", r.Next(40, 61)));

                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("STR") / 2, Stat("STR"))));
                    CombatSkills.Add(new StatData("Throwing", r.Next(Stat("STR") / 2, Stat("STR"))));
                    ElementType = "Plant";

                    Name = name; Race = "Treant";
                    if (r.Next(0, 2) == 0) Gender = "F"; else Gender = "M";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    SetPersonalityTraits();
                    Inventory.Add(new Item("Firewood", 5, 5, r.Next(5, 25), "Material"));

                    MoveList.Add(new Move("Slam", "STR\nBrawl\ncSP -12\ntHP -8\ntSP -4\nPlant"));
                    MoveList.Add(new Move("Rock Throw", "STR\nThrowing\ncSP -12\ntHP -8\ntSP -4\nRock"));
                    break;
                case "Dryad":
                    Attributes.Add(new StatData("STR", r.Next(30, 51)));
                    Attributes.Add(new StatData("DEX", r.Next(40, 61)));
                    Attributes.Add(new StatData("END", r.Next(35, 56)));
                    Attributes.Add(new StatData("INT", r.Next(50, 71)));
                    Attributes.Add(new StatData("WIS", r.Next(55, 76)));
                    Attributes.Add(new StatData("CHR", r.Next(70, 91)));

                    CombatSkills.Add(new StatData("ForceOfNature", r.Next(Stat("WIS")/2, Stat("WIS"))));
                    CombatSkills.Add(new StatData("Psionics", r.Next((Stat("WIS") + Stat("CHR")) / 4, (Stat("WIS") + Stat("CHR"))/2)));
                    CombatSkills.Add(new StatData("Persuasion", r.Next(Stat("CHR")/2, Stat("CHR"))));

                    Name = name; Class = "Nymph"; Race = "Dryad";
                    Gender = "F"; ElementType = "Plant";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    SetPersonalityTraits();

                    MoveList.Add(new Move("Living Roots", "WIS\nForceOfNature\ncMP -15\ntHP -4\ntSP -11\nPlant"));
                    MoveList.Add(new Move("Charm", "CHR\nPersuasion\ncSP -20\ntWP -20\nPsy"));
                    MoveList.Add(new Move("Dominate Mind", "INT\nPsionics\ncMP -20\ntWP -20\nPsy"));
                    break;
                case "Zombie":
                    Attributes.Add(new StatData("STR", r.Next(45, 66)));
                    Attributes.Add(new StatData("DEX", r.Next(10, 31)));
                    Attributes.Add(new StatData("END", r.Next(60, 81)));
                    Attributes.Add(new StatData("INT", 15));
                    Attributes.Add(new StatData("WIS", r.Next(10, 31)));
                    Attributes.Add(new StatData("CHR", r.Next(5, 26)));

                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("DEX") / 2, Stat("DEX"))));

                    Name = name; Class = "Undead"; Race = "Undead";
                    if (r.Next(0, 2) == 0) Gender = "F"; else Gender = "M"; ElementType = "Poison";
                    HP = maxHP; MP = maxMP; WP = maxWP;

                    MoveList.Add(new Move("Slam", "STR\nBrawl\ncSP -12\ntHP -8\ntSP -4\nPoison"));
                    MoveList.Add(new Move("Bite", "DEX\nBrawl\ncSP -6\ntHP -3\ntSP -2\nPoison"));
                    break;
                case "Ghoul":
                    Attributes.Add(new StatData("STR", r.Next(45, 66)));
                    Attributes.Add(new StatData("DEX", r.Next(55, 76)));
                    Attributes.Add(new StatData("END", r.Next(30, 51)));
                    Attributes.Add(new StatData("INT", r.Next(15, 36)));
                    Attributes.Add(new StatData("WIS", r.Next(30, 51)));
                    Attributes.Add(new StatData("CHR", r.Next(10, 31)));

                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("DEX") / 2, Stat("DEX"))));

                    Name = name; Class = "Monster"; Race = "Ghoul";
                    if (r.Next(0, 2) == 0) Gender = "F"; else Gender = "M"; ElementType = "Poison";
                    HP = maxHP; MP = maxMP; WP = maxWP;

                    MoveList.Add(new Move("Bite", "DEX\nBrawl\ncSP -6\ntHP -3\ntSP -2\nPoison"));
                    MoveList.Add(new Move("Claws", "STR\nBrawl\ncSP -4\ntHP -4\nPoison"));
                    break;
                case "Ghost":
                    Attributes.Add(new StatData("STR", r.Next(15, 36)));
                    Attributes.Add(new StatData("DEX", r.Next(45, 66)));
                    Attributes.Add(new StatData("END", r.Next(30, 51)));
                    Attributes.Add(new StatData("INT", r.Next(30, 51)));
                    Attributes.Add(new StatData("WIS", r.Next(40, 61)));
                    Attributes.Add(new StatData("CHR", r.Next(65, 86)));

                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("DEX") / 2, Stat("DEX"))));
                    CombatSkills.Add(new StatData("Psionics", r.Next(Stat("CHR") / 2, Stat("CHR"))));

                    Name = name; Class = "Ghost"; Race = "Ghost"; ElementType = "Void";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    SetPersonalityTraits(); SetCharBio();

                    MoveList.Add(new Move("Withering Touch", "DEX\nBrawl\ncSP -4\ntHP -4\nVoid"));
                    MoveList.Add(new Move("Horrifying Visage", "CHR\nPsionics\ncMP -20\ntWP -20\nPsy"));
                    MoveList.Add(new Move("Possession", "CHR\nPsionics\ncMP -20\ntWP -20\nPsy"));
                    break;
                case "Shadow":
                    Attributes.Add(new StatData("STR", r.Next(10, 31)));
                    Attributes.Add(new StatData("DEX", r.Next(50, 71)));
                    Attributes.Add(new StatData("END", r.Next(45, 66)));
                    Attributes.Add(new StatData("INT", r.Next(10, 31)));
                    Attributes.Add(new StatData("WIS", r.Next(30, 51)));
                    Attributes.Add(new StatData("CHR", r.Next(20, 41)));

                    CombatSkills.Add(new StatData("Entropy", r.Next(Stat("DEX") / 2, Stat("DEX"))));

                    Name = name; Class = "Shadow"; Race = "Ghost"; ElementType = "Void";
                    HP = maxHP; MP = maxMP; WP = maxWP;

                    MoveList.Add(new Move("Drain Magic", "DEX\nEntropy\ncSP -13\ntMP -13\nVoid"));
                    MoveList.Add(new Move("Drain Will", "DEX\nEntropy\ncSP -13\ntWP -13\nVoid"));
                    MoveList.Add(new Move("Weakness", "DEX\nEntropy\ncSP -13\ntSP -13\nVoid"));
                    MoveList.Add(new Move("Shadow Embrace", "DEX\nEntropy\ncSP -40\ntHP -40\nVoid"));
                    break;
                case "Wraith":
                    Attributes.Add(new StatData("STR", r.Next(10, 31)));
                    Attributes.Add(new StatData("DEX", r.Next(60, 81)));
                    Attributes.Add(new StatData("END", r.Next(60, 81)));
                    Attributes.Add(new StatData("INT", r.Next(40, 61)));
                    Attributes.Add(new StatData("WIS", r.Next(50, 71)));
                    Attributes.Add(new StatData("CHR", r.Next(55, 76)));

                    CombatSkills.Add(new StatData("Entropy", r.Next(Stat("DEX") / 2, Stat("DEX"))));

                    Name = name; Class = "Wraith"; Race = "Ghost"; ElementType = "Void";
                    HP = maxHP; MP = maxMP; WP = maxWP;

                    MoveList.Add(new Move("Wither mana", "DEX\nEntropy\ncSP -14\ntMP -14\nVoid"));
                    MoveList.Add(new Move("Wither will", "DEX\nEntropy\ncSP -14\ntWP -14\nVoid"));
                    MoveList.Add(new Move("Weakness", "DEX\nEntropy\ncSP -13\ntSP -13\nVoid"));
                    MoveList.Add(new Move("Wither life", "DEX\nEntropy\ncSP -14\ntHP -14\nVoid"));
                    break;
                case "Skeleton":
                    Attributes.Add(new StatData("STR", r.Next(30, 51)));
                    Attributes.Add(new StatData("DEX", r.Next(50, 71)));
                    Attributes.Add(new StatData("END", r.Next(55, 76)));
                    Attributes.Add(new StatData("INT", r.Next(10, 31)));
                    Attributes.Add(new StatData("WIS", r.Next(20, 41)));
                    Attributes.Add(new StatData("CHR", r.Next(5, 26)));

                    CombatSkills.Add(new StatData("Accuracy", r.Next(Stat("DEX") / 3, Stat("DEX"))));
                    CombatSkills.Add(new StatData("ShortBlade", r.Next(Stat("DEX") / 3, Stat("DEX"))));
                    CombatSkills.Add(new StatData("LongBlade", r.Next(Stat("STR") / 3, Stat("STR"))));

                    Name = name; Class = "Undead"; Race = "Undead"; ElementType = "Void";
                    HP = maxHP; MP = maxMP; WP = maxWP;

                    Inventory.Add(new Item("Sword", 70, 10, "STR\nLongBlade\ncSP -10\ntHP -10"));
                    Inventory.Add(new Item("Short Bow", 70, 9, "DEX\nAccuracy\ncSP -10\ntHP -10\nUses Arrow"));
                    Inventory.Add(new Item("Arrow", 1, 0.025, 20, "DEX\nShortBlade\ncSP -4\ntHP -4"));
                    break;
                case "Harpy":
                    Attributes.Add(new StatData("STR", r.Next(40, 61)));
                    Attributes.Add(new StatData("DEX", r.Next(45, 66)));
                    Attributes.Add(new StatData("END", r.Next(40, 61)));
                    Attributes.Add(new StatData("INT", r.Next(15, 36)));
                    Attributes.Add(new StatData("WIS", r.Next(30, 51)));
                    Attributes.Add(new StatData("CHR", r.Next(45, 66)));

                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("DEX") / 2, Stat("DEX"))));
                    CombatSkills.Add(new StatData("Psionics", r.Next(Stat("CHR") / 2, Stat("CHR"))));

                    Name = name; Class = "Monster"; Race = "Harpy"; Gender = "F";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    SetPersonalityTraits();

                    MoveList.Add(new Move("Claws", "STR\nBrawl\ncSP -4\ntHP -4\nPoison"));
                    MoveList.Add(new Move("Luring Song", "CHR\nPsionics\ncSP -40\ntaWP -13\nPsy"));
                    MoveList.Add(new Move("Soothing Song", "CHR\nPsionics\ncSP -40\ntaWP -13\nPsy"));
                    break;
                case "Banshee":
                    Attributes.Add(new StatData("STR", 5));
                    Attributes.Add(new StatData("DEX", r.Next(50, 71)));
                    Attributes.Add(new StatData("END", r.Next(30, 51)));
                    Attributes.Add(new StatData("INT", r.Next(40, 61)));
                    Attributes.Add(new StatData("WIS", r.Next(35, 56)));
                    Attributes.Add(new StatData("CHR", r.Next(65, 86)));

                    CombatSkills.Add(new StatData("Entropy", r.Next(Stat("INT") / 2, Stat("INT"))));
                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("DEX") / 2, Stat("DEX"))));
                    CombatSkills.Add(new StatData("Psionics", r.Next((Stat("WIS") + Stat("CHR")) / 4, (Stat("WIS") + Stat("CHR")) / 2)));

                    Name = name; Class = "Undead"; Race = "Undead";
                    Gender = "F"; ElementType = "Void";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    SetPersonalityTraits();

                    MoveList.Add(new Move("Corrupting Touch", "DEX\nBrawl\ncSP -4\ntHP -4\nVoid"));
                    MoveList.Add(new Move("Horrifying Visage", "CHR\nPsionics\ncMP -20\ntWP -20\nPsy"));
                    MoveList.Add(new Move("Wail", "INT\nEntropy\ncMP -20\ncSP -20\ntHP -24\ntWP -10\ntSP -4\nPsy"));
                    break;
                case "Succubus":
                    Attributes.Add(new StatData("STR", r.Next(20, 41)));
                    Attributes.Add(new StatData("DEX", r.Next(65, 86)));
                    Attributes.Add(new StatData("END", r.Next(40, 66)));
                    Attributes.Add(new StatData("INT", r.Next(55, 76)));
                    Attributes.Add(new StatData("WIS", r.Next(40, 61)));
                    Attributes.Add(new StatData("CHR", 100));

                    CombatSkills.Add(new StatData("Entropy", r.Next(Stat("INT")/2, Stat("INT"))));
                    CombatSkills.Add(new StatData("Brawl", r.Next(Stat("DEX")/2, Stat("DEX"))));
                    CombatSkills.Add(new StatData("Psionics", r.Next((Stat("WIS") + Stat("CHR")) / 4, (Stat("WIS") + Stat("CHR"))/2)));
                    CombatSkills.Add(new StatData("Persuasion", r.Next(Stat("CHR")/2, Stat("CHR"))));

                    Name = name; Class = "Fiend"; Race = "Fiend";
                    Gender = "F"; ElementType = "Void";
                    HP = maxHP; MP = maxMP; WP = maxWP;
                    SetPersonalityTraits();
                    if (r.Next(0, 2) == 0) Coins = r.Next(0, 75);

                    MoveList.Add(new Move("Draining Claws", "DEX\nBrawl\ncSP -10\ntHP -5\nDrain\nVoid"));
                    MoveList.Add(new Move("Demonic Charm", "CHR\nPsionics\ncMP -20\ntWP -20\nPsy"));
                    MoveList.Add(new Move("Draining Kiss", "INT\nEntropy\ncMP -20\ncSP -20\ntWP -18\ntHP -18\ntSP -4\nPsy"));
                    break;
                case "Empty":
                    Attributes.Add(new StatData("STR", 0));
                    Attributes.Add(new StatData("DEX", 0));
                    Attributes.Add(new StatData("END", 0));
                    Attributes.Add(new StatData("INT", 0));
                    Attributes.Add(new StatData("WIS", 0));
                    Attributes.Add(new StatData("CHR", 0));

                    CombatSkills.Add(new StatData("Pyrokinetic", 0)); //0
                    CombatSkills.Add(new StatData("Aerotheurge", 0));//1
                    CombatSkills.Add(new StatData("Hydrosophist", 0));//2
                    CombatSkills.Add(new StatData("Geomagnetic", 0));//3

                    CombatSkills.Add(new StatData("PowerOfLight", 0));//4
                    CombatSkills.Add(new StatData("ForceOfNature", 0));//5
                    CombatSkills.Add(new StatData("Entropy", 0));//6
                    CombatSkills.Add(new StatData("Psionics", 0));//7

                    CombatSkills.Add(new StatData("Accuracy", 0));//8
                    CombatSkills.Add(new StatData("Throwing", 0));//9
                    CombatSkills.Add(new StatData("ShortBlade", 0));//10
                    CombatSkills.Add(new StatData("Whip", 0));//11
                    CombatSkills.Add(new StatData("Stave", 0));//12
                    CombatSkills.Add(new StatData("Polearms", 0));//13

                    CombatSkills.Add(new StatData("Brawl", 0));//14
                    CombatSkills.Add(new StatData("LongBlade", 0));//15
                    CombatSkills.Add(new StatData("Axe", 0));//16
                    CombatSkills.Add(new StatData("BluntWeapon", 0));//17
                    CombatSkills.Add(new StatData("Block", 0));//18

                    CombatSkills.Add(new StatData("Persuasion", 0));//1
                    CombatSkills.Add(new StatData("Performance", 0));//2

                    SetPersonalityTraits();
                    GenerateAvatar();
                    break;*/
                default:
                    if (DefaultRaces.Contains(name)) { Race = name; generateUnitData(); }
                    else generateUnitData();
                    break;
            }
        }
        /*
         * 1) Сделать так чтобы зелья тоже использовались
         * 2) Запилить систему инвентаря/экипировки
         * 3) Починить систему генерации персонажей/умений и т.п.
         * 4) Добавить проверку на отсутствие необходимого предмета с возвратом
         * */

        public void generateUnitData()
        {
            Attributes.Add(new StatData("STR", RollAttr()));
            Attributes.Add(new StatData("DEX", RollAttr()));
            Attributes.Add(new StatData("END", RollAttr()));
            Attributes.Add(new StatData("INT", RollAttr()));
            Attributes.Add(new StatData("WIS", RollAttr()));
            Attributes.Add(new StatData("CHR", RollAttr()));

            {
                int balanceRate = Attributes.Sum(x => x.Value);
                if (balanceRate < 60) LVL = 1;
                else if (balanceRate < 80) LVL = 2;
                else if (balanceRate < 95) LVL = 3;
                else if (balanceRate < 110) LVL = 4;
                else LVL = 5;
            }

            CombatSkills.Add(new StatData("Pyrokinetic", r.Next(Stat("INT"), Stat("INT")*5))); //0
            CombatSkills.Add(new StatData("Aerotheurge", r.Next(Stat("INT"), Stat("INT")*5)));//1
            CombatSkills.Add(new StatData("Hydrosophist", r.Next(Stat("INT"), Stat("INT")*5)));//2
            CombatSkills.Add(new StatData("Geomagnetic", r.Next(Stat("INT"), Stat("INT")*5)));//3

            CombatSkills.Add(new StatData("PowerOfLight", r.Next(Stat("WIS"), Stat("WIS")*5)));//4
            CombatSkills.Add(new StatData("ForceOfNature", r.Next(Stat("WIS"), Stat("WIS")*5)));//5
            CombatSkills.Add(new StatData("Entropy", r.Next((Stat("WIS") + Stat("INT")) / 2, (Stat("WIS") + Stat("INT"))*5 / 2)));//6
            CombatSkills.Add(new StatData("Psionics", r.Next((Stat("WIS") + Stat("CHR")) / 2, (Stat("WIS") + Stat("CHR"))*5 / 2)));//7

            CombatSkills.Add(new StatData("SharpShooting", r.Next(Stat("DEX"), Stat("DEX")*5)));//8
            CombatSkills.Add(new StatData("Throwing", r.Next(Stat("DEX"), Stat("DEX")*5)));//9
            CombatSkills.Add(new StatData("ShortBlade", r.Next(Stat("DEX"), Stat("DEX")*5)));//10

            CombatSkills.Add(new StatData("Brawl", r.Next(Stat("STR"), Stat("STR")*5)));//11
            CombatSkills.Add(new StatData("Sword", r.Next(Stat("STR"), Stat("STR")*5)));//12
            CombatSkills.Add(new StatData("Axe", r.Next(Stat("STR"), Stat("STR")*5)));//13
            CombatSkills.Add(new StatData("Blunt", r.Next(Stat("STR"), Stat("STR")*5)));//14

            Coins = r.Next(0, Cost / 2);
            if (Race == null) GenerateRace();
            else SetRaceBonuces();
            setSubclass();
            setStarterPack();
            switch (r.Next(0, 2))
            {
                case 0:
                    Gender = "F"; break;
                case 1:
                    Gender = "M"; break;
                default: break;
            }
            HP = maxHP; MP = maxMP; WP = maxWP;
            CombatSkills = CombatSkills.OrderBy(o => o.Value).ToList();
            CombatSkills.RemoveRange(0, r.Next(11, 15));
            GenerateCodename();
            GenerateAvatar();
        }

        public int Stat(string name)
        {
            List<StatData> chosenOne = new List<StatData>(Attributes);
            chosenOne.AddRange(CombatSkills);
            if (chosenOne.FirstOrDefault(x => x.Name == name) != null)
                return chosenOne.FirstOrDefault(x => x.Name == name).Value;
            else return 0;
        }

        public int RollAttr()
        {
            List<int> rolls = new List<int> { r.Next(1, 7), r.Next(1, 7), r.Next(1, 7), r.Next(1, 7) };
            rolls.Sort(); rolls.Remove(rolls[0]);
            return rolls.Sum();
        }

        public void InventoryChanged()
        {
            RaisePropertyChanged("Inventory");
            RaisePropertyChanged("carryWeight");
            RaisePropertyChanged("maxCarryWeight");
        }

        public void IncStat(string name, int value)
        {
            List<StatData> chosenOne = new List<StatData>(Attributes);
            chosenOne.AddRange(CombatSkills);
            chosenOne.Find(x => x.Name == name).Value += value;
        }

        public List<StatData> AllStats()
        {
            List<StatData> all = new List<StatData> { };
            all.AddRange(Attributes);
            all.AddRange(CombatSkills);
            return all;
        }

        public List<StatData> AllSkills()
        {
            List<StatData> all = new List<StatData> { };
            all.AddRange(CombatSkills);
            return all;
        }

        public void EncountStat(string name, int value)
        {
            List<StatData> chosenOne = new List<StatData>(Attributes);
            chosenOne.AddRange(CombatSkills);
            chosenOne.Find(x => x.Name == name).EXP += value;
            if (chosenOne.Find(x => x.Name == name).EXP >= 10) {
                chosenOne.Find(x => x.Name == name).EXP = 0;
                if (chosenOne.Find(x => x.Name == name).Value < 80)
                    chosenOne.Find(x => x.Name == name).Value++;
            }
        }

        public int Armor(string type)
        {
            int totalArmor = 0;
            foreach (Item i in Inventory)
            {
                string s = i.Description;
                string[] itemProps = s.Split('\n');
                for (int j = 0; j < itemProps.Length; j++)
                {
                    string[] prop = itemProps[j].Split(' ');
                    if (prop[0] == "PA" && type.ToLower() == "physical") totalArmor += Int32.Parse(prop[1]);
                    else if (prop[0] == "MA" && type.ToLower() == "magic") totalArmor += Int32.Parse(prop[1]);
                }
            }
            return totalArmor;
        }

        public void SetRaceBonuces()
        {
            switch (Race)
            {
                case "Elf":
                    IncStat("DEX", 2);
                    IncStat("INT", 2);
                    IncStat("CHR", 2);
                    List<StatData> highElfList = new List<StatData> { CombatSkills[0], CombatSkills[1], CombatSkills[2], CombatSkills[3], CombatSkills[5], CombatSkills[6], CombatSkills[7], CombatSkills[8], CombatSkills[10], CombatSkills[11] };
                    List<StatData> highElfSorted = highElfList.OrderByDescending(o => o.Value).ToList();
                    highElfSorted[0].Value += 6;
                    break;
                case "Human":
                    IncStat("STR", 1);
                    IncStat("DEX", 1);
                    IncStat("END", 1);
                    IncStat("INT", 1);
                    IncStat("WIS", 1);
                    IncStat("CHR", 1);
                    break;
                case "Orc":
                    IncStat("STR", 2);
                    IncStat("END", 1);
                    List<StatData> orcList = new List<StatData> { CombatSkills[15], CombatSkills[16], CombatSkills[17] };
                    List<StatData> orcListSorted = orcList.OrderByDescending(o => o.Value).ToList();
                    orcListSorted[0].Value += 14;
                    break;
                default: break;
            }
        }

        public void GenerateRace()
        {
            Race = DefaultRaces[r.Next(0, 2)];
            SetRaceBonuces();
        }

        public void Jump()
        {
            RaisePropertyChanged("HP_MOD");
            RaisePropertyChanged("MP_MOD");
            RaisePropertyChanged("SP_MOD");
            RaisePropertyChanged("WP_MOD");
            RaisePropertyChanged("maxCarryWeight");
        }

        public void GenerateCodename()
        {
            if (File.Exists("f_codenames.txt") && Gender == "F")
            {
                string[] nicknameCollection = File.ReadAllText("f_codenames.txt").Split('\n');
                Name = nicknameCollection[r.Next(0, nicknameCollection.Length)].Trim('\r');
            }
            else if (File.Exists("m_codenames.txt") && Gender == "M")
            {
                string[] nicknameCollection = File.ReadAllText("m_codenames.txt").Split('\n');
                Name = nicknameCollection[r.Next(0, nicknameCollection.Length)].Trim('\r');
            }
            else if (File.Exists("codenames.txt"))
            {
                string[] nicknameCollection = File.ReadAllText("codenames.txt").Split('\n');
                Name = nicknameCollection[r.Next(0, nicknameCollection.Length)].Trim('\r');
            }
            else
            {
                Name = "#" + r.Next(0, 1000000);
            }
        }

        public void GenerateAvatar()
        {
            if (!File.Exists(Avatar))
            {
                Avatar = "Resources/empty.png";
            }
        }

        public void setStarterPack()
        {
            MoveList.Add(Move.Find("Punch"));
            switch (Class)
            {
                case "Wizard":
                    List<StatData> WizMagicList = new List<StatData> { CombatSkills[0], CombatSkills[1], CombatSkills[2], CombatSkills[3] };
                    List<StatData> WizMagicSortedList = WizMagicList.OrderByDescending(o => o.Value).ToList();
                    if (WizMagicSortedList[0].Value <= 25) Inventory.Add(Item.Find("Novice Robe"));
                    else if (WizMagicSortedList[0].Value < 50) Inventory.Add(Item.Find("Apprentice Robe"));
                    else if (WizMagicSortedList[0].Value < 70) Inventory.Add(Item.Find("Adept Robe"));
                    if (r.Next(0,2) == 0 && WizMagicSortedList[0].Value >= 40 && WizMagicSortedList[1].Value >= 40) {
                        Subclass = "Elementalist";
                        if (Stat("ShortBlade") >= 40)
                        {
                            Inventory.Add(Item.Find("Dagger"));

                            if (Stat("Hydrosophist") >= 40) { MoveList.Add(Move.Find("Ice Dagger")); }
                            if (Stat("Geomagnetic") >= 40) { MoveList.Add(Move.Find("Rock Dagger")); }
                        }
                        if (Stat("Pyrokinetic") >= 25) { MoveList.Add(Move.Find("Fire Blast")); }
                        if (Stat("Aerotheurge") >= 25) { MoveList.Add(Move.Find("Spark")); }
                        if (Stat("Hydrosophist") >= 25) { MoveList.Add(Move.Find("Ice Shard")); }
                        if (Stat("Geomagnetic") >= 25) { MoveList.Add(Move.Find("Rock Throw")); }
                    } else
                    {
                        switch (WizMagicSortedList[0].Name)
                        {
                            case "Pyrokinetic":
                                Subclass = "Pyromancer";
                                if (Stat("ShortBlade") >= 40)
                                {
                                    Inventory.Add(Item.Find("Dagger"));
                                }
                                MoveList.Add(Move.Find("Fire Blast"));
                                if (Stat("Pyrokinetic") >= 40) MoveList.Add(Move.Find("Haste"));
                                if (Stat("Pyrokinetic") >= 60) MoveList.Add(Move.Find("Explosion"));
                                break;
                            case "Aerotheurge":
                                Subclass = "Stormcaller";
                                if (Stat("ShortBlade") >= 40)
                                {
                                    Inventory.Add(Item.Find("Dagger"));
                                }
                                MoveList.Add(Move.Find("Spark"));
                                if (Stat("Aerotheurge") >= 40) MoveList.Add(Move.Find("Windfury"));
                                if (Stat("Aerotheurge") >= 60) MoveList.Add(Move.Find("Tailwind"));
                                break;
                            case "Hydrosophist":
                                Subclass = "Cryomancer";
                                if (Stat("ShortBlade") >= 40)
                                {
                                    Inventory.Add(Item.Find("Dagger"));
                                    MoveList.Add(Move.Find("Ice Dagger"));
                                }
                                if (Stat("Hydrosophist") >= 40)
                                    if (r.Next(0, 2) == 0) MoveList.Add(Move.Find("Restoration"));
                                    else MoveList.Add(Move.Find("Mana Flow"));
                                switch (r.Next(0, 3))
                                {
                                    case 0:
                                        MoveList.Add(Move.Find("Ice Shard"));
                                        break;
                                    case 1:
                                        MoveList.Add(Move.Find("Steam Lance"));
                                        break;
                                    case 2:
                                        MoveList.Add(Move.Find("Deep Freeze"));
                                        break;
                                    default: break;
                                }
                                if (Stat("Hydrosophist") >= 60) MoveList.Add(Move.Find("Blizzard"));
                                break;
                            case "Geomagnetic":
                                Subclass = "Geomancer";
                                if (Stat("ShortBlade") >= 40)
                                {
                                    Inventory.Add(Item.Find("Dagger"));
                                    MoveList.Add(Move.Find("Rock Dagger"));
                                }
                                MoveList.Add(Move.Find("Rock Throw"));
                                if (Stat("Hydrosophist") >= 40) MoveList.Add(Move.Find("Earth Strike"));
                                if (Stat("Hydrosophist") >= 60) MoveList.Add(Move.Find("Impalement"));
                                break;
                            default: break;
                        }
                    }
                    break;
                case "Priest":
                    Subclass = "Priest";
                    Inventory.Add(Item.Find("Robe of innocence"));
                    if (Stat("ShortBlade") >= 40)
                    {
                        Inventory.Add(Item.Find("Dagger"));
                    }
                    if (Stat("PowerOfLight") >= 20)
                    {
                        MoveList.Add(Move.Find("Dazzling Light"));
                        MoveList.Add(Move.Find("Small Heal"));
                        if (Stat("PowerOfLight") >= 50) MoveList.Add(Move.Find("Mass Heal"));
                        if (Stat("PowerOfLight") >= 75) MoveList.Add(Move.Find("Dazzling Flash"));
                    };
                    break;
                case "Druid":
                    Inventory.Add(Item.Find("Green Wear"));
                    Subclass = "Druid";

                    if (Stat("ShortBlade") >= 40)
                    {
                        Inventory.Add(Item.Find("Dagger"));
                        MoveList.Add(Move.Find("Claw Cut"));
                    }
                    switch(r.Next(0,5))
                    {
                        case 0:
                            if (Stat("ForceOfNature") >= 20) MoveList.Add(Move.Find("Razor feather"));
                            if (Stat("ForceOfNature") >= 40) MoveList.Add(Move.Find("Feather wave"));
                            break;
                        case 1:
                            if (Stat("ForceOfNature") >= 20) MoveList.Add(Move.Find("Thorn shot"));
                            if (Stat("ForceOfNature") >= 40) MoveList.Add(Move.Find("Thorns wave"));
                            break;
                        case 2:
                            if (Stat("ForceOfNature") >= 20) MoveList.Add(Move.Find("Poison shot"));
                            if (Stat("ForceOfNature") >= 40) MoveList.Add(Move.Find("Web"));
                            break;
                        case 3:
                            if (Stat("ForceOfNature") >= 20) MoveList.Add(Move.Find("Poison Spores"));
                            if (Stat("ForceOfNature") >= 40) MoveList.Add(Move.Find("Pheromone Spores"));
                            break;
                        default: break;
                    }
                    switch (r.Next(0, 2))
                    {
                        case 0:
                            MoveList.Add(Move.Find("Living Roots"));
                            break;
                        case 1:
                            MoveList.Add(Move.Find("Razor Leafs"));
                            break;
                        default: break;
                    }
                    break;
                case "Witch Blade":
                    Subclass = "Witch Blade";
                    if (r.Next(0,2)==0) Inventory.Add(Item.Find("Hide Armor"));
                    else Inventory.Add(Item.Find("Studded Leather Armor"));
                    List<StatData> WBSkillList = new List<StatData> { CombatSkills[12], CombatSkills[13], CombatSkills[14] };
                    List<StatData> WBSkillSortedList = WBSkillList.OrderByDescending(o => o.Value).ToList();
                    switch (WBSkillSortedList[0].Name)
                    {
                        case "Sword":
                            if (r.Next(0, 2) == 0 || maxCarryWeight - carryWeight - 17 <= 0)
                            {
                                Inventory.Add(Item.Find("Sword"));
                                if (r.Next(0, 2) == 0 || Class == "Paladin") Inventory.Add(Item.Find("Shield"));
                            }
                            else
                            {
                                Inventory.Add(Item.Find("Greatsword"));
                            }
                            break;
                        case "Blunt":
                            if (r.Next(0, 2) == 0 || maxCarryWeight - carryWeight - 25 <= 0)
                            {
                                Inventory.Add(Item.Find("Mace"));
                                if (r.Next(0, 2) == 0 || Class == "Paladin") Inventory.Add(Item.Find("Shield"));
                            }
                            else
                            {
                                Inventory.Add(Item.Find("Warhammer"));
                            }
                            break;
                        case "Axe":
                            if (r.Next(0, 2) == 0 || maxCarryWeight - carryWeight - 21 <= 0)
                            {
                                Inventory.Add(Item.Find("War Axe"));
                                if (r.Next(0, 2) == 0 || Class == "Paladin") Inventory.Add(Item.Find("Shield"));
                            }
                            else
                            {
                                Inventory.Add(Item.Find("Battleaxe"));
                            }
                            break;
                        default: break;
                    }
                    if (Stat("Pyrokinetic") >= 40) { MoveList.Add(Move.Find("Fire Blast")); }
                    if (Stat("Aerotheurge") >= 40) { MoveList.Add(Move.Find("Spark")); }
                    if (Stat("Hydrosophist") >= 40) { MoveList.Add(Move.Find("Ice Shard")); }
                    if (Stat("Geomagnetic") >= 40) { MoveList.Add(Move.Find("Rock Throw")); }
                    break;
                case "Paladin":
                case "Barbarian":
                case "Warrior":
                    if (r.Next(0,2)==0||Class=="Barbarian") Inventory.Add(Item.Find("Hide Armor"));
                    else Inventory.Add(Item.Find("Studded Leather Armor"));
                    List<StatData> WarSkillList = new List<StatData> { CombatSkills[12], CombatSkills[13], CombatSkills[14] };
                    List<StatData> WarSkillSortedList = WarSkillList.OrderByDescending(o => o.Value).ToList();
                    switch (WarSkillSortedList[0].Name)
                    {
                        case "Sword":
                            Subclass = "Swordsman";
                            if (r.Next(0, 2) == 0 || maxCarryWeight - carryWeight - 17 <= 0)
                            {
                                Inventory.Add(Item.Find("Sword"));
                                if (r.Next(0, 2) == 0 || Class == "Paladin") Inventory.Add(Item.Find("Shield"));
                            }
                            else
                            {
                                Inventory.Add(Item.Find("Greatsword"));
                            }
                            break;
                        case "Blunt":
                            Subclass = "Smasher";
                            if (r.Next(0, 2) == 0 || maxCarryWeight - carryWeight - 25 <= 0)
                            {
                                Inventory.Add(Item.Find("Mace"));
                                if (r.Next(0, 2) == 0 || Class == "Paladin") Inventory.Add(Item.Find("Shield"));
                            }
                            else
                            {
                                Inventory.Add(Item.Find("Warhammer"));
                            }
                            break;
                        case "Axe":
                            Subclass = "Butcher";
                            if (r.Next(0, 2) == 0 || maxCarryWeight - carryWeight - 21 <= 0)
                            {
                                Inventory.Add(Item.Find("War Axe"));
                                if (r.Next(0, 2) == 0 || Class == "Paladin") Inventory.Add(Item.Find("Shield"));
                            }
                            else
                            {
                                Inventory.Add(Item.Find("Battleaxe"));
                            }
                            break;
                        default: break;
                    }
                    if (Class == "Paladin") Subclass = "Paladin";
                    List<StatData> WarriorMagicList = new List<StatData> { CombatSkills[0], CombatSkills[1], CombatSkills[2], CombatSkills[3], CombatSkills[4], CombatSkills[5], CombatSkills[6], CombatSkills[7] };
                    List<StatData> WarMagicSortedList = WarriorMagicList.OrderByDescending(o => o.Value).ToList();
                    if (WarMagicSortedList[0].Value >= 40)
                    switch (WarMagicSortedList[0].Name)
                    {
                        case "Pyrokinetic":
                            MoveList.Add(Move.Find("Fire Blast"));
                            break;
                        case "Aerotheurge":
                            MoveList.Add(Move.Find("Spark"));
                            break;
                        case "Hydrosophist":
                            MoveList.Add(Move.Find("Ice Shard"));
                            break;
                        case "Geomagnetic":
                            MoveList.Add(Move.Find("Rock Throw"));
                            break;
                        case "PowerOfLight":
                            MoveList.Add(Move.Find("Dazzling Light"));
                            MoveList.Add(Move.Find("Small Heal"));
                            break;
                        case "ForceOfNature":
                            MoveList.Add(Move.Find("Wild Fury"));
                            MoveList.Add(Move.Find("Living Roots"));
                            break;
                        case "Entropy":
                            MoveList.Add(Move.Find("Shadow Blast"));
                            MoveList.Add(Move.Find("Drain Life"));
                            break;
                        case "Psionics":
                            MoveList.Add(Move.Find("Will Break"));
                            MoveList.Add(Move.Find("Psy Blast"));
                            break;
                        default: break;
                    }
                    break;
                case "Warlock":
                    Subclass = "Warlock";
                    if (Stat("Entropy") <= 25) Inventory.Add(Item.Find("Novice Robe"));
                    else if (Stat("Entropy") < 50) Inventory.Add(Item.Find("Apprentice Robe"));
                    else if (Stat("Entropy") < 70) Inventory.Add(Item.Find("Adept Robe"));
                    if (Stat("ShortBlade") >= 40)
                    {
                        Inventory.Add(Item.Find("Sacrificial Dagger"));
                    }
                    if (Stat("Entropy") >= 40) MoveList.Add(Move.Find("Life Tap"));
                    MoveList.Add(Move.Find("Shadow Blast"));
                    MoveList.Add(Move.Find("Drain Life"));
                    break;
                case "Psionic":
                    Subclass = "Psionic";
                    if (Stat("Psionic") <= 25) Inventory.Add(Item.Find("Novice Robe"));
                    else if (Stat("Psionic") < 50) Inventory.Add(Item.Find("Apprentice Robe"));
                    else if (Stat("Psionic") < 70) Inventory.Add(Item.Find("Adept Robe"));
                    if (Stat("ShortBlade") >= 40)
                    {
                        Inventory.Add(Item.Find("Dagger"));
                        MoveList.Add(Move.Find("Psy Blade"));
                    }
                    MoveList.Add(Move.Find("Psy Blast"));
                    if (Stat("Psionics") >= 40 && r.Next(0,2)==0) MoveList.Add(Move.Find("Paralyse"));
                    if (Stat("Psionics") >= 50) MoveList.Add(Move.Find("Will Break"));
                    if (Stat("Psionics") >= 60) MoveList.Add(Move.Find("Psy Storm"));
                    break;
                case "Sniper":
                case "Witch Hunter":
                case "Ranger":
                    Inventory.Add(Item.Find("Leather Armor"));
                    switch (r.Next(0, 4))
                    {
                        case 0:
                            Subclass = "Archer";
                            Inventory.Add(Item.Find("Short Bow"));
                            Inventory.Add(Item.Find("Arrow"));
                            break;
                        case 1:
                            Subclass = "Archer";
                            Inventory.Add(Item.Find("Long Bow"));
                            Inventory.Add(Item.Find("Arrow"));
                            break;
                        case 2:
                            Subclass = "Arbalester";
                            Inventory.Add(Item.Find("Hand Crossbow"));
                            Inventory.Add(Item.Find("Bolt(Small)"));
                            break;
                        case 3:
                            Subclass = "Arbalester";
                            Inventory.Add(Item.Find("Crossbow"));
                            Inventory.Add(Item.Find("Bolt(Medium)"));
                            break;
                        default: break;
                    }
                    List<StatData> RangerMagicList = new List<StatData> { CombatSkills[0], CombatSkills[1], CombatSkills[2], CombatSkills[3], CombatSkills[4], CombatSkills[5], CombatSkills[6], CombatSkills[7] };
                    List<StatData> RangerMagicSortedList = RangerMagicList.OrderByDescending(o => o.Value).ToList();
                    if (RangerMagicSortedList[0].Value >= 40)
                    switch (RangerMagicSortedList[0].Name)
                    {
                        case "Pyrokinetic":
                            MoveList.Add(Move.Find("Fire Blast"));
                            break;
                        case "Aerotheurge":
                            MoveList.Add(Move.Find("Spark"));
                            break;
                        case "Hydrosophist":
                            MoveList.Add(Move.Find("Ice Shard"));
                            break;
                        case "Geomagnetic":
                            MoveList.Add(Move.Find("Rock Throw"));
                            break;
                        case "PowerOfLight":
                            MoveList.Add(Move.Find("Dazzling Light"));
                            MoveList.Add(Move.Find("Small Heal"));
                            break;
                        case "ForceOfNature":
                            MoveList.Add(Move.Find("Living Roots"));
                            break;
                        case "Entropy":
                            MoveList.Add(Move.Find("Shadow Blast"));
                            break;
                        case "Psionics":
                            MoveList.Add(Move.Find("Psy Blast"));
                            if (Stat("Psionics") >= 30) MoveList.Add(Move.Find("Will Break"));
                            break;
                        default: break;
                    }
                    break;
                case "Rogue":
                    Subclass = "Rogue";
                    Inventory.Add(Item.Find("Leather Armor"));
                    Inventory.Add(Item.Find("Dagger"));
                    List<StatData> RogueMagicList = new List<StatData> { CombatSkills[0], CombatSkills[1], CombatSkills[2], CombatSkills[3], CombatSkills[4], CombatSkills[5], CombatSkills[6], CombatSkills[7] };
                    List<StatData> RogueMagicSortedList = RogueMagicList.OrderByDescending(o => o.Value).ToList();
                    if (RogueMagicSortedList[0].Value >= 40)
                    switch (RogueMagicSortedList[0].Name)
                    {
                        case "Pyrokinetic":
                            MoveList.Add(Move.Find("Fire Blast"));
                            break;
                        case "Aerotheurge":
                            MoveList.Add(Move.Find("Spark"));
                            break;
                        case "Hydrosophist":
                            MoveList.Add(Move.Find("Ice Dagger"));
                            MoveList.Add(Move.Find("Ice Shard"));
                            break;
                        case "Geomagnetic":
                            MoveList.Add(Move.Find("Rock Dagger"));
                            MoveList.Add(Move.Find("Rock Throw"));
                            break;
                        case "PowerOfLight":
                            MoveList.Add(Move.Find("Dazzling Light"));
                            MoveList.Add(Move.Find("Small Heal"));
                            break;
                        case "ForceOfNature":
                            MoveList.Add(Move.Find("Living Roots"));
                            break;
                        case "Entropy":
                            MoveList.Add(Move.Find("Shadow Blast"));
                            break;
                        case "Psionics":
                            MoveList.Add(Move.Find("Psy Blade"));
                            if (Stat("Psionics") >= 30) MoveList.Add(Move.Find("Will Break"));
                            MoveList.Add(Move.Find("Psy Blast"));
                            break;
                        default: break;
                    }
                    break;
                case "Monk":
                    Subclass = "Monk";
                    List<StatData> MonkMagicList = new List<StatData> { CombatSkills[0], CombatSkills[1], CombatSkills[2], CombatSkills[3], CombatSkills[4], CombatSkills[5], CombatSkills[6], CombatSkills[7] };
                    List<StatData> MonkMagicSortedList = MonkMagicList.OrderByDescending(o => o.Value).ToList();
                    if (MonkMagicSortedList[0].Value >= 40)
                    switch (MonkMagicSortedList[0].Name)
                    {
                        case "Pyrokinetic":
                            MoveList.Add(Move.Find("Fire Blast"));
                            break;
                        case "Aerotheurge":
                            MoveList.Add(Move.Find("Spark"));
                            break;
                        case "Hydrosophist":
                            MoveList.Add(Move.Find("Ice Shard"));
                            break;
                        case "Geomagnetic":
                            MoveList.Add(Move.Find("Rock Throw"));
                            break;
                        case "PowerOfLight":
                            MoveList.Add(Move.Find("Small Heal"));
                            break;
                        case "ForceOfNature":
                            MoveList.Add(Move.Find("Living Roots"));
                            break;
                        case "Entropy":
                            MoveList.Add(Move.Find("Shadow Blast"));
                            break;
                        case "Psionics":
                            if (Stat("Psionics") >= 50) MoveList.Add(Move.Find("Will Break"));
                            MoveList.Add(Move.Find("Psy Blast"));
                            break;
                        default: break;
                    }
                    break;
                case "Thief":
                    Subclass = "Thief";
                    if (r.Next(0,2)==0) Inventory.Add(Item.Find("Leather Armor"));
                    if (Stat("SharpShooting") >= 20)
                    {
                        switch (r.Next(0, 4))
                        {
                            case 0:
                                Inventory.Add(Item.Find("Short Bow"));
                                Inventory.Add(Item.Find("Arrow"));
                                break;
                            case 1:
                                Inventory.Add(Item.Find("Long Bow"));
                                Inventory.Add(Item.Find("Arrow"));
                                break;
                            case 2:
                                Inventory.Add(Item.Find("Hand Crossbow"));
                                Inventory.Add(Item.Find("Bolt(Small)"));
                                break;
                            case 3:
                                Inventory.Add(Item.Find("Crossbow"));
                                Inventory.Add(Item.Find("Bolt(Medium)"));
                                break;
                            default: break;
                        }
                    }
                    if (Stat("ShortBlade") >= 20)
                    {
                        Inventory.Add(Item.Find("Dagger"));
                    }
                    List<StatData> ThiefMagicList = new List<StatData> { CombatSkills[2], CombatSkills[3], CombatSkills[6], CombatSkills[7] };
                    List<StatData> ThiefMagicSortedList = ThiefMagicList.OrderByDescending(o => o.Value).ToList();
                    if (ThiefMagicSortedList[0].Value >= 35)
                    switch (ThiefMagicSortedList[0].Name)
                    {
                        case "Hydrosophist":
                            if (Stat("ShortBlade") >= 20) MoveList.Add(Move.Find("Ice Dagger"));
                            MoveList.Add(Move.Find("Ice Shard"));
                            break;
                        case "Geomagnetic":
                            if (Stat("ShortBlade") >= 20) MoveList.Add(Move.Find("Rock Dagger"));
                            MoveList.Add(Move.Find("Rock Throw"));
                            break;
                        case "Entropy":
                            MoveList.Add(Move.Find("Shadow Blast"));
                            break;
                        case "Psionics":
                            if (Stat("ShortBlade") >= 20) MoveList.Add(Move.Find("Psy Blade"));
                            if (Stat("Psionics") >= 30) MoveList.Add(Move.Find("Will Break"));
                            MoveList.Add(Move.Find("Psy Blast"));
                            break;
                        default: break;
                    }
                    break;
                default: break;
            }
            if (WeightMod > 1.0) {
                if (maxCarryWeight - carryWeight + 15 >= 0) Inventory.Add(Item.Find("Small Backpack"));
                else if (maxCarryWeight - carryWeight + 30 >= 0) Inventory.Add(Item.Find("Medium Backpack"));
                else Inventory.Add(Item.Find("Large Backpack"));
            }
        }

        public void setSubclass()
        {
            List<StatData> list = new List<StatData>(CombatSkills);
            List<StatData> sortedList = list.OrderByDescending(o => o.Value).ToList();
            int i = 0;
            while (Class == null && i <= sortedList.Count)
            {
                if (Inventory.Count > 0 || MoveList.Count > 0) break;
                switch (sortedList[i].Name)
                {
                    case "Pyrokinetic":
                    case "Aerotheurge":
                    case "Hydrosophist":
                    case "Geomagnetic":
                        Class = "Wizard";
                        if (sortedList[i + 1].Name == "SharpShooting") Class = "Witch Hunter";
                        else if (sortedList[i + 1].Name == "Sword") Class = "Witch Blade";
                        break;
                    case "PowerOfLight":
                        Class = "Priest";
                        if ((Stat("STR") + Stat("END")) / 2 >= 10) Class = "Paladin";
                        break;
                    case "ForceOfNature":
                        Class = "Druid";
                        break;
                    case "Sword":
                    case "Blunt":
                    case "Axe":
                        Class = "Warrior";
                        if (Stat("INT") < 10) Class = "Barbarian";
                        break;
                    case "Entropy":
                        Class = "Warlock";
                        break;
                    case "Psionics":
                        Class = "Psionic";
                        break;
                    case "SharpShooting":
                        Class = "Ranger";
                        break;
                    case "ShortBlade":
                        Class = "Rogue";
                        break;
                    case "Brawl":
                        Class = "Monk";
                        break;
                    default:
                        i++;
                        break;
                }
            }
        }

        public void Promote(int exp)
        {
            if (LVL == 0) return;
            EXP += exp;
            while (EXP>=EXP4NextLVL)
            {
                EXP -= EXP4NextLVL;
                atPoints += 1;
                skPoints1 += 2 + (Stat("INT") / 5);
                if (LVL < 99 && LVL > 0) LVL++;
                else { LVL = 0; EXP = 0; }
            }
        }
        public void RemoveItem(string name) => Inventory.Remove(Inventory.Where(x => x.Name == name).FirstOrDefault());

        public void ReplaceItem(Item oldItem, Item newItem)
        {
            int index = Inventory.IndexOf(Inventory.Where(x => x == oldItem).FirstOrDefault());
            if (index != -1)
                Inventory[index] = newItem;
        }

        private void SubPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "MP_MOD":
                    RaisePropertyChanged("maxMP");
                    MP += MP_MOD;
                    if (MP > maxMP) MP = maxMP;
                    break;
                case "HP_MOD":
                    RaisePropertyChanged("maxHP");
                    HP += HP_MOD;
                    if (HP > maxHP) HP = maxHP;
                    break;
                case "SP_MOD":
                    RaisePropertyChanged("maxSP");
                    SP += SP_MOD;
                    if (SP > maxSP) SP = maxSP;
                    break;
                case "WP_MOD":
                    RaisePropertyChanged("maxWP");
                    WP += WP_MOD;
                    if (WP > maxWP) WP = maxWP;
                    break;
                case "MP":
                    if (MP > maxMP) MP = maxMP;
                    else if (MP < 0) MP = 0;
                    break;
                case "HP":
                    if (HP > maxHP) HP = maxHP;
                    else if (HP < 0) HP = 0;
                    break;
                case "SP":
                    if (SP > maxSP) SP = maxSP;
                    break;
                case "WP":
                    if (WP > maxWP) WP = maxWP;
                    else if (WP < 0) WP = 0;
                    break;
                case "LVL":
                    RaisePropertyChanged("EXP4NextLVL");
                    break;
                default: break;
            }
        }

    }
    [Magic]
    public class StatData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public string Name { get; set; }
        public int Value { get; set; }
        public int EXP { get; set; }

        public StatData() { }
        public StatData(string name, int value)
        {
            Name = name;
            Value = value;
        }
    }
}
