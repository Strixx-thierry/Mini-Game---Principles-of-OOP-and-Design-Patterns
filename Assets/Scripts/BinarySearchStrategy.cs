using System.Collections;
using UnityEngine;

// Checks the middle bar and discards the half that cannot hold the target. Needs a sorted list. O(log n).
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

            viz.Examine(mid);
            yield return new WaitForSeconds(stepDelay);

            int value = viz.GetValue(mid);
            if (value == targetValue)
            {
                viz.MarkFound(mid);
                yield break;
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
