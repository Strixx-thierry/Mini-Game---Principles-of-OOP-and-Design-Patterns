// Question.cs
//
// OOP - ABSTRACTION:
// "Question" is an ABSTRACT base class. It describes WHAT every question has (a prompt and a
// list of answer options) and WHAT it must do (say whether a chosen option is correct),
// without deciding HOW that check works. You can never create a plain "Question" - only a
// concrete subclass such as SortingQuestion.
public abstract class Question
{
    // OOP - ENCAPSULATION:
    // The data is exposed through read-only properties (private set), so other scripts can
    // read a question but cannot change its prompt or options after it is created.
    public string Prompt { get; private set; }
    public string[] Options { get; private set; }

    protected Question(string prompt, string[] options)
    {
        Prompt = prompt;
        Options = options;
    }

    // OOP - POLYMORPHISM:
    // Every subclass provides its own version of IsCorrect. The game holds a "Question"
    // reference and calls IsCorrect(i) without caring which concrete subclass it really is.
    public abstract bool IsCorrect(int chosenIndex);
}
