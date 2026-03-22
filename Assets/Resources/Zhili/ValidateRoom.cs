using UnityEngine;
using System.Collections.Generic;

public class ValidatedRoom : Room
{
    public bool HasUpExit = false;
    public bool HasDownExit = false;
    public bool HasLeftExit = false;
    public bool HasRightExit = false;

    public bool HasUpToDown = false;
    public bool HasUpToLeft = false;
    public bool HasUpToRight = false;
    public bool HasDownToLeft = false;
    public bool HasDownToRight = false;
    public bool HasLeftToRight = false;
	int[,] _indexGrid = new int[LevelGenerator.ROOM_WIDTH, LevelGenerator.ROOM_HEIGHT];

    public class SearchVertex
    {
        private Vector2Int gridPos;
        
    }
	
	public void ValidateExits()
    {
        loadData();
        Vector2Int topExit = new Vector2Int(4, 0);
        Vector2Int downExit = new Vector2Int(4, LevelGenerator.ROOM_HEIGHT - 1);
        Vector2Int leftExit = new Vector2Int(0, 3);
        Vector2Int rightExit = new Vector2Int(LevelGenerator.ROOM_WIDTH - 1, 3);

        if (_indexGrid[topExit.x, topExit.y] == 0)
            HasUpExit = true;
        else
            HasUpExit = false;

        if (_indexGrid[downExit.x, downExit.y] == 0)
            HasDownExit = true;
        else
            HasDownExit = false;

        if (_indexGrid[leftExit.x, leftExit.y] == 0)
            HasLeftExit = true;
        else
            HasLeftExit = false;

        if (_indexGrid[rightExit.x, rightExit.y] == 0)
            HasRightExit = true;
        else
            HasRightExit = false;

        HasUpToDown = HasPath(topExit, downExit);
        HasUpToLeft = HasPath(topExit, leftExit);
        HasUpToRight = HasPath(topExit, rightExit);
        HasDownToLeft = HasPath(downExit, leftExit);
        HasDownToRight = HasPath(downExit, rightExit);
        HasLeftToRight = HasPath(leftExit, rightExit);

    }

    
    bool HasPath(Vector2Int start, Vector2Int target)
    {
        List<Vector2Int> openSet = new List<Vector2Int>();
        List<Vector2Int> closedSet = new List<Vector2Int>();

        openSet.Add(start);

        while (openSet.Count > 0)
        {
            Vector2Int currentNode = openSet[openSet.Count - 1];
            openSet.RemoveAt(openSet.Count - 1);
            closedSet.Add(currentNode);

            if (currentNode == target)
                return true;

            Vector2Int upNeighbor = new Vector2Int(currentNode.x, currentNode.y + 1);
            Vector2Int downNeighbor = new Vector2Int(currentNode.x, currentNode.y - 1);
            Vector2Int leftNeighbor = new Vector2Int(currentNode.x - 1, currentNode.y);
            Vector2Int rightNeighbor = new Vector2Int(currentNode.x + 1, currentNode.y);

            if (CheckNeighbor(upNeighbor, closedSet))
                openSet.Add(upNeighbor);
            if (CheckNeighbor(downNeighbor, closedSet))
                openSet.Add(downNeighbor);
            if (CheckNeighbor(leftNeighbor, closedSet))
                openSet.Add(leftNeighbor);
            if (CheckNeighbor(rightNeighbor, closedSet))
                openSet.Add(rightNeighbor);

        }

        return false;
    }
    

    bool CheckNeighbor(Vector2Int node, List<Vector2Int> closedSet)
    {
        return IsInBounds(node) && closedSet.Contains(node) == false && _indexGrid[node.x, node.y] == 0;
    }

    bool IsInBounds(Vector2Int node)
    {
        return node.x >= 0 && node.x < LevelGenerator.ROOM_WIDTH && node.y >= 0 && node.y < LevelGenerator.ROOM_HEIGHT;
    }


    public bool MeetsConstraints(ExitConstraint constraints)
    {
        if (constraints.upExitRequired && HasUpExit == false)
            return false;
        if (constraints.downExitRequired && HasDownExit == false)
            return false;
        if (constraints.leftExitRequired && HasLeftExit == false)
            return false;
        if (constraints.rightExitRequired && HasRightExit == false)
            return false;

        if (constraints.upExitRequired && constraints.downExitRequired && HasUpToDown == false)
            return false;
        if (constraints.upExitRequired && constraints.leftExitRequired && HasUpToLeft == false)
            return false;
        if (constraints.upExitRequired && constraints.rightExitRequired && HasUpToRight == false)
            return false;
        if (constraints.downExitRequired && constraints.leftExitRequired && HasDownToLeft == false)
            return false;
        if (constraints.downExitRequired && constraints.rightExitRequired && HasDownToRight == false)
            return false;
        if (constraints.leftExitRequired && constraints.rightExitRequired && HasLeftToRight == false)
            return false;

        return true;
    }

	public virtual void loadData()
	{

		string initialGridString = designedRoomFile.text;
		string[] rows = initialGridString.Trim().Split('\n');
		int width = rows[0].Trim().Split(',').Length;
		int height = rows.Length;
		if (height != LevelGenerator.ROOM_HEIGHT)
		{
			throw new UnityException(string.Format("Error in room by {0}. Wrong height, Expected: {1}, Got: {2}", roomAuthor, LevelGenerator.ROOM_HEIGHT, height));
		}
		if (width != LevelGenerator.ROOM_WIDTH)
		{
			throw new UnityException(string.Format("Error in room by {0}. Wrong width, Expected: {1}, Got: {2}", roomAuthor, LevelGenerator.ROOM_WIDTH, width));
		}
		_indexGrid = new int[width, height];
		for (int r = 0; r < height; r++)
		{
			string row = rows[height - r - 1];
			string[] cols = row.Trim().Split(',');
			for (int c = 0; c < width; c++)
			{
				_indexGrid[c, r] = int.Parse(cols[c]);
			}
		}
	}
}