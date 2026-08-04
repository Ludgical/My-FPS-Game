using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    private static References refs;
    
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject openWallPrefab;
    [SerializeField] private GameObject doorPrefab;

    private static float CenterToWall;
    private static float CenterToDoor;
    private static float CenterToCenter;
    
    /// Left, Right, Up, Down
    public static readonly Vector2[] Directions = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };
    
    /// All the challenge rooms that have been created
    private static List<Room> rooms = new ();
    
    /// Rectangles that cover the space that is occupied by rooms
    private static List<Rectangle> occupiedSpace = new ();
    
    private class Room
    {
        public static GameObject OpenWallPrefab;
        public static GameObject DoorPrefab;
        
        public readonly float x;
        public readonly float y;
        public readonly int depth;
        public readonly Vector2 entranceSide;
        // Side of the room : prefab
        public readonly Dictionary<Vector2, GameObject> walls = new();
        public readonly Dictionary<Vector2, GameObject> doors = new();

        public Room(float x, float y, int depth, Vector2 entranceSide)
        {
            this.x = x;
            this.y = y;
            this.depth = depth;
            this.entranceSide = entranceSide;
            foreach (var direction in Directions)
            {
                walls[direction] = null;
                doors[direction] = null;
            }
            
            //Add the room to the occupied space so nothing generated inside the room
            occupiedSpace.Add(new Rectangle(x, y));

            //The challenge rooms coming from the main room
            //have an open wall where the door is
            CreateOpenWall(entranceSide, createDoor: false);
        }
        
        /// Create a wall with a gap in the middle on the <c>direction</c> side of the room.
        /// Also create a door if <c>createDoor</c> is true
        public void CreateOpenWall(Vector2 direction, bool createDoor)
        {
            //Create the open wall
            var wallX = x + CenterToWall * direction.x;
            var wallZ = y + CenterToWall * direction.y;
            var rotation = RotationFromDirection(direction);
            
            walls[direction] = Instantiate(
                OpenWallPrefab, 
                new Vector3(wallX, refs.gameData.wallY, wallZ), 
                Quaternion.Euler(0, rotation, 0));

            if (createDoor)
            {
                //Create the door
                var doorX = x + CenterToDoor * direction.x;
                var doorZ = y + CenterToDoor * direction.y;
                
                doors[direction] = Instantiate(
                    DoorPrefab,
                    new Vector3(doorX, refs.gameData.wallY, doorZ),
                    Quaternion.Euler(0, rotation, 0));
            }
        }
    }

    private class Branch
    {
        public readonly float x;
        public readonly float y;
        public readonly Room parent;
        public readonly Vector2 direction;

        public Branch(float x, float y, Room parent, Vector2 direction)
        {
            this.x = x;
            this.y = y;
            this.parent = parent;
            this.direction = direction;
        }
    }

    private class Rectangle
    {
        //Sides of the rectangle
        private readonly float x1;
        private readonly float x2;
        private readonly float y1;
        private readonly float y2;
        
        /// Create a rectangle where <c>x1</c> and <c>x2</c> are the x-coordinates of the vertical sides and
        /// <c>y1</c> and <c>y2</c> are the y-coordinates of the horizontal sides
        public Rectangle(float x1, float x2, float y1, float y2)
        {
            this.x1 = Mathf.Min(x1, x2);
            this.x2 = Mathf.Max(x1, x2);
            this.y1 = Mathf.Min(y1, y2);
            this.y2 = Mathf.Max(y1, y2);
        }
        
        /// Create a square centered at <c>x</c>, <c>y</c> that is the size of a challenge room
        public Rectangle(float x, float y) : this(
            x - CenterToDoor, x + CenterToDoor, 
            y - CenterToDoor, y + CenterToDoor) { }

        public bool CollidesWith(Rectangle rect)
        {
            return rect.x1 < x2 && rect.x2 > x1 && rect.y1 < y2 && rect.y2 > y1;
        }
    }
    
    private void Start()
    {
        refs = References.Refs;
        
        Room.DoorPrefab = doorPrefab;
        Room.OpenWallPrefab = openWallPrefab;

        CenterToWall = refs.gameData.CenterToWall;
        CenterToDoor = refs.gameData.CenterToDoor;
        CenterToCenter = refs.gameData.CenterToCenter;
        
        GenerateMap();

        GenerateChallenges();
    }
    
    /// Generate a system of rooms where the challenges will be
    private void GenerateMap()
    {
        //Make sure no rooms generate in the start room or main room
        occupiedSpace = new Rectangle[]
        {
            new (-25.5f, 25.5f, -11, 62.5f)
        }.ToList();
        
        //Create the initial challenge rooms
        rooms = new Room[]
        {
            new (-48, 37, depth: 1, entranceSide: new Vector2(1, 0)),
            new (48, 37, depth: 1, entranceSide: new Vector2(-1, 0)),
            new (0, 85, depth: 1, entranceSide: new Vector2(0, -1))
        }.ToList();
        
        //Generate 4 new rooms
        for (var i = 0; i < 4; i++)
        {
            GenerateRoom();
        }

        //Create all the missing walls
        foreach (var room in rooms)
        {
            CreateWalls(room);
        }
    }
    
    /// Generate a room and a door coming out from an already existing room
    private void GenerateRoom()
    {
        // Contains all the ways a new room can be generated
        var branches = new List<Branch>();
        
        //Add a branch from every room in every direction
        foreach (var room in rooms)
        {
            foreach (var direction in Directions)
            {
                //Create a branch from the room and add it to the branches
                var newX = room.x + CenterToCenter * direction.x;
                var newY = room.y + CenterToCenter * direction.y;
                branches.Add(new Branch(newX, newY, room, direction));
            }
        }
        
        //Get random branches from the list until one can generate a room
        Branch branch;
        do
        {
            branch = branches[Random.Range(0, branches.Count)];
            branches.Remove(branch);
        } while (!CanGenerateRoomFromBranch(branch));
        
        branches.Clear();
        GenerateRoomFromBranch(branch);
    }

    private bool CanGenerateRoomFromBranch(Branch branch)
    {
        //A chain of rooms can't be longer than 3
        if (branch.parent.depth == 3)
            return false;

        //The room can't generate inside another room
        var branchRect = new Rectangle(branch.x, branch.y);
        if (occupiedSpace.Any(rect => rect.CollidesWith(branchRect)))
            return false;

        return true;
    }

    /// Create a room with properties from a branch
    private void GenerateRoomFromBranch(Branch branch)
    {
        var room = new Room(branch.x, branch.y, branch.parent.depth + 1, branch.direction * -1);

        //Create an open wall and a door in the parent of the branch going into the new room
        branch.parent.CreateOpenWall(branch.direction, createDoor: true);
        //Create an open wall in the new room where the door going into the room is
        room.CreateOpenWall(branch.direction * -1, createDoor: false);
        
        rooms.Add(room);
    }
    
    /// Add walls to the room where there are not already open walls
    private void CreateWalls(Room room)
    {
        //Go over every place there can be a wall
        foreach (var direction in Directions)
        {
            var wall = room.walls[direction];
            
            //If there is a wall, skip it
            if (wall != null)
                continue;
            
            //If there is not a wall, create one
            var wallX = room.x + CenterToWall * direction.x;
            var wallZ = room.y + CenterToWall * direction.y;
            var rotation = RotationFromDirection(direction);
            
            //Create a wall with the x, y and rotation
            Instantiate(
                wallPrefab,
                new Vector3(wallX, refs.gameData.wallY, wallZ),
                Quaternion.Euler(0, rotation, 0));
        }
    }
    
    /// Return the rotation a wall or door should have when it's on the <c>direction</c> side of the room
    private static float RotationFromDirection(Vector2 direction)
    {
        if (direction == Directions[0])
            return -90;
        if (direction == Directions[1])
            return 90;
        if (direction == Directions[2])
            return 180;
        if (direction == Directions[3])
            return 0;
        throw new Exception("Invalid direction: " + direction);
    }

    private void GenerateChallenges()
    {
        var roomAmount = rooms.Count;
        var positions = new Vector2[roomAmount];
        var entranceSides = new Vector2[roomAmount];
        var doors = new List<DoorScript>[roomAmount];

        for (var i = 0; i < roomAmount; i++)
        {
            var room = rooms[i];
            positions[i] = new Vector2(room.x, room.y);
            entranceSides[i] = room.entranceSide;
            doors[i] = room.doors.Values.Where(door => door != null)
                .Select(door => door.GetComponent<DoorScript>()).ToList();
        }
        
        refs.challengeGenerator.GenerateChallenges(positions, entranceSides, doors);
    }
}
