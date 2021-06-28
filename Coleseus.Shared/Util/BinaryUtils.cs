
using System.Text;

public class BinaryUtils
{

    private const string HEXES = "0123456789ABCDEF";

    public static string getHexString(byte[] raw)
    {
        return getHexString(raw, null);
    }

    public static string getHexString(byte[] raw, string separator)
    {
        bool sep = (null != separator) && !("".Equals(separator));

        if (raw == null)
        {
            return null;
        }
        StringBuilder hex = new StringBuilder(2 * raw.Length);
        foreach (byte b in raw)
        {
            hex.Append(HEXES[((b & 0xF0) >> 4)]);
            hex.Append(
                HEXES[(b & 0x0F)]);
            if (sep)
            {
                hex.Append(separator);
            }
        }
        return hex.ToString();
    }

}
