using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPGenerator : MonoBehaviour
{
    [System.Serializable]
    public struct Room
    {
        public string name;
        public int leftX;
        public int botY;
        public int rightX;
        public int topY;

        public int divides;

        public Room(int _leftX, int _botY, int _rightX, int _topY, int _divides)
        {
            leftX = _leftX;
            botY = _botY;
            rightX = _rightX;
            topY = _topY;
            divides = _divides;
            name = "(" + leftX + "," + botY + ") , (" + rightX + "," + topY + ")";
        }

        public int Width()
        {
            return rightX - leftX;
        }

        public int Height()
        {
            return topY - botY;
        }

        public float MidWidth()
        {
            return (float)(rightX - leftX) / 2;
        }
        public float MidHeight()
        {
            return (float)(topY - botY) / 2;
        }
    }


    public int iniSizeX;
    public int iniSizeY;
    public int minRoomSizeX;
    public int minRoomSizeY;

    [SerializeField] private List<Room> rooms;

    [ContextMenu("Start")]
    void Start()
    {
        rooms = new List<Room>();
        Room room = new Room(0, 0, iniSizeX, iniSizeY, 0);
        Divide(room);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Room room in rooms)
        {
            Gizmos.DrawLine(new Vector3(room.leftX, room.topY), new Vector3(room.rightX, room.topY));
            Gizmos.DrawLine(new Vector3(room.rightX, room.topY), new Vector3(room.rightX, room.botY));
        }
        Gizmos.DrawLine(new Vector3(0, iniSizeY), new Vector3(0, 0));
        Gizmos.DrawLine(new Vector3(0, 0), new Vector3(iniSizeX, 0));
    }



    private bool CanDivide(Room room)
    {
        if (room.divides < 4 && room.Height() > 2 * minRoomSizeY + 2 && room.Width() > 2 * minRoomSizeX + 2)
        {
            return true;
        }
        return false;
    }

    private void Divide(Room room)
    {
        if (CanDivide(room))
        {
            if (room.divides % 2 == 0)
            {
                int slice = Random.Range(room.leftX + minRoomSizeX, room.rightX - minRoomSizeX);
                Room subroom1 = new Room(room.leftX, room.botY, slice, room.topY, room.divides + 1);
                Room subroom2 = new Room(slice, room.botY, room.rightX, room.topY, room.divides + 1);
                Divide(subroom1);
                Divide(subroom2);
            }
            else
            {
                int slice = Random.Range(room.botY + minRoomSizeY, room.topY - minRoomSizeY);
                Room subroom1 = new Room(room.leftX, room.botY, room.rightX, slice, room.divides + 1);
                Room subroom2 = new Room(room.leftX, slice, room.rightX, room.topY, room.divides + 1);
                Divide(subroom1);
                Divide(subroom2);
            }
        }
        else
        {
            rooms.Add(room);
        }
    }
}
