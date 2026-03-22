using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomPicker : Room
{
    public List<ValidatedRoom> RoomChoices;

    public override Room createRoom(ExitConstraint requiredExits)
    {
        // foreach (ValidatedRoom validatedRoom in RoomChoices)
        // {
        //     room.validation
        // }
        List<ValidatedRoom> validRooms = new List<ValidatedRoom>();

        foreach (ValidatedRoom room in RoomChoices)
        {
            room.ValidateExits();
            if (room.MeetsConstraints(requiredExits))
                validRooms.Add(room);
        }

        ValidatedRoom roomPrefab = GlobalFuncs.randElem(validRooms);
        return roomPrefab.GetComponent<Room>().createRoom(requiredExits);
    }
}