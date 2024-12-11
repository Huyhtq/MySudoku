using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private SubGrid _subGridPrefab;
    [SerializeField] private Transform gridParent;

    [SerializeField] private float xOffset = 2.0f;
    [SerializeField] private float yOffset = 2.0f;
    [SerializeField] private float spawnX = 0f;
    [SerializeField] private float spawnY = 0f;

    private Cell[,] cells;
    private const int GridSize = 9;

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
}
