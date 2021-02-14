using UnityEditor;
using UnityEngine;

namespace Mazes
{
    public class Mask
    {
        public readonly int Columns;
        public readonly int Rows;
        public readonly bool[,] Cells;

        public Mask(int columns, int rows)
        {
            Columns = columns;
            Rows = rows;

            Cells = new bool[Columns, Rows];

            for (int col = 0; col < Columns; ++col)
            {
                for (int row = 0; row < Rows; ++row)
                {
                    Cells[col, row] = true;
                }
            }
        }

        public void SetCell(int col, int row, bool isOn)
        {
            Cells[col, row] = isOn;
        }

        public static Vector2Int GetRandomLocation(Mask mask)
        {
            if (mask.Count() == 0) return new Vector2Int(-1, -1);
                
            int randomColumn = Random.Range(0, mask.Columns);
            int randomRow = Random.Range(0, mask.Rows);
            while (mask.Cells[randomColumn, randomRow] == false)
            {
                randomColumn = Random.Range(0, mask.Columns);
                randomRow = Random.Range(0, mask.Rows);
            }

            return new Vector2Int(randomColumn, randomRow);
        }

        public int Count()
        {
            int count = 0;
            foreach (bool cell in Cells)
            {
                if (cell) ++count;
            }

            return count;
        }
    }
}