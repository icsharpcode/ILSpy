// The constants are out of range for the enums' underlying types, so they must not be
// recovered as the members whose bit patterns they happen to truncate to (Val1 in both
// cases): the enum arithmetic would compute a different result than the IL does.
public enum ByteEnum : byte
{
	Val1 = 112
}
public static class EnumArithmeticOutOfRange
{
	public static int SubtractFromUShortEnum(UShortEnum value)
	{
		return (int)value - -501;
	}
	public static int SubtractFromByteEnum(ByteEnum value)
	{
		return (int)value - 70000;
	}
}
public enum UShortEnum : ushort
{
	Val1 = 65035
}
