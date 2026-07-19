using System.Collections;
using UnityEngine;

// Swaps out-of-order neighbours until big values bubble to the end. O(n^2).
public class BubbleSortStrategy : ISortStrategy
{
    public string Name { get { return "Bubble Sort"; } }

    public IEnumerator Sort(SortVisualizer viz, float stepDelay)
    {
        int n = viz.Count;

        // Each pass puts the largest remaining value in place, so the range shrinks.
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                viz.Highlight(j, j + 1);
                yield return new WaitForSeconds(stepDelay);

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
