using System;
using UnityEngine;

public static class Utilities
{
    public static string[,] GetMatrixFromResourcesData()
    {
        string[,] shapes = new string[Consts.Rows, Consts.Columns];

        TextAsset txt = Resources.Load("debugLevel") as TextAsset;
        string level = txt.text;

        string[] lines = level.Split(new string[] { Environment.NewLine, "\n" }, StringSplitOptions.RemoveEmptyEntries);
        for (int row = 0; row < Consts.Rows; row++)
        {
            string[] items = lines[row].Split('|');
            for (int column = 0; column < Consts.Columns; column++)
            {
                shapes[row, column] = items[column];
            }
        }
        return shapes;

    }
}