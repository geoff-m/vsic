using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using sicdisasm;
using SICXE;

namespace Visual_SICXE
{
    public partial class DisassemblyWindow : Form
    {
        public DisassemblyWindow()
        {
            InitializeComponent();
            disasm = new Disassembler();
        }

        Disassembler disasm;

        public void UpdateDisassembly(int startAddress, int stopAddress)
        {
            var m = ((MainForm)Owner)?.Machine;
            if (m == null)
                return;

            m.Memory.Seek(startAddress, System.IO.SeekOrigin.Begin);
            var res = disasm.DisassembleWithContinue(m.Memory, startAddress, stopAddress - startAddress);
            
            
        }
    }
}
