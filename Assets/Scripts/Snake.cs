using UnityEngine;
using System.Collections.Generic;

public class Snake : MonoBehaviour
{
    private readonly List<Vector2Int> snakeMovePositionList = new();
    private readonly List<Transform> snakeBodyTransformList = new();
    private GameHandler gameHandler;
    private Vector2Int gridMoveDirection;
    private Vector2Int gridPosition;
    private float gridMoveTimer;
    private float gridMoveTimerMax;
    
    private void Awake()
    {
        gameHandler = FindAnyObjectByType<GameHandler>();
        gridPosition = new Vector2Int(10, 10);
        gridMoveTimerMax= .5f;
        gridMoveTimer = gridMoveTimerMax;
        gridMoveDirection = new Vector2Int(1, 0);
        snakeMovePositionList.Add(gridPosition);
    }

    private void Update()
    {
        if (gameHandler.IsGameOver)
        {
            return;
        }

        HandleInput();
        
        HandleGridMovement();
    }

    private void HandleInput()
    {
         if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (gridMoveDirection.y != -1)
            {
            gridMoveDirection.x = 0;
            gridMoveDirection.y = +1;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (gridMoveDirection.y != +1)
            {
            gridMoveDirection.x = 0;
            gridMoveDirection.y = -1;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (gridMoveDirection.x != +1)
            {
            gridMoveDirection.x = -1;
            gridMoveDirection.y = 0;
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (gridMoveDirection.x != -1)
            {
            gridMoveDirection.x = +1;
            gridMoveDirection.y = 0; 
            }
        }

    }

    private void HandleGridMovement()
    {
         gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;
             Vector2Int nextGridPosition = gridPosition + gridMoveDirection;

             if (gameHandler.IsPositionBlocked(gridPosition, nextGridPosition))
             {
                 gameHandler.EndGame();
                 return;
             }

             gridPosition = nextGridPosition;

            bool ateFood = gameHandler.SnakeMoved(gridPosition);

            snakeMovePositionList.Insert(0, gridPosition);

            if (ateFood)
            {
                CreateSnakeBody();
            }
            else
            {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }

            UpdateSnakeBodyPositions();
        }
        transform.position = new Vector3(gridPosition.x, gridPosition.y, 0);
        transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirection) - 90);
    }

    private void CreateSnakeBody()
    {
        GameObject snakeBody = new("SnakeBody", typeof(SpriteRenderer));
        snakeBody.GetComponent<SpriteRenderer>().sprite = GameAssets.i.snakeBodySprite;
        snakeBody.GetComponent<SpriteRenderer>().sortingOrder = 0;
        snakeBodyTransformList.Add(snakeBody.transform);
    }

    private void UpdateSnakeBodyPositions()
    {
        for (int i = 0; i < snakeBodyTransformList.Count; i++)
        {
            Vector2Int bodyPosition = snakeMovePositionList[i + 1];
            snakeBodyTransformList[i].position = new Vector3(bodyPosition.x, bodyPosition.y, 0);
        }
    }

    public void ResetSnake()
    {
        foreach (Transform bodyTransform in snakeBodyTransformList)
        {
            Destroy(bodyTransform.gameObject);
        }

        snakeBodyTransformList.Clear();
        snakeMovePositionList.Clear();
        gridPosition = new Vector2Int(10, 10);
        gridMoveDirection = new Vector2Int(1, 0);
        gridMoveTimer = gridMoveTimerMax;
        snakeMovePositionList.Add(gridPosition);
        transform.position = new Vector3(gridPosition.x, gridPosition.y, 0);
        transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirection) - 90);
    }

    private float GetAngleFromVector(Vector2Int dir)
    {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }
}
