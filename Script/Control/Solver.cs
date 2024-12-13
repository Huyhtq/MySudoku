using System;
using System.Collections.Generic;
using static Generator;

public interface ISudokuSolverStrategy
{
    int[,] Solve(int[,] unsolvedPuzzle);
}

public class BacktrackingSolver : ISudokuSolverStrategy
{
    private const int BoardSize = 9;
    private const int SubGridSize = 3;
    private const int EmptyCell = 0;

    public int[,] Solve(int[,] unsolvedPuzzle)
    {
        int[,] solvedPuzzle = new int[BoardSize, BoardSize];
        Array.Copy(unsolvedPuzzle, solvedPuzzle, unsolvedPuzzle.Length);
        BackTrack(solvedPuzzle, 0, 0);
        return solvedPuzzle;
    }

    private bool BackTrack(int[,] puzzle, int row, int col)
    {
        if (row == BoardSize)
        {
            return true;
        }

        if (col == BoardSize)
        {
            return BackTrack(puzzle, row + 1, 0);
        }

        if (puzzle[row, col] != EmptyCell)
        {
            return BackTrack(puzzle, row, col + 1);
        }

        for (int i = 1; i <= BoardSize; i++)
        {
            if (IsValid(i, row, col, puzzle))
            {
                puzzle[row, col] = i;
                if (BackTrack(puzzle, row, col + 1))
                {
                    return true;
                }
                puzzle[row, col] = EmptyCell;
            }
        }

        return false;
    }

    public static bool HasUniqueSolution(int[,] puzzle)
    {
        int solutionCount = 0;

        bool SolveWithCount(int[,] board, int r, int c)
        {
            if (r == 9)
            {
                solutionCount++;
                return solutionCount > 1; 
            }

            if (c == 9)
            {
                return SolveWithCount(board, r + 1, 0);
            }

            if (board[r, c] != 0)
            {
                return SolveWithCount(board, r, c + 1);
            }

            for (int num = 1; num <= 9; num++)
            {
                if (IsValid(num, r, c, board))
                {
                    board[r, c] = num;
                    if (SolveWithCount(board, r, c + 1)) return true;
                    board[r, c] = 0;
                }
            }

            return false;
        }

        SolveWithCount((int[,])puzzle.Clone(), 0, 0);
        return solutionCount == 1;
    }

    private static bool IsValid(int val, int row, int col, int[,] board)
    {
        for (int i = 0; i < 9; i++)
        {
            if (board[row, i] == val || board[i, col] == val)
            {
                return false;
            }
        }

        int subGridRow = row / 3 * 3;
        int subGridCol = col / 3 * 3;

        for (int r = subGridRow; r < subGridRow + 3; r++)
        {
            for (int c = subGridCol; c < subGridCol + 3; c++)
            {
                if (board[r, c] == val)
                {
                    return false;
                }
            }
        }

        return true;
    }
}

public class DancingLinksSolver : ISudokuSolverStrategy
{
    public int[,] Solve(int[,] unsolvedPuzzle)
    {
        DancingLinks dlx = new DancingLinks(unsolvedPuzzle);
        return dlx.Solve();
    }

    public static bool HasUniqueSolution(int[,] puzzle)
    {
        var dlx = new DancingLinks(puzzle);
        int solutionCount = 0;

        void OnSolutionFound(int[,] solvedBoard)
        {
            solutionCount++;
        }

        dlx.SolveWithCallback(OnSolutionFound);

        return solutionCount == 1;
    }
}

public class DancingLinks
{
    private const int BoardSize = 9;

    private class Node
    {
        public Node Left, Right, Up, Down;
        public Node ColumnHeader;
        public int RowIndex, ColumnIndex;

        public Node() { }
    }

    private Node[,] grid;
    private Node header;
    private int[,] initialBoard;

    public DancingLinks(int[,] puzzle)
    {
        initialBoard = puzzle;
        BuildGrid(puzzle);
    }

    public void SolveWithCallback(Action<int[,]> onSolutionFound)
    {
        Solve(0, onSolutionFound);
    }

    private bool Solve(int step, Action<int[,]> onSolutionFound)
    {
        onSolutionFound(GetSolvedBoard());
        return true;
    }

    private int[,] GetSolvedBoard()
    {
        return new int[9, 9];
    }

    private void BuildGrid(int[,] unsolvedPuzzle)
    {
        header = new Node(); // Dummy header node
        List<Node> columnHeaders = new List<Node>();

        // Create column headers
        for (int i = 0; i < 4 * BoardSize * BoardSize; i++)
        {
            Node columnHeader = new Node();
            columnHeader.Up = columnHeader;
            columnHeader.Down = columnHeader;

            if (columnHeaders.Count > 0)
            {
                Node last = columnHeaders[^1];
                last.Right = columnHeader;
                columnHeader.Left = last;
            }

            columnHeaders.Add(columnHeader);
        }

        // Link the last column to the header
        if (columnHeaders.Count > 0)
        {
            columnHeaders[^1].Right = header;
            header.Left = columnHeaders[^1];
        }

        header.Right = columnHeaders[0];
        columnHeaders[0].Left = header;

        // Map puzzle constraints to exact cover rows
        for (int row = 0; row < BoardSize; row++)
        {
            for (int col = 0; col < BoardSize; col++)
            {
                int cellValue = unsolvedPuzzle[row, col];
                if (cellValue == 0)
                {
                    for (int val = 1; val <= BoardSize; val++)
                    {
                        AddRow(columnHeaders, row, col, val);
                    }
                }
                else
                {
                    AddRow(columnHeaders, row, col, cellValue);
                }
            }
        }
    }

    private void AddRow(List<Node> columnHeaders, int row, int col, int value)
    {
        // Calculate constraints indices
        int rowConstraint = row * BoardSize + col;
        int columnConstraint = BoardSize * BoardSize + (row * BoardSize + value - 1);
        int blockConstraint = 2 * BoardSize * BoardSize + (3 * (row / 3) + col / 3) * BoardSize + value - 1;

        // Add nodes to represent constraints
        Node first = null, last = null;

        foreach (int constraint in new[] { rowConstraint, columnConstraint, blockConstraint })
        {
            Node columnHeader = columnHeaders[constraint];
            Node newNode = new Node
            {
                ColumnHeader = columnHeader,
                RowIndex = row,
                ColumnIndex = col
            };

            // Link vertically
            newNode.Up = columnHeader.Up;
            newNode.Down = columnHeader;
            columnHeader.Up.Down = newNode;
            columnHeader.Up = newNode;

            // Link horizontally
            if (first == null)
            {
                first = newNode;
            }
            else
            {
                newNode.Left = last;
                last.Right = newNode;
            }

            last = newNode;
        }

        if (first != null && last != null)
        {
            last.Right = first;
            first.Left = last;
        }
    }


    public int[,] Solve()
    {
        List<Node> solution = new List<Node>();
        if (Search(solution))
        {
            return ReconstructSolution(solution);
        }

        return null; // No solution
    }

    private bool Search(List<Node> solution)
    {
        if (header.Right == header)
        {
            return true; // No columns left, solution found
        }

        Node column = ChooseColumn();
        Cover(column);

        for (Node row = column.Down; row != column; row = row.Down)
        {
            solution.Add(row);

            for (Node node = row.Right; node != row; node = node.Right)
            {
                Cover(node.ColumnHeader);
            }

            if (Search(solution))
            {
                return true;
            }

            solution.RemoveAt(solution.Count - 1);

            for (Node node = row.Left; node != row; node = node.Left)
            {
                Uncover(node.ColumnHeader);
            }
        }

        Uncover(column);
        return false;
    }

    private Node ChooseColumn()
    {
        Node bestColumn = null;
        int minSize = int.MaxValue;

        for (Node column = header.Right; column != header; column = column.Right)
        {
            int size = 0;
            for (Node node = column.Down; node != column; node = node.Down)
            {
                size++;
            }

            if (size < minSize)
            {
                minSize = size;
                bestColumn = column;
            }
        }

        return bestColumn;
    }

    private void Cover(Node column)
    {
        column.Right.Left = column.Left;
        column.Left.Right = column.Right;

        for (Node row = column.Down; row != column; row = row.Down)
        {
            for (Node node = row.Right; node != row; node = node.Right)
            {
                node.Down.Up = node.Up;
                node.Up.Down = node.Down;
            }
        }
    }

    private void Uncover(Node column)
    {
        for (Node row = column.Up; row != column; row = row.Up)
        {
            for (Node node = row.Left; node != row; node = node.Left)
            {
                node.Down.Up = node;
                node.Up.Down = node;
            }
        }

        column.Right.Left = column;
        column.Left.Right = column;
    }

    private int[,] ReconstructSolution(List<Node> solution)
    {
        int[,] solvedPuzzle = new int[BoardSize, BoardSize];

        foreach (Node node in solution)
        {
            int row = node.RowIndex;
            int col = node.ColumnIndex;
            int value = (node.ColumnHeader.Left.ColumnIndex % BoardSize) + 1;

            solvedPuzzle[row, col] = value;
        }

        return solvedPuzzle;
    }

}


public class SudokuSolverContext
{
    private ISudokuSolverStrategy _solverStrategy;

    public void SetSolverStrategy(DifficultyLevel difficultyLevel)
    {
        switch (difficultyLevel)
        {
            case DifficultyLevel.VERY_HARD:
            case DifficultyLevel.EXTREME:
                _solverStrategy = new DancingLinksSolver();
                break;
            default:
                _solverStrategy = new BacktrackingSolver();
                break;
        }
    }

    public int[,] Solve(int[,] puzzle, DifficultyLevel difficultyLevel)
    {
        SetSolverStrategy(difficultyLevel);
        if (_solverStrategy == null)
        {
            throw new InvalidOperationException("Solver strategy is not set.");
        }
        return _solverStrategy.Solve(puzzle);
    }
}
