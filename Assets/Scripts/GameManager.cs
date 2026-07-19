// Singleton owning the game state; observes UIManager's events instead of polling buttons.

using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Wiring (auto-found if left empty)")]
    public UIManager ui;
    public SortVisualizer viz;

    [Header("Rules")]
    public int pointsPerCorrect = 100;
    public int questionsPerLevel = 3;
    public int levelsToWin = 3;

    // Private state; other scripts only get the read-only properties.
    private int score;
    private int level;
    private int correctThisLevel;
    private int questionNumber; // counting the question q1,q2
    public int Score { get { return score; } }
    public int Level { get { return level; } }

    private readonly string[] sortOptions = { "Bubble Sort", "Selection Sort", "Insertion Sort", "Merge Sort" };
    private readonly string[] searchOptions = { "Linear Search", "Binary Search", "Jump Search", "Interpolation Search" };

    // The algorithms we can actually show.
    private ISortStrategy[] sortStrategies;
    private ISearchStrategy[] searchStrategies;

    // Base type, so we do not care which subclass it is.
    private Question currentQuestion;

    private void Awake()
    {
        // Keep the first instance, destroy any duplicate.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Fall back if the scene builder did not assign them.
        if (ui == null) ui = FindFirstObjectByType<UIManager>();
        if (viz == null) viz = FindFirstObjectByType<SortVisualizer>();

        sortStrategies = new ISortStrategy[] { new BubbleSortStrategy(), new SelectionSortStrategy() };
        searchStrategies = new ISearchStrategy[] { new LinearSearchStrategy(), new BinarySearchStrategy() };
    }

    private void Start()
    {
        // Subscribe to the UI's events.
        ui.OnNewGame += StartGame;
        ui.OnContinue += StartGame;      // restart for now; no save system yet
        ui.OnPlayAgain += StartGame;
        ui.OnBackToMenu += ui.ShowStart;
        ui.OnOptionClicked += HandleAnswer;

        if (viz != null) viz.autoDemo = false; // we drive the bars now, not the demo loop
        ui.ShowStart();
    }

    private void OnDestroy()
    {
        // Stop listening when this object goes away.
        if (ui == null) return;
        ui.OnNewGame -= StartGame;
        ui.OnContinue -= StartGame;
        ui.OnPlayAgain -= StartGame;
        ui.OnBackToMenu -= ui.ShowStart;
        ui.OnOptionClicked -= HandleAnswer;
    }

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

        // Number the prompt so it is obvious the question changed.
        ui.ShowQuestion("Q" + questionNumber + "   -   " + currentQuestion.Prompt,
            currentQuestion.Options);

        // Testing helper - remove before submitting.
        Debug.Log("[Q" + questionNumber + "] Correct answer: " + currentQuestion.CorrectAnswerName);

        // The question knows how to show itself.
        currentQuestion.Present(viz);
    }

    // Coin-flip between a sorting and a searching question, so runs mix both.
    private Question CreateRandomQuestion()
    {
        bool doSorting = Random.value < 0.5f;

        if (doSorting)
        {
            // The strategy's name is the correct answer.
            ISortStrategy s = sortStrategies[Random.Range(0, sortStrategies.Length)];
            int correct = System.Array.IndexOf(sortOptions, s.Name);
            return new SortingQuestion(s, sortOptions, correct);
        }
        else
        {
            ISearchStrategy s = searchStrategies[Random.Range(0, searchStrategies.Length)];
            int target = Random.Range(1, viz.barCount + 1); // a height that exists in the bars
            int correct = System.Array.IndexOf(searchOptions, s.Name);
            return new SearchingQuestion(s, target, searchOptions, correct);
        }
    }

    // Runs whenever the player clicks an answer button.
    private void HandleAnswer(int chosenIndex)
    {
        if (currentQuestion == null) return; // ignore clicks after the game ended

        if (currentQuestion.IsCorrect(chosenIndex))
        {
            score += pointsPerCorrect;
            correctThisLevel++;
            ui.SetScore(score);

            if (correctThisLevel >= questionsPerLevel)
            {
                level++;
                correctThisLevel = 0;

                // Cleared every level - win.
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
            // One wrong answer ends the run.
            currentQuestion = null;
            ui.ShowEnd(false, score);
        }
    }
}
