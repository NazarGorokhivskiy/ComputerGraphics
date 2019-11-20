using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;

public class ItemArray
{
    private Item[,] matrix = new Item[Consts.Rows, Consts.Columns];

    public Item this[int row, int column]
    {
        get
        {
            return matrix[row, column];
        }
        set
        {
            matrix[row, column] = value;
        }
    }


    public void GetRandomRowColumn(out int row, out int column)
    {
        do
        {
            row = random.Next(0, Consts.Rows);
            column = random.Next(0, Consts.Columns);
        } while (
            matrix[row, column] != null
        );
    }


    public List<ItemMovementDetails> MoveHorizontal(HorizontalMovement horizontalMovement)
    {
        ResetWasJustDuplicatedValues();

        var movementDetails = new List<ItemMovementDetails>();

        int relativeColumn = horizontalMovement == HorizontalMovement.Left ? -1 : 1;
        var columnNumbers = Enumerable.Range(0, Consts.Columns);

        if (horizontalMovement == HorizontalMovement.Right)
        {
            columnNumbers = columnNumbers.Reverse();
        }

        for (int row = Consts.Rows - 1; row >= 0; row--)
        {
            foreach (int column in columnNumbers)
            {
                if (matrix[row, column] == null) continue;

                ItemMovementDetails imd = AreTwoItemsSame(row, column, row, column + relativeColumn);
                if (imd != null)
                {
                    movementDetails.Add(imd);
                    continue;
                }

                int columnFirstNullItem = -1;

                int numberOfItemsToTake = horizontalMovement == HorizontalMovement.Left
                ? column : Consts.Columns - column;

                bool emptyItemFound = false;

                foreach (var tempColumnFirstNullItem in columnNumbers.Take(numberOfItemsToTake))
                {
                    columnFirstNullItem = tempColumnFirstNullItem;
                    if (matrix[row, columnFirstNullItem] == null)
                    {
                        emptyItemFound = true;
                        break;
                    }
                }

                if (!emptyItemFound)
                {
                    continue;
                }

                ItemMovementDetails newImd =
                MoveItemToNullPositionAndCheckIfSameWithNextOne
                (row, row, row, column, columnFirstNullItem, columnFirstNullItem + relativeColumn);

                movementDetails.Add(newImd);
            }
        }
        return movementDetails;
    }



    public List<ItemMovementDetails> MoveVertical(VerticalMovement verticalMovement)
    {
        ResetWasJustDuplicatedValues();

        var movementDetails = new List<ItemMovementDetails>();

        int relativeRow = verticalMovement == VerticalMovement.Bottom ? -1 : 1;
        var rowNumbers = Enumerable.Range(0, Consts.Rows);

        if (verticalMovement == VerticalMovement.Top)
        {
            rowNumbers = rowNumbers.Reverse();
        }

        for (int column = 0; column < Consts.Columns; column++)
        {
            foreach (int row in rowNumbers)
            {
                if (matrix[row, column] == null) continue;

                ItemMovementDetails imd = AreTwoItemsSame(row, column, row + relativeRow, column);
                if (imd != null)
                {
                    movementDetails.Add(imd);

                    continue;
                }

                int rowFirstNullItem = -1;

                int numberOfItemsToTake = verticalMovement == VerticalMovement.Bottom
                ? row : Consts.Rows - row;


                bool emptyItemFound = false;

                foreach (var tempRowFirstNullItem in rowNumbers.Take(numberOfItemsToTake))
                {
                    rowFirstNullItem = tempRowFirstNullItem;
                    if (matrix[rowFirstNullItem, column] == null)
                    {
                        emptyItemFound = true;
                        break;
                    }
                }

                if (!emptyItemFound)
                {
                    continue;
                }

                ItemMovementDetails newImd =
                MoveItemToNullPositionAndCheckIfSameWithNextOne(row, rowFirstNullItem, rowFirstNullItem + relativeRow, column, column, column);

                movementDetails.Add(newImd);
            }
        }
        return movementDetails;
    }

    private ItemMovementDetails MoveItemToNullPositionAndCheckIfSameWithNextOne
    (int oldRow, int newRow, int itemToCheckRow, int oldColumn, int newColumn, int itemToCheckColumn)
    {
        matrix[newRow, newColumn] = matrix[oldRow, oldColumn];
        matrix[oldRow, oldColumn] = null;

        ItemMovementDetails imd2 = AreTwoItemsSame(newRow, newColumn, itemToCheckRow,
            itemToCheckColumn);

        if (imd2 != null) return imd2;

        return new ItemMovementDetails(newRow, newColumn, matrix[newRow, newColumn].gameObject, null);
    }


    private ItemMovementDetails AreTwoItemsSame(
        int originalRow, int originalColumn, int toCheckRow, int toCheckColumn)
    {
        if (toCheckRow < 0 || toCheckColumn < 0 || toCheckRow >= Consts.Rows || toCheckColumn >= Consts.Columns)
            return null;


        if (matrix[originalRow, originalColumn] != null && matrix[toCheckRow, toCheckColumn] != null
                && matrix[originalRow, originalColumn].Value == matrix[toCheckRow, toCheckColumn].Value
                && !matrix[toCheckRow, toCheckColumn].WasJustDuplicated)
        {
            //double the value, since the item is duplicated
            matrix[toCheckRow, toCheckColumn].Value *= 2;
            matrix[toCheckRow, toCheckColumn].WasJustDuplicated = true;
            //make a copy of the gameobject to be moved and then disappear
            var GOToAnimateScaleCopy = matrix[originalRow, originalColumn].gameObject;
            //remove this item from the array
            matrix[originalRow, originalColumn] = null;
            return new ItemMovementDetails(toCheckRow, toCheckColumn, matrix[toCheckRow, toCheckColumn].gameObject, GOToAnimateScaleCopy);

        }

        return null;
    }

    private void ResetWasJustDuplicatedValues()
    {
        for (int row = 0; row < Consts.Rows; row++)
            for (int column = 0; column < Consts.Columns; column++)
            {
                if (matrix[row, column] != null && matrix[row, column].WasJustDuplicated)
                    matrix[row, column].WasJustDuplicated = false;
            }
    }

    private System.Random random = new System.Random();
}

public enum HorizontalMovement { Left, Right };
public enum VerticalMovement { Top, Bottom };
