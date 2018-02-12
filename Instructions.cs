using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vsic
{
    public enum Register : byte
    {
        A = 0,
        X = 1,
        L = 2,
        PC = 8,
        CC = 9,

        B = 3,
        S = 4,
        T = 5,
        F = 6
    }
    public enum Mnemonic : byte
    {
        // Arithmetic
        ADD = 0x18,
        SUB = 0x1C,
        MUL = 0x20,
        DIV = 0x24,

        // Bitwise
        AND = 0x40,
        OR = 0x44,
        SHIFTL = 0xA4,
        SHIFTR = 0xA8,

        // Flow control
        J = 0x3C,
        JEQ = 0x30,
        JGT = 0x34,
        JLT = 0x38,
        JSUB = 0x48,
        RSUB = 0x4C,

        // Registers
        LDA = 0x00,
        LDL = 0x08,
        LDX = 0x04,
        STA = 0x0C,
        STL = 0x14,
        STX = 0x10,
        CLEAR = 0xB4,
        RMO = 0xAC,

        // I/O
        RD = 0xD8,
        TD = 0xE0,
        WD = 0xDC,
        STCH = 0x54,

        // Other
        COMP = 0x28,
        COMPR = 0xA0,
        TIX = 0x2C
    }

    public enum Flag : byte
    {
        N = 0x1,
        I = 0x2,
        X = 0x4,
        B = 0x8,
        P = 0x10,
        E = 0x20
    }
}
