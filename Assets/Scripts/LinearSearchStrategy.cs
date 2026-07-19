using System.Collections;
using UnityEngine;

// Checks every bar left to right until it finds the target. O(n).
public class LinearSearchStrategy : ISearchStrategy
{
    public string Name { get { return "Linear Search"; } }

    public IEnumerator Search(SortVisualizer viz, int targetValue, float stepDelay)
    {
        for (int i = 0; i < viz.Count; i++)
        {
            viz.Examine(i);
            yield return new WaitForSeconds(stepDelay);

            if (viz.GetValue(i) == targetValue)
            {
                viz.MarkFound(i);
                yield break;
            }
        }
    }
}
