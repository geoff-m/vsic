
namespace sicsim
{
    enum AddressingMode
    {
        // These are the three that can be specified in a program.
        Simple = 1,
        Immediate = 2,
        Indirect = 3,


        // These do not exist as far as a program is concerned, but are of concern to assemblers and machines.
        RelativeToProgramCounter = 8,
        RelativeToBase = 9
            // RelativeToIndex...?
    }
}
