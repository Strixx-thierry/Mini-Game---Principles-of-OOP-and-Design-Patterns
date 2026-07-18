// BubbleSortStrategy.cs
using System.Collections;
using UnityEngine;

// PATTERN - STRATEGY (a concrete strategy)  +  ALGORITHM 1 (sorting).
//
// Bubble Sort walks the list again and again, comparing each pair of NEIGHBOURS and
// swapping them if they are in the wrong order. Big values slowly "bubble" to the end.
// Visually it does LOTS of small adjacent swaps, which is how the player can recognise it.
//
// Big O: O(n^2) comparisons in the worst and average case, O(1) extra memory.
public class BubbleSortStrategy : ISortStrategy
{
    public string Name { get { return "Bubble Sort"; } }

    public IEnumerator Sort(SortVisualizer viz, float stepDelay)
    {
        int n = viz.Count;

        // After each outer pass the largest remaining value is in place, so we shrink range.
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                // Show which two neighbours we are comparing.
                viz.Highlight(j, j + 1);
                yield return new WaitForSeconds(stepDelay);

                // Wrong order? swap them.
                if (viz.GetValue(j) > viz.GetValue(j + 1))
                {
                    viz.Swap(j, j + 1);
                    yield return new WaitForSeconds(stepDelay);
                }
            }
        }

        viz.ClearHighlight();
    }
}
