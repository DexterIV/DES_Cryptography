using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Cryptography.TripleDes
{
    public class DesEncryption
    {
        private readonly DesProvider _des = new DesProvider();
        private readonly KeyProvider _key = new KeyProvider();
        private BitArray _leftSide;
        private BitArray _rightSide;
        private BitArray _tempLeft;

        public string Encrypt(string text, string key)
        {
            text = string.Join(string.Empty,
                text.Select(
                    c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                )
            );

            key = string.Join(string.Empty,
                key.Select(
                    c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                )
            );

 

            var keyBits = new BitArray(key.Select(c => c == '1').ToArray());

            var result = "";

            //split the string into blocks of 64 bits
            var output = Enumerable.Range(0, text.Length / 64)
                .Select(x => text.Substring(x * 64, 64)).ToList();

            if (text.Length % 64 != 0)
            {
                var missing = text.Skip(output.Count * 64).Take(64).ToList();
                while (missing.Count() != 64)
                {
                    missing.Add('0');
                }

                output.Add(string.Join("", missing));
            }

            var newInput = output.ToArray();

            var subKeys = _key.GenerateKeys(keyBits); 
            for (var i = 0; i < newInput.Length; i++)
            {
                var bits = new BitArray(newInput[i].Select(c => c == '1').ToArray());

                //Initial Permutation
                var chunk = _des.InitialPermutation(bits);

                //split the chunk in two halves 32 bit each
                var sides = chunk.Split();

                _leftSide = sides[0];
                _rightSide = sides[1];


                _leftSide = sides[1];
                _tempLeft = sides[0];

                for (var j = 0; j < 16; j++)
                {
                    _rightSide = _des.RoundFunction(_rightSide, subKeys[j]); //rn = f(rn-1,kn)
                    _rightSide = _tempLeft.Xor(_rightSide); //rn = ln-1 xor rn

                    _tempLeft = _leftSide;
                    _leftSide = _rightSide;
                }

                var finalPerm = _rightSide.Append(_tempLeft);
                finalPerm = _des.FinalPermutation(finalPerm);
                result += BinaryStringToHexString(finalPerm);
            }
            return result;
        }

        public string Decrypt(string cipher, string key)
        {
            cipher = string.Join(string.Empty,
                cipher.Select(
                    c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                )
            );

            key = string.Join(string.Empty,
                key.Select(
                    c => Convert.ToString(Convert.ToInt32(c.ToString(), 16), 2).PadLeft(4, '0')
                )
            );

            var keyBits = new BitArray(key.Select(c => c == '1').ToArray());

            var result = "";

            //split the string into blocks of 64 bits
            var output = Enumerable.Range(0, cipher.Length / 64)
                .Select(x => cipher.Substring(x * 64, 64)).ToList();


            var newInput = output.ToArray();

            var subKeys = _key.GenerateKeys(keyBits).Reverse().ToArray();
            for (var i = 0; i < newInput.Length; i++)
            {
                var bits = new BitArray(newInput[i].Select(c => c == '1').ToArray());
                //Initial Permutation
                var chunk = _des.InitialPermutation(bits);

                //split the chunk in two halves 32 bit each
                var sides = chunk.Split();

                _leftSide = sides[0];
                _rightSide = sides[1];

                

                _leftSide = sides[1];
                _tempLeft = sides[0];

                for (var j = 0; j < 16; j++)
                {
                    _rightSide = _des.RoundFunction(_rightSide, subKeys[j]); //rn = f(rn-1,kn)
                    _rightSide = _tempLeft.Xor(_rightSide); //rn = ln-1 xor rn

                    _tempLeft = _leftSide;
                    _leftSide = _rightSide;
                }

                var finalPerm = _rightSide.Append(_tempLeft);
                finalPerm = _des.FinalPermutation(finalPerm);
                result += BinaryStringToHexString(finalPerm);
            }
            return result;
        }

        private string BinaryStringToHexString(BitArray bits)
        {
            var sb = new StringBuilder(bits.Length / 4);

            for (var i = 0; i < bits.Length; i += 4)
            {
                var v = (bits[i] ? 8 : 0) |
                        (bits[i + 1] ? 4 : 0) |
                        (bits[i + 2] ? 2 : 0) |
                        (bits[i + 3] ? 1 : 0);

                sb.Append(v.ToString("X1")); // Or "X1"
            }

            return sb.ToString();
        }
    }
}