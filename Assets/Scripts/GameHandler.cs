using UnityEngine;

public class GameHandler : MonoBehaviour
{
    private LevelGrid levelgrid;
    private Snake snake;
    private int score;
    private GUIStyle gameOverLabelStyle;

    public bool IsGameOver { get; private set; }

    private void Start()
    {
        Debug.Log("GameHandler.Start");
        snake = FindAnyObjectByType<Snake>();
        levelgrid = new LevelGrid(20, 20);
    }

    private void Update()
    {
        if (IsGameOver && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            RestartGame();
        }
    }

    public bool SnakeMoved(Vector2Int snakeGridPosition)
    {
        bool ateFood = levelgrid.SnakeMoved(snakeGridPosition);
        if (ateFood)
        {
            score++;
        }

        return ateFood;
    }

    public bool IsPositionBlocked(Vector2Int fromPosition, Vector2Int toPosition)
    {
        Vector2 start = new(fromPosition.x, fromPosition.y);
        Vector2 end = new(toPosition.x, toPosition.y);
        Vector2 direction = end - start;
        Vector2 boxSize = Vector2.one * 0.9f;

        return Physics2D.OverlapBox(end, boxSize, 0f) != null
            || Physics2D.BoxCast(start, boxSize, 0f, direction.normalized, direction.magnitude);
    }

    public void EndGame()
    {
        IsGameOver = true;
    }

    private void RestartGame()
    {
        IsGameOver = false;
        score = 0;
        levelgrid.Reset();
        snake.ResetSnake();
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 30), $"Pontos: {score}");

        if (!IsGameOver)
        {
            return;
        }

        if (gameOverLabelStyle == null)
        {
            gameOverLabelStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 22,
                wordWrap = true
            };
        }

        Rect panel = new(Screen.width / 2f - 190f, Screen.height / 2f - 75f, 380f, 150f);
        GUI.Box(panel, GUIContent.none);
        GUI.Label(panel, $"Fim de jogo\nPontuação: {score}\nPressione Enter para começar novamente", gameOverLabelStyle);
    }
}
