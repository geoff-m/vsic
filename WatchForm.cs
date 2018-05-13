using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace vsic
{
    public partial class WatchForm : Form
    {
        public WatchForm()
        {
            InitializeComponent();
            
        }

        HashSet<Watch> symbols = new HashSet<Watch>();
        public int WatchCount => symbols.Count;
        
        public void AddWatch(string name, int startAddress, int size)
        {
            var w = new Watch(name, startAddress, size);
            if (!symbols.Add(w))
            {
                throw new ArgumentException("A watch with that name already exists!");
            }

            var newItem = new ListViewItem(name);
            newItem.SubItems.Add(startAddress.ToString("X6"));
            newItem.SubItems.Add(size.ToString());
            watchLV.Items.Add(newItem);
        }

        
        

    }

    public class Watch : ListViewItem
    {
        public Watch(string name, int startAddress, int size)
        {
            Name = name;
            StartAddress = startAddress;
            Size = size;
        }

        public int StartAddress
        { get; set; }

        public int Size
        { get; set; }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
