using System;
using System.Collections.Generic;
using System.Text;

namespace LinakDeskClient
{
    internal class ArrayHelper
    {
        public static T[] CombineTwoArrays<T>(T[] a1, T[] a2)
        {
            T[] arrayCombined = new T[a1.Length + a2.Length];
            Array.Copy(a1, 0, arrayCombined, 0, a1.Length);
            Array.Copy(a2, 0, arrayCombined, a1.Length, a2.Length);
            return arrayCombined;
        }
    }
}
