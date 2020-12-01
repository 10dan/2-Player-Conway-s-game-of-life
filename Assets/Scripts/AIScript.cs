using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript {
    private static int[,] board;
    private static int w, h;
    public void PredictNextMove(Cell[,] cells, int difficulty) {

        //First convert the cells into int array to make more efficient.
        w = cells.GetLength(0);
        h = cells.GetLength(1);
        board = BoardConverter.ConvertToInt(cells);

        //Go through every possible dead cell on their side.
        int[,] finalResult = new int[w, h];//Stores how many cells it will have after "n" cycles.
        for (int x = w / 2; x < w; x++) { //W/2 to only allow them to place on right hand side.
            for (int y = 0; y < h; y++) {
                finalResult[x, y] = 0;
                int[,] copy = board.Clone() as int[,];
                if (copy[x, y] == 0) { //If a cell is empty,
                    copy[x, y] = 2; //Test what happens if you place a cell here.
                    for (int i = 0; i < difficulty; i++) { //Difficulty is the number of cycles it predicts into.
                        int[,] next = copy.Clone() as int[,];
                        next = BoardConverter.PredictNextBoard(copy);
                        copy = next;
                    }
                    finalResult[x, y] = BoardConverter.CountCells(copy, 2);
                }
            }
        }

        //Now pick the cell that gives AI best chance of winning.
        int maxX = w / 2;
        int maxY = 0;
        for (int x = 0; x < w; x++) {
            for (int y = 0; y < h; y++) {
                if (finalResult[x, y] > finalResult[maxX, maxY]) {
                    maxX = x;
                    maxY = y;
                } 
                if(finalResult[maxX, maxY] == 0) {
                    int newX = w / 2;
                    int newY = 0;
                    while (cells[newX, newY].state != Cell.CellState.Dead) {
                        newX = (int)Mathf.Round(UnityEngine.Random.Range(w / 2, w));
                        newY = (int)Mathf.Round(UnityEngine.Random.Range(0, h));
                    }
                    maxX = newX;
                    maxY = newY;
                }
            }
        }
        if (cells[maxX, maxY].state != Cell.CellState.Alive1) {
            cells[maxX, maxY].state = Cell.CellState.Alive2;
        }
    }
}
