using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AdventOfCode._2016
{
    public class Day5
    {
        public static void Main()
        {
            const int PASS_LENGTH = 8;

            var input = File.ReadAllText("2016/input5.txt");
            var md5 = MD5.Create();

            var pass1 = new char[PASS_LENGTH];
            var pass2 = new char[PASS_LENGTH] { '.', '.','.','.','.','.','.','.'};
            var pass1Index = 0;
            var pass2Count = 0;
            ulong i = 0;

            while (pass1Index < PASS_LENGTH || pass2Count < PASS_LENGTH)
            {
                var testStr = $"{input}{i}";
                var testStrBytes = Encoding.ASCII.GetBytes(testStr);
                
                var hash = md5.ComputeHash(testStrBytes);
                var hashStr = BitConverter
                    .ToString(hash)
                    .Replace("-", string.Empty);

                if (hashStr.StartsWith("00000"))
                {
                    if (pass1Index < PASS_LENGTH)
                    {
                        pass1[pass1Index] = hashStr[5];
                        ++pass1Index;
                    }

                    var pass2Pos = hashStr[5] - '0';
                    if (pass2Pos < PASS_LENGTH)
                    {
                        var pass2Char = hashStr[6];
                        if (pass2[pass2Pos] == '.')
                        {
                            pass2[pass2Pos] = pass2Char;
                            ++pass2Count;
                        }
                    }
                }

                ++i;
            }

            Console.WriteLine(new string(pass1).ToLower());
            Console.WriteLine(new string(pass2).ToLower());
            Console.ReadLine();
        }
    }
}