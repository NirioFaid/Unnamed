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
        //cost/type
        //damage/type
        //effect/type
        /*public int[] cost { get; set; }
        public int[] casterMods { get; set; }
        public int[] targetMods { get; set; }
        public int[] casterAreaMods { get; set; }
        public int[] targetAreaMods { get; set; }*/
        public string Descr { get; set; }
        public bool IsActive { get; set; } = true;

        public Move()
        {
            Name = "";
            Descr = "";
        }

        public Move(string name, string description)
        {
            Name = name;
            Descr = description;
        }

        /*public Move(string name, int[] cost, string description)
        {
            Name = name;
            Descr = description;
            this.cost = cost;
        }*/

        public static Move Find(string name) => List.FirstOrDefault(x => x.Name == name);
        public static List<Move> List { get; set; } = new List<Move> {
            new Move("Punch", "STR\nBrawl\ncSP -5\ntHP -3\ntSP -2"),

            new Move("Fire Punch", "STR\nBrawl\ncSP -5\ncMP -5\ntHP -10\nFire"),
            new Move("Fire Blast", "INT\nPyrokinetic\ncMP -10\ntHP -10\nFire"),
            new Move("Haste", "INT\nPyrokinetic\ncMP -10\ncWP -10\ncHP -10\ncSP +40\nFire"),
            new Move("Explosion", "INT\nPyrokinetic\ncMP -30\ntaHP -10\nFire"),

            new Move("Static Punch", "STR\nBrawl\ncSP -5\ncMP -7\ntHP -8\ntMP -4\nStorm"),
            new Move("Spark", "INT\nAerotheurge\ncMP -12\ntHP -6\ntMP -6\nStorm"),
            new Move("Windfury", "INT\nAerotheurge\ncMP -5\ncWP -5\ncSP +15\nStorm"),
            new Move("Tailwind", "INT\nAerotheurge\ncMP -25\ncWP -20\ncaSP +5\ntaSP -5\nStorm"),

            new Move("Restoration", "WIS\nHydrosophist\ncMP -10\ncHP +10\nWater"),
            new Move("Mana Flow", "WIS\nHydrosophist\ncWP -10\ncMP +10\nWater"),
            new Move("Water Punch", "STR\nBrawl\ncSP -5\ncMP -4\ntHP -3\ntSP -3\ntWP -3\nWater"),
            new Move("Ice Dagger", "DEX\nShortBlade\ncSP -5\ncMP -5\ntHP -6\ntSP -4\nIce"),
            new Move("Ice Shard", "INT\nHydrosophist\ncMP -12\ntHP -6\ntSP -6\nIce"),
            new Move("Steam Lance", "INT\nHydrosophist\ncMP -12\ntHP -6\ntSP -3\ntWP -3\nWater"),
            new Move("Deep Freeze", "INT\nHydrosophist\ncMP -12\ntHP -2\ntSP -10\nIce"),
            new Move("Blizzard", "INT\nHydrosophist\ncMP -30\ntaSP -10\nIce"),

            new Move("Earth Punch", "STR\nBrawl\ncSP -5\ncMP -8\ntHP -8\ntSP -5\nEarth"),
            new Move("Rock Dagger", "DEX\nShortBlade\ncSP -5\ncMP -4\ntHP -7\ntSP -2\nRock"),
            new Move("Rock Throw", "INT\nGeomagnetic\ncMP -12\ntHP -8\ntSP -4\nRock"),
            new Move("Earth Strike", "INT\nGeomagnetic\ncMP -12\ntHP -7\ntSP -5\nEarth"),
            new Move("Impalement", "INT\nGeomagnetic\ncMP -24\ntaHP -5\ntaSP -3\nRock"),

            new Move("Dazzling Punch", "STR\nBrawl\ncMP -6\ncSP -6\ntHP -6\ntSP -6\nLight"),
            new Move("Dazzling Light", "WIS\nPowerOfLight\ncMP -10\ntHP -5\ntSP -5\nLight"),
            new Move("Small Heal", "WIS\nPowerOfLight\ncMP -5\ncHP +5\nLight"),
            new Move("Mass Heal", "WIS\nPowerOfLight\ncMP -15\ncaHP +5\nLight"),
            new Move("Dazzling Flash", "WIS\nPowerOfLight\ncMP -30\ntaHP -5\ntaSP -5\nLight"),

            new Move("Claw Cut", "DEX\nShortBlade\ncSP -7\ncMP -7\ntHP -14"),
            new Move("Bestial Punch", "STR\nBrawl\ncSP -6\ncMP -6\ntHP -8\ntSP -4"),

            new Move("Claws", "STR\nBrawl\ncMP -2\ncSP -8\ntHP -10"),
            new Move("Summon Wolf", "WIS\nForceOfNature\ncMP -10\ncWP -10\nSummon Wolf"),
            new Move("Wild Fury", "END\nForceOfNature\ncHP -10\ncSP +15"),

            new Move("Razor feather", "DEX\nThrowing\ncMP -2\ncSP -2\ntHP -2\ntSP -2"),
            new Move("Raven strike", "WIS\nForceOfNature\ncMP -10\ncWP -10\ntHP -5\ntSP -7\nSummon Raven"),
            new Move("Feather wave", "WIS\nForceOfNature\ncMP -12\ntaSP -4"),

            new Move("Thorn shot", "INT\nForceOfNature\ncMP -2\ncSP -2\ntHP -4\nPlant"),
            new Move("Insect strike", "WIS\nForceOfNature\ncMP -10\ncWP -10\ntHP -6\ntSP -12\nBug\nSummon Insect swarm"),
            new Move("Thorns wave", "WIS\nForceOfNature\ncMP -12\ntaHP -4\nPlant"),

            new Move("Poison shot", "INT\nForceOfNature\ncMP -12\ntHP -3\ntSP -9\nPoison"),
            new Move("Summon Giant Spider", "WIS\nForceOfNature\ncMP -15\ncWP -15\nSummon Giant spider"),
            new Move("Web", "WIS\nForceOfNature\ncMP -12\ntSP -12\nBug"),

            new Move("Poison Spores", "INT\nForceOfNature\ncMP -15\ntHP -5\ntSP -10\nPoison"),
            new Move("Create Mushroom", "WIS\nForceOfNature\ncMP -5\ncWP -5\nSummon Mushroom"),
            new Move("Pheromone Spores", "INT\nForceOfNature\ncMP -15\ntSP -5\ntWP -10\nPoison"),

            new Move("Living Roots", "INT\nForceOfNature\ncMP -15\ntHP -4\ntSP -11\nPlant"),
            new Move("Venom shot", "INT\nForceOfNature\ncMP -12\ntHP -8\ntSP -6\nPoison"),
            new Move("Pheromone Strike", "INT\nForceOfNature\ncMP -15\ntSP -5\ntWP -10\nPoison"),
            new Move("Razor Leafs", "INT\nForceOfNature\ncMP -6\ntHP -4\ntSP -2\nPlant"),

            new Move("Void Punch", "STR\nBrawl\ncSP -5\ncMP -5\ncHP -5\ntHP -15\nVoid"),
            new Move("Draining Claws", "STR\nBrawl\ncSP -8\ncMP -2\ntHP -5\nDrain\nVoid"),
            new Move("Life Tap", "INT\nEntropy\ncHP -10\ncMP +10\nVoid"),

            new Move("Ghost call", "INT\nEntropy\ncWP -5\ntHP -10\ncMP -15\ntWP -5\nSummon Ghost"),
            new Move("Raise Skeleton", "INT\nEntropy\ncWP -5\ntHP -10\ncMP -15\ntWP -5\nSummon Skeleton"),
            new Move("Raise Zombie", "INT\nEntropy\ncWP -5\ntHP -10\ncMP -15\ntWP -5\nSummon Zombie"),

            new Move("Shadow Blast", "INT\nEntropy\ncMP -10\ncHP -5\ntHP -15\nVoid"),
            new Move("Drain Life", "INT\nEntropy\ncMP -10\ntHP -5\nDrain\nVoid"),
            new Move("Entropic Punch", "STR\nBrawl\ncSP -5\ncMP -5\ncHP -5\ntHP -15\nVoid"),
            new Move("Draining Punch", "STR\nBrawl\ncSP -5\ncMP -5\ntHP -5\nDrain\nVoid"),

            new Move("Psy Blade", "DEX\nShortBlade\ncSP -7\ncMP -3\ncWP -5\ntHP -8\ntWP -7\nPsy"),
            new Move("Will Smash", "STR\nBrawl\ncSP -8\ncWP -4\ntWP -6\ntHP -6\nPsy"),
            new Move("Psy Blast", "WIS\nPsionics\ncMP -10\ntHP -6\ntWP -4\nPsy"),
            new Move("Paralyse", "WIS\nPsionics\ncMP -10\ntWP -4\ntSP -6\nPsy"),
            new Move("Will Break", "WIS\nPsionics\ncMP -10\ntWP -10\nPsy"),
            new Move("Psy Storm", "WIS\nPsionics\ncMP -18\ntaWP -6\nPsy"),

            new Move("Ice Staff", "DEX\nStave\ncSP -10\ncMP -4\ntHP -7\ntSP -7\nIce"),
            new Move("Rock Staff", "DEX\nStave\ncSP -10\ncMP -4\ntHP -10\ntSP -4\nRock"),
            new Move("Psionic Staff", "DEX\nStave\ncWP -5\ncSP -10\ntSP -5\ntWP -10\nPsy"),
            new Move("Vine Staff", "DEX\nStave\ncMP -4\ncSP -10\ntHP -14\nPlant"),

            new Move("Fire Whip", "DEX\nWhip\ncSP -10\ncMP -5\ntHP -10\ntWP -5\nFire"),
            new Move("Storm Whip", "DEX\nWhip\ncSP -10\ncMP -5\ntSP -7\ntMP -5\ntWP -3\nStorm"),
            new Move("Water Whip", "DEX\nWhip\ncSP -10\ncMP -5\ntSP -10\ntWP -5\nWater"),
            new Move("Ice Whip", "DEX\nWhip\ncSP -13\ncMP -5\ntSP -16\ntWP -2\nIce"),
            new Move("Rock Whip", "DEX\nWhip\ncSP -10\ncMP -10\ntSP -10\ntHP -5\ntWP -5\nRock"),
            new Move("Holy Whip", "DEX\nWhip\ncSP -10\ncMP -5\ntHP -5\ntSP -10\nLight"),
            new Move("Vine Whip", "DEX\nWhip\ncSP -10\ncMP -5\ntSP -10\ntWP -5\nPlant"),
            new Move("Psy Whip", "WIS\nWhip\ncSP -15\ncWP -10\ntWP -20\ntHP -5\nPsy"),

            new Move("Distracting talk", "CHR\nPersuasion\ncSP -10\ntSP -10"),
            new Move("Seducting speech", "CHR\nPersuasion\ncSP -10\ntWP -10"),
            new Move("Battle cry", "CHR\nPersuasion\ncSP -20\ncWP -6\ncaWP +4\ncaWP +4"),

            new Move("Flirtatious Wink", "CHR\nPerformance\ncSP -5\ntWP -4\nPsy"),
            new Move("Luring Dance", "CHR\nPersuasion\ncSP -12\ntWP -5\ntSP -5\nPsy"),
            new Move("Out With a Bang", "DEX\nAcrobatics\ncSP -10\ntHP -4\ntSP -6"),
            new Move("Dancing Flames", "CHR\nPyrokinetics\ncSP -15\ntaHP -1\ntaWP -2\ntHP -2\nFire"),
            new Move("Sparkling Dance", "CHR\nAerotheurge\ncMP -2\ntSP -4\ntaMP -1\ntaHP -1\nStorm"),
            new Move("Frosty Dance", "CHR\nHydrosophist\ncMP -2\ntSP -5\ntaSP -1\nIce"),
            new Move("Dancing earth", "CHR\nGeomagnetic\ncMP -5\ncSP -8\ntaSP -4\nEarth"),
            new Move("Dancing Lights", "CHR\nPowerOfLight\ncSP -10\ncMP-3\ncaHP +2\ntaSP -2\nLight"),
            new Move("Dancing roots", "CHR\nForceOfNature\ncMP -15\ntaHP -1\ntaSP -4\nPlant"),
            new Move("Draining Bachata", "CHR\nEntropy\ncSP -15\ncMP -5\ntHP -4\ntWP -4\ntSP -4\nDrain\nVoid"),
            new Move("Flamenco", "CHR\nPerformance\ncSP -15\ntWP -5\ntaWP -3\nPsy"),

            new Move("Dragon Claw", "STR\nBrawl\ncSP -8\ncMP -2\ntHP -10"),
            new Move("Fire Breath", "INT\nPyrokinetic\ncMP -24\ncSP -10\ntaHP -17\nFire"),
            new Move("Storm Breath", "INT\nAerotheurge\ncMP -24\ncSP -10\ntaHP -10\ntaMP -7\nStorm"),
            new Move("Ice Breath", "INT\nHydrosophist\ncMP -24\ncSP -10\ntaHP -6\ntaSP -11\nIce"),
            new Move("Earth Wave", "INT\nGeomagnetic\ncMP -24\ncSP -10\ntaHP -10\ntaSP -5\ncaSP -2\nEarth"),
            new Move("Regeneration", "WIS\nPowerOfLight\ncMP -25\ncSP -10\ncHP +35\nLight"),
            new Move("Poison Breath", "INT\nForceOfNature\ncMP -24\ncSP -10\ntaHP -13\ntSP -4\nPoison"),
            new Move("Deadly Breath", "WIS\nEntropy\ncMP -25\ncSP -10\ncHP -5\ntaHP -20\nVoid"),
            new Move("Dragon Will", "WIS\nPsionics\ncMP -25\ncSP -10\ncWP -5\ntaWP -20\nPsy"),

            new Move("Void Touch", "WIS\nEntropy\ncMP -8\ncSP -2\ntHP -4\ntWP -6\nVoid"),
            new Move("Void Storm", "INT\nAerotheurge\ncMP -30\ncaHP -10\ncaMP -10\ntaHP -20\ntaMP -15\nVoid"),
            new Move("Absolute Zero", "INT\nHydrosophist\ncMP -30\ncaSP -15\ntaHP -10\ntaSP -20\nIce"),
            new Move("Void Thorns", "INT\nForceOfNature\ncMP -30\ntaHP -5\ntSP -10\nVoid"),
            new Move("Absorb Life", "INT\nEntropy\ncMP -34\ntHP -17\nDrain\nVoid"),
            new Move("Void Embrace", "WIS\nPsionics\ncMP -30\ntSP -10\ntWP -20\nVoid"),

            new Move("Demon Claw", "STR\nBrawl\ncSP -8\ntHP -8"),

            new Move("Iron Fist", "STR\nBrawl\ncSP -10\ntHP -6\ntSP -4\nSteel"),

            new Move("Infernal Embrace", "INT\nPyrokinetic\ncMP -30\ncSP -10\ntHP -30\ntSP -10\nFire"),
            new Move("Ion Storm", "INT\nAerotheurge\ncaHP -10\ncaMP -20\ntaHP -10\ntaMP -20\nStorm"),
            new Move("Cold Embrace", "INT\nHydrosophist\ncMP -30\ncSP -10\ntHP -15\ntSP -25\nIce"),
            new Move("Stone Rain", "INT\nGeomagnetic\ncMP -24\ncaHP -10\ntaHP -12\ntaSP -10\nRock"),
            new Move("Wine Frenzy", "WIS\nForceOfNature\ncMP -30\ncWP -10\ncaSP -10\ntaHP -10\ntaSP -20\nPlant"),
            new Move("Hungry Tentacles", "INT\nEntropy\ncMP -30\ncWP -15\ncaSP -10\ncaWP -5\ntaHP -10\ntaSP -10\ntaMP -5\ntaWP -5\nVoid"),
            new Move("Dominate Mind", "WIS\nPsionics\ncMP -20\ncWP -10\ntWP -30\nPsy")
        };
    }
}
