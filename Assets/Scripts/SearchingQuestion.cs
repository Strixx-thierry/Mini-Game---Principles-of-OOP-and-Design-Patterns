// SearchingQuestion.cs
//
// OOP - INHERITANCE:
// A SearchingQuestion IS-A Question. Like SortingQuestion it reuses the base class, but it
// shows a SEARCH instead of a sort: the bars are put in order, one bar is marked as the
// target, and the player watches how the algorithm hunts for it.
public class SearchingQuestion : Question
{
    private readonly ISearchStrategy strategy;
    private readonly int targetValue; // the bar height the search is looking for

    public SearchingQuestion(ISearchStrategy strategy, int targetValue, string[] options, int correctIndex)
        : base("Watch the search. Which SEARCH algorithm is running?", options, correctIndex)
    {
        this.strategy = strategy;
        this.targetValue = targetValue;
    }

    // OOP - POLYMORPHISM: a searching question shows itself by sorting the bars, marking the
    // target, then running the search. Same Present() call, completely different behaviour.
    public override void Present(SortVisualizer viz)
    {
        viz.SortAscending();          // binary search only works on a sorted list
        viz.SetTarget(targetValue);   // colour the bar we are hunting for
        viz.PlaySearch(strategy, targetValue);
    }
}
