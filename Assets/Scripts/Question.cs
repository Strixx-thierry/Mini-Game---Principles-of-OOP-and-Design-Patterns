// Abstract base: says what every question has and must do, not how.
public abstract class Question
{
    // Read-only to the outside; the answer index stays hidden.
    public string Prompt { get; private set; }
    public string[] Options { get; private set; }
    protected int CorrectIndex { get; private set; }

    protected Question(string prompt, string[] options, int correctIndex)
    {
        Prompt = prompt;
        Options = options;
        CorrectIndex = correctIndex;
    }

    public bool IsCorrect(int chosenIndex)
    {
        return chosenIndex == CorrectIndex;
    }

    // For logging.
    public string CorrectAnswerName
    {
        get { return Options[CorrectIndex]; }
    }

    // Polymorphism: each subclass shows itself differently. GameManager just calls Present().
    public abstract void Present(SortVisualizer viz);
}
