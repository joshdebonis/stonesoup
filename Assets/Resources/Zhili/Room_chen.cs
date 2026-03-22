using UnityEngine;

// A room that is generated purely from a designed text file.
// (i.e., uses Room.fillRoom)
public class Room_chen : Room
{
    public override void fillRoom(LevelGenerator ourGenerator, ExitConstraint requiredExits)
    {
        base.fillRoom(ourGenerator, requiredExits);
    }
}