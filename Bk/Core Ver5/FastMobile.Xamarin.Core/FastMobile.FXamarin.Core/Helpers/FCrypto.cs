using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FastMobile.FXamarin.Core
{
    public static class FCrypto
    {
        public static readonly string Key;

        private static readonly byte[] Bytes;

        static FCrypto()
        {
            Key = UTF8Encoding.UTF8.GetString(HexToBytes("4640737424313530382345496E766F696365"));
            Bytes = new byte[] { 0x46, 0x40, 0x73, 0x74, 0x45, 0x2D, 0x49, 0x6E, 0x76, 0x6F, 0x69, 0x63, 0x65, 0x23, 0x31, 0x35, 0x30, 0x38 };
        }

        public static byte[] HexToBytes(string Hex)
        {
            int numBytes = Hex.Length / 2;
            byte[] bytes = new byte[numBytes - 1 + 1];
            for (int n = 0; n <= numBytes - 1; n++)
            {
                string hexByte = Hex.Substring(n * 2, 2);
                bytes[n] = (byte)int.Parse(hexByte, System.Globalization.NumberStyles.HexNumber);
            }
            return bytes;
        }

        public static string BytesToHex(byte[] bytes)
        {
            var hex = new StringBuilder();
            bytes.ForEach((x) => hex.AppendFormat("{0:X2}", x));
            return hex.ToString();
        }

        public static string AESEncrypt(this string plainText, string key)
        {
#if DEBUG
            try
            {
#endif
                byte[] clearBytes = Encoding.Unicode.GetBytes(plainText);
                string result = "";
                using (var encryptor = Aes.Create())
                {
                    var pdb = new Rfc2898DeriveBytes(key, Bytes);
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(clearBytes, 0, clearBytes.Length);
                            cs.Close();
                        }
                        result = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return result;
#if DEBUG
            }
            catch (Exception ex)
            {
                Xamarin.Forms.MessagingCenter.Send(new FMessage(ex.Message + Environment.NewLine + ex.StackTrace), FChannel.ALERT_BY_MESSAGE);
                return string.Empty;
            }
#endif
        }

        public static string AESDecrypt(this string cipherText, string key)
        {
#if DEBUG
            try
            {
#endif
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                string result = "";
                using (var encryptor = Aes.Create())
                {
                    var pdb = new Rfc2898DeriveBytes(key, Bytes);
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        result = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return result;
#if DEBUG
            }
            catch (Exception ex)
            {
                Xamarin.Forms.MessagingCenter.Send(new FMessage(ex.Message + Environment.NewLine + ex.StackTrace), FChannel.ALERT_BY_MESSAGE);
                return string.Empty;
            }
#endif
        }

        public static string MD5(this object input)
        {
            var md = MD5CryptoServiceProvider.Create();
            var enc = new UTF8Encoding();
            var sb = new StringBuilder();

            input ??= "";
            byte[] buffer = enc.GetBytes(input.ToString());
            byte[] hash = md.ComputeHash(buffer);

            hash.ForEach((x) => sb.Append(x.ToString("x2")));
            return sb.ToString();
        }

        public static string Crc32(this object hash)
        {
            var b = new uint[] { 0000000000, 1996959894, 3993919788, 2567524794, 0124634137, 1886057615, 3915621685, 2657392035, 0249268274, 2044508324, 3772115230, 2547177864, 0162941995, 2125561021, 3887607047, 2428444049, 0498536548, 1789927666, 4089016648, 2227061214, 0450548861, 1843258603, 4107580753, 2211677639, 0325883990, 1684777152, 4251122042, 2321926636, 0335633487, 1661365465, 4195302755, 2366115317, 0997073096, 1281953886, 3579855332, 2724688242, 1006888145, 1258607687, 3524101629, 2768942443, 0901097722, 1119000684, 3686517206, 2898065728, 0853044451, 1172266101, 3705015759, 2882616665, 0651767980, 1373503546, 3369554304, 3218104598, 0565507253, 1454621731, 3485111705, 3099436303, 0671266974, 1594198024, 3322730930, 2970347812, 0795835527, 1483230225, 3244367275, 3060149565, 1994146192, 0031158534, 2563907772, 4023717930, 1907459465, 0112637215, 2680153253, 3904427059, 2013776290, 0251722036, 2517215374, 3775830040, 2137656763, 0141376813, 2439277719, 3865271297, 1802195444, 0476864866, 2238001368, 4066508878, 1812370925, 0453092731, 2181625025, 4111451223, 1706088902, 0314042704, 2344532202, 4240017532, 1658658271, 0366619977, 2362670323, 4224994405, 1303535960, 0984961486, 2747007092, 3569037538, 1256170817, 1037604311, 2765210733, 3554079995, 1131014506, 0879679996, 2909243462, 3663771856, 1141124467, 0855842277, 2852801631, 3708648649, 1342533948, 0654459306, 3188396048, 3373015174, 1466479909, 0544179635, 3110523913, 3462522015, 1591671054, 0702138776, 2966460450, 3352799412, 1504918807, 0783551873, 3082640443, 3233442989, 3988292384, 2596254646, 0062317068, 1957810842, 3939845945, 2647816111, 0081470997, 1943803523, 3814918930, 2489596804, 0225274430, 2053790376, 3826175755, 2466906013, 0167816743, 2097651377, 4027552580, 2265490386, 0503444072, 1762050814, 4150417245, 2154129355, 0426522225, 1852507879, 4275313526, 2312317920, 0282753626, 1742555852, 4189708143, 2394877945, 0397917763, 1622183637, 3604390888, 2714866558, 0953729732, 1340076626, 3518719985, 2797360999, 1068828381, 1219638859, 3624741850, 2936675148, 0906185462, 1090812512, 3747672003, 2825379669, 0829329135, 1181335161, 3412177804, 3160834842, 0628085408, 1382605366, 3423369109, 3138078467, 0570562233, 1426400815, 3317316542, 2998733608, 0733239954, 1555261956, 3268935591, 3050360625, 0752459403, 1541320221, 2607071920, 3965973030, 1969922972, 0040735498, 2617837225, 3943577151, 1913087877, 0083908371, 2512341634, 3803740692, 2075208622, 0213261112, 2463272603, 3855990285, 2094854071, 0198958881, 2262029012, 4057260610, 1759359992, 0534414190, 2176718541, 4139329115, 1873836001, 0414664567, 2282248934, 4279200368, 1711684554, 0285281116, 2405801727, 4167216745, 1634467795, 0376229701, 2685067896, 3608007406, 1308918612, 0956543938, 2808555105, 3495958263, 1231636301, 1047427035, 2932959818, 3654703836, 1088359270, 0936918000, 2847714899, 3736837829, 1202900863, 0817233897, 3183342108, 3401237130, 1404277552, 0615818150, 3134207493, 3453421203, 1423857449, 0601450431, 3009837614, 3294710456, 1567103746, 0711928724, 3020668471, 3272380065, 1510334235, 0755167117 };
            var f = uint.MaxValue;
            foreach (char h in hash?.ToString())
                f = ((f >> 8) ^ (b[(f ^ h) & 255]) >> 5);
            return (f ^ -1).ToString();
        }
    }
}