using TMPro;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public enum CellState
    {
        Locked,
        Unlocked,
        Incorrect
    }

    [HideInInspector] public int Value;
    [HideInInspector] public int Row;
    [HideInInspector] public int Col;
    [HideInInspector] public CellState State;

    [SerializeField] private SpriteRenderer _backgroundSprite;
    [SerializeField] private TMP_Text _valueText;

    [Header("Color Configurations")]
    [SerializeField] private CellColorConfig _startUnlockedColor;
    [SerializeField] private CellColorConfig _startLockedColor;
    [SerializeField] private CellColorConfig _highlightLockedColor;
    [SerializeField] private CellColorConfig _highlightUnlockedColor;
    [SerializeField] private CellColorConfig _highlightWrongColor;
    [SerializeField] private CellColorConfig _selectedColor;
    [SerializeField] private CellColorConfig _selectedWrongColor;
    [SerializeField] private CellColorConfig _resetColor;
    [SerializeField] private CellColorConfig _resetWrongColor;
    [SerializeField] private CellColorConfig _hintColor;


    private bool _isHighlighted;

    public delegate void CellClicked(int value);
    public static event CellClicked OnCellClicked;

    /// <summary>
    /// Initializes the cell with a value, row, and column.
    /// </summary>
    /// <param name="value">The value to initialize the cell with. A value of 0 indicates an unlocked cell.</param>
    /// <param name="row">The row index of the cell in the grid.</param>
    /// <param name="col">The column index of the cell in the grid.</param>
    public void Init(int value, int row, int col)
    {
        Value = value;
        Row = row;
        Col = col;

        _isHighlighted = false;

        if (value == 0)
        {
            State = CellState.Unlocked;
            SetCellAppearance(_startUnlockedColor);
            _valueText.text = string.Empty;
        }
        else
        {
            State = CellState.Locked;
            SetCellAppearance(_startLockedColor);
            _valueText.text = Value.ToString();
        }
    }


    private void OnMouseDown()
    {
        if (Value != 0)
        {
            OnCellClicked?.Invoke(Value);
        }
    }

    /// <summary>
    /// Highlights the cell.
    /// </summary>
    public void Highlight()
    {
        if (State == CellState.Locked)
        {
            SetCellAppearance(_highlightLockedColor);
        }
        else if (State == CellState.Incorrect)
        {
            SetCellAppearance(_highlightWrongColor);
        }
        else
        {
            SetCellAppearance(_highlightUnlockedColor);
        }

        _isHighlighted = true;
    }

    /// <summary>
    /// Selects the cell.
    /// </summary>
    public void Select()
    {
        if (State == CellState.Incorrect)
        {
            SetCellAppearance(_selectedWrongColor);
        }
        else
        {
            SetCellAppearance(_selectedColor);
        }
    }


    /// <summary>
    /// Resets the cell to its original state.
    /// </summary>
    public void Reset()
    {
        if (State == CellState.Locked)
        {
            SetCellAppearance(_startLockedColor);
        }
        else if (State == CellState.Incorrect)
        {
            SetCellAppearance(_resetWrongColor);
        }
        else
        {
            SetCellAppearance(_resetColor);
        }

        _isHighlighted = false;
    }

    /// <summary>
    /// Updates the value of the cell.
    /// </summary>
    /// <param name="value">The new value of the cell.</param>
    public void UpdateValue(int value)
    {
        Value = value;
        _valueText.text = Value == 0 ? "" : Value.ToString();
    }

    /// <summary>
    /// Updates the cell to a win state.
    /// </summary>
    public void UpdateWin()
    {
        SetCellAppearance(_startLockedColor);
    }

    /// <summary>
    /// Highlights the cell as a hint.
    /// </summary>
    /// <param name="value">The value to display as a hint.</param>
    public void HighlightHint(int value)
    {
        SetCellAppearance(_hintColor);
        UpdateValue(value);
        Invoke(nameof(ClearHighlight), 2f);
    }

    private void ClearHighlight()
    {
        Reset();
    }

    /// <summary>
    /// Checks if the cell is highlighted.
    /// </summary>
    public bool IsHighlighted()
    {
        return _isHighlighted;
    }

    /// <summary>
    /// Sets the highlighted state of the cell.
    /// </summary>
    /// <param name="highlight">Whether the cell should be highlighted.</param>
    public void SetHighlighted(bool highlight)
    {
        _isHighlighted = highlight;

        if (highlight)
        {
            Highlight();
        }
        else
        {
            Reset();
        }
    }

    /// <summary>
    /// Sets the appearance of the cell based on the provided color configuration.
    /// </summary>
    /// <param name="colorConfig">The color configuration to apply.</param>
    private void SetCellAppearance(CellColorConfig colorConfig)
    {
        _backgroundSprite.color = colorConfig.cellColor;
        _valueText.color = colorConfig.textColor;
    }
}

[System.Serializable]
public class CellColorConfig
{
    public Color cellColor;
    public Color textColor;
}