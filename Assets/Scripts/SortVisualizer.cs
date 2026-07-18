// SortVisualizer.cs
//
// Draws a row of vertical bars (an array shown as heights) and gives the sorting strategies
// simple operations to work with: read a value, swap two bars, and highlight the two bars
// being compared. The strategies (BubbleSortStrategy / SelectionSortStrategy) animate by
// calling these while pausing between steps, so the player watches the algorithm run.
//
// It also owns the Fisher-Yates shuffle (ALGORITHM 2) used to scramble the bars each round.
//
// TEMPORARY: Start() runs an auto-demo that shuffles and sorts on a loop so you can see it
// working. That demo is removed once real questions drive the visualizer.

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SortVisualizer : MonoBehaviour
{
    [Header("Setup")]
    public int barCount = 8;         // how many bars to sort
    public float stepDelay = 0.15f;  // pause between algorithm steps (bigger = slower/clearer)

    // Where to draw the bars. If the Scene Builder assigns a slot inside the game panel,
    // the bars appear there. If left empty, the visualizer makes its own canvas (handy for
    // testing in an empty scene).
    public RectTransform barsParent;

    // TEMPORARY: when true, Start() runs the auto-demo loop so you can watch it. The real
    // question flow (added next) sets this false and drives the sort itself.
    public bool autoDemo = true;

    private int[] values;            // the array being sorted (heights of the bars)
    private Image[] bars;            // one Image per value
    private RectTransform container; // holds the bars

    private readonly Color normalColor = new Color(0.35f, 0.60f, 0.90f);
    private readonly Color highlightColor = new Color(0.95f, 0.80f, 0.20f);

    // layout numbers (worked out once in BuildBars)
    private float slot;   // horizontal space per bar
    private float barWidth;
    private float heightUnit; // pixels of height per +1 of value

    private void Start()
    {
        BuildBars();

        // ---- TEMPORARY demo so we can SEE it before questions exist ----
        if (autoDemo) StartCoroutine(DemoLoop());
    }

    // ------------------------------------------------------------------
    // Operations the sorting strategies use.
    // ------------------------------------------------------------------

    public int Count { get { return values.Length; } }

    public int GetValue(int index)
    {
        return values[index];
    }

    // Swap two bars: swap their values, then resize both to match.
    public void Swap(int a, int b)
    {
        int temp = values[a];
        values[a] = values[b];
        values[b] = temp;

        ResizeBar(a);
        ResizeBar(b);
    }

    // Colour the two bars currently being compared; everything else back to normal.
    public void Highlight(int a, int b)
    {
        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].color = (i == a || i == b) ? highlightColor : normalColor;
        }
    }

    public void ClearHighlight()
    {
        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].color = normalColor;
        }
    }

    private Coroutine sortRoutine;

    // Runs a chosen sorting strategy on the bars. Stops any sort already in progress first,
    // so starting a new question does not leave an old animation running.
    public void PlaySort(ISortStrategy strategy)
    {
        if (sortRoutine != null) StopCoroutine(sortRoutine);
        sortRoutine = StartCoroutine(strategy.Sort(this, stepDelay));
    }

    // ------------------------------------------------------------------
    // ALGORITHM 2 - Fisher-Yates shuffle (O(n)): scramble the bars fairly.
    // Walk from the last index down; swap each item with a random earlier-or-equal one.
    // ------------------------------------------------------------------
    public void Shuffle()
    {
        for (int i = values.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1); // random index from 0..i
            int temp = values[i];
            values[i] = values[j];
            values[j] = temp;
        }

        for (int i = 0; i < bars.Length; i++)
        {
            ResizeBar(i);
        }
        ClearHighlight();
    }

    // ------------------------------------------------------------------
    // Building the bars (plumbing).
    // ------------------------------------------------------------------

    private void BuildBars()
    {
        if (barsParent != null)
        {
            // Draw into the slot the Scene Builder gave us (inside the game panel).
            container = barsParent;
        }
        else
        {
            // No slot assigned: make our own canvas + centred container (empty-scene test).
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                GameObject canvasGO = new GameObject("Canvas");
                canvas = canvasGO.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                canvasGO.AddComponent<GraphicRaycaster>();
            }

            GameObject containerGO = new GameObject("SortArea");
            containerGO.transform.SetParent(canvas.transform, false);
            container = containerGO.AddComponent<RectTransform>();
            container.anchorMin = new Vector2(0.5f, 0.5f);
            container.anchorMax = new Vector2(0.5f, 0.5f);
            container.pivot = new Vector2(0.5f, 0.5f);
            container.anchoredPosition = new Vector2(0, 40);
            container.sizeDelta = new Vector2(1200, 520);
        }

        // Work out sizes from the container width/height and bar count.
        float areaWidth = container.rect.width;
        float areaHeight = container.rect.height;
        slot = areaWidth / barCount;
        barWidth = slot * 0.7f;
        heightUnit = (areaHeight - 40f) / barCount;

        // Start with values 1..barCount (they get shuffled by the demo / question flow).
        values = new int[barCount];
        for (int i = 0; i < barCount; i++)
        {
            values[i] = i + 1;
        }

        // Create one bar Image per value.
        bars = new Image[barCount];
        for (int i = 0; i < barCount; i++)
        {
            GameObject barGO = new GameObject("Bar" + i);
            barGO.transform.SetParent(container, false);
            RectTransform rt = barGO.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0, 0); // bottom-left of the container
            rt.anchorMax = new Vector2(0, 0);
            rt.pivot = new Vector2(0.5f, 0); // grow upward from the bottom
            rt.anchoredPosition = new Vector2(slot * (i + 0.5f), 0);

            Image img = barGO.AddComponent<Image>();
            img.color = normalColor;
            bars[i] = img;

            ResizeBar(i);
        }
    }

    // Set bar i's size to match its current value.
    private void ResizeBar(int i)
    {
        bars[i].rectTransform.sizeDelta = new Vector2(barWidth, values[i] * heightUnit);
    }

    // ------------------------------------------------------------------
    // TEMPORARY demo loop - alternates the two algorithms so you can compare them.
    // ------------------------------------------------------------------
    private IEnumerator DemoLoop()
    {
        ISortStrategy[] strategies = { new BubbleSortStrategy(), new SelectionSortStrategy() };
        int which = 0;

        while (true)
        {
            Shuffle();
            yield return new WaitForSeconds(0.6f);

            ISortStrategy strategy = strategies[which % strategies.Length];
            Debug.Log("Sorting with: " + strategy.Name);
            yield return StartCoroutine(strategy.Sort(this, stepDelay));

            yield return new WaitForSeconds(1.2f);
            which++;
        }
    }
}
