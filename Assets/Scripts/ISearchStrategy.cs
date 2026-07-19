using System.Collections;

// Strategy pattern again, for searching.
public interface ISearchStrategy
{
    string Name { get; } // e.g. "Linear Search"

    IEnumerator Search(SortVisualizer viz, int targetValue, float stepDelay);
}
