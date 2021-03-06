﻿namespace SICXE
{
    public enum Register : byte
    {
        A = 0,  // accumulator
        X = 1,  // index
        S = 4,  // general
        T = 5,  // general
        F = 6,  // floating accumulator (not implemented)

        L = 2,  // return address
        B = 3,  // base address
        PC = 8, // instruction pointer
        CC = 9  // status word (result of comparison)
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
        RSUB = 0x4C,

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
        LDCH = 0x50,
        STCH = 0x54,

        // I/O
        RD = 0xD8, // not implemented
        TD = 0xE0, // not implemented
        WD = 0xDC, // not implemented
        
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
