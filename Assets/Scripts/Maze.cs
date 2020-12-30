using System;
using System.Collections.Generic;
using System.Text;
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

    public Grid Grid;
    public int Column;
    public int Row;

    public readonly HashSet<Cell> Links = new HashSet<Cell>();

    public Cell(Grid grid, int column, int row)
    {
        Grid = grid;
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
            North = Maze.HasNorthCell(Row, Grid.Rows) ? Maze.GetNorthCell(Grid, Column, Row) : null,
            East = Maze.HasEastCell(Column, Grid.Columns) ? Maze.GetEastCell(Grid, Column, Row) : null,
            South = Maze.HasSouthCell(Row) ? Maze.GetSouthCell(Grid, Column, Row) : null,
            West = Maze.HasWestCell(Column) ? Maze.GetWestCell(Grid, Column, Row) : null
        };

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
    public static int GetCellIndex(int column, int row, int columns)
    {
        return row * columns + column;
    }

    public static Cell GetCell(Grid grid, int column, int row)
    {
        return grid.Cells[GetCellIndex(column, row, grid.Columns)];
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
            else
            {
                //Do Nothing
            }
        }
    }
}