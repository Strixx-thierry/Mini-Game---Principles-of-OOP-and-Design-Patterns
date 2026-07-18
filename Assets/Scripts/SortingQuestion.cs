// SortingQuestion.cs
//
// OOP - INHERITANCE:
// A SortingQuestion IS-A Question. It reuses the prompt + options from the base class and
// adds the two things a "watch the bars" question needs: which sorting Strategy to SHOW,
// and which answer option is the correct one.
//
// This is the "show, don't tell" question: the player watches the algorithm run on the bars
// and then picks its name - no reading required.
public class SortingQuestion : Question
{
    // The algorithm to actually run on the bars (a Strategy object). The GameManager plays
    // this so the player can SEE the behaviour.
    public ISortStrategy Strategy { get; private set; }

    // ENCAPSULATION: the correct answer is hidden here.
    private readonly int correctIndex;

    public SortingQuestion(ISortStrategy strategy, string[] options, int correctIndex)
        : base("Watch the bars. Which sorting algorithm is running?", options)
    {
        Strategy = strategy;
        this.correctIndex = correctIndex;
    }

    // OOP - POLYMORPHISM: this subclass's rule for "correct".
    public override bool IsCorrect(int chosenIndex)
    {
        return chosenIndex == correctIndex;
    }
}
