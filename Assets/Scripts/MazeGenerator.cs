using System.Linq;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] Maze.GenerationAlgorithm _generationAlgorithm;
    [SerializeField] MazeTiles _mazeTiles;
    [SerializeField] Color _mazeColor;
    [SerializeField] int _columns = 20;
    [SerializeField] int _rows = 20;
    
    void Start()
    {
        Grid grid = new Grid(_columns, _rows);

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
        //var djikstraDistances = Maze.CalculateDistancesFromRoot(grid, grid.Cells[0]);
        //var path = Maze.CalculateLongestPath(grid);
        //var path = Maze.CalculatePath(grid, grid.Cells[0], grid.Cells[grid.Cells.Length - 1]);

        var middleCell = Maze.GetCell(grid, grid.Columns / 2, grid.Rows / 2);
        var distances = Maze.CalculateDistancesFromRoot(grid, middleCell);
        var maxDistance = distances.Values.Max();

        foreach (var cell in grid.Cells)
        {
            var tile = GetTilePrefab(cell);
            var position = new Vector3(cell.Column * 2, 0, cell.Row * 2);
            var mazeTile = Instantiate(tile, position, tile.transform.rotation);
            
            var tileMaterialPropertyBlock = new MaterialPropertyBlock();
            var distance = distances[cell];
            float h, s, v;
            Color.RGBToHSV(_mazeColor, out h, out s, out v);
            s = 1 - ((maxDistance - distance) / (float)maxDistance);
            
            tileMaterialPropertyBlock.SetColor("_Color", Color.HSVToRGB(h, s, v));
            // if (path.Contains(cell))
            // {
                 var renderer = mazeTile.GetComponent<MeshRenderer>();
                 renderer.SetPropertyBlock(tileMaterialPropertyBlock);
            // }
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