// BinarySearchStrategy.cs
using System.Collections;
using UnityEngine;

// PATTERN - STRATEGY (concrete)  +  ALGORITHM (searching).
//
// Binary Search only works on a SORTED list. It looks at the MIDDLE bar, then throws away the
// half that cannot contain the target and repeats. Visually it jumps to the middle and keeps
// halving - big jumps, very few checks - which is how the player tells it from Linear Search.
//
// Big O: O(log n) - each step removes half the remaining bars.
public class BinarySearchStrategy : ISearchStrategy
{
    public string Name { get { return "Binary Search"; } }

    public IEnumerator Search(SortVisualizer viz, int targetValue, float stepDelay)
    {
        int low = 0;
        int high = viz.Count - 1;

        while (low <= high)
        {
            int mid = (low + high) / 2;

            viz.Examine(mid); // highlight the middle bar we are checking
            yield return new WaitForSeconds(stepDelay);

            int value = viz.GetValue(mid);
            if (value == targetValue)
            {
                viz.MarkFound(mid);
                yield break; // found it
            }
            else if (value < targetValue)
            {
                low = mid + 1;  // target is in the right half
            }
            else
            {
                high = mid - 1; // target is in the left half
            }
        }
    }
}
