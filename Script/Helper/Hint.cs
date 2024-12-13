using TMPro;
using UnityEngine;

public class Hint : MonoBehaviour
{
    [SerializeField] private TMP_Text _hintText;

    private const int GridSize = 9;
    private const int SubGridSize = 3;

    private Cell[,] _cells;
    private int _maxHints = 3;
    private int _remainingHints;

    #region Initialization
    /// <summary>
    /// Initializes the hint system with the provided cell grid.
    /// </summary>
    /// <param name="gridCells">The 2D array of cells.</param>
    public void Initialize(Cell[,] gridCells)
    {
        _cells = gridCells;
        _remainingHints = _maxHints;
        UpdateHintUI();
    }

    /// <summary>
    /// Resets the hints to the maximum available.
    /// </summary>
    public void ResetHints()
    {
        _remainingHints = _maxHints;
        UpdateHintUI();
        Debug.Log("Hints reset.");
    }

    /// <summary>
    /// Clears all used hints and resets the count.
    /// </summary>
    public void ClearHints()
    {
        ResetHints();
        Debug.Log("Hints cleared.");
    }
    #endregion

    #region Hint Logic
    /// <summary>
    /// Provides a hint by filling in a cell with its correct value if possible.
    /// </summary>
    /// <returns>True if a hint was successfully provided, false otherwise.</returns>
    public bool ProvideHint()
    {
        if (_cells == null)
        {
            Debug.LogError("Cells array is not initialized! Ensure Initialize() is called.");
            return false;
        }

        if (_remainingHints <= 0)
        {
            Debug.Log("No hints remaining!");
            return false;
        }

        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                if (_cells[row, col].Value == 0)
                {
                    int correctValue = FindCorrectValue(row, col);
                    if (correctValue != -1)
                    {
                        _cells[row, col].HighlightHint(correctValue);

                        _remainingHints--;
                        UpdateHintUI();
                        Debug.Log($"Hint provided: Row {row}, Col {col} = {correctValue}");
                        Debug.Log($"Hints remaining: {_remainingHints}");
                        return true;
                    }
                }
            }
        }

        Debug.Log("No hints available. The puzzle might be complete!");
        return false;
    }

    /// <summary>
    /// Finds the correct value for a given cell based on Sudoku rules.
    /// </summary>
    /// <param name="row">The row index of the cell.</param>
    /// <param name="col">The column index of the cell.</param>
    /// <returns>The correct value for the cell, or -1 if no valid value is found.</returns>
    private int FindCorrectValue(int row, int col)
    {
        for (int value = 1; value <= GridSize; value++)
        {
            if (IsValid(row, col, value))
            {
                return value;
            }
        }
        return -1;
    }

    /// <summary>
    /// Checks if a value is valid for a given cell based on Sudoku rules.
    /// </summary>
    /// <param name="row">The row index of the cell.</param>
    /// <param name="col">The column index of the cell.</param>
    /// <param name="value">The value to validate.</param>
    /// <returns>True if the value is valid, false otherwise.</returns>
    private bool IsValid(int row, int col, int value)
    {
        // Check row
        for (int i = 0; i < GridSize; i++)
        {
            if (_cells[row, i].Value == value) return false;
        }

        // Check column
        for (int i = 0; i < GridSize; i++)
        {
            if (_cells[i, col].Value == value) return false;
        }

        // Check subgrid
        int subGridRowStart = row - row % SubGridSize;
        int subGridColStart = col - col % SubGridSize;

        for (int r = subGridRowStart; r < subGridRowStart + SubGridSize; r++)
        {
            for (int c = subGridColStart; c < subGridColStart + SubGridSize; c++)
            {
                if (_cells[r, c].Value == value) return false;
            }
        }

        return true;
    }
    #endregion

    #region Hint Management
    /// <summary>
    /// Handles the logic when the hint button is pressed.
    /// </summary>
    public void HintButton()
    {
        if (ProvideHint())
        {
            Debug.Log("Hint provided.");
        }
        else
        {
            Debug.Log("Unable to provide a hint.");
        }
    }

    /// <summary>
    /// Sets the remaining hint count.
    /// </summary>
    /// <param name="hints">The number of remaining hints.</param>
    public void SetHints(int hints)
    {
        _remainingHints = hints;
        UpdateHintUI();
    }

    /// <summary>
    /// Gets the remaining hint count.
    /// </summary>
    /// <returns>The number of remaining hints.</returns>
    public int GetHints()
    {
        return _remainingHints;
    }
    #endregion

    #region UI Updates
    /// <summary>
    /// Updates the hint count displayed in the UI.
    /// </summary>
    private void UpdateHintUI()
    {
        _hintText.text = $"Hint: {_remainingHints}/{_maxHints}";
    }
    #endregion
}
