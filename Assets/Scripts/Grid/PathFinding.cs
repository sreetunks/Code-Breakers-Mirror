using System;
using System.Collections.Generic;
using UnityEngine;

namespace Grid
{
    public class PathFinding : MonoBehaviour
    {
        public static void GetPath(GridPosition start, GridPosition target, ref List<GridPosition> outPath)
        {
            outPath.Clear();
            GridSystem.TryGetGridCellState(target, out var targetCellState);
            
            if (targetCellState is GridCellState.Impassable or GridCellState.Occupied) return; //Prevents walking to a location that is impassable or occupied

            //Do-While Control Var
            var count = 0; 

            //Initial Setup Vars to be used in loop as to maintain copies for backup
            var position = start;
            var lastPos = position;
            
            do
            {
                var posS = position;
                var minDistance = 10000f;
                for(var i = 1; i <= 4; i++)
                {
                    var j = 0; 
                    var k = 0;
                    switch (i)
                    {
                        case 1: j=1; k=0; break;
                        case 2: j=0; k=1; break;
                        case 3: j=-1; k=0; break;
                        case 4: j=0; k=-1; break;
                    }
                    var gp = new GridPosition(posS.X + j, posS.Z + k); // Get the GridPosition
                    if (gp == lastPos) continue;
                    GridSystem.TryGetGridCellState(gp, out var cellState);    //Get the state of that GridPosition
                    if (cellState is not (GridCellState.Impassable or GridCellState.Occupied))
                        if (minDistance > GridSystem.GetDistance(target, gp))
                        {
                            position = gp;
                            minDistance = GridSystem.GetDistance(target, gp);
                        }
                }

                if (minDistance != 10000f)
                {
                    outPath.Add(position);
                    lastPos = posS;
                }
                else
                {
                    (lastPos, position) = (position, lastPos);
                    outPath.RemoveAt(outPath.Count - 1);
                }

                if (count++ < 10) continue;
            #if UNITY_EDITOR
                print("Reached max iterations");
            #endif
                break;

            } while (position != target && count < 10);
        }
    }
}
