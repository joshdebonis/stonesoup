using UnityEngine;

namespace Lam
{
    public class RoomTest : Room
    {
        [Range(0, 1)] public float[] Probabilities;

        public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
        {
            string initialGridString = designedRoomFile.text;
            string[] rows = initialGridString.Trim().Split('\n');
            int width = rows[0].Trim().Split(',').Length;
            int height = rows.Length;
            if (height != LevelGenerator.ROOM_HEIGHT)
            {
                throw new UnityException(string.Format("Error in room by {0}. Wrong height, Expected: {1}, Got: {2}",
                    roomAuthor, LevelGenerator.ROOM_HEIGHT, height));
            }

            if (width != LevelGenerator.ROOM_WIDTH)
            {
                throw new UnityException(string.Format("Error in room by {0}. Wrong width, Expected: {1}, Got: {2}",
                    roomAuthor, LevelGenerator.ROOM_WIDTH, width));
            }

            int[,] indexGrid = new int[width, height];
            for (int r = 0; r < height; r++)
            {
                string row = rows[height - r - 1];
                string[] cols = row.Trim().Split(',');
                for (int c = 0; c < width; c++)
                {
                    indexGrid[c, r] = int.Parse(cols[c]);
                }
            }

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int tileIndex = indexGrid[i, j];
                    
                    if (ShouldIgnoreForExit(requiredExits, i, j, width, height))
                        continue;

                    if (tileIndex == 0)
                    {
                        continue; // 0 is nothing.
                    }

                    GameObject tileToSpawn;
                    if (tileIndex < LevelGenerator.LOCAL_START_INDEX)
                    {
                        if (Random.value > Probabilities[tileIndex])
                        {
                            continue;
                            //tileToSpawn = ourGenerator.globalTilePrefabs[0];
                        }
                        else
                        {
                            tileToSpawn = ourGenerator.globalTilePrefabs[tileIndex - 1];
                        }
                    }
                    else
                    {
                        if (Random.value > Probabilities[tileIndex])
                        {
                            continue;
                            //tileToSpawn = ourGenerator.globalTilePrefabs[0];
                        }
                        else
                        {
                            tileToSpawn = localTilePrefabs[tileIndex - LevelGenerator.LOCAL_START_INDEX];
                        }
                    }

                    Tile.spawnTile(tileToSpawn, transform, i, j);
                }
            }
        }
        
        private bool ShouldIgnoreForExit(
            ExitConstraint exits,
            int x,
            int y,
            int width,
            int height)
        {
            int middleLeft = width / 2 - 1;
            int middleRight = width / 2;

            // 🔼 上出口
            if (exits.upExitRequired && y == height - 1)
            {
                if (x == middleLeft || x == middleRight)
                    return true;
            }

            // 🔽 下出口
            if (exits.downExitRequired && y == 0)
            {
                if (x == middleLeft || x == middleRight)
                    return true;
            }

            // ◀ 左出口
            if (exits.leftExitRequired && x == 0)
            {
                int middleBottom = height / 2 - 1;
                int middleTop = height / 2;

                if (y == middleBottom || y == middleTop)
                    return true;
            }

            // ▶ 右出口
            if (exits.rightExitRequired && x == width - 1)
            {
                int middleBottom = height / 2 - 1;
                int middleTop = height / 2;

                if (y == middleBottom || y == middleTop)
                    return true;
            }

            return false;
        }
    }
}