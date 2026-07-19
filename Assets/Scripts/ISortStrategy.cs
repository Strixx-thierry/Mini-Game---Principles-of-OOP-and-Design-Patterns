using System.Collections;

// Strategy pattern: each sorting algorithm is a swappable object behind this interface.
public interface ISortStrategy
{
    string Name { get; } // e.g. "Bubble Sort" - also the correct answer

    IEnumerator Sort(SortVisualizer viz, float stepDelay);
}
