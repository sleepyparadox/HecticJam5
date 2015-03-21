using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HecticUFO
{
    public class Cell
    {
        List<Brush> Set = new List<Brush>();
        public void Apply(Brush brush)
        {
            if (Set.Contains(brush))
                return;
            else
                Set.Add(brush);
        }
        
        public void Clear(Brush brush)
        {
            if (!Set.Contains(brush))
                return;
            else
                Set.Remove(brush);
        }

        public bool Contains(Brush brush)
        {
            return Set.Contains(brush);
        }

        public static Cell[,] ParseCells(string csv, out int mapSize)
        {
            Cell[,] parsed = null;
            var lines = csv.Split('\n');
            mapSize = 0;
            for (var i = 0; i < lines.Length; ++i)
            {
                var cells = lines[i].Split(',');
                if (i == 0)
                {
                    mapSize = int.Parse(cells[1]);
                    parsed = new Cell[mapSize, mapSize];
                    for (var x = 0; x < mapSize; ++x)
                    {
                        for (var y = 0; y < mapSize; ++y)
                        {
                            parsed[x, y] = new Cell();
                        }
                    }
                }
                else
                {
                    var y = i - 1;
                    for (var x = 0; x < cells.Length && x < mapSize && y < mapSize; ++x)
                    {
                        if (string.IsNullOrEmpty(cells[x]))
                            continue;
                        var vals = cells[x].Contains(" ") ? cells[x].Split(' ') : new string[] { cells[x] };
                        var cell = parsed[x, y];
                        foreach (var val in vals)
                        {
                            if (string.IsNullOrEmpty(val))
                                continue;
                            try
                            {
                                cell.Apply((Brush)Enum.Parse(typeof(Brush), val));
                            }
                            catch (Exception e)
                            {
                                //Debug.LogWarning(e.ToString());
                            }
                        }
                    }
                }
            }
            return parsed;
        }

        internal string ToCsvCell()
        {
            var s = "";
            foreach (var brush in Set)
                s += brush.ToString() + " ";
            s += ",";

            if (Set.Count > 1)
                Debug.Log("Cell has " + Set.Count + " brushes, writing \"" + s + "\"");

            return s;
        }
    }
}
