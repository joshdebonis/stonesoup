using UnityEngine;
using System;

public class RRandRoom : Room
{
    public TextAsset[] designedRoomFiles;
    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits) {
        requiredExits.addDirConstraint((Dir)UnityEngine.Random.Range(0, 4));
        if(UnityEngine.Random.Range(0f,1f)<0.5f){
            requiredExits.addDirConstraint((Dir)UnityEngine.Random.Range(0, 4));
        }
		string initialGridString = designedRoomFiles[UnityEngine.Random.Range(0, designedRoomFiles.Length)].text;
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
        ClearExitPaths(ref indexGrid, requiredExits);
		
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

            int steps = 1;

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