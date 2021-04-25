using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardConverter {
    private static bool wrapAround = true;

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
        //Assign wraparound here as this is only called once.
        wrapAround = SettingsHolder.WrapAround;
        int w = copy.GetLength(0);
        int h = copy.GetLength(1);
        //Clone the array so we have somwhere to record changes.
        int[,] nextBoard = copy.Clone() as int[,];
        for (int x = 0; x < w; x++) {
            for (int y = 0; y < h; y++) {
                Vector2Int count = CountNeighbours(copy, x, y);
                int sum = count.x + count.y;
                //If the cells is unoccupied and has 3 neighbours.
                if (nextBoard[x, y] == 0 && sum == 3) {
                    if (count.x > count.y) {
                        //birth it to player 1.
                        nextBoard[x, y] = 1;
                    } else { //Or player 2.
                        nextBoard[x, y] = 2;
                    }
                } //Kill it if its lonely or overpopulated.
                if (sum < 2 || sum > 3) {
                    nextBoard[x, y] = 0;
                }
            }
        }
        return nextBoard;
    }

    //Returns the number of cells per team in a vector (#p1 cells, # p2 cells)
    public static Vector2Int CountNeighbours(int[,] boardIn, int x, int y) {
        int w = boardIn.GetLength(0);
        int h = boardIn.GetLength(1);
        int value1 = 0; //Count player 1 neighbouring cells.
        int value2 = 0; //And player 2.
        for (var j = -1; j <= 1; j++) {
            //If we are looking off the side of the map & we dont want to wrap around continue.
            if (!wrapAround && y + j < 0 || y + j >= h) continue;
            //Otherwise we set the y1 coordinate.
            int y1 = (y + j + h) % h;
            for (var i = -1; i <= 1; i++) {
                if (!wrapAround && x + i < 0 || x + i >= w) continue;
                int x1 = (x + i + w) % w;
                if (boardIn[x1, y1] == 1) {
                    value1++;
                } else if (boardIn[x1, y1] == 2) {
                    value2++;
                }
            }
        }
        //If the cell we are looking at is not dead, then subtract 1 to avoid counting it.
        if (boardIn[x, y] == 1) {
            value1--;
        }
        if (boardIn[x, y] == 2) {
            value2--;
        }
        return new Vector2Int(value1, value2);
    }

    //Convert a cell list into int array, easier to work with
    public static int[,] ConvertToInt(Cell[,] cells) {
        int w = cells.GetLength(0);
        int h = cells.GetLength(1);
        //Create 2d int array with same dimensions
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
                    if (cells[x, y].state != Cell.CellState.Dead) {
                        cells[x, y].PlayDeathFX(cells[x,y].state); //Play death particle. (pass it it's prev state to set colour)
                    }
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