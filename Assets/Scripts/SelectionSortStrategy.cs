// SelectionSortStrategy.cs
using System.Collections;
using UnityEngine;

// PATTERN - STRATEGY (a second concrete strategy)  +  ALGORITHM 1 (sorting).
//
// Selection Sort scans the unsorted part to FIND THE SMALLEST value, then does a single
// swap to move it into place. Visually it makes only ONE swap per pass (not many little
// ones like Bubble Sort), which is how the player tells the two apart.
//
// Big O: O(n^2) comparisons, but at most O(n) swaps, and O(1) extra memory.
//
// Because it implements the SAME ISortStrategy interface as BubbleSortStrategy, the game
// can run either one through the exact same code - that is the point of the Strategy pattern.
public class SelectionSortStrategy : ISortStrategy
{
    public string Name { get { return "Selection Sort"; } }

    public IEnumerator Sort(SortVisualizer viz, float stepDelay)
    {
        int n = viz.Count;

        for (int i = 0; i < n - 1; i++)
        {
            // Assume the first unsorted item is the smallest, then look for a smaller one.
            int smallest = i;
            for (int j = i + 1; j < n; j++)
            {
                viz.Highlight(smallest, j); // comparing current-smallest against candidate
                yield return new WaitForSeconds(stepDelay);

                if (viz.GetValue(j) < viz.GetValue(smallest))
                {
                    smallest = j;
                }
            }

            // One swap to drop the smallest value into its final spot.
            if (smallest != i)
            {
                viz.Swap(i, smallest);
                yield return new WaitForSeconds(stepDelay);
            }
        }

        viz.ClearHighlight();
    }
}
