using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Biz
{
    public class LicenseUtil
    {
        static char[] hexDigits = {	  '0', '1', '2', '3', '4', '5', '6', '7',
									  '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

        public string GetKey(int seed, KeyByteSet[] keyByteSets)
        {
            if (keyByteSets.Length < 2)
            {
                throw new InvalidOperationException("The KeyByteSet array must be of length 2 or greater.");
            }

            Array.Sort(keyByteSets, new KeyByteSetComparer());

            bool allKeyByteNosDistinct = true;

            var keyByteCheckedNos = new List<int>();

            int maxKeyByteNo = 0;

            foreach (var keyByteSet in keyByteSets)
            {
                if(!(keyByteCheckedNos.Contains(keyByteSet.KeyByteNo)))
                {
                    keyByteCheckedNos.Add(keyByteSet.KeyByteNo);

                    if(keyByteSet.KeyByteNo > maxKeyByteNo)
                    {
                        maxKeyByteNo = keyByteSet.KeyByteNo;
                    }
                }
                else
                {
                    allKeyByteNosDistinct = false;
                    break;
                }
            }

            if(!allKeyByteNosDistinct)
            {
                throw new InvalidOperationException("The KeyByteSet array contained at least 1 item with a duplicate KeyByteNo value.");
            }

            if(maxKeyByteNo != keyByteSets.Length)
            {
                throw new InvalidOperationException("The values for KeyByteNo in each KeyByteSet item must be sequential and start with the number 1.");
            }

            var keyBytes = new byte[keyByteSets.Length];

            for(int i = 0; i < keyByteSets.Length; i++)
            {
                keyBytes[i] = GetKeyByte(
                                    seed, 
                                    keyByteSets[i].KeyByteA, 
                                    keyByteSets[i].KeyByteB, 
                                    keyByteSets[i].KeyByteC
                              );
            }


            string result = seed.ToString("X8"); // 8 digit hex;

            for(int i = 0; i < keyBytes.Length; i++)
            {
                result = result + keyBytes[i].ToString("X2");
            }

            result = result + GetChecksum(result);

         
            int startPos = 7;

            while (startPos < (result.Length - 1))
            {
                result = result.Insert(startPos, "-");

                startPos = startPos + 7;
            }

            return result;
        }

    }

     public class KeyByteSetComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            KeyByteSet kbs1 = (KeyByteSet)x;
            KeyByteSet kbs2 = (KeyByteSet)y;

            if (kbs1.KeyByteNo > kbs2.KeyByteNo)
            {
                return 1;
            }

            if (kbs1.KeyByteNo < kbs2.KeyByteNo)
            {
                return -1;
            }

            return 0;
        }
    }
        
        
    
}
