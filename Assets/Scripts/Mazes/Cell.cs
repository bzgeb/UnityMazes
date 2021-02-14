using System;
using System.Collections.Generic;

namespace Mazes
{
    public class Cell
    {
        readonly Grid _grid;
        public int Column;
        public int Row;
        public readonly HashSet<Cell> Links = new HashSet<Cell>();

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
                North = Maze.HasNorthCell(_grid, Column, Row) ? Maze.GetNorthCell(_grid, Column, Row) : null,
                East = Maze.HasEastCell(_grid, Column, Row) ? Maze.GetEastCell(_grid, Column, Row) : null,
                South = Maze.HasSouthCell(_grid, Column, Row) ? Maze.GetSouthCell(_grid, Column, Row) : null,
                West = Maze.HasWestCell(_grid, Column, Row) ? Maze.GetWestCell(_grid, Column, Row) : null
            };

            return result;
        }

        public List<Cell> GetNeighboursList()
        {
            var result = new List<Cell>();
            if (Maze.HasNorthCell(_grid, Column, Row)) result.Add(Maze.GetNorthCell(_grid, Column, Row));
            if (Maze.HasEastCell(_grid, Column, Row)) result.Add(Maze.GetEastCell(_grid, Column, Row));
            if (Maze.HasSouthCell(_grid, Column, Row)) result.Add(Maze.GetSouthCell(_grid, Column, Row));
            if (Maze.HasWestCell(_grid, Column, Row)) result.Add(Maze.GetWestCell(_grid, Column, Row));

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
}