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

    // The four labels shown on the answer buttons (last two are decoys we don't implement).
    private readonly string[] optionNames = { "Bubble Sort", "Selection Sort", "Insertion Sort", "Merge Sort" };

    // The sorting algorithms we can actually SHOW (Strategy objects).
    private ISortStrategy[] strategies;

    private SortingQuestion currentQuestion;

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

        strategies = new ISortStrategy[] { new BubbleSortStrategy(), new SelectionSortStrategy() };
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
        currentQuestion = CreateRandomSortingQuestion();

        // Show the question number in the prompt so it is obvious the question changed.
        ui.ShowQuestion("Q" + questionNumber + "   -   " + currentQuestion.Prompt,
            currentQuestion.Options);

        // SPEED-RUN HELPER: print the correct answer to the Console so you can test fast.
        // (Remove this line before the final submission if you like.)
        Debug.Log("[Q" + questionNumber + "] Correct answer: " + currentQuestion.Strategy.Name);

        // ...then SHOW the algorithm: scramble the bars, then run the chosen Strategy.
        viz.Shuffle();
        viz.PlaySort(currentQuestion.Strategy);
    }

    // Builds a question whose correct answer is whichever algorithm we actually run.
    private SortingQuestion CreateRandomSortingQuestion()
    {
        // Pick one of the algorithms we can visualise (Bubble or Selection).
        int pick = Random.Range(0, strategies.Length);
        ISortStrategy strategy = strategies[pick];

        // The correct option is the one whose label matches the running algorithm's name.
        int correctIndex = System.Array.IndexOf(optionNames, strategy.Name);

        return new SortingQuestion(strategy, optionNames, correctIndex);
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
