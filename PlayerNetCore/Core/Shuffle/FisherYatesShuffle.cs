//From answer of question: https://stackoverflow.com/questions/1816534/random-playlist-algorithm
using NekoPlayer.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace NekoPlayer.Core.Shuffle
{
    /// <summary>
    /// Fisher–Yates shuffle algorithm
    /// an simple algorithm to shuffle track indexes.
    /// </summary>
    public class FisherYatesShuffle : IShuffle
    {
        public int[] GetRandomize(int count, int seed = 0)
        {
            int[] array = new int[count];
            Random random = new Random(seed);
            for (int i = array.Length - 1; i > 0; i--)
            {
                int secondIndex = random.Next(i + 1);
                Swap(array, i, secondIndex);
            }
            return array;
        }
        private static void Swap(IList<int> array, int firstIndex, int secondIndex)
        {
            if (array[firstIndex] == 0)
            {
                array[firstIndex] = firstIndex;
            }
            if (array[secondIndex] == 0)
            {
                array[secondIndex] = secondIndex;
            }
            int temp = array[secondIndex];
            array[secondIndex] = array[firstIndex];
            array[firstIndex] = temp;
        }
    }
}
