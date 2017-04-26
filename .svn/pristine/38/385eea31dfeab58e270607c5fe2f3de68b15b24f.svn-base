using System;
using System.Collections.Generic;

namespace Atlass.LAS.Lib.Utilities
{
    public class TcSort
    {
        static public void Quicksort(IList<Double> prmElements, int prmLeft, int prmRight)
        {
            int left = prmLeft, right = prmRight;
            Double pivot = prmElements[(prmLeft + prmRight) / 2];
            Double tempElement;

            while (left <= right)
            {
                while (prmElements[left] < pivot)
                {
                    left++;
                }

                while (prmElements[right] > pivot)
                {
                    right--;
                }

                if (left <= right)
                {
                    // Swap
                    tempElement = prmElements[left];
                    prmElements[left] = prmElements[right];
                    prmElements[right] = tempElement;

                    left++;
                    right--;
                }
            }

            // Recursive calls
            if (prmLeft < right)
            {
                Quicksort(prmElements, prmLeft, right);
            }

            if (left < prmRight)
            {
                Quicksort(prmElements, left, prmRight);
            }
        }
    }
}