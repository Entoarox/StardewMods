namespace Entoarox.Framework.Experimental.BinObject
{
    public enum BinType
    {
        Null        = 0b00000000,
        Map         = 0b00000001,
        List        = 0b00000010,
        String      = 0b00000011,
        BoolFalse   = 0b00000100,
        BoolTrue    = 0b00000101,
        Single      = 0b00000110,
        Double      = 0b00000111,
        Int8        = 0b00001000,
        Int16       = 0b00001001,
        Int32       = 0b00001010,
        Int64       = 0b00001011,
        UInt8       = 0b00001100,
        UInt16      = 0b00001101,
        UInt32      = 0b00001110,
        UInt64      = 0b00001111,
        /*
        Unused1     = 0b00010000,
        ...
        UnusedX     = 0b11111111
        */
    }
}
