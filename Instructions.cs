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
        ADDR = 0x90,
        SUB = 0x1C,
        SUBR = 0x94,
        MUL = 0x20,
        MULR = 0x98,
        DIV = 0x24,
        DIVR = 0x9C,

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
        JSUB = 0x48, // not implemented
        RSUB = 0x4C, // not implemented

        // Registers
        LDA = 0x00,
        LDB = 0x68,
        LDL = 0x08,
        LDS = 0x6C,
        LDT = 0x74,
        LDX = 0x04,
        STA = 0x0C,
        STB = 0x78,
        STL = 0x14,
        STS = 0X7C,
        STT = 0x84,
        STX = 0x10,
        CLEAR = 0xB4,
        RMO = 0xAC,

        // I/O
        RD = 0xD8, // not implemented
        TD = 0xE0, // not implemented
        WD = 0xDC, // not implemented
        STCH = 0x54, // not implemented

        // Other
        COMPR = 0xA0,
        COMP = 0x28,
        TIX = 0x2C,
        TIXR = 0xB8
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
