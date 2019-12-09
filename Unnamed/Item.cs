using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Unnamed
{
    [Magic]
    public class Item : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public string Name { get; set; }
        public int Cost { get; set; }
        public double Weight { get; set; }
        public string Description { get; set; }
        public int Stack { get; set; } = 1;
        public string Type { get; set; }
        public double TotalWeight => Weight * Stack;
        public int TotalCost => Cost * Stack;
        public Move ItemMove { get; set; }

        public Item()
        {
            Name = "default";
            Cost = 0;
            Weight = 0;
            Description = "";
        }

        public Item(string name, int cost, double weight, string type, string description)
        {
            Name = name;
            Cost = cost;
            Weight = weight;
            Type = type;
            Description = description;
        }

        public Item(string name, int cost, double weight, string type, Move move, string description)
        {
            Name = name;
            Cost = cost;
            Weight = weight;
            Type = type;
            ItemMove = move;
            Description = description;
        }

        public Item(string name, int cost, double weight, string type, int stack, Move move, string description)
        {
            Name = name;
            Cost = cost;
            Weight = weight;
            Stack = stack;
            Type = type;
            ItemMove = move;
            Description = description;
        }

        public static Item Find(string name) => List.FirstOrDefault(x => x.Name == name);
        public static List<Item> List { get; set; } = new List<Item> {
            new Item("Knuckles", 25, 2, "Weapon(Brawl)", new Move("Knuckle Punch", "STR", "Brawl", "cSP -6\ntHP -5\ntSP -1", "", "6 DMG"), "Brawl weapon\n6 DMG"),
            new Item("Steel Gauntlet", 125, 5, "Weapon(Brawl)", new Move("Gauntlet Punch", "STR", "Brawl", "cSP -10\ntHP -8\ntSP -2", "Steel", "10 DMG"), "Brawl weapon\n10 DMG"),
            new Item("Metal Claws", 125, 4, "Weapon(Brawl)", new Move("Cutting Claws", "STR", "Brawl", "cSP -12\ntHP -12", "Steel", "12 DMG"), "Brawl weapon\n12 DMG"),
            new Item("Sword", 70, 10,"Weapon(Sword)", new Move("Sword strike", "STR", "Sword", "cSP -10\ntHP -10", "Steel", "10 DMG"), "Sword\n10 DMG"),
            new Item("War Axe", 80, 12,"Weapon(Axe)", new Move("Axe strike", "STR", "Axe", "cSP -12\ntHP -12", "Steel", "12 DMG"), "Axe\n12 DMG"),
            new Item("Mace", 90, 14,"Weapon(Blunt)", new Move("Mace strike", "STR", "Blunt", "cSP -14\ntHP -14", "Steel", "14 DMG"), "Blunt weapon\n14 DMG"),
            new Item("Greatsword", 140, 17,"Weapon(Sword)", new Move("Greatsword strike", "STR", "Sword", "cSP -17\ntHP -17", "Steel", "17 DMG"), "Sword\n17 DMG"),
            new Item("Battleaxe", 160, 21,"Weapon(Axe)", new Move("Battleaxe strike", "STR", "Axe", "cSP -21\ntHP -21", "Steel", "21 DMG"), "Axe\n21 DMG"),
            new Item("Warhammer", 180, 25,"Weapon(Blunt)", new Move("Warhammer strike", "STR", "Blunt", "cSP -25\ntHP -25", "Steel", "25 DMG"), "Blunt weapon\n25 DMG"),
            new Item("Katana", 2800, 7,"Weapon(Sword)", new Move("Katana strike", "STR", "Sword", "cSP -11\ntHP -11", "Steel", "11 DMG"), "Sword\n11 DMG"),
            //new Item("Combat Oil Umbrella", 125, 6, "STR\nBluntWeapon\ncSP -10\ntHP -3\ntSP -7\nNormal\n[Block] chance"),

            new Item("Dagger", 25, 2.5,"Weapon(Short)", new Move("Dagger strike", "DEX", "ShortBlade", "cSP -5\ntHP -5", "Steel", "5 DMG"), "Short blade\n5 DMG"),
            new Item("Sickle", 42, 3,"Weapon(Short)", new Move("Sickle strike", "DEX", "ShortBlade", "cSP -7\ntHP -6\ntSP -1", "Steel", "7 DMG"), "Short blade\n7 DMG"),
            new Item("Short Bow", 70, 9,"Weapon(Bow)", new Move("Fast shot", "DEX", "SharpShooting", "cSP -7\ntHP -10\nUses Arrow", "", "10 DMG\nArrow required"), "Ranged\n10 DMG\nUses Arrow"),
            new Item("Long Bow", 80, 11,"Weapon(Bow)", new Move("Long shot", "DEX", "SharpShooting", "cSP -9\ntHP -12\nUses Arrow", "", "12 DMG\nArrow required"), "Ranged\n12 DMG\nUses Arrow"),
            new Item("Hand Crossbow", 60, 7,"Weapon(Crossbow)", new Move("Quick shot", "DEX", "SharpShooting", "cSP -5\ntHP -8\nUses Bolt(Small)", "", "8 DMG\nBolt(Small) required"), "Ranged\n8 DMG\nUses Bolt(Small)"),
            new Item("Crossbow", 90, 14,"Weapon(Crossbow)", new Move("Snipe shot", "DEX", "SharpShooting", "cSP -10\ntHP -15\nUses Bolt(Medium)", "", "15 DMG\nBolt(Medium) required"), "Ranged\n15 DMG\nUses Bolt(Medium)"),
            new Item("Arrow", 1, 0.025, "Ammo", 20, null, "Bow ammo"),
            new Item("Bolt(Small)", 1, 0.23, "Ammo", 20, null, "Hand Crossbow ammo"),
            new Item("Bolt(Medium)", 1, 0.45, "Ammo", 20, null, "Crossbow ammo"),
            new Item("Bladed Boomerang", 60, 1, "Weapon(Throwing)", new Move("Boomerang blade", "DEX", "Throwing", "cSP -8\ntHP -8", "Steel", "8 DMG"), "Throwing weapon\n8 DMG"),
            new Item("Tessen", 25, 3, "Weapon(Short+Block)", new Move("Tessen strike", "DEX", "ShortBlade", "cSP -8\ntHP -8", "Steel", "8 DMG"), "ShortBlade weapon\n8 DMG\nBlock chance"),
            new Item("Shuriken", 1, 0.1,"Weapon(Throwing)", 10, new Move("Shuriken", "DEX", "Throwing", "cSP -1\ntHP -4\nUses Shuriken", "Steel", "4 DMG"), "Throwing weapon\n4 DMG\nConsumable"),
            new Item("Knife", 1, 0.2,"Weapon(Throwing)", 10,new Move("Knife Throw", "DEX", "Throwing", "cSP -2\ntHP -6\nUses Knife", "Steel", "6 DMG"), "Throwing weapon\n6 DMG\nConsumable"),
            new Item("Kunai", 1, 1,"Weapon(Throwing)", 4,new Move("Kunai", "DEX", "Throwing", "cSP -3\ntHP -8\nUses Kunai", "Steel", "8 DMG"), "Throwing weapon\n8 DMG\nConsumable"),

            new Item("Green wear", 75, 1, "Robe", "SPR +3\nMPR +3"),
            new Item("Robe of innocence", 125, 1,"Robe", "MPR +6\nWPR +4"),
            new Item("Novice Robe", 125, 2,"Robe", "MA +6\nMPR +3"),
            new Item("Apprentice Robe", 350, 2,"Robe", "MA +10\nMPR +5"),
            new Item("Adept Robe", 725, 2,"Robe", "MA +14\nMPR +7"),
            new Item("Expert Robe", 2480, 2,"Robe", "MA +20\nMPR +10"),
            new Item("Master Robe", 4275, 2,"Robe", "MA +30\nMPR +15"),

            new Item("Healing potion", 25, 0.5, "Consumable",new Move("Healing potion", "END", "END", "cSP -10\ncHP +20\nConsumable", "", "10 SP, Heals *20* HP"), "10 SP, Heals *20* HP\nConsumable"),
            new Item("Mana potion", 25, 0.5, "Consumable",new Move("Mana potion", "INT", "INT", "cSP -10\ncMP +20\nConsumable", "", "10 SP, Restores *20* MP"), "10 SP, Restores *20* MP\nConsumable"),
            new Item("Will potion", 25, 0.5, "Consumable",new Move("Will potion", "WIS", "WIS", "cSP -10\ncWP +20\nConsumable", "", "10 SP, Restores *20* WP"), "10 SP, Restores *20* WP\nConsumable"),
            new Item("Freeze bomb", 55, 1,"Greenade",new Move("Freeze bomb", "DEX", "STR", "cSP -12\ntaSP -6\nConsumable", "Ice", "6 SP DMG to all enemies"), "Freezes enemies (-SP)\nConsumable"),
            new Item("Charming bomb", 135, 1,"Greenade",new Move("Charming bomb", "DEX", "STR", "cSP -12\ntaWP -6\nConsumable", "Psy", "6 WP DMG to all enemies"), "Charming enemies (-WP)\nConsumable"),
            new Item("Alchemist's fire", 155, 1,"Greenade",new Move("Alchemist's fire", "DEX", "STR", "cSP -12\ntHP -6\ntaHP -6\nConsumable", "Fire", "6 HP Fire DMG to target and all enemies"), "Burning enemies (-HP)\nConsumable"),
            new Item("Storm bomb", 55, 1,"Greenade",new Move("Storm bomb", "DEX", "STR", "cSP -12\ntaHP -3\ntaMP -3\nConsumable", "Storm", "3 HP & MP Storm DMG to all enemies"), "Electrifies enemies (-HP&MP)\nConsumable"),
            new Item("Rock bomb", 55, 1,"Greenade",new Move("Rock bomb", "DEX", "STR", "cSP -12\ntHP -6\ntaHP -6\nConsumable", "Rock", "6 HP Rock DMG to target and all enemies"), "Rocks to enemies (-HP)\nConsumable"),
            new Item("Poison bomb", 55, 1,"Greenade",new Move("Poison bomb", "DEX", "STR", "cSP -12\ntaHP -3\ntaSP -1\ntaWP -2\nConsumable", "Poison", "3 HP, 2 WP & 1 SP Poison DMG to all enemies"), "Poison enemies\nConsumable"),
            new Item("Void bomb", 55, 1,"Greenade",new Move("Void bomb", "DEX", "STR", "cSP -16\ncaHP -6\ntaHP -10\ntaWP -2\nConsumable", "Void", "Deals damage to everyone, especially enemies"), "Mess everything with void bomb\nConsumable"),

            new Item("Padded Armor", 45, 3,"Armor", "PA +4"),
            new Item("Leather Armor", 75, 5,"Armor", "PA +8"),
            new Item("Hide Armor", 75, 7,"Armor", "PA +12"),
            new Item("Studded Leather Armor", 338, 10,"Armor", "PA +18"),
            new Item("Chain Shirt", 375, 12,"Armor", "PA +24"),
            new Item("Breastplate", 1500, 12,"Armor", "PA +28"),
            new Item("Scale Mail", 715, 16,"Armor", "PA +36"),
            //new Item("Spiked Armor", 1040, 16, "END\nMelee\ncSP -10\ntHP -5\ntSP -5\nPA +36"),
            new Item("Half Plate", 5625, 20,"Armor", "PA +50"),
            new Item("Chain Mail", 1040, 25,"Armor", "PA +56"),
            new Item("Plate Armor", 11250, 29,"Armor", "PA +64"),
            new Item("Shield", 75, 7,"Shield", "Chance to [Block] incoming attack"),

            new Item("Small Backpack", 15, 3,"Backpack", "CW +15"),
            new Item("Medium Backpack", 80, 3,"Backpack", "CW +30"),
            new Item("Large Backpack", 100, 3,"Backpack", "CW +45"),
            new Item("Spacious Backpack", 150, 3,"Backpack", "CW +60"),
            new Item("Advanced Backpack", 250, 3,"Backpack", "CW +90"),
            new Item("Camping Supplies", 15, 3, "Supplies", 4, null, "Need to rest outdoors, consumable"),

            new Item("Sacrificial Dagger", 135, 3,"Weapon(Short)", new Move("Draining strike", "DEX", "ShortBlade", "cSP -6\ntHP -4\nDrain", "Steel", "4 DMG"), "Short blade\n4 DMG\nDrain"),
            new Item("Chillrend", 4600, 7,"Weapon(Sword)", new Move("Freezing strike", "STR", "Sword", "cSP -12\ntHP -10\ntSP -2", "Ice", "12 DMG"), "Frost sword\n12 DMG"),
            new Item("Benizakura", 6666, 8,"Weapon(Sword)", new Move("Dawn of Blood", "STR", "Sword", "cSP -10\ncWP -10\ncHP -5\ntHP -14\ntWP -6\nDrain", "Void", "Drains much, gains even more"), "Crimson sword of ultimate power at the great cost..."),
            new Item("Dawnbreaker", 4740, 7,"Weapon(Sword)", new Move("Sunlight Owerdrive", "STR", "Sword", "cSP -10\ncWP -5\ntHP -12\ntWP -3", "Light", "12 DMG"), "Uses will to deal more damage and make enemies kneel sooner"),
            new Item("Healing stone", 4444, 2,"Artefact", "HPR +4"),
        };
    }
}
