using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private SubGrid _subGridPrefab;
    [SerializeField] private Transform gridParent;

    [SerializeField] private float xOffset = 2.0f;
    [SerializeField] private float yOffset = 2.0f;
    [SerializeField] private float spawnX = 0f;
    [SerializeField] private float spawnY = 0f;

    public Cell[,] cells;
    private const int GridSize = 9;
    private Cell selectedCell;

     public int GetGridSize()
    {
        return GridSize;
    }

    /// <summary>
    /// Initializes the grid by clearing the old grid and spawning a new one based on the puzzle grid.
    /// </summary>
    public void Initialize(int[,] puzzleGrid)
    {
        ClearGrid();
        cells = new Cell[GridSize, GridSize];
        SpawnGrid(puzzleGrid);
    }

    /// <summary>
    /// Spawns the grid by instantiating subgrids and initializing each cell with its value.
    /// </summary>
    private void SpawnGrid(int[,] puzzleGrid)
    {
        for (int i = 0; i < GridSize; i++)
        {
            Vector3 spawnPos = CalculateSpawnPosition(i);
            SubGrid subGrid = Instantiate(_subGridPrefab, spawnPos, Quaternion.identity, gridParent);

            for (int j = 0; j < GridSize; j++)
            {
                int row = (i / 3) * 3 + j / 3;
                int col = (i % 3) * 3 + j % 3;
                Cell cell = subGrid.cells[j];
                cell.Init(puzzleGrid[row, col], row, col);
                cells[row, col] = cell;
            }
        }
    }

    /// <summary>
    /// Calculates the spawn position for each subgrid in the grid.
    /// </summary>
    private Vector3 CalculateSpawnPosition(int index)
    {
        int row = index / 3;  
        int col = index % 3;  

        float posX = spawnX + col * xOffset; 
        float posY = spawnY + row * yOffset; 

        return new Vector3(posX, posY, 0);
    }

    /// <summary>
    /// Clears the current grid by destroying all child objects under gridParent.
    /// </summary>
    private void ClearGrid()
    {
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Process the selected cell when clicked (only if it is not locked).
    /// </summary>
    public void HandleCellClick(Cell cell)
    {
        if (cell.State == Cell.CellState.Locked) return;

        selectedCell = cell;
        HighlightCells();
    }

    /// <summary>
    /// Highlights the selected cell's row, column, and subgrid.
    /// </summary>
    private void HighlightCells()
    {
        int row = selectedCell.Row;
        int col = selectedCell.Col;

        // Highlight row, column, and subgrid
        for (int i = 0; i < GridSize; i++)
        {
            // Highlight row and column
            cells[row, i].Highlight();
            cells[i, col].Highlight();
        }

        // Highlight subgrid
        int subGridRow = row - row % 3;
        int subGridCol = col - col % 3;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                cells[subGridRow + i, subGridCol + j].Highlight();
            }
        }

        // Highlight the selected cell itself
        selectedCell.Select();
    }

    /// <summary>
    /// Processes a specific cell by checking its value and performing logic based on the game rules.
    /// </summary>
    /// <param name="row">Row index of the cell.</param>
    /// <param name="col">Column index of the cell.</param>
    /// <returns>True if the action is valid, false otherwise.</returns>
    public bool ProcessCell(int row, int col)
    {
        if (cells[row, col].State == Cell.CellState.Locked)
        {
            Debug.LogWarning("This cell is locked and cannot be changed.");
            return false;
        }
        
        bool isCorrect = ValidateCell(row, col);
        if (isCorrect)
        {
            cells[row, col].Select(); // Highlight as correct
        }
        else
        {
            cells[row, col].State = Cell.CellState.Incorrect;
            cells[row, col].Select(); // Highlight as incorrect
        }

        return isCorrect;
    }

    /// <summary>
    /// Resets the grid to its default state, removing all highlights or temporary changes.
    /// </summary>
    public void ResetGrid()
    {
        foreach (Cell cell in cells)
        {
            if (cell != null)
            {
                cell.Reset();
            }
        }
    }

    /// <summary>
    /// Validates the value in the cell based on game rules.
    /// </summary>
    /// <param name="row">Row index of the cell.</param>
    /// <param name="col">Column index of the cell.</param>
    /// <returns>True if the cell is valid, false otherwise.</returns>
    private bool ValidateCell(int row, int col)
    {
        // Placeholder logic for validation (replace with your own game rules)
        int cellValue = cells[row, col].Value;
        return cellValue > 0; // Example: Any non-zero value is considered correct
    }

    /// <summary>
    /// Update the selected cell's value when a number button is pressed.
    /// </summary>
    public void UpdateCellValue(int newValue)
    {
        if (selectedCell != null)
        {
            selectedCell.UpdateValue(newValue);
            selectedCell.Reset(); // Optionally reset the highlight after updating
            HighlightRelatedCells();
        }
    }

    /// <summary>
    /// Highlights cells with the same value as the selected one or in the same row and column.
    /// </summary>
    private void HighlightRelatedCells()
    {
        if (selectedCell == null) return;

        int selectedValue = selectedCell.Value;
        for (int i = 0; i < GridSize; i++)
        {
            for (int j = 0; j < GridSize; j++)
            {
                Cell cell = cells[i, j];

                if (cell != selectedCell)
                {
                    // Highlight cells with the same value
                    if (cell.Value == selectedValue)
                    {
                        cell.Highlight(); // Highlight cells with the same value
                    }
                    // Highlight cells in the same row or column
                    else if (cell.Row == selectedCell.Row || cell.Col == selectedCell.Col)
                    {
                        cell.Highlight(); // Highlight cells in the same row or column
                    }
                    else
                    {
                        cell.Reset(); // Reset non-matching cells
                    }
                }
            }
        }
    }

    /// <summary>
    /// Set the selected cell.
    /// </summary>
    public void SetSelectedCell(Cell cell)
    {
        selectedCell = cell;
    }

    /// <summary>
    /// Checks if the entire grid is solved.
    /// </summary>
    /// <returns>True if the grid is solved, false otherwise.</returns>
    public bool IsGridSolved()
    {
        // Loop through all cells and check if each one is correct.
        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                if (!ValidateCell(row, col)) // If any cell is invalid, return false
                {
                    return false;
                }
            }
        }

        // If all cells are valid, return true
        return true;
    }
}
