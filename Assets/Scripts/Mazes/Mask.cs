using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

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

        public Mask(Texture2D textureMask)
        {
            Columns = textureMask.width;
            Rows = textureMask.height;
            Cells = new bool[Columns, Rows];
            for (int col = 0; col < Columns; ++col)
            {
                for (int row = 0; row < Rows; ++row)
                {
                    Cells[col, row] = true;
                }
            }

            for (int col = 0; col < Columns; ++col)
            {
                for (int row = 0; row < Rows; ++row)
                {
                    if (textureMask.GetPixel(col, row) == Color.black)
                    {
                        Cells[col, row] = false;
                    }
                }
            }
        }

        public Mask(StringReader stringReader)
        {
            // Collect the masked cells
            string line = "";
            int row = 0;
            int col = 0;
            List<Vector2Int> maskedCells = new List<Vector2Int>();
            while ((line = stringReader.ReadLine()) != null)
            {
                col = 0;
                foreach (var character in line)
                {
                    switch (Char.ToLower(character))
                    {
                        case '.':
                            ++col;
                            break;
                        case 'x':
                            maskedCells.Add(new Vector2Int(col, row));
                            ++col;
                            break;
                    }
                }

                ++row;
            }

            // Initialize the Cells
            Columns = col;
            Rows = row;
            Cells = new bool[Columns, Rows];
            for (col = 0; col < Columns; ++col)
            {
                for (row = 0; row < Rows; ++row)
                {
                    Cells[col, row] = true;
                }
            }

            // Mask the Cells
            foreach (var maskedCell in maskedCells)
            {
                Cells[maskedCell.x, Rows - maskedCell.y] = false;
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