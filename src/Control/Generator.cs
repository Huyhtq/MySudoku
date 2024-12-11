using System.Collections.Generic;
using UnityEngine;

public static class Generator
{
    private const int GridSize = 9;
    private const int SubGridSize = 3;
    private const int MinSquaresToRemove = 20;
    private const int MaxSquaresToRemove = 70;

    public enum DifficultyLevel
    {
        EASY,
        MEDIUM,
        HARD,
        VERY_HARD,
        EXTREME,
        CAMPAIGN
    }

    public static int[,] GeneratePuzzle(DifficultyLevel difficultyLevel)
    {
        var grid = new int[GridSize, GridSize];
        InitializeGrid(grid);

        int squaresToRemove = GetSquaresToRemove(difficultyLevel);

        RemoveSquares(grid, squaresToRemove, difficultyLevel);

        return grid;
    }

    private static void InitializeGrid(int[,] grid)
    {
        List<int> numbers = GenerateShuffledNumbers();
        for (int i = 0; i < GridSize; i++)
        {
            grid[0, i] = numbers[i];
        }

        FillGrid(1, 0, grid);
    }

    private static void RemoveSquares(int[,] grid, int squaresToRemove, DifficultyLevel difficultyLevel)
    {
        while (squaresToRemove > 0)
        {
            int row = Random.Range(0, GridSize);
            int col = Random.Range(0, GridSize);

            if (grid[row, col] != 0)
            {
                int temp = grid[row, col];
                grid[row, col] = 0;

                if (!HasUniqueSolution(grid, difficultyLevel))
                {
                    grid[row, col] = temp; 
                }
                else
                {
                    squaresToRemove--;
                }
            }
        }
    }

    private static int GetSquaresToRemove(DifficultyLevel difficultyLevel)
    {
        return difficultyLevel switch
        {
            DifficultyLevel.EASY => Random.Range(MinSquaresToRemove, MinSquaresToRemove + 10),
            DifficultyLevel.MEDIUM => Random.Range(MinSquaresToRemove + 10, MinSquaresToRemove + 20),
            DifficultyLevel.HARD => Random.Range(MinSquaresToRemove + 20, MinSquaresToRemove + 30),
            DifficultyLevel.VERY_HARD => Random.Range(MinSquaresToRemove + 30, MinSquaresToRemove + 40),
            DifficultyLevel.EXTREME => Random.Range(MinSquaresToRemove + 40, MaxSquaresToRemove),
            _ => GetSquaresToRemove((DifficultyLevel)Random.Range(0, 5))
        };
    }

    private static bool FillGrid(int row, int col, int[,] grid)
    {
        if (row == GridSize) return true;
        if (col == GridSize) return FillGrid(row + 1, 0, grid);

        foreach (int num in GenerateShuffledNumbers())
        {
            if (IsValid(num, row, col, grid))
            {
                grid[row, col] = num;

                if (FillGrid(row, col + 1, grid))
                {
                    return true;
                }
            }
        }

        grid[row, col] = 0; 
        return false;
    }

    private static bool IsValid(int value, int row, int col, int[,] grid)
    {
        for (int i = 0; i < GridSize; i++)
        {
            if (grid[row, i] == value || grid[i, col] == value)
            {
                return false;
            }
        }

        int subGridRowStart = (row / SubGridSize) * SubGridSize;
        int subGridColStart = (col / SubGridSize) * SubGridSize;

        for (int r = subGridRowStart; r < subGridRowStart + SubGridSize; r++)
        {
            for (int c = subGridColStart; c < subGridColStart + SubGridSize; c++)
            {
                if (grid[r, c] == value)
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static bool HasUniqueSolution(int[,] grid, DifficultyLevel difficultyLevel)
    {
        return difficultyLevel == DifficultyLevel.EXTREME
            ? DancingLinksSolver.HasUniqueSolution(grid)
            : BacktrackingSolver.HasUniqueSolution(grid);
    }

    private static List<int> GenerateShuffledNumbers()
    {
        List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        for (int i = numbers.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            (numbers[i], numbers[randomIndex]) = (numbers[randomIndex], numbers[i]);
        }

        return numbers;
    }
}
