using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Unnamed
{
    [Magic]
    public class Move : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public string Name { get; set; }
        public string Attr { get; set; }
        public string Skill { get; set; }
        public int cSP { get; set; } = 0;
        public int cHP { get; set; } = 0;
        public int cMP { get; set; } = 0;
        public int cWP { get; set; } = 0;
        public int tSP { get; set; } = 0;
        public int tHP { get; set; } = 0;
        public int tMP { get; set; } = 0;
        public int tWP { get; set; } = 0;
        public int caSP { get; set; } = 0;
        public int caHP { get; set; } = 0;
        public int caMP { get; set; } = 0;
        public int caWP { get; set; } = 0;
        public int taSP { get; set; } = 0;
        public int taHP { get; set; } = 0;
        public int taMP { get; set; } = 0;
        public int taWP { get; set; } = 0;
        public string DmgType { get; set; }
        public string Descr { get; set; }
        public bool IsActive { get; set; } = true;
        public string ConsumingItem { get; set; }
        public bool IsDrain { get; set; } = false;

        public Move()
        {
            Name = "";
            Descr = "";
        }

        public Move(string name, string attr, string skill, string mods, string type, string description)
        {//general
            Name = name;
            Attr = attr;
            Skill = skill;
            Descr = description;
            DmgType = type;
            string[] modsList = mods.Split('\n');
            foreach (string i in modsList)
            {
                string[] j = i.Split(' ');
                if (j.Count() > 2) foreach (string s in j) { if (s == j[0] || s == j[1]) continue; else j[1] += " " + s; }
                switch (j[0])
                {
                    case "cSP":
                        cSP = int.Parse(j[1]);
                        break;
                    case "cHP":
                        cHP = int.Parse(j[1]);
                        break;
                    case "cMP":
                        cMP = int.Parse(j[1]);
                        break;
                    case "cWP":
                        cWP = int.Parse(j[1]);
                        break;
                    case "tSP":
                        tSP = int.Parse(j[1]);
                        break;
                    case "tHP":
                        tHP = int.Parse(j[1]);
                        break;
                    case "tMP":
                        tMP = int.Parse(j[1]);
                        break;
                    case "tWP":
                        tWP = int.Parse(j[1]);
                        break;
                    case "caSP":
                        caSP = int.Parse(j[1]);
                        break;
                    case "caHP":
                        caHP = int.Parse(j[1]);
                        break;
                    case "caMP":
                        caMP = int.Parse(j[1]);
                        break;
                    case "caWP":
                        caWP = int.Parse(j[1]);
                        break;
                    case "taSP":
                        taSP = int.Parse(j[1]);
                        break;
                    case "taHP":
                        taHP = int.Parse(j[1]);
                        break;
                    case "taMP":
                        taMP = int.Parse(j[1]);
                        break;
                    case "taWP":
                        taWP = int.Parse(j[1]);
                        break;
                    case "Drain":
                        IsDrain = true;
                        break;
                    case "Uses":
                        ConsumingItem = j[1];
                        break;
                    case "Consumable":
                        ConsumingItem = Name;
                        break;
                    default:
                        break;
                }
            }
        }

        public static Move Find(string name) => List.FirstOrDefault(x => x.Name == name);
        public static List<Move> List { get; set; } = new List<Move> {
            //new Move("Punch", "STR\nBrawl\ncSP -5\ntHP -3\ntSP -2"),
            new Move("Punch", "STR", "Brawl", "cSP -5\ntHP -3\ntSP -2", "", "5 SP\nPunch the enemy!"),

            //new Move("Fire Blast", "INT\nPyrokinetic\ncMP -10\ntHP -10\nFire"),
            new Move("Fire Blast", "INT", "Pyrokinetic", "cSP -6\ncMP -4\ntHP -10", "Fire", "6 SP / 4 MP\nFire blast!"),
            //new Move("Haste", "INT\nPyrokinetic\ncMP -10\ncWP -10\ncHP -10\ncSP +40\nFire"),
            new Move("Haste", "INT", "Pyrokinetic", "cMP -5\ncWP -10\ncHP -5\ncSP +10", "Fire", "+10 SP at the cost of 5 HP, 5 MP & 10 WP\nHaste!"),
            //new Move("Explosion", "INT\nPyrokinetic\ncMP -30\ntaHP -10\nFire"),
            new Move("Explosion", "INT", "Pyrokinetic", "cSP -15\ncMP -15\ntaHP -10", "Fire", "15 SP / 15 MP\nDeals fire damage to all enemies!\nExplosion!"),

            //new Move("Spark", "INT\nAerotheurge\ncMP -12\ntHP -6\ntMP -6\nStorm"),
            new Move("Spark", "INT", "Aerotheurge", "cSP -6\ncMP -5\ntHP -6\ntMP -5", "Storm", "6 SP / 5 MP\nSpark!"),
            //new Move("Windfury", "INT\nAerotheurge\ncMP -5\ncWP -5\ncSP +15\nStorm"),
            new Move("Windfury", "INT", "Aerotheurge", "cSP +5\ncMP -5\ncWP -5", "Storm", "+5 SP at the cost of 5 MP & 5 WP"),
            //new Move("Tailwind", "INT\nAerotheurge\ncMP -25\ncWP -20\ncaSP +5\ntaSP -5\nStorm"),
            new Move("Tailwind", "INT", "Aerotheurge", "cSP -15\ncMP -15\ncaSP +5\ntaSP -5", "Storm", "15 SP / 15 MP\nIncreases ally SP and decreases enemy SP"),

            //new Move("Restoration", "WIS\nHydrosophist\ncMP -10\ncHP +10\nWater"),
            new Move("Restoration", "WIS", "Hydrosophist", "cSP -5\ncMP -5\ncHP +10", "Water", "5 SP / 5 MP\nRestores *10* HP"),
            //new Move("Mana Flow", "WIS\nHydrosophist\ncWP -10\ncMP +10\nWater"),
            new Move("Mana Flow", "WIS", "Hydrosophist", "cSP -5\ncWP -5\ncMP +10", "Water", "5 SP / 5 WP\nRestores *10* MP"),
            //new Move("Ice Dagger", "DEX\nShortBlade\ncSP -5\ncMP -5\ntHP -6\ntSP -4\nIce"),
            new Move("Ice Dagger", "DEX", "ShortBlade", "cSP -5\ncMP -3\ntHP -5\ntSP -3", "Ice", "5 SP / 3 MP\nCut and stab with ice dagger!"),
            //new Move("Ice Shard", "INT\nHydrosophist\ncMP -12\ntHP -6\ntSP -6\nIce"),
            new Move("Ice Shard", "INT", "Hydrosophist", "cSP -6\ncMP -5\ntHP -6\ntSP -5", "Ice", "6 SP / 5 MP\nLaunch the ice shard!"),
            //new Move("Steam Lance", "INT\nHydrosophist\ncMP -12\ntHP -6\ntSP -3\ntWP -3\nWater"),
            new Move("Steam Lance", "INT", "Hydrosophist", "cSP -6\ncMP -6\ntHP -6\ntSP -3\ntWP -3", "Water", "6 SP / 6 MP\nSteam lance!"),
            //new Move("Deep Freeze", "INT\nHydrosophist\ncMP -12\ntHP -2\ntSP -10\nIce"),
            new Move("Deep Freeze", "INT", "Hydrosophist", "cSP -6\ncMP -6\ntHP -2\ntSP -10", "Ice", "6 SP / 6 MP\nDeals great SP damage!"),
            //new Move("Blizzard", "INT\nHydrosophist\ncMP -30\ntaSP -10\nIce"),
            new Move("Blizzard", "INT", "Hydrosophist", "cSP -15\ncMP -15\ntaSP -10", "Ice", "15 SP / 15 MP\nDeals great SP damage to all enemies!"),

            //new Move("Rock Dagger", "DEX\nShortBlade\ncSP -5\ncMP -4\ntHP -7\ntSP -2\nRock"),
            new Move("Rock Dagger", "DEX", "ShortBlade", "cSP -5\ncMP -2\ntHP -5\ntSP -2", "Rock", "5 SP / 2 MP\nCut and stab with rock dagger!"),
            //new Move("Rock Throw", "INT\nGeomagnetic\ncMP -12\ntHP -8\ntSP -4\nRock"),
            new Move("Rock Throw", "INT", "Geomagnetic", "cSP -6\ncMP -5\ntHP -8\ntSP -3", "Rock", "6 SP / 5 MP\nThrow the rock!"),
            //new Move("Earth Strike", "INT\nGeomagnetic\ncMP -12\ntHP -7\ntSP -5\nEarth"),
            new Move("Earth Strike", "INT", "Geomagnetic", "cSP -6\ncMP -5\ntHP -6\ntSP -5", "Earth", "6 SP / 5 MP\nStrike with the power of Earth itself!"),
            //new Move("Impalement", "INT\nGeomagnetic\ncMP -24\ntaHP -5\ntaSP -3\nRock"),
            new Move("Impalement", "INT", "Geomagnetic", "cSP -15\ncMP -15\ntaHP -6\ntaSP -4", "Rock", "15 SP / 15 MP\nImpale enemies with stone peaks!"),

            //new Move("Dazzling Light", "WIS\nPowerOfLight\ncMP -10\ntHP -5\ntSP -5\nLight"),
            new Move("Dazzling Light", "WIS", "PowerOfLight", "cSP -6\ncMP -4\ntHP -5\ntSP -5", "Light", "6 SP / 4 MP\nBlind and burn enemy with the Power Of Light!"),
            //new Move("Small Heal", "WIS\nPowerOfLight\ncMP -5\ncHP +5\nLight"),
            new Move("Healing Light", "WIS", "PowerOfLight", "cSP -3\ncMP -3\ncHP +6", "Light", "3 SP / 3 MP\nRestores *6* HP"),
            //new Move("Mass Heal", "WIS\nPowerOfLight\ncMP -15\ncaHP +5\nLight"),
            new Move("Mass Heal", "WIS", "PowerOfLight", "cSP -9\ncMP -9\ncaHP +6", "Light", "9 SP / 9 MP\nRestores *6* HP to every ally!"),
            //new Move("Dazzling Flash", "WIS\nPowerOfLight\ncMP -30\ntaHP -5\ntaSP -5\nLight"),
            new Move("Dazzling Flash", "WIS", "PowerOfLight", "cSP -15\ncMP -15\ntaHP -5\ntaSP -5", "Light", "15 SP / 15 MP\nBurns and blinds all enemies with the Power Of Light!"),

            //new Move("Razor feather", "DEX\nThrowing\ncMP -2\ncSP -2\ntHP -2\ntSP -2"),
            new Move("Razor feather", "DEX", "Throwing", "cSP -4\ncMP -2\ntHP -4\ntSP -2", "", "4 SP / 2 MP\nThrow razor sharp feather!"),
            //new Move("Feather wave", "WIS\nForceOfNature\ncMP -12\ntaSP -4"),
            new Move("Feather wave", "WIS", "ForceOfNature", "cSP -6\ncMP -6\ntaSP -4", "", "6 SP / 6 MP\nUse feathers to make enemies feel uncomfortable!"),
            //new Move("Thorn shot", "INT\nForceOfNature\ncMP -2\ncSP -2\ntHP -4\nPlant"),
            new Move("Thorn shot", "WIS", "ForceOfNature", "cSP -4\ncMP -2\ntHP -6", "Plant", "4 SP / 2 MP\nLet's shoot da thorn!"),
            //new Move("Thorns wave", "WIS\nForceOfNature\ncMP -12\ntaHP -4\nPlant"),
            new Move("Thorns wave", "WIS", "ForceOfNature", "cSP -9\ncMP -9\ntaHP -6", "Plant", "9 SP / 9 MP\nCover them with thorns!"),
            //new Move("Poison shot", "INT\nForceOfNature\ncMP -12\ntHP -3\ntSP -9\nPoison"),
            new Move("Poison shot", "WIS", "ForceOfNature", "cSP -8\ncMP -4\ntHP -6\ntSP -6", "Poison", "8 SP / 4 MP\nLet's shoot da poison!"),
            //new Move("Web", "WIS\nForceOfNature\ncMP -12\ntSP -12\nBug"),
            new Move("Web", "WIS", "ForceOfNature", "cSP -8\ncMP -4\ntSP -12", "Bug", "8 SP / 4 MP\nLet's shoot da web!"),
            //new Move("Poison Spores", "INT\nForceOfNature\ncMP -15\ntHP -5\ntSP -10\nPoison"),
            new Move("Poison Spores", "INT", "ForceOfNature", "cSP -10\ncMP -5\ntHP -8\ntSP -7", "Poison", "10 SP / 5 MP\nLet's shoot da poison!"),
            //new Move("Pheromone Spores", "INT\nForceOfNature\ncMP -15\ntSP -5\ntWP -10\nPoison"),
            new Move("Pheromone Spores", "INT", "ForceOfNature", "cSP -10\ncMP -5\ntSP -5\ntWP -10", "Poison", "10 SP / 5 MP\nPheromones time!"),
            //new Move("Living Roots", "INT\nForceOfNature\ncMP -15\ntHP -4\ntSP -11\nPlant"),
            new Move("Living Roots", "INT", "ForceOfNature", "cSP -10\ncMP -5\ntHP -5\ntSP -10", "Plant", "10 SP / 5 MP\nRooting for you!"),

            //new Move("Life Tap", "INT\nEntropy\ncHP -10\ncMP +10\nVoid"),
            new Move("Life Tap", "INT", "Entropy", "cHP -10\ncMP +10", "Void", "Restores *10* MP for *10* HP"),
            //new Move("Shadow Blast", "INT\nEntropy\ncMP -10\ncHP -5\ntHP -15\nVoid"),
            new Move("Shadow Blast", "INT", "Entropy", "cSP -8\ncMP -2\ncHP -5\ntHP -15", "Void", "Blast empowered with blood"),
            //new Move("Drain Life", "INT\nEntropy\ncMP -10\ntHP -5\nDrain\nVoid"),
            new Move("Drain Life", "INT", "Entropy", "cSP -8\ncMP -2\ntHP -5\nDrain", "Void", "Drain 5"),

            //new Move("Psy Blade", "DEX\nShortBlade\ncSP -7\ncMP -3\ncWP -5\ntHP -8\ntWP -7\nPsy"),
            new Move("Psy Blade", "DEX", "ShortBlade", "cSP -7\ncMP -3\ncWP -5\ntHP -8\ntWP -7", "Psy", "Psy blade"),
            //new Move("Psy Blast", "WIS\nPsionics\ncMP -10\ntHP -6\ntWP -4\nPsy"),
            new Move("Psy Blast", "WIS", "Psionics", "cSP -7\ncMP -3\ntHP -6\ntWP -4", "Psy", "Psy blast!"),
            //new Move("Paralyse", "WIS\nPsionics\ncMP -10\ntWP -4\ntSP -6\nPsy"),
            new Move("Paralyse", "WIS", "Psionics", "cSP -5\ncMP -5\ntSP -6\ntWP -4", "Psy", "Paralyse!"),
            //new Move("Will Break", "WIS\nPsionics\ncMP -10\ntWP -10\nPsy"),
            new Move("Will Break", "WIS", "Psionics", "cSP -7\ncMP -5\ntSP -2\ntWP -10", "Psy", "Breaking down the will!"),
            //new Move("Psy Storm", "WIS\nPsionics\ncMP -18\ntaWP -6\nPsy"),
            new Move("Psy Storm", "WIS", "Psionics", "cSP -15\ncMP -15\ntaWP -10", "Psy", "Psyonic storm!"),
            /*
            new Move("Fire Whip", "DEX\nWhip\ncSP -10\ncMP -5\ntHP -10\ntWP -5\nFire"),
            new Move("Storm Whip", "DEX\nWhip\ncSP -10\ncMP -5\ntSP -7\ntMP -5\ntWP -3\nStorm"),
            new Move("Water Whip", "DEX\nWhip\ncSP -10\ncMP -5\ntSP -10\ntWP -5\nWater"),
            new Move("Ice Whip", "DEX\nWhip\ncSP -13\ncMP -5\ntSP -16\ntWP -2\nIce"),
            new Move("Rock Whip", "DEX\nWhip\ncSP -10\ncMP -10\ntSP -10\ntHP -5\ntWP -5\nRock"),
            new Move("Holy Whip", "DEX\nWhip\ncSP -10\ncMP -5\ntHP -5\ntSP -10\nLight"),
            new Move("Vine Whip", "DEX\nWhip\ncSP -10\ncMP -5\ntSP -10\ntWP -5\nPlant"),
            new Move("Psy Whip", "WIS\nWhip\ncSP -15\ncWP -10\ntWP -20\ntHP -5\nPsy"),
            */
        };
    }
}
