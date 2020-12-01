using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardConverter {

    //Returns vector2 int
    public static int CountCells(int[,] copy, int cellType) {
        int w = copy.GetLength(0);
        int h = copy.GetLength(1);
        int results = 0;
        for (int x = 0; x < w; x++) {
            for (int y = 0; y < h; y++) {
                if (copy[x, y] == cellType) {
                    results++;
                } 
            }
        }
        return results;
    }

    public static int[,] PredictNextBoard(int[,] copy) {
        int w = copy.GetLength(0);
        int h = copy.GetLength(1);
        int[,] nextBoard = copy.Clone() as int[,];
        for (int x = 0; x < w; x++) {
            for (int y = 0; y < h; y++) {
                Vector2Int count = CountNeighbours(copy, x, y);
                int sum = count.x + count.y;
                if (nextBoard[x, y] == 0 && sum == 3) {
                    if (count.x > count.y) {
                        nextBoard[x, y] = 1;
                    } else {
                        nextBoard[x, y] = 2;
                    }
                }
                if (sum < 2 || sum > 3) {
                    nextBoard[x, y] = 0;
                }
            }
        }
        return nextBoard;
    }

    public static Vector2Int CountNeighbours(int[,] boardIn, int x, int y) {
        int w = boardIn.GetLength(0);
        int h = boardIn.GetLength(1);
        int value1 = 0;
        int value2 = 0;
        for (var j = -1; j <= 1; j++) {
            if (!BoardBehavior.wrapAround && y + j < 0 || y + j >= h) continue;
            int y1 = (y + j + h) % h;
            for (var i = -1; i <= 1; i++) {
                if (!BoardBehavior.wrapAround && x + i < 0 || x + i >= w) continue;
                int x1 = (x + i + w) % w;
                if (boardIn[x1, y1] == 1) {
                    value1++;
                } else if (boardIn[x1, y1] == 2) {
                    value2++;
                }
            }
        }
        if (boardIn[x, y] == 1) {
            value1--;
        }
        if (boardIn[x, y] == 2) {
            value2--;
        }
        return new Vector2Int(value1, value2);
    }

    public static int[,] ConvertToInt(Cell[,] cells) {
        int w = cells.GetLength(0);
        int h = cells.GetLength(1);
        int[,] converted = new int[w, h];
        for (int x = 0; x < w; x++) {
            for (int y = 0; y < h; y++) {
                if (cells[x, y].state == Cell.CellState.Dead) {
                    converted[x, y] = 0;
                } else if (cells[x, y].state == Cell.CellState.Alive1) {
                    converted[x, y] = 1;
                } else {
                    converted[x, y] = 2;
                }
            }
        }
        return converted;
    }

    //For debug
    public static void PrintBoard(int[,] whichBoard) {
        int w = whichBoard.GetLength(0);
        int h = whichBoard.GetLength(1);
        Debug.Log("-----------");
        for (int y = h - 1; y >= 0; y--) {
            string p = "";
            for (int x = 0; x < w; x++) {
                p += whichBoard[x, y];
            }
            Debug.Log(p);
        }
    }

    public static void UpdateCells(Cell[,] cells) {
        int w = cells.GetLength(0);
        int h = cells.GetLength(1);
        int[,] intBoard = ConvertToInt(cells);
        int[,] nextBoard = PredictNextBoard(intBoard);
        for (int x = 0; x < w; x++) {
            for (int y = 0; y < h; y++) {
                if (nextBoard[x, y] == 0) {
                    cells[x, y].state = Cell.CellState.Dead;
                } else if (nextBoard[x, y] == 1) {
                    cells[x, y].state = Cell.CellState.Alive1;
                } else {
                    cells[x, y].state = Cell.CellState.Alive2;
                }
            }
        }
    }
}