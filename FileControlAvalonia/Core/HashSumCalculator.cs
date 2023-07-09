using Avalonia.Markup.Xaml.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileControlAvalonia.Core
{
    public class HashSumCalculator
    {
        public static string CalculateMD5Hash(string filePath)
        {
            using (var md5 = MD5.Create())
            {

                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hashBytes = md5.ComputeHash(stream);
                    string hashString = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
                    return hashString;
                }
            }
        }
    }
}
