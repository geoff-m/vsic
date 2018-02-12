
namespace vsic
{
    enum AddressingMode
    {
        NotSet = 0,      // Should not be used. Indicates an error.
        Immediate = 1,   // Operand is *immediate.
        Simple = 2,      // Operand is **immediate.
        Indirect = 3     // Operand is immediate.
    }
}
