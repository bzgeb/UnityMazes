using UnityEngine;

namespace Mazes
{
    public class Grid
    {
        public readonly Cell[,] Cells;
        public readonly int Columns;
        public readonly int Rows;

        public readonly Mask Mask;
        public readonly bool HasMask;

        public Grid(int columns, int rows, Mask mask = null)
        {
            Columns = columns;
            Rows = rows;
            Mask = mask;

            Cells = new Cell[Columns, Rows];
            HasMask = mask != null;
            PrepareGrid(this, mask);
        }

        void PrepareGrid(Grid grid, Mask mask)
        {
            for (int col = 0; col < grid.Columns; ++col)
            {
                for (int row = 0; row < grid.Rows; ++row)
                {
                    if (grid.HasMask)
                    {
                        if (mask.Cells[col, row])
                        {
                            grid.Cells[col, row] = new Cell(grid, col, row);
                        }
                    }
                    else
                    {
                        grid.Cells[col, row] = new Cell(grid, col, row);
                    }
                }
            }
        }

        public Cell GetCell(int column, int row)
        {
            return Cells[column, row];
        }

        public Cell GetRandomCell()
        {
            if (HasMask)
            {
                Vector2Int pos = Mask.GetRandomLocation(Mask);
                return Cells[pos.x, pos.y];
            }
            else
            {
                return GetCell(Random.Range(0, Columns), Random.Range(0, Rows));
            }
        }
    }
}