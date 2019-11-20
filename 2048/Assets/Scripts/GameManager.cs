using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    Playing,
    Won
}

public class GameManager : MonoBehaviour
{
    private GameState gameState = GameState.Playing;
    ItemArray matrix;
    public GameObject GO2, GO4, GO8, GO16, GO32, GO64, GO128, GO256, GO512, GO1024, GO2048, blankGO;
    public Text ScoreText, DebugText;
    private float distance = 0.109f;

    public IInputDetector inputDetector;

    private int ZIndex = 0, score = 0;


    // Use this for initialization
    void Start()
    {
        InitialPositionBackgroundSprites();

        Initialize();

        inputDetector = GetComponent<IInputDetector>();
    }

    public void Initialize()
    {
        if (matrix != null)
            for (int row = 0; row < Consts.Rows; row++)
                for (int column = 0; column < Consts.Columns; column++)
                {
                    if (matrix[row, column] != null && matrix[row, column].gameObject != null)
                        Destroy(matrix[row, column].gameObject);
                }

        matrix = new ItemArray();

        CreateNewItem();
        CreateNewItem();

        score = 0;
        UpdateScore(0);

        gameState = GameState.Playing;
    }

    private void CreateNewItem(int value = 2, int? row = null, int? column = null)
    {
        int randomRow, randomColumn;

        if (row == null && column == null)
        {
            matrix.GetRandomRowColumn(out randomRow, out randomColumn);
        }
        else
        {
            randomRow = row.Value;
            randomColumn = column.Value;
        }

        var newItem = new Item();
        newItem.Row = randomRow;
        newItem.Column = randomColumn;
        newItem.Value = value;

        GameObject newGo = GetGOBasedOnValue(value);
        newGo.transform.localScale = new Vector3(1f, 1f, 1f);

        newItem.gameObject = Instantiate(newGo, this.transform.position +
        new Vector3(randomColumn + randomColumn * distance, randomRow + randomRow * distance, ZIndex),
        Quaternion.identity) as GameObject;

        matrix[randomRow, randomColumn] = newItem;
    }



    private void InitialPositionBackgroundSprites()
    {
        for (int row = 0; row < Consts.Rows; row++)
        {
            for (int column = 0; column < Consts.Columns; column++)
            {
                Instantiate(blankGO, this.transform.position +
                new Vector3(column + column * distance, row + row * distance, ZIndex), Quaternion.identity);
            }
        }
    }

    void Update()
    {
        if (gameState != GameState.Playing) return;

        InputDirection? value = inputDetector.DetectInputDirection();

        if (!value.HasValue) return;

        List<ItemMovementDetails> movementDetails = new List<ItemMovementDetails>();

        if (value == InputDirection.Left)
            movementDetails = matrix.MoveHorizontal(HorizontalMovement.Left);

        else if (value == InputDirection.Right)
            movementDetails = matrix.MoveHorizontal(HorizontalMovement.Right);

        else if (value == InputDirection.Top)
            movementDetails = matrix.MoveVertical(VerticalMovement.Top);

        else if (value == InputDirection.Bottom)
            movementDetails = matrix.MoveVertical(VerticalMovement.Bottom);

        if (movementDetails.Count > 0)
        {
            StartCoroutine(AnimateItems(movementDetails));
        }
    }

    IEnumerator AnimateItems(IEnumerable<ItemMovementDetails> movementDetails)
    {
        List<GameObject> objectsToDestroy = new List<GameObject>();
        foreach (var item in movementDetails)
        {
            //calculate the new position in the world space
            var newGoPosition = new Vector3(item.NewColumn + item.NewColumn * distance,
                item.NewRow + item.NewRow * distance, ZIndex);

            //move it there
            var tween =
                item.GOToAnimatePosition.transform.positionTo(Consts.AnimationDuration, newGoPosition);
            tween.autoRemoveOnComplete = true;

            if (item.GOToAnimateScale != null)
            {
                var duplicatedItem = matrix[item.NewRow, item.NewColumn];

                UpdateScore(duplicatedItem.Value);

                if (duplicatedItem.Value == 2048)
                {
                    gameState = GameState.Won;
                    yield return new WaitForEndOfFrame();
                }

                var newGO = Instantiate(GetGOBasedOnValue(duplicatedItem.Value), newGoPosition, Quaternion.identity) as GameObject;
                newGO.transform.localScale = new Vector3(0f, 0f, 0f);
                newGO.transform.scaleTo(Consts.AnimationDuration, 1.0f);

                matrix[item.NewRow, item.NewColumn].gameObject = newGO;

                var moveTween = new GoTween(item.GOToAnimateScale.transform, Consts.AnimationDuration, new GoTweenConfig().position(newGoPosition));
                var scaleTween = new GoTween(item.GOToAnimateScale.transform, Consts.AnimationDuration, new GoTweenConfig().scale(0.1f));

                var chain = new GoTweenChain();
                chain.autoRemoveOnComplete = true;
                chain.append(moveTween).appendDelay(Consts.AnimationDuration).append(scaleTween);
                chain.play();

                objectsToDestroy.Add(item.GOToAnimateScale);
                objectsToDestroy.Add(item.GOToAnimatePosition);
            }
        }

        CreateNewItem();
        //hold on till the animations finish
        yield return new WaitForSeconds(Consts.AnimationDuration * movementDetails.Count() * 3);
        foreach (var go in objectsToDestroy)
            Destroy(go);
    }

    private void UpdateScore(int toAdd)
    {
        score += toAdd;
        ScoreText.text = "Score: " + score;
    }

    private GameObject GetGOBasedOnValue(int value)
    {
        GameObject newGo = null;
        switch (value)
        {
            case 2: newGo = GO2; break;
            case 4: newGo = GO4; break;
            case 8: newGo = GO8; break;
            case 16: newGo = GO16; break;
            case 32: newGo = GO32; break;
            case 64: newGo = GO64; break;
            case 128: newGo = GO128; break;
            case 256: newGo = GO256; break;
            case 512: newGo = GO512; break;
            case 1024: newGo = GO1024; break;
            case 2048: newGo = GO2048; break;
            default:
                throw new System.Exception("Uknown value:" + value);
        }
        return newGo;
    }
}

