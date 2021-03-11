using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript {
    private static int[,] board;
    private static int w, h;
    public Boolean isThinking = false;
    public void PredictNextMove(Cell[,] cells) {
        //Allow the other scripts know if the AI is thinking.
        isThinking = true;

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
                    for (int i = 0; i < SettingsHolder.AIDifficulty; i++) { //Difficulty is the number of cycles it predicts into.
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
                if (finalResult[maxX, maxY] == 0) {
                    int newX = w / 2;
                    int newY = 0;
                    int count = 0; //Make sure it doesnt get in an infinite loop, limit number of iterations.
                    //While the cell we are looking at is currently occupied.
                    while ((cells[newX, newY].state != Cell.CellState.Dead) && (count < SettingsHolder.AIDifficulty*5)) {
                        //Pick a random spot and hope it's empty.
                        newX = (int)Mathf.Round(UnityEngine.Random.Range(w / 2, w));
                        newY = (int)Mathf.Round(UnityEngine.Random.Range(0, h));
                        //This improves the AI, reduces risk of bad guesses. (better to place near live cells)
                        try {
                            if ( //If any of the surrounding cells are not dead & the selected cell is dead.
                                cells[newX - 1, newY].state != Cell.CellState.Dead ||
                                cells[newX + 1, newY].state != Cell.CellState.Dead ||
                                cells[newX, newY - 1].state != Cell.CellState.Dead ||
                                cells[newX, newY + 1].state != Cell.CellState.Dead &&
                                cells[newX, newY].state == Cell.CellState.Dead) {
                                break;
                            }
                        } catch (Exception e) {

                        }
                        count++;
                    }

                    maxX = newX;
                    maxY = newY;
                }
            }
        }
        if (cells[maxX, maxY].state != Cell.CellState.Alive1) {
            cells[maxX, maxY].state = Cell.CellState.Alive2;
        }
        //Disable loading message.
        isThinking = false;
    }
}
