namespace Suinet.Wallet
{
    public static class Base64Converter
    {
        public static int Uint6ToB64(int nUint6)
        {
            if (nUint6 < 26)
            {
                return nUint6 + 65;
            }
            else if (nUint6 < 52)
            {
                return nUint6 + 71;
            }
            else if (nUint6 < 62)
            {
                return nUint6 - 4;
            }
            else if (nUint6 == 62)
            {
                return 43;
            }
            else if (nUint6 == 63)
            {
                return 47;
            }
            else
            {
                return 65;
            }
        }

        public static string ToB64(byte[] aBytes)
        {
            int nMod3 = 2;
            string sB64Enc = string.Empty;

            for (int nLen = aBytes.Length, nUint24 = 0, nIdx = 0; nIdx < nLen; nIdx++)
            {
                nMod3 = nIdx % 3;
                if (nIdx > 0 && ((nIdx * 4) / 3) % 76 == 0)
                {
                    sB64Enc += string.Empty;
                }
                nUint24 |= aBytes[nIdx] << ((16 >> nMod3) & 24);
                if (nMod3 == 2 || aBytes.Length - nIdx == 1)
                {
                    sB64Enc += char.ConvertFromUtf32(
                        Uint6ToB64((nUint24 >> 18) & 63)) +
                        char.ConvertFromUtf32(Uint6ToB64((nUint24 >> 12) & 63)) +
                        char.ConvertFromUtf32(Uint6ToB64((nUint24 >> 6) & 63)) +
                        char.ConvertFromUtf32(Uint6ToB64(nUint24 & 63));
                    nUint24 = 0;
                }
            }

            return sB64Enc.Substring(0, sB64Enc.Length - 2 + nMod3) +
                   (nMod3 == 2 ? string.Empty : nMod3 == 1 ? "=" : "==");
        }
    }
}
