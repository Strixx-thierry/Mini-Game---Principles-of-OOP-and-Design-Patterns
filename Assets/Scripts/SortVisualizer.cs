// Draws the array as bars and gives the strategies their operations: read, swap, highlight.

using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SortVisualizer : MonoBehaviour
{
    [Header("Setup")]
    public int barCount = 8;
    public float stepDelay = 0.15f;  // pause between steps; bigger = slower and clearer

    // Where to draw. Left empty, the visualizer builds its own canvas (empty-scene testing).
    public RectTransform barsParent;

    // Standalone demo loop. GameManager turns this off and drives the bars itself.
    public bool autoDemo = true;

    private int[] values;            // the array being sorted (bar heights)
    private Image[] bars;            // one Image per value
    private RectTransform container;

    private readonly Color normalColor = new Color(0.35f, 0.60f, 0.90f);
    private readonly Color highlightColor = new Color(0.95f, 0.80f, 0.20f);

    // Searching-round colours.
    private readonly Color examineColor = new Color(0.95f, 0.80f, 0.20f); // being checked
    private readonly Color targetColor = new Color(0.85f, 0.35f, 0.85f);  // what we hunt for
    private readonly Color foundColor = new Color(0.30f, 0.85f, 0.40f);
    private int targetIndex = -1;

    // Layout, worked out once in BuildBars.
    private float slot;       // horizontal space per bar
    private float barWidth;
    private float heightUnit; // pixels of height per +1 of value

    private void Start()
    {
        BuildBars();

        if (autoDemo) StartCoroutine(DemoLoop());
    }

    public int Count { get { return values.Length; } }

    public int GetValue(int index)
    {
        return values[index];
    }

    // Swap the values, then resize both bars to match.
    public void Swap(int a, int b)
    {
        int temp = values[a];
        values[a] = values[b];
        values[b] = temp;

        ResizeBar(a);
        ResizeBar(b);
    }

    // Colour the two bars being compared; everything else back to normal.
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

    // Stops any animation still running, so a new question does not overlap the old one.
    public void PlaySort(ISortStrategy strategy)
    {
        if (sortRoutine != null) StopCoroutine(sortRoutine);
        sortRoutine = StartCoroutine(strategy.Sort(this, stepDelay));
    }
  
    // Fisher-Yates, O(n): swap each item with a random earlier-or-equal one.
    public void Shuffle()
    {
        for (int i = values.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            int temp = values[i];
            values[i] = values[j];
            values[j] = temp;
        }

        for (int i = 0; i < bars.Length; i++)
        {
            ResizeBar(i);
        }
        targetIndex = -1;
        ClearHighlight();
    }
 
    // ---- Searching support ----

    public void SortAscending()
    {
        for (int i = 0; i < values.Length; i++)
        {
            values[i] = i + 1;
        }
        for (int i = 0; i < bars.Length; i++)
        {
            ResizeBar(i);
        }
        targetIndex = -1;
        ClearHighlight();
    }

    // Colour the bar being hunted for.
    public void SetTarget(int targetValue)
    {
        targetIndex = System.Array.IndexOf(values, targetValue);
        for (int i = 0; i < bars.Length; i++)
        {
            bars[i].color = (i == targetIndex) ? targetColor : normalColor;
        }
    }

    // Highlight the bar being checked, keeping the target visible.
    public void Examine(int index)
    {
        for (int i = 0; i < bars.Length; i++)
        {
            Color c = normalColor;
            if (i == targetIndex) c = targetColor;
            if (i == index) c = examineColor;
            bars[i].color = c;
        }
    }

    public void MarkFound(int index)
    {
        bars[index].color = foundColor;
    }

    public void PlaySearch(ISearchStrategy strategy, int targetValue)
    {
        if (sortRoutine != null) StopCoroutine(sortRoutine);
        sortRoutine = StartCoroutine(strategy.Search(this, targetValue, stepDelay));
    }
 
    // ---- Building the bars ----

    private void BuildBars()
    {
        if (barsParent != null)
        {
            container = barsParent;
        }
        else
        {
            // No slot assigned, so build our own canvas and centred container.
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

        // Sizes come from the container and the bar count.
        float areaWidth = container.rect.width;
        float areaHeight = container.rect.height;
        slot = areaWidth / barCount;
        barWidth = slot * 0.7f;
        heightUnit = (areaHeight - 40f) / barCount;

        // Values 1..barCount, shuffled later by whatever drives the round.
        values = new int[barCount];
        for (int i = 0; i < barCount; i++)
        {
            values[i] = i + 1;
        }

        // One Image per value.
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

    private void ResizeBar(int i)
    {
        bars[i].rectTransform.sizeDelta = new Vector2(barWidth, values[i] * heightUnit);
    }
  
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
