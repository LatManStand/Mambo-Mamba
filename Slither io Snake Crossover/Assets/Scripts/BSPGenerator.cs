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
        /// <summary>
        /// How many times did the original room divide to get a smaller room
        /// </summary>
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
        /// <summary>
        /// Return middle width of the room
        /// </summary>
        public float MidWidth()
        {
            return (float)(rightX - leftX) / 2;
        }
        /// <summary>
        /// Return middle height of the room
        /// </summary>
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
    private void Awake()
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


    /// <summary>
    /// Checks if a room is large enough to divide into two other rooms bigger than minimum size.
    /// </summary>
    /// <returns>True if big enough and not divided deep enough, false if not</returns>
    private bool CanDivide(Room room)
    {
        if (room.divides < maxDivides && room.Height() > 2 * minRoomSizeY + 2 && room.Width() > 2 * minRoomSizeX + 2)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Divide a room into 2 others if its possible
    /// </summary>
    /// <param name="room">Room to divide</param>
    private void Divide(Room room)
    {
        if (CanDivide(room))
        {
            // If its the first room (entire map), cut in half exactly in the middle vertically
            if (room.divides == 0)
            {
                int slice = (int)room.MidWidth();
                Room subroom1 = new Room(room.leftX, room.botY, slice, room.topY, room.divides + 1);
                Room subroom2 = new Room(slice, room.botY, room.rightX, room.topY, room.divides + 1);
                Divide(subroom1);
                Divide(subroom2);
            }
            // If the divide is even, cut it vertically at a random point
            else if (room.divides % 2 == 0)
            {
                int slice = Random.Range(room.leftX + minRoomSizeX, room.rightX - minRoomSizeX);
                Room subroom1 = new Room(room.leftX, room.botY, slice, room.topY, room.divides + 1);
                Room subroom2 = new Room(slice, room.botY, room.rightX, room.topY, room.divides + 1);
                Divide(subroom1);
                Divide(subroom2);
            }
            // If the divide is odd, cut it horizontally at a random point
            else
            {
                int slice = Random.Range(room.botY + minRoomSizeY, room.topY - minRoomSizeY);
                Room subroom1 = new Room(room.leftX, room.botY, room.rightX, slice, room.divides + 1);
                Room subroom2 = new Room(room.leftX, slice, room.rightX, room.topY, room.divides + 1);
                Divide(subroom1);
                Divide(subroom2);
            }
        }
        // If the room can't divide, add it to the rooms array
        else
        {
            rooms.Add(room);
        }
    }

    /// <summary>
    /// Instantiate walls
    /// </summary>
    private void Spriting()
    {
        foreach (Room rom in rooms)
        {
            // If room isnt at the top
            if (rom.topY < iniSizeY)
            {
                Room above = RoomAbove(rom);
                // If there's a room on top
                if (above.rightX != -1)
                {
                    Vector2 mid = GetMidPoint(rom, above);
                    for (int i = rom.leftX + 1; i <= rom.rightX; i++)
                    {
                        // Instantiate walls between the two rooms leaving a door (empty) in the middle
                        if (i < mid.x - doorSize || i > mid.x + doorSize)
                        {
                            GameObject aux = Instantiate(HorizontalPrefab, new Vector3(i, mid.y), Quaternion.identity);
                            aux.name = (rom.name + " Horizontal");
                            aux.transform.SetParent(WallParent.transform);
                        }
                    }
                }
            }

            // If room isnt at the right
            if (rom.rightX < iniSizeX)
            {
                Room right = RoomRight(rom);
                // If there's a room on the right
                if (right.rightX != -1)
                {
                    Vector2 mid = GetMidPoint(rom, right);
                    for (int i = rom.botY + 1; i < rom.topY; i++)
                    {
                        // Instantiate walls between the two rooms leaving a door (empty) in the middle
                        if (i < mid.y - doorSize || i > mid.y + doorSize)
                        {
                            GameObject aux = Instantiate(VerticalPrefab, new Vector3(mid.x, i), Quaternion.identity);
                            aux.name = (rom.name + " Vertical");
                            aux.transform.SetParent(WallParent.transform);
                        }
                    }
                }
            }
        }

        // Instantiate walls at the right and the left
        for (int i = 0; i < iniSizeY; i++)
        {
            GameObject aux = Instantiate(VerticalPrefab, new Vector3(0, i), Quaternion.identity);
            aux.name = "Left Wall";
            aux.transform.SetParent(WallParent.transform);

            GameObject aux2 = Instantiate(VerticalPrefab, new Vector3(iniSizeX, i), Quaternion.identity);
            aux2.name = "Right Wall";
            aux2.transform.SetParent(WallParent.transform);
        }

        // Instantiate walls at top and at bottom
        for (int i = 0; i < iniSizeX; i++)
        {
            GameObject aux = Instantiate(HorizontalPrefab, new Vector3(i, 0), Quaternion.identity);
            aux.name = "Bot Wall";
            aux.transform.SetParent(WallParent.transform);

            GameObject aux2 = Instantiate(HorizontalPrefab, new Vector3(i, iniSizeY), Quaternion.identity);
            aux2.name = "Top Wall";
            aux2.transform.SetParent(WallParent.transform);
        }
    }

    /// <summary>
    /// Check if a point is inside a room
    /// </summary>
    /// <param name="point"></param>
    /// <param name="room"></param>
    /// <returns></returns>
    private bool IsInside(Vector2 point, Room room)
    {
        return point.x < room.rightX && point.x < room.leftX && point.y < room.topY && point.y > room.botY;
    }

    /// <summary>
    /// Returns the room above another one
    /// </summary>
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
            }
        }
        return new Room(-1, -1, -1, -1, -1);
    }

    /// <summary>
    /// Returns the room right of another one
    /// </summary>
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
            }
        }
        return new Room(-1, -1, -1, -1, -1);

    }

    /// <summary>
    /// Calculates the middle point where two rooms collide
    /// </summary>
    /// <returns>The middle point of the wall between two rooms</returns>
    private Vector2 GetMidPoint(Room room1, Room room2)
    {
        int x = 0, y = 0;
        int min = 0, max = 0;
        // If first room is below second room
        if (room1.topY == room2.botY)
        {
            min = Mathf.Max(room1.leftX, room2.leftX);
            max = Mathf.Min(room1.rightX, room2.rightX);
            y = room1.topY;
            x = (min + max) / 2;
        }
        // If first room is above second room
        else if (room1.botY == room2.topY)
        {
            min = Mathf.Max(room1.leftX, room2.leftX);
            max = Mathf.Min(room1.rightX, room2.rightX);
            y = room1.botY;
            x = (min + max) / 2;
        }
        // If first room is left of second room
        else if (room1.leftX == room2.rightX)
        {
            min = Mathf.Max(room1.botY, room2.botY);
            max = Mathf.Min(room1.topY, room2.topY);
            x = room1.leftX;
            y = (min + max) / 2;
        }
        // If first room is right of second room
        else
        {
            min = Mathf.Max(room1.botY, room2.botY);
            max = Mathf.Min(room1.topY, room2.topY);
            x = room1.rightX;
            y = (min + max) / 2;
        }

        return new Vector2(x, y);
    }

    /// <summary>
    /// Returns a random tile away from walls in a random room
    /// </summary>
    /// <returns>Random spawnable fruit position</returns>
    public Vector2 GetRandomTile()
    {
        int room = Random.Range(0, rooms.Count);
        int x = Random.Range(rooms[room].leftX + fruitMargin, rooms[room].rightX - fruitMargin);
        int y = Random.Range(rooms[room].botY + fruitMargin, rooms[room].topY - fruitMargin);
        return new Vector2(x, y);
    }

    /// <summary>
    /// Returns middle point of the bot left corner room
    /// </summary>
    public Vector3 SpawnPoint()
    {
        Vector3 aux = new Vector3();
        aux.x = (rooms[0].leftX + rooms[0].rightX) / 2;
        aux.y = (rooms[0].botY + rooms[0].topY) / 2;
        return aux;
    }

    /// <summary>
    /// Compares spawn room height and width
    /// </summary>
    /// <returns>True if wider, false if higher</returns>
    public bool FirstRoomIsWider()
    {
        return rooms[0].Width() > rooms[0].Height();
    }
}
