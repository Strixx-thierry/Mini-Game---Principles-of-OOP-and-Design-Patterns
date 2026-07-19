// A Question that shows a search running, then asks the player to name it.
public class SearchingQuestion : Question
{
    private readonly ISearchStrategy strategy;
    private readonly int targetValue; // the bar height to look for

    public SearchingQuestion(ISearchStrategy strategy, int targetValue, string[] options, int correctIndex)
        : base("Watch the search. Which SEARCH algorithm is running?", options, correctIndex)
    {
        this.strategy = strategy;
        this.targetValue = targetValue;
    }

    // Same Present() call as SortingQuestion, different behaviour.
    public override void Present(SortVisualizer viz)
    {
        viz.SortAscending();          // binary search needs a sorted list
        viz.SetTarget(targetValue);
        viz.PlaySearch(strategy, targetValue);
    }
}
