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
    public int maxDivides;

    [SerializeField] private List<Room> rooms;

    public int doorSize;
    public int fruitMargin;

    public GameObject VerticalPrefab;
    public GameObject HorizontalPrefab;

    public GameObject WallParent;

    [ContextMenu("Start")]
    void OnEnable()
    {
        rooms = new List<Room>();
        Room room = new Room(0, 0, iniSizeX, iniSizeY, 0);
        Divide(room);
        Spriting();
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
        if (room.divides < maxDivides && room.Height() > 2 * minRoomSizeY + 2 && room.Width() > 2 * minRoomSizeX + 2)
        {
            return true;
        }
        return false;
    }

    private void Divide(Room room)
    {
        if (CanDivide(room))
        {
            if (room.divides == 0)
            {
                int slice = (int)room.MidWidth();
                Room subroom1 = new Room(room.leftX, room.botY, slice, room.topY, room.divides + 1);
                Room subroom2 = new Room(slice, room.botY, room.rightX, room.topY, room.divides + 1);
                Divide(subroom1);
                Divide(subroom2);
            }
            else if (room.divides % 2 == 0)
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

    private void Spriting()
    {
        foreach (Room rom in rooms)
        {
            if (rom.topY < iniSizeY)
            {
                Room above = RoomAbove(rom);
                if (above.rightX != -1)
                {
                    Vector2 mid = GetMidPoint(rom, above);
                    for (int i = rom.leftX + 1; i <= rom.rightX; i++)
                    {
                        if (i < mid.x - doorSize || i > mid.x + doorSize)
                        {
                            GameObject aux = Instantiate(HorizontalPrefab, new Vector3(i, mid.y), Quaternion.identity);
                            aux.name = (rom.name + " Horizontal");
                            aux.transform.SetParent(WallParent.transform);
                        }
                    }
                }
            }

            if (rom.rightX < iniSizeX)
            {
                Room right = RoomRight(rom);
                if (right.rightX != -1)
                {
                    Vector2 mid = GetMidPoint(rom, right);
                    for (int i = rom.botY + 1; i < rom.topY; i++)
                    {
                        if (i < mid.y - doorSize || i > mid.y + doorSize)
                        {
                            GameObject aux = Instantiate(VerticalPrefab, new Vector3(i, mid.y), Quaternion.identity);
                            aux.name = (rom.name + " Vertical");
                            aux.transform.SetParent(WallParent.transform);
                        }
                    }
                }
            }
        }

        for (int i = 0; i < iniSizeY; i++)
        {
            Instantiate(VerticalPrefab, new Vector3(0, i), Quaternion.identity).name = "Left Wall";
        }

        for (int i = 0; i < iniSizeY; i++)
        {
            Instantiate(VerticalPrefab, new Vector3(iniSizeX, i), Quaternion.identity).name = "Right Wall";
        }

        for (int i = 0; i < iniSizeX; i++)
        {
            Instantiate(HorizontalPrefab, new Vector3(i, 0), Quaternion.identity).name = "Bot Wall";
        }

        for (int i = 0; i < iniSizeX; i++)
        {
            Instantiate(HorizontalPrefab, new Vector3(i, iniSizeY), Quaternion.identity).name = "Top Wall";
        }
    }

    private bool IsInside(Vector2 point, Room room)
    {
        return point.x < room.rightX && point.x < room.leftX && point.y < room.topY && point.y > room.botY;
    }

    private Room RoomAbove(Room room)
    {
        foreach (Room rom in rooms)
        {
            if (rom.botY == room.topY)
            {

                if ((rom.rightX <= room.rightX && rom.rightX >= room.leftX + minRoomSizeX) || (rom.leftX >= room.leftX && rom.leftX <= room.rightX - minRoomSizeX))
                {
                    return rom;
                }
                //return rom;
            }
        }
        return new Room(-1, -1, -1, -1, -1);
    }

    private Room RoomRight(Room room)
    {
        foreach (Room rom in rooms)
        {
            if (rom.leftX == room.rightX)
            {

                if ((rom.topY <= room.topY && rom.topY >= room.botY + minRoomSizeY) || (rom.botY >= room.botY && rom.botY <= room.topY - minRoomSizeY))
                {
                    return rom;
                }

                //return rom;
            }
        }
        return new Room(-1, -1, -1, -1, -1);

    }

    private Vector2 GetMidPoint(Room room1, Room room2)
    {
        int x = 0, y = 0;
        int min = 0, max = 0;

        if (room1.topY == room2.botY) // Este
        {
            min = Mathf.Max(room1.leftX, room2.leftX);
            max = Mathf.Min(room1.rightX, room2.rightX);
            y = room1.topY;
            x = (min + max) / 2;
        }
        else if (room1.botY == room2.topY)
        {
            min = Mathf.Max(room1.leftX, room2.leftX);
            max = Mathf.Min(room1.rightX, room2.rightX);
            y = room1.botY;
            x = (min + max) / 2;
        }
        else if (room1.leftX == room2.rightX)
        {
            min = Mathf.Max(room1.botY, room2.botY);
            max = Mathf.Min(room1.topY, room2.topY);
            x = room1.leftX;
            y = (min + max) / 2;
        }
        else  // Y este
        {
            min = Mathf.Max(room1.botY, room2.botY);
            max = Mathf.Min(room1.topY, room2.topY);
            x = room1.rightX;
            y = (min + max) / 2;
        }


        return new Vector2(x, y);


    }

    public Vector2 GetRandomTile()
    {
        int room = Random.Range(0, rooms.Count);
        int x = Random.Range(rooms[room].leftX + fruitMargin, rooms[room].rightX - fruitMargin);
        int y = Random.Range(rooms[room].botY + fruitMargin, rooms[room].topY - fruitMargin);
        return new Vector2(x, y);
    }

    public Vector3 SpawnPoint()
    {
        Vector3 aux = new Vector3();
        aux.x = (rooms[0].leftX + rooms[0].rightX) / 2;
        aux.y = (rooms[0].botY + rooms[0].topY) / 2;
        //return new Vector2((rooms[0].leftX + rooms[0].rightX) / 2, (rooms[0].botY + rooms[0].topY) / 2);
        return aux;
    }
}
