using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

public class Grid
{
    public readonly Cell[] Cells;
    public readonly int Columns;
    public readonly int Rows;

    public Grid(int columns, int rows)
    {
        Columns = columns;
        Rows = rows;

        Cells = new Cell[Columns * Rows];

        for (int col = 0; col < Columns; ++col)
        {
            for (int row = 0; row < Rows; ++row)
            {
                int index = Maze.GetCellIndex(col, row, Columns);
                Cells[index] = new Cell(this, col, row);
            }
        }
    }
}

public class Cell
{
    public struct Neighbours
    {
        public Cell North;
        public Cell East;
        public Cell South;
        public Cell West;
    }

    [Flags]
    public enum Walls
    {
        North = 0b0001,
        East = 0b0010,
        South = 0b0100,
        West = 0b1000
    }

    readonly Grid _grid;
    public int Column;
    public int Row;

    public readonly HashSet<Cell> Links = new HashSet<Cell>();

    public Cell(Grid grid, int column, int row)
    {
        _grid = grid;
        Column = column;
        Row = row;
    }

    public void Link(Cell cell, bool bidirectional)
    {
        Links.Add(cell);
        if (bidirectional)
        {
            cell.Link(this, false);
        }
    }

    public void Unlink(Cell cell, bool bidirectional)
    {
        Links.Remove(cell);
        if (bidirectional)
        {
            cell.Unlink(this, false);
        }
    }

    public Neighbours GetNeighbours()
    {
        var result = new Neighbours
        {
            North = Maze.HasNorthCell(Row, _grid.Rows) ? Maze.GetNorthCell(_grid, Column, Row) : null,
            East = Maze.HasEastCell(Column, _grid.Columns) ? Maze.GetEastCell(_grid, Column, Row) : null,
            South = Maze.HasSouthCell(Row) ? Maze.GetSouthCell(_grid, Column, Row) : null,
            West = Maze.HasWestCell(Column) ? Maze.GetWestCell(_grid, Column, Row) : null
        };

        return result;
    }

    public List<Cell> GetNeighboursList()
    {
        var result = new List<Cell>();
        if (Maze.HasNorthCell(Row, _grid.Rows)) result.Add(Maze.GetNorthCell(_grid, Column, Row));
        if (Maze.HasEastCell(Column, _grid.Columns)) result.Add(Maze.GetEastCell(_grid, Column, Row));
        if (Maze.HasSouthCell(Row)) result.Add(Maze.GetSouthCell(_grid, Column, Row));
        if (Maze.HasWestCell(Column)) result.Add(Maze.GetWestCell(_grid, Column, Row));

        return result;
    }

    public Walls GetWalls()
    {
        var result = new Walls();
        var neighbours = GetNeighbours();

        if (neighbours.North == null || !Links.Contains(neighbours.North))
        {
            result |= Walls.North;
        }

        if (neighbours.East == null || !Links.Contains(neighbours.East))
        {
            result |= Walls.East;
        }

        if (neighbours.South == null || !Links.Contains(neighbours.South))
        {
            result |= Walls.South;
        }

        if (neighbours.West == null || !Links.Contains(neighbours.West))
        {
            result |= Walls.West;
        }

        return result;
    }
}

public static class Maze
{
    public enum GenerationAlgorithm
    {
        BinaryTree,
        Sidewinder,
        AldousBroder,
        Wilson
    }

    public static int GetCellIndex(int column, int row, int columns)
    {
        return row * columns + column;
    }

    public static Cell GetCell(Grid grid, int column, int row)
    {
        return grid.Cells[GetCellIndex(column, row, grid.Columns)];
    }

    public static Cell GetRandomCell(Grid grid)
    {
        return GetCell(grid, Random.Range(0, grid.Columns), Random.Range(0, grid.Rows));
    }

    public static bool HasNorthCell(int row, int rows)
    {
        return row + 1 < rows;
    }

    public static Cell GetNorthCell(Grid grid, int column, int row)
    {
        return GetCell(grid, column, row + 1);
    }

    public static bool HasEastCell(int column, int columns)
    {
        return column + 1 < columns;
    }

    public static Cell GetEastCell(Grid grid, int column, int row)
    {
        return GetCell(grid, column + 1, row);
    }

    public static bool HasWestCell(int column)
    {
        return column - 1 >= 0;
    }

    public static Cell GetWestCell(Grid grid, int column, int row)
    {
        return GetCell(grid, column - 1, row);
    }

    public static bool HasSouthCell(int row)
    {
        return row - 1 >= 0;
    }

    public static Cell GetSouthCell(Grid grid, int column, int row)
    {
        return GetCell(grid, column, row - 1);
    }

    public static void GenerateBinaryTree(Grid grid)
    {
        foreach (var cell in grid.Cells)
        {
            bool hasNorthCell = HasNorthCell(cell.Row, grid.Rows);
            bool hasEastCell = HasEastCell(cell.Column, grid.Columns);

            if (hasNorthCell && hasEastCell)
            {
                if (Random.value > 0.5f)
                {
                    cell.Link(GetNorthCell(grid, cell.Column, cell.Row), true);
                }
                else
                {
                    cell.Link(GetEastCell(grid, cell.Column, cell.Row), true);
                }
            }
            else if (!hasNorthCell && hasEastCell)
            {
                cell.Link(GetEastCell(grid, cell.Column, cell.Row), true);
            }
            else if (hasNorthCell && !hasEastCell)
            {
                cell.Link(GetNorthCell(grid, cell.Column, cell.Row), true);
            }
        }
    }

    public static void GenerateSidewinder(Grid grid)
    {
        var run = new List<Cell>(grid.Columns);
        //For Each Row
        for (int i = 0; i < grid.Cells.Length; i += grid.Columns)
        {
            //For each cell in a row
            for (int c = i; c < i + grid.Columns; ++c)
            {
                var cell = grid.Cells[c];
                run.Add(cell);

                bool hasNorthCell = HasNorthCell(cell.Row, grid.Rows);
                bool hasEastCell = HasEastCell(cell.Column, grid.Columns);
                bool shouldCloseOut = !hasEastCell || (hasNorthCell && Random.value > 0.5f);

                if (shouldCloseOut)
                {
                    var member = SampleUtil<Cell>.Sample(run);
                    if (HasNorthCell(member.Row, grid.Rows))
                        member.Link(GetNorthCell(grid, member.Column, member.Row), true);
                    run.Clear();
                }
                else
                {
                    cell.Link(GetEastCell(grid, cell.Column, cell.Row), true);
                }
            }
        }
    }

    public static void GenerateAldousBroder(Grid grid)
    {
        var cell = GetRandomCell(grid);
        int unvisitied = grid.Cells.Length - 1;

        while (unvisitied > 0)
        {
            var neighbours = cell.GetNeighboursList();
            var neighbour = SampleUtil<Cell>.Sample(neighbours);

            if (neighbour.Links.Count == 0)
            {
                cell.Link(neighbour, true);
                unvisitied -= 1;
            }

            cell = neighbour;
        }
    }

    public static void GenerateWilson(Grid grid)
    {
        var unvisitedCells = new List<Cell>(grid.Cells);

        var firstCell = SampleUtil<Cell>.Sample(unvisitedCells);
        unvisitedCells.Remove(firstCell);

        while (unvisitedCells.Count > 0)
        {
            var cell = SampleUtil<Cell>.Sample(unvisitedCells);
            List<Cell> path = new List<Cell> {cell};

            while (unvisitedCells.Contains(cell))
            {
                cell = SampleUtil<Cell>.Sample(cell.GetNeighboursList());
                var cellIndex = path.IndexOf(cell);

                if (cellIndex == -1)
                    path.Add(cell);
                else
                    path = path.GetRange(0, cellIndex + 1);
            }

            for (int i = 0; i < path.Count - 1; ++i)
            {
                path[i].Link(path[i + 1], true);
                unvisitedCells.Remove(path[i]);
            }
        }
    }

    public static Dictionary<Cell, int> CalculateDistancesFromRoot(Grid grid, Cell root)
    {
        var result = new Dictionary<Cell, int>(grid.Cells.Length);
        var frontier = new HashSet<Cell> {root};

        result.Add(root, 0);
        while (frontier.Count > 0)
        {
            var newFrontier = new HashSet<Cell>();
            foreach (Cell cell in frontier)
            {
                foreach (Cell link in cell.Links)
                {
                    if (result.ContainsKey(link)) continue;

                    result.Add(link, result[cell] + 1);
                    newFrontier.Add(link);
                }
            }

            frontier = newFrontier;
        }

        return result;
    }

    public static List<Cell> CalculatePath(Grid grid, Cell root, Cell goal)
    {
        var result = new List<Cell>();
        var current = goal;
        result.Add(current);

        var distances = CalculateDistancesFromRoot(grid, root);

        while (current != root)
        {
            foreach (var link in current.Links)
            {
                if (distances[link] < distances[current])
                {
                    result.Add(link);
                    current = link;
                    break;
                }
            }
        }

        result.Reverse();
        return result;
    }

    public static List<Cell> CalculateLongestPath(Grid grid)
    {
        var distances = CalculateDistancesFromRoot(grid, grid.Cells[0]);
        var maxDistanceCell = distances.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

        distances.Clear();
        distances = CalculateDistancesFromRoot(grid, maxDistanceCell);
        var newMaxDistanceCell = distances.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

        return CalculatePath(grid, maxDistanceCell, newMaxDistanceCell);
    }
}