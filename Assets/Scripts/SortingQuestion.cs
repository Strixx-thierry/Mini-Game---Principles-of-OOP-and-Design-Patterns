// SortingQuestion.cs
//
// OOP - INHERITANCE:
// A SortingQuestion IS-A Question. It reuses the prompt/options/correct-index handling from
// the base class and adds the one thing it needs: which sorting Strategy to SHOW.
//
// "Show, don't tell": the player watches the algorithm run on the bars, then picks its name.
public class SortingQuestion : Question
{
    // The algorithm to run on the bars (a Strategy object).
    private readonly ISortStrategy strategy;

    public SortingQuestion(ISortStrategy strategy, string[] options, int correctIndex)
        : base("Watch the bars. Which SORTING algorithm is running?", options, correctIndex)
    {
        this.strategy = strategy;
    }

    // OOP - POLYMORPHISM: a sorting question shows itself by scrambling then sorting the bars.
    public override void Present(SortVisualizer viz)
    {
        viz.Shuffle();
        viz.PlaySort(strategy);
    }
}
