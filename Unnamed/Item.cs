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

        public Item()
        {
            Name = "default";
            Cost = 0;
            Weight = 0;
            Description = "";
        }

        public Item(string name, int cost, double weight, string description)
        {
            Name = name;
            Cost = cost;
            Weight = weight;
            Description = description;
        }

        public Item(string name, int cost, double weight, string type, string description)
        {
            Name = name;
            Cost = cost;
            Weight = weight;
            Type = type;
            Description = description;
        }

        public Item(string name, int cost, double weight, int stack, string description)
        {
            Name = name;
            Cost = cost;
            Weight = weight;
            Stack = stack;
            Description = description;
        }
    }
}
