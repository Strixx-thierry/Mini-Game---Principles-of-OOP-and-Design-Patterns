// LinearSearchStrategy.cs
using System.Collections;
using UnityEngine;

// PATTERN - STRATEGY (concrete)  +  ALGORITHM (searching).
//
// Linear Search checks EVERY bar one by one, left to right, until it finds the target.
// Visually it sweeps across the whole row - that steady left-to-right scan is how the player
// recognises it.
//
// Big O: O(n) - in the worst case it looks at all n bars. Works on any order (no sorting needed).
public class LinearSearchStrategy : ISearchStrategy
{
    public string Name { get { return "Linear Search"; } }

    public IEnumerator Search(SortVisualizer viz, int targetValue, float stepDelay)
    {
        for (int i = 0; i < viz.Count; i++)
        {
            viz.Examine(i); // highlight the bar we are checking now
            yield return new WaitForSeconds(stepDelay);

            if (viz.GetValue(i) == targetValue)
            {
                viz.MarkFound(i);
                yield break; // found it - stop
            }
        }
    }
}
