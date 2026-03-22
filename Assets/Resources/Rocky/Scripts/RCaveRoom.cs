using UnityEngine;
using System;
using System.Collections.Generic;

public class RCaveRoom : Room
{
    public int expandRadius = 1;
    public int thiefCount = 2, thiefIndex;
    public int shieldCount = 2, shieldIndex;
    public int botIndex;
    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits) {
        requiredExits.addDirConstraint((Dir)UnityEngine.Random.Range(0, 4));
        if(UnityEngine.Random.Range(0f,1f)<0.5f){
            requiredExits.addDirConstraint((Dir)UnityEngine.Random.Range(0, 4));
        }
		string initialGridString = designedRoomFile.text;
		string[] rows = initialGridString.Trim().Split('\n');
		int width = rows[0].Trim().Split(',').Length;
		int height = rows.Length;
		if (height != LevelGenerator.ROOM_HEIGHT) {
			throw new UnityException(string.Format("Error in room by {0}. Wrong height, Expected: {1}, Got: {2}", roomAuthor, LevelGenerator.ROOM_HEIGHT, height));
		}
		if (width != LevelGenerator.ROOM_WIDTH) {
			throw new UnityException(string.Format("Error in room by {0}. Wrong width, Expected: {1}, Got: {2}", roomAuthor, LevelGenerator.ROOM_WIDTH, width));
		}
		int[,] indexGrid = new int[width, height];
		for (int r = 0; r < height; r++) {
			string row = rows[height-r-1];
			string[] cols = row.Trim().Split(',');
			for (int c = 0; c < width; c++) {
				indexGrid[c, r] = int.Parse(cols[c]);
			}
		}
        GrowWall(ref indexGrid, requiredExits);
		
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				int tileIndex = indexGrid[i, j];
				if (tileIndex == 0) {
					continue; // 0 is nothing.
				}
				GameObject tileToSpawn;
				if (tileIndex < LevelGenerator.LOCAL_START_INDEX) {
					tileToSpawn = ourGenerator.globalTilePrefabs[tileIndex-1];
				}
				else {
					tileToSpawn = localTilePrefabs[tileIndex-LevelGenerator.LOCAL_START_INDEX];
				}
				Tile.spawnTile(tileToSpawn, transform, i, j);
			}
		}
    }
    void GrowWall(ref int[,] indexGrid, ExitConstraint requiredExits)
    {
        AssignRandomCells(ref indexGrid, UnityEngine.Random.Range(1,6), 1);
        int m = indexGrid.GetLength(0);
        int n = indexGrid.GetLength(1);

        var toExpand = new List<(int x, int y)>();

        // find all 1s that don't have a neighboring 1
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (indexGrid[i, j] != 1)
                    continue;

                bool hasNeighborOne = false;

                int[,] dirs = { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } };
                for (int d = 0; d < 4; d++)
                {
                    int ni = i + dirs[d, 0];
                    int nj = j + dirs[d, 1];

                    if (ni >= 0 && ni < m && nj >= 0 && nj < n)
                    {
                        if (indexGrid[ni, nj] == 1)
                        {
                            hasNeighborOne = true;
                            break;
                        }
                    }
                }

                if (!hasNeighborOne)
                {
                    for (int dx = -expandRadius; dx <= expandRadius; dx++)
                    {
                        for (int dy = -expandRadius; dy <= expandRadius; dy++)
                        {
                            int ni = i + dx;
                            int nj = j + dy;

                            if (ni < 0 || ni >= m || nj < 0 || nj >= n)
                                continue;

                            int val = indexGrid[ni, nj];
                            if (val == 0 || val == 1)
                            {
                                toExpand.Add((ni, nj));
                            }
                        }
                    }
                }
            }
        }

        // expand
        foreach (var (x, y) in toExpand)
        {
            indexGrid[x, y] = 1;
        }

        // spawn thieves and shields
        AssignRandomCells(ref indexGrid, thiefCount, thiefIndex);
        AssignRandomCells(ref indexGrid, shieldCount, shieldIndex);
        AssignRandomCells(ref indexGrid, 1, botIndex, false);

        ClearExitPaths(ref indexGrid, requiredExits);
    }

    void AssignRandomCells(ref int[,] grid, int spawnCount, int spawnValue, bool random=true)
    {
        if(random)
            spawnCount+=UnityEngine.Random.Range(-1,1);
        int m = grid.GetLength(0);
        int n = grid.GetLength(1);

        var zeroCells = new List<(int x, int y)>();
        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (grid[i, j] == 0)
                {
                    zeroCells.Add((i, j));
                }
            }
        }

        int count = Math.Min(spawnCount, zeroCells.Count);
        var rand = new System.Random();

        for (int i = 0; i < count; i++)
        {
            int r = rand.Next(zeroCells.Count);
            var (x, y) = zeroCells[r];

            grid[x, y] = spawnValue;
            zeroCells.RemoveAt(r);
        }
    }
    void ClearExitPaths(ref int[,] indexGrid, ExitConstraint exitConstraint)
    {
        int m = indexGrid.GetLength(0);
        int n = indexGrid.GetLength(1);

        float centerX = (m - 1) / 2f;
        float centerY = (n - 1) / 2f;

        void ClearLine(ref int[,] indexGrid, float startX, float startY)
        {
            float dx = centerX - startX;
            float dy = centerY - startY;

            float length = (float)Math.Sqrt(dx * dx + dy * dy);
            if (length == 0f)
                return;

            int steps = (int)Math.Ceiling(length);

            for (int i = 0; i <= steps; i++)
            {
                float t = i / (float)steps;

                float px = startX + dx * t;
                float py = startY + dy * t;

                int ix = (int)Math.Round(px);
                int iy = (int)Math.Round(py);

                if (ix >= 0 && ix < m && iy >= 0 && iy < n)
                {
                    if (indexGrid[ix, iy] == 1)
                    {
                        indexGrid[ix, iy] = 0;
                    }
                }
            }
        }

        // Up
        if (exitConstraint.upExitRequired)
        {
            float startX = centerX;
            float startY = n-1;
            ClearLine(ref indexGrid, startX, startY);
            indexGrid[(int)startX+1, (int)startY]=0;
        }

        // Down
        if (exitConstraint.downExitRequired)
        {
            float startX = centerX;
            float startY = 0;
            ClearLine(ref indexGrid, startX, startY);
            indexGrid[(int)startX+1, (int)startY]=0;
        }

        // Left
        if (exitConstraint.leftExitRequired)
        {
            float startX = 0;
            float startY = centerY;
            ClearLine(ref indexGrid, startX, startY);
            indexGrid[(int)startX, (int)startY]=0;
        }

        // Right
        if (exitConstraint.rightExitRequired)
        {
            float startX = m-1;
            float startY = centerY;
            ClearLine(ref indexGrid, startX, startY);
            indexGrid[(int)startX, (int)startY]=0;
        }
    }
}