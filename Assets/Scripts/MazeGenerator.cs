using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public MazeTile TileNoWalls;
    public MazeTile TileAllWalls;

    public MazeTile TileOneWallN;
    public MazeTile TileOneWallE;
    public MazeTile TileOneWallS;
    public MazeTile TileOneWallW;

    public MazeTile TileTwoWallCornerES;
    public MazeTile TileTwoWallCornerNE;
    public MazeTile TileTwoWallCornerSW;
    public MazeTile TileTwoWallCornerWN;

    public MazeTile TileTwoWallHallEW;
    public MazeTile TileTwoWallHallNS;

    public MazeTile TileThreeWallESW;
    public MazeTile TileThreeWallNES;
    public MazeTile TileThreeWallSWN;
    public MazeTile TileThreeWallWNE;

    void Start()
    {
        Grid grid = new Grid(10, 10);
        Maze.GenerateBinaryTree(grid);

        int i = 0;
        foreach (var cell in grid.Cells)
        {
            var tile = GetTilePrefab(cell);
            var position = new Vector3(cell.Column * 2, 0, cell.Row * 2);
            Instantiate(tile, position, tile.transform.rotation);
        }
    }

    MazeTile GetTilePrefab(Cell cell)
    {
        MazeTile result = TileNoWalls;
        
        var walls = cell.GetWalls();
        switch (walls)
        {
            case (Cell.Walls) 0b0001:
                result = TileOneWallN;
                break;
            case (Cell.Walls) 0b0010:
                result = TileOneWallE;
                break;
            case (Cell.Walls) 0b0011:
                result = TileTwoWallCornerNE;
                break;
            case (Cell.Walls) 0b0100:
                result = TileOneWallS;
                break;
            case (Cell.Walls) 0b0101:
                result = TileTwoWallHallNS;
                break;
            case (Cell.Walls) 0b0110:
                result = TileTwoWallCornerES;
                break;
            case (Cell.Walls) 0b0111:
                result = TileThreeWallNES;
                break;
            case (Cell.Walls) 0b1000:
                result = TileOneWallW;
                break;
            case (Cell.Walls) 0b1001:
                result = TileTwoWallCornerWN;
                break;
            case (Cell.Walls) 0b1010:
                result = TileTwoWallHallEW;
                break;
            case (Cell.Walls) 0b1011:
                result = TileThreeWallWNE;
                break;
            case (Cell.Walls) 0b1100:
                result = TileTwoWallCornerSW;
                break;
            case (Cell.Walls) 0b1101:
                result = TileThreeWallSWN;
                break;
            case (Cell.Walls) 0b1110:
                result = TileThreeWallESW;
                break;
            case (Cell.Walls) 0b1111:
                result = TileAllWalls;
                break;
        }

        return result;
    }
}