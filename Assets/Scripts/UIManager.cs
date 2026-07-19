// One scene, three panels; buttons report clicks as events for GameManager to handle.

using System;            // for Action (event callbacks)
using UnityEngine;
using UnityEngine.UI;    // Text, Button, Image

public class UIManager : MonoBehaviour
{
    // ---- Panels (whole screens we turn on/off) ----

    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject endPanel;

    // ---- Start menu buttons ----
    public Button newGameButton;
    public Button continueButton;
    public Button exitButton;

    // ---- Game HUD ----
    public Text levelText;
    public Text scoreText;
    public Text questionText;
    public Image timerFill;
    public Button[] optionButtons;
    public Text[] optionLabels;

    // ---- End screen ----
    public Text endTitleText;
    public Text endScoreText;
    public Button playAgainButton;
    public Button menuButton;

    // ---- Events GameManager subscribes to ----
    public event Action OnNewGame;
    public event Action OnContinue;
    public event Action<int> OnOptionClicked; // which answer (0..3) was clicked
    public event Action OnPlayAgain;
    public event Action OnBackToMenu;

    private void Awake()
    {
        // --- wire the menu buttons to their events ---
        if (newGameButton != null) newGameButton.onClick.AddListener(() => Raise(OnNewGame));
        if (continueButton != null) continueButton.onClick.AddListener(() => Raise(OnContinue));
        if (exitButton != null) exitButton.onClick.AddListener(QuitGame);

        // --- wire the four answer buttons (each reports its own index) ---
        if (optionButtons != null)
        {
            for (int i = 0; i < optionButtons.Length; i++)
            {
                if (optionButtons[i] == null) continue;
                int index = i; // copy so each button remembers its own number
                optionButtons[i].onClick.AddListener(() =>
                {
                    if (OnOptionClicked != null) OnOptionClicked(index);
                });
            }
        }

        // --- wire the end-screen buttons ---
        if (playAgainButton != null) playAgainButton.onClick.AddListener(() => Raise(OnPlayAgain));
        if (menuButton != null) menuButton.onClick.AddListener(() => Raise(OnBackToMenu));
    }

    private void Start()
    {
        // Default to the menu; GameManager reacts to the events and drives the rest.
        ShowStart();
        SetTimer(1f);
    }

    // ---- Panel switching ----

    public void ShowStart()
    {
        SetPanels(true, false, false);
    }

    public void ShowGame()
    {
        SetPanels(false, true, false);
    }

    // success = true shows a win screen, false shows a lose screen.
    public void ShowEnd(bool success, int score)
    {
        SetPanels(false, false, true);

        if (endTitleText != null)
        {
            endTitleText.text = success ? "SUCCESS!" : "GAME OVER";
            endTitleText.color = success ? new Color(0.3f, 0.8f, 0.4f) : new Color(0.85f, 0.3f, 0.3f);
        }
        if (endScoreText != null) endScoreText.text = "Final Score: " + score;
    }

    private void SetPanels(bool start, bool game, bool end)
    {
        if (startPanel != null) startPanel.SetActive(start);
        if (gamePanel != null) gamePanel.SetActive(game);
        if (endPanel != null) endPanel.SetActive(end);
    }

    // ---- HUD updates during a round ----

    public void SetLevel(int level)
    {
        if (levelText != null) levelText.text = "Level " + level;
    }

    public void SetScore(int score)
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
    }

    // fraction goes from 1 (full time) down to 0 (time up).
    public void SetTimer(float fraction)
    {
        if (timerFill != null) timerFill.fillAmount = Mathf.Clamp01(fraction);
    }

    // Shows a prompt and up to 4 options; unused buttons hide.
    public void ShowQuestion(string prompt, string[] options)
    {
        if (questionText != null) questionText.text = prompt;

        for (int i = 0; i < optionButtons.Length; i++)
        {
            bool used = i < options.Length;
            optionButtons[i].gameObject.SetActive(used);
            if (used) optionLabels[i].text = options[i];
        }
    }

    private void QuitGame()
    {
        Debug.Log("EXIT");
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // stops Play mode in the Editor
#endif
    }

    // Null-safe raise for the parameterless events.
    private void Raise(Action e)
    {
        if (e != null) e();
    }
}
