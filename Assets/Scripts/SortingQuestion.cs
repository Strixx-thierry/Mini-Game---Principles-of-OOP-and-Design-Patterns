// A Question that shows a sort running, then asks the player to name it.
public class SortingQuestion : Question
{
    private readonly ISortStrategy strategy;

    public SortingQuestion(ISortStrategy strategy, string[] options, int correctIndex)
        : base("Watch the bars. Which SORTING algorithm is running?", options, correctIndex)
    {
        this.strategy = strategy;
    }

    // Scramble, then sort.
    public override void Present(SortVisualizer viz)
    {
        viz.Shuffle();
        viz.PlaySort(strategy);
    }
}
