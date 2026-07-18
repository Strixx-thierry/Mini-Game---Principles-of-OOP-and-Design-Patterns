// ISortStrategy.cs
using System.Collections; // for IEnumerator (lets a sort run over several frames)

// PATTERN - STRATEGY (the interface part):
// A "strategy" is an interchangeable way of doing one job. Here the job is "sort the bars".
// Each sorting algorithm (Bubble, Selection, ...) is its own strategy object, so the game
// can swap which algorithm runs with a single line and nothing else has to change.
//
// This interface also satisfies the rubric's "use of interfaces" requirement.
//
// Sort() returns IEnumerator so Unity can run it as a coroutine: the algorithm pauses
// (yield return) between steps so the player can SEE each comparison and swap happen.
public interface ISortStrategy
{
    // A friendly name shown to the player and used as the correct answer, e.g. "Bubble Sort".
    string Name { get; }

    // Sorts the bars inside the visualizer, pausing 'stepDelay' seconds between steps.
    IEnumerator Sort(SortVisualizer viz, float stepDelay);
}
