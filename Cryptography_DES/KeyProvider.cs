using System.Collections;
using System.Diagnostics;
using System.Linq;

namespace Cryptography.TripleDes
{
    public class KeyProvider
    {
        private static readonly int[] _permutedChoice1Vector = new int[56]
        {
            57, 49, 41, 33, 25, 17, 9, 1, 58, 50, 42, 34, 26, 18, 10, 2, 59, 51, 43, 35, 27, 19, 11, 3, 60, 52, 44, 36,
            63, 55, 47, 39, 31, 23, 15, 7, 62, 54, 46, 38, 30, 22, 14, 6, 61, 53, 45, 37, 29, 21, 13, 5, 28, 20, 12, 4
        };


        private static readonly int[] _permutedChoice2Vector = new int[48]
        {
            14, 17, 11, 24, 1, 5, 3, 28, 15, 6, 21, 10, 23, 19, 12, 4, 26, 8, 16, 7, 27, 20, 13, 2, 41, 52, 31, 37, 47,
            55, 30, 40, 51, 45, 33, 48, 44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32
        };

        private BitArray _firstHalf;
        private BitArray _secondHalf;


        private BitArray ShiftLeft(BitArray key, int shift)
        {
            var output = new BitArray(28);
            var index = 0;
            for (var i = shift; index < key.Length; i++)
                output[index++] = key[i % key.Length];
            return output;
        }

        private BitArray ShiftRight(BitArray key, int shift)
        {
            var output = new BitArray(28);

            for (var i = 0; i < output.Length; i++)
                output[(i + 1) % output.Length] = key[i];


            if (shift == 2)
            {
                var firstOutput = output;
                output = new BitArray(28);

                for (var i = 0; i < output.Length; i++)
                    output[(i + 1) % output.Length] = firstOutput[i];
            }


            return output;
        }

        private BitArray PermuteChoice1(BitArray bitArray)
        {
            var output = new BitArray(56);

            for (var i = 0; i < _permutedChoice1Vector.Length; i++)
                output[i] = bitArray[_permutedChoice1Vector[i] - 1];

            return output;
        }

        private BitArray PermuteChoice2(BitArray bitArray)
        {
            var output = new BitArray(48);

            for (var i = 0; i < _permutedChoice2Vector.Length; i++)
                output[i] = bitArray[_permutedChoice2Vector[i] - 1];

            return output;
        }

        #region EncryptionKeys

        public BitArray[] GenerateKeys(BitArray primaryKey)
        {
            var output = new BitArray[16];

            var noParityBits = PermuteChoice1(primaryKey);

            var halves = noParityBits.Split();

            _firstHalf = halves[0];
            _secondHalf = halves[1];

            for (var i = 0; i < 16; i++)
            {
                output[i] = GenerateKeys(_firstHalf, _secondHalf, i);
                Debug.WriteLine(i);
                output[i].Debug();
            }

            return output;
        }

        private BitArray GenerateKeys(BitArray left, BitArray right, int round)
        {
            BitArray key;
            int shift;
            if (round == 0 || round == 1 || round == 8 || round == 15)
                shift = 1;
            else
                shift = 2;

            _firstHalf = ShiftLeft(left, shift);
            _secondHalf = ShiftLeft(right, shift);

            key = _firstHalf.Append(_secondHalf);
            key = PermuteChoice2(key);
            return key;
        }

        #endregion      
    }
}