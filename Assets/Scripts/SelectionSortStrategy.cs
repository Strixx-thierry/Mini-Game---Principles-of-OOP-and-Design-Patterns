using System.Collections;
using UnityEngine;

// Finds the smallest value then places it with one swap per pass. O(n^2).
public class SelectionSortStrategy : ISortStrategy
{
    public string Name { get { return "Selection Sort"; } }

    public IEnumerator Sort(SortVisualizer viz, float stepDelay)
    {
        int n = viz.Count;

        for (int i = 0; i < n - 1; i++)
        {
            // Assume the first unsorted item is smallest, then look for a smaller one.
            int smallest = i;
            for (int j = i + 1; j < n; j++)
            {
                viz.Highlight(smallest, j);
                yield return new WaitForSeconds(stepDelay);

                if (viz.GetValue(j) < viz.GetValue(smallest))
                {
                    smallest = j;
                }
            }

            // One swap drops it into its final spot.
            if (smallest != i)
            {
                viz.Swap(i, smallest);
                yield return new WaitForSeconds(stepDelay);
            }
        }

        viz.ClearHighlight();
    }
}
