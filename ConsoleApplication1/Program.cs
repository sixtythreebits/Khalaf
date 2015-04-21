using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var keyString = "012345678901234567890123";
            var keyBytes = Encoding.UTF8.GetBytes(keyString);
            var mac = new MACTripleDES(keyBytes);
            var data = "please authenticate me example number one oh one point seven niner";
            var macResult = mac.ComputeHash(Encoding.UTF8.GetBytes(data));            
            Console.WriteLine(BitConverter.ToString(macResult));

            var d = mac.(macResult);
            Console.WriteLine(BitConverter.ToString(d));
        }
    }
}
