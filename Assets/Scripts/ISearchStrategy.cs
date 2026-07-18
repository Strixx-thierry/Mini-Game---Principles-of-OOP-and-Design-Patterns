// ISearchStrategy.cs
using System.Collections;

// PATTERN - STRATEGY (a second Strategy family, this time for searching):
// Each searching algorithm (Linear, Binary, ...) is its own strategy object with the same
// Search() method, so the game can run any of them through identical code.
//
// Returns IEnumerator so the search runs as a coroutine and pauses between steps, letting the
// player SEE which bars it checks and in what order.
public interface ISearchStrategy
{
    // Name shown to the player and used as the correct answer, e.g. "Linear Search".
    string Name { get; }

    // Looks for 'targetValue' among the bars, pausing 'stepDelay' seconds between checks.
    IEnumerator Search(SortVisualizer viz, int targetValue, float stepDelay);
}
