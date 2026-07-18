// Question.cs
//
// OOP - ABSTRACTION:
// "Question" is an ABSTRACT base class. It describes WHAT every question has (a prompt, a
// list of options, and which option is correct) and WHAT it must do (show itself on screen),
// without deciding HOW. You can never create a plain "Question" - only a concrete subclass
// such as SortingQuestion or SearchingQuestion.
public abstract class Question
{
    // OOP - ENCAPSULATION: data is exposed read-only; the correct index is hidden (protected).
    public string Prompt { get; private set; }
    public string[] Options { get; private set; }
    protected int CorrectIndex { get; private set; }

    protected Question(string prompt, string[] options, int correctIndex)
    {
        Prompt = prompt;
        Options = options;
        CorrectIndex = correctIndex;
    }

    // Shared for every question: is the clicked option the correct one?
    public bool IsCorrect(int chosenIndex)
    {
        return chosenIndex == CorrectIndex;
    }

    // Handy for logging / debugging: the text of the correct answer.
    public string CorrectAnswerName
    {
        get { return Options[CorrectIndex]; }
    }

    // OOP - POLYMORPHISM:
    // Each question type shows ITSELF differently on the bars - a sorting question animates a
    // sort, a searching question animates a search. GameManager just calls Present() and does
    // not need to know which concrete type it is holding.
    public abstract void Present(SortVisualizer viz);
}
