using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace AgonylMonsterEditor
{
    public static class Utils
    {
        public static Dictionary<ushort, Monster> MonsterList;

        public static byte[] SkipAndTakeLinqShim(ref byte[] originalBytes, int take, int skip = 0)
        {
            if (skip + take > originalBytes.Length)
            {
                return new byte[] { };
            }

            var outByte = new byte[take];
            Array.Copy(originalBytes, skip, outByte, 0, take);
            return outByte;
        }

        public static ushort BytesToUInt16(byte[] bytes)
        {
            return BitConverter.ToUInt16(bytes, 0);
        }

        public static uint BytesToUInt32(byte[] bytes)
        {
            return BitConverter.ToUInt32(bytes, 0);
        }

        public static string GetMyDirectory()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        }

        public static void LoadMonsterData()
        {
            var file = GetMyDirectory() + Path.DirectorySeparatorChar + "MON.bin";
            if (!File.Exists(file))
            {
                _ = MessageBox.Show("MON.bin not found hence names might not be accurate", "Agonyl Monster Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var npcDataFile = File.ReadAllBytes(file);
                MonsterList = new Dictionary<ushort, Monster>();
                for (var i = 4; i < npcDataFile.Length; i += 96)
                {
                    if (MonsterList.ContainsKey(BytesToUInt16(SkipAndTakeLinqShim(ref npcDataFile, i, 2))))
                    {
                        continue;
                    }

                    var item = new Monster()
                    {
                        Id = BytesToUInt16(SkipAndTakeLinqShim(ref npcDataFile, i, 2)),
                        Name = System.Text.Encoding.Default.GetString(SkipAndTakeLinqShim(ref npcDataFile, i + 4, 30)).Trim(),
                    };

                    MonsterList.Add(item.Id, item);
                }
            }
        }

        public static bool IsEmptyData(ushort data)
        {
            return data == 0 || data == 0xffff;
        }

        public static bool IsEmptyData(uint data)
        {
            return data == 0 || data == 0xffffffff;
        }

        public static bool IsEmptyData(string data)
        {
            return string.IsNullOrEmpty(data);
        }

        public static void ReplaceBytesAt(ref byte[] source, int startIndex, byte[] toReplace)
        {
            if (startIndex >= source.Length)
            {
                return;
            }

            for (var i = 0; i < toReplace.Length; i++)
            {
                if (startIndex >= source.Length)
                {
                    break;
                }

                source[startIndex] = toReplace[i];
                startIndex++;
            }
        }
    }
}
