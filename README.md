# Algorithm Arena

A small Unity quiz game for the **OOP & Design Patterns** assignment. An algorithm runs on
screen (bars sorting, or a search hunting for a target) and you identify it.

## How to Run
1. Open the project folder in Unity Hub.
2. Open `Assets/Scenes/Main.unity`.
3. Press **Play**.

## How to Play
- Click **NEW GAME**.
- Watch the bars. The prompt tells you if it is a **sorting** or a **searching** round.
- Click the button with the correct algorithm's name.
- 3 correct answers = level up. Clear 3 levels = **SUCCESS**. A wrong answer = **GAME OVER**.

## Graded concepts
**OOP principles**
- Abstraction: `Question.cs` (abstract base), `ISortStrategy.cs`, `ISearchStrategy.cs`
- Inheritance: `SortingQuestion.cs`, `SearchingQuestion.cs` (both `: Question`)
- Polymorphism: `Question.Present()` and the strategy `Sort()` / `Search()` methods
- Encapsulation: private state in `GameManager.cs`, `Question.cs`

**Design patterns**
- Singleton: `GameManager.cs` (`GameManager.Instance`)
- Observer: `UIManager.cs` raises events, `GameManager.cs` subscribes
- Strategy: `ISortStrategy` / `ISearchStrategy` + their implementations

**Algorithms**
- Sorting (O(n^2)): `BubbleSortStrategy.cs`, `SelectionSortStrategy.cs`
- Shuffle (O(n)): `SortVisualizer.cs` (`Shuffle()`, Fisher-Yates)
- Searching: `LinearSearchStrategy.cs` (O(n)), `BinarySearchStrategy.cs` (O(log n))
