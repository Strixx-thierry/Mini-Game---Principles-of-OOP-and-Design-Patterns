// GameManager.cs
//
// PATTERN - SINGLETON (creational):
// There is exactly ONE GameManager, reachable from anywhere as GameManager.Instance. It owns
// the game's global state (score, level, current question) so that state lives in one place
// instead of being scattered around.
//
// PATTERN - OBSERVER (behavioral):
// GameManager does not poll the buttons. Instead it SUBSCRIBES to the events that UIManager
// raises (OnNewGame, OnOptionClicked, ...). When the player clicks, UIManager broadcasts and
// GameManager reacts. UI and logic stay decoupled.
//
// It also drives the "show, don't tell" loop: pick a sorting Strategy, run it on the bars,
// and mark the matching option as the correct answer.

using UnityEngine;

public class GameManager : MonoBehaviour
{
    // --- Singleton access point ---
    public static GameManager Instance { get; private set; }

    [Header("Wiring (auto-found if left empty)")]
    public UIManager ui;
    public SortVisualizer viz;

    [Header("Rules")]
    public int pointsPerCorrect = 100;
    public int questionsPerLevel = 3;
    public int levelsToWin = 3;

    // OOP - ENCAPSULATION: state is private; other scripts read it through the properties.
    private int score;
    private int level;
    private int correctThisLevel;
    private int questionNumber; // counts questions across the whole run (Q1, Q2, ...)
    public int Score { get { return score; } }
    public int Level { get { return level; } }

    // Answer labels for each category (the last two of each are decoys we don't implement).
    private readonly string[] sortOptions = { "Bubble Sort", "Selection Sort", "Insertion Sort", "Merge Sort" };
    private readonly string[] searchOptions = { "Linear Search", "Binary Search", "Jump Search", "Interpolation Search" };

    // The algorithms we can actually SHOW (Strategy objects).
    private ISortStrategy[] sortStrategies;
    private ISearchStrategy[] searchStrategies;

    // Held as the BASE type Question - GameManager does not care which subclass it is.
    private Question currentQuestion;

    private void Awake()
    {
        // SINGLETON: keep the first instance, destroy any duplicate.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Find the helpers if the scene builder did not assign them.
        if (ui == null) ui = FindFirstObjectByType<UIManager>();
        if (viz == null) viz = FindFirstObjectByType<SortVisualizer>();

        sortStrategies = new ISortStrategy[] { new BubbleSortStrategy(), new SelectionSortStrategy() };
        searchStrategies = new ISearchStrategy[] { new LinearSearchStrategy(), new BinarySearchStrategy() };
    }

    private void Start()
    {
        // OBSERVER: subscribe to the UI's events.
        ui.OnNewGame += StartGame;
        ui.OnContinue += StartGame;      // CONTINUE = restart for now (save system comes later)
        ui.OnPlayAgain += StartGame;
        ui.OnBackToMenu += ui.ShowStart;
        ui.OnOptionClicked += HandleAnswer;

        if (viz != null) viz.autoDemo = false; // GameManager drives the bars now, not the demo
        ui.ShowStart();
    }

    private void OnDestroy()
    {
        // Good practice: stop listening when this object goes away.
        if (ui == null) return;
        ui.OnNewGame -= StartGame;
        ui.OnContinue -= StartGame;
        ui.OnPlayAgain -= StartGame;
        ui.OnBackToMenu -= ui.ShowStart;
        ui.OnOptionClicked -= HandleAnswer;
    }

    // ------------------------------------------------------------------

    private void StartGame()
    {
        score = 0;
        level = 1;
        correctThisLevel = 0;
        questionNumber = 0;
        ui.SetScore(score);
        ui.SetLevel(level);
        ui.ShowGame();
        NextQuestion();
    }

    private void NextQuestion()
    {
        questionNumber++;
        currentQuestion = CreateRandomQuestion();

        // Show the question number in the prompt so it is obvious the question changed.
        ui.ShowQuestion("Q" + questionNumber + "   -   " + currentQuestion.Prompt,
            currentQuestion.Options);

        // SPEED-RUN HELPER: print the correct answer to the Console so you can test fast.
        // (Remove this line before the final submission if you like.)
        Debug.Log("[Q" + questionNumber + "] Correct answer: " + currentQuestion.CorrectAnswerName);

        // POLYMORPHISM: a sorting question animates a sort, a searching question animates a
        // search. We just call Present() - the question knows how to show itself.
        currentQuestion.Present(viz);
    }

    // Randomly build either a sorting question or a searching question, so runs mix both.
    private Question CreateRandomQuestion()
    {
        bool doSorting = Random.value < 0.5f;

        if (doSorting)
        {
            // Pick a sort we can visualise (Bubble or Selection); its name is the answer.
            ISortStrategy s = sortStrategies[Random.Range(0, sortStrategies.Length)];
            int correct = System.Array.IndexOf(sortOptions, s.Name);
            return new SortingQuestion(s, sortOptions, correct);
        }
        else
        {
            // Pick a search (Linear or Binary) and a random target height to hunt for.
            ISearchStrategy s = searchStrategies[Random.Range(0, searchStrategies.Length)];
            int target = Random.Range(1, viz.barCount + 1); // a height that exists in the bars
            int correct = System.Array.IndexOf(searchOptions, s.Name);
            return new SearchingQuestion(s, target, searchOptions, correct);
        }
    }

    // OBSERVER callback: runs whenever the player clicks an answer button.
    private void HandleAnswer(int chosenIndex)
    {
        if (currentQuestion == null) return; // ignore clicks after the game ended

        if (currentQuestion.IsCorrect(chosenIndex))
        {
            score += pointsPerCorrect;
            correctThisLevel++;
            ui.SetScore(score);

            // Every few correct answers, level up.
            if (correctThisLevel >= questionsPerLevel)
            {
                level++;
                correctThisLevel = 0;

                // Cleared all levels? That is a WIN.
                if (level > levelsToWin)
                {
                    currentQuestion = null;
                    ui.ShowEnd(true, score);
                    return;
                }
                ui.SetLevel(level);
            }

            NextQuestion();
        }
        else
        {
            // One wrong answer ends the run (a FAIL).
            currentQuestion = null;
            ui.ShowEnd(false, score);
        }
    }
}
