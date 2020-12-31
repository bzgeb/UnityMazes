using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] Maze.GenerationAlgorithm _generationAlgorithm;
    [SerializeField] MazeTiles _mazeTiles;
    
    void Start()
    {
        Grid grid = new Grid(10, 10);
        
        switch (_generationAlgorithm)
        {
            case Maze.GenerationAlgorithm.BinaryTree:
                Maze.GenerateBinaryTree(grid);
                break;
            case Maze.GenerationAlgorithm.Sidewinder:
                Maze.GenerateSidewinder(grid);
                break;
        }

        InstantiateMazeTiles(grid);
    }

    void InstantiateMazeTiles(Grid grid)
    {
        foreach (var cell in grid.Cells)
        {
            var tile = GetTilePrefab(cell);
            var position = new Vector3(cell.Column * 2, 0, cell.Row * 2);
            Instantiate(tile, position, tile.transform.rotation);
        }
    }

    MazeTile GetTilePrefab(Cell cell)
    {
        MazeTile result = _mazeTiles.TileNoWalls;

        var walls = cell.GetWalls();
        switch (walls)
        {
            case (Cell.Walls) 0b0001:
                result = _mazeTiles.TileOneWallN;
                break;
            case (Cell.Walls) 0b0010:
                result = _mazeTiles.TileOneWallE;
                break;
            case (Cell.Walls) 0b0011:
                result = _mazeTiles.TileTwoWallCornerNE;
                break;
            case (Cell.Walls) 0b0100:
                result = _mazeTiles.TileOneWallS;
                break;
            case (Cell.Walls) 0b0101:
                result = _mazeTiles.TileTwoWallHallNS;
                break;
            case (Cell.Walls) 0b0110:
                result = _mazeTiles.TileTwoWallCornerES;
                break;
            case (Cell.Walls) 0b0111:
                result = _mazeTiles.TileThreeWallNES;
                break;
            case (Cell.Walls) 0b1000:
                result = _mazeTiles.TileOneWallW;
                break;
            case (Cell.Walls) 0b1001:
                result = _mazeTiles.TileTwoWallCornerWN;
                break;
            case (Cell.Walls) 0b1010:
                result = _mazeTiles.TileTwoWallHallEW;
                break;
            case (Cell.Walls) 0b1011:
                result = _mazeTiles.TileThreeWallWNE;
                break;
            case (Cell.Walls) 0b1100:
                result = _mazeTiles.TileTwoWallCornerSW;
                break;
            case (Cell.Walls) 0b1101:
                result = _mazeTiles.TileThreeWallSWN;
                break;
            case (Cell.Walls) 0b1110:
                result = _mazeTiles.TileThreeWallESW;
                break;
            case (Cell.Walls) 0b1111:
                result = _mazeTiles.TileAllWalls;
                break;
        }

        return result;
    }
}