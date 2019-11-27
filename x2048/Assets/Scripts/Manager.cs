using Assets.Scripts;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Manager : MonoBehaviour
{
    public static readonly float[] gridY = new float[4] { 0.95f, 2.65f, 4.35f, 6.05f };

    public Tile[,] grid = new Tile[4, 4];

    public GameObject scoreText;
    public GameObject gameEndText;
    public GameObject tileFab;

    public bool isMoveFinished;

    private readonly float MOVEMENT_DELAY = 0.3f;
    private readonly Vector3 INITIAL_CAMERA_POSITION = new Vector3(-4f, 5f, 0f);
    private readonly Vector3 GAME_END_CAMERA_POSITION = new Vector3(4.8f, 7.6f, 0f);

    private bool spawnWaiting;
    private int score;
    private bool go = true;
    private bool isGameWon;
    private bool isGameOver;

    // clear our grid
    private void Start()
    {
        Cursor.visible = false;

        Array.Clear(grid, 0, grid.Length);
        isMoveFinished = false;

        scoreText.transform.position = Camera.main.WorldToViewportPoint(INITIAL_CAMERA_POSITION);
        scoreText.GetComponent<GUIText>().text = "Score : " + score;

        Spawn();
        Spawn();
    }

    IEnumerator DelayGameMovement()
    {
        go = false;
        yield return new WaitForSeconds(MOVEMENT_DELAY);
        go = true;
    }

    private void Update()
    {
        if (isMoveFinished && spawnWaiting) Spawn();

        if (isMoveFinished & !isGameOver && go)
        {
            if (Input.GetKey(KeyCode.UpArrow)) Move(KeyCode.UpArrow);
            if (Input.GetKey(KeyCode.DownArrow)) Move(KeyCode.DownArrow);
            if (Input.GetKey(KeyCode.LeftArrow)) Move(KeyCode.LeftArrow);
            if (Input.GetKey(KeyCode.RightArrow)) Move(KeyCode.RightArrow);
        }

        if (isGameWon) ShowGameEndMessage("You win!");

        if (isGameOver) ShowGameEndMessage("Game over :(");
    }

    private void ShowGameEndMessage(string message)
    {
        gameEndText.transform.position = Camera.main.WorldToViewportPoint(GAME_END_CAMERA_POSITION);
        gameEndText.GetComponent<GUIText>().text = message;
    }

    public void Move(KeyCode keyCode)
    {
        if (isGameWon || isGameOver || !isMoveFinished) return;

        StartCoroutine(DelayGameMovement());

        isMoveFinished = false;
        Vector2 vector = keyCode.GetDirection();
        bool canCellsMove = false;

        int[] xCoords = { 0, 1, 2, 3 };
        int[] yCoords = { 0, 1, 2, 3 };
        
        if (vector == Vector2.up) Array.Reverse(yCoords);
        if (vector == Vector2.right) Array.Reverse(xCoords);

        foreach (int x in xCoords)
        {
            foreach (int y in yCoords)
            {
                if (grid[x, y] != null)
                {
                    grid[x, y].combined = false;
                    Vector2 cell;
                    Vector2 nextCellPosition = new Vector2(x, y);

                    do
                    {
                        cell = nextCellPosition;
                        nextCellPosition = new Vector2(cell.x + vector.x, cell.y + vector.y);
                    } while (IsInArea(nextCellPosition) && grid[Mathf.RoundToInt(nextCellPosition.x), Mathf.RoundToInt(nextCellPosition.y)] == null);

                    int nextX = Mathf.RoundToInt(nextCellPosition.x);
                    int nextY = Mathf.RoundToInt(nextCellPosition.y);

                    // if cell is empty, move it
                    if (IsInArea(nextCellPosition) && !grid[nextX, nextY].combined && grid[nextX, nextY].tileValue == grid[x, y].tileValue)
                    {
                        score += grid[x, y].tileValue * 2;
                        scoreText.GetComponent<GUIText>().text = "Score : " + score;
                        grid[x, y].Move(nextX, nextY);
                        canCellsMove = true;

                        if ((grid[nextX, nextY].tileValue * 2) == 2048) isGameWon = true;
                    }
                    else
                    {
                        if (grid[x, y].Move(Mathf.RoundToInt(cell.x), Mathf.RoundToInt(cell.y))) canCellsMove = true;
                    }
                }
            }
        }

        if (canCellsMove)
        {
            spawnWaiting = true;
        }
        else
        {
            // Check if we can move cells in any direction
            for (int x = 0; x <= 3; x++)
            {
                for (int y = 0; y <= 3; y++)
                {
                    if (grid[x, y] == null)
                    {
                        canCellsMove = true;
                    }
                }
            }

            if (!canCellsMove) isGameOver = true;
        }
        isMoveFinished = true;
    }

    private bool IsInArea(Vector2 vec)
    {
        return 0 <= vec.x && vec.x <= 3 && 0 <= vec.y && vec.y <= 3;
    }

    private void Spawn()
    {
        spawnWaiting = false;

        int x;
        int y;

        while (true)
        {
            x = Random.Range(0, 4);
            y = Random.Range(0, 4);

            if (grid[x, y] == null) break;
        }

        int startValue = (Random.value < 0.2f) ? 4 : 2;

        GameObject tempTile = Instantiate(tileFab, CoordinatesUtil.FromGridToCoordinates(x, y), Quaternion.Euler(0, 0, 0));
        grid[x, y] = tempTile.GetComponent<Tile>();
        grid[x, y].tileValue = startValue;
        grid[x, y].manager = this;
    }
}