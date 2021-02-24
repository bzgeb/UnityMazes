using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Mazes
{
    public class MazeGenerator : MonoBehaviour
    {
        [SerializeField] Maze.GenerationAlgorithm _generationAlgorithm;
        [SerializeField] MazeTiles _mazeTiles;
        [SerializeField] Color _mazeColor;
        [SerializeField] int _columns = 20;
        [SerializeField] int _rows = 20;
        [SerializeField] TextAsset _asciiMask;
        bool _hasMask;

        readonly List<MazeTile> _liveMazeTiles = new List<MazeTile>();

        void Start()
        {
            CalculateAverageDeadEnds(1);

            _hasMask = _asciiMask != null;

            Mask mask = null;
            if (_hasMask)
            {
                StringReader stringReader = new StringReader(_asciiMask.text);
                mask = new Mask(stringReader);
                _columns = mask.Columns;
                _rows = mask.Rows;
            }

            var grid = new Grid(_columns, _rows, mask);

            switch (_generationAlgorithm)
            {
                case Maze.GenerationAlgorithm.BinaryTree:
                    Maze.GenerateBinaryTree(grid);
                    break;
                case Maze.GenerationAlgorithm.Sidewinder:
                    Maze.GenerateSidewinder(grid);
                    break;
                case Maze.GenerationAlgorithm.AldousBroder:
                    Maze.GenerateAldousBroder(grid);
                    break;
                case Maze.GenerationAlgorithm.Wilson:
                    Maze.GenerateWilson(grid);
                    break;
                case Maze.GenerationAlgorithm.HuntAndKill:
                    Maze.GenerateHuntAndKill(grid);
                    break;
                case Maze.GenerationAlgorithm.RecursiveBacktracker:
                    Maze.GenerateRecursiveBacktracker(grid);
                    break;
            }

            InstantiateMazeTiles(grid, true);
        }

        public void InstantiateMazeTiles(Grid grid, bool colorByDistance)
        {
            for (int i = _liveMazeTiles.Count - 1; i >= 0; --i)
            {
                Destroy(_liveMazeTiles[i].gameObject);
            }

            _liveMazeTiles.Clear();
            //var djikstraDistances = Maze.CalculateDistancesFromRoot(grid, grid.Cells[0]);
            //var path = Maze.CalculateLongestPath(grid);
            //var path = Maze.CalculatePath(grid, grid.Cells[0], grid.Cells[grid.Cells.Length - 1]);

            var middleCell = grid.GetCell(grid.Columns / 2, grid.Rows / 2);
            var distances = Maze.CalculateDistancesFromRoot(grid, middleCell);
            var maxDistance = distances.Values.Max();

            foreach (var cell in grid.Cells)
            {
                if (cell == null) continue;

                MazeTile tile = GetTilePrefab(cell);
                Vector3 position = new Vector3(cell.Column * 2, 0, cell.Row * 2);
                MazeTile mazeTile = Instantiate(tile, position, tile.transform.rotation);
                _liveMazeTiles.Add(mazeTile);

                if (colorByDistance)
                {
                    var tileMaterialPropertyBlock = new MaterialPropertyBlock();
                    var distance = distances[cell];
                    float h, s, v;
                    Color.RGBToHSV(_mazeColor, out h, out s, out v);
                    s = 1 - ((maxDistance - distance) / (float) maxDistance);

                    tileMaterialPropertyBlock.SetColor("_Color", Color.HSVToRGB(h, s, v));

                    var tileRenderer = mazeTile.GetComponent<MeshRenderer>();
                    tileRenderer.SetPropertyBlock(tileMaterialPropertyBlock);
                }
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

        void CalculateAverageDeadEnds(int tries)
        {
            var algorithms = new List<Maze.GenerateMaze>
            {
                Maze.GenerateBinaryTree,
                Maze.GenerateSidewinder,
                Maze.GenerateAldousBroder,
                Maze.GenerateWilson,
                Maze.GenerateHuntAndKill,
                Maze.GenerateRecursiveBacktracker
            };

            StringBuilder result = new StringBuilder();

            foreach (var algorithm in algorithms)
            {
                int averageDeadEnds = 0;
                for (int i = 0; i < tries; ++i)
                {
                    var grid = new Grid(_columns, _rows);
                    algorithm(grid);

                    var deadEnds = Maze.CalculateDeadEnds(grid);
                    averageDeadEnds += deadEnds.Count;
                }

                averageDeadEnds /= tries;
                result.Append($"{algorithm.Method.Name}: {averageDeadEnds}\n");
            }

            Debug.Log(result.ToString());
        }
    }
}