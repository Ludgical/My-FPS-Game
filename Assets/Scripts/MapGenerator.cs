using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenerator : MonoBehaviour
{
    private class Room
    {
        public static GameObject OpenWallPrefab;
        public static GameObject DoorPrefab;
        
        public readonly float x;
        public readonly float y;
        public readonly int depth;
        public readonly Dictionary<Vector2, GameObject> walls = new();
        public readonly Dictionary<Vector2, GameObject> doors = new();

        public Room(float x, float y, int depth, Vector2? entranceSide = null)
        {
            this.x = x;
            this.y = y;
            this.depth = depth;
            foreach (var direction in Directions)
            {
                walls[direction] = null;
                doors[direction] = null;
            }

            //The challenge rooms coming from the main room
            //have an open wall where the door is
            if (entranceSide != null)
                CreateOpenWall(entranceSide.Value, createDoor: false);
        }
        
        /// Create a wall with a gap in the middle on the <c>direction</c> side of the room.
        /// Also create a door if <c>createDoor</c> is true
        public void CreateOpenWall(Vector2 direction, bool createDoor)
        {
            var wallX = x + CenterToWall * direction.x;
            var wallZ = y + CenterToWall * direction.y;
            var rotation = RotationFromDirection(direction);
            
            //Create a wall with the x, y and rotation and put it in the walls dictionary
            walls[direction] = Instantiate(
                OpenWallPrefab, 
                new Vector3(wallX, refs.gameData.wallY, wallZ), 
                Quaternion.Euler(0, rotation, 0));

            if (createDoor)
            {
                var doorX = x + CenterToDoor * direction.x;
                var doorZ = y + CenterToDoor * direction.y;
                
                //Create a door with the x, y and rotation and put it in the doors dictionary
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
        [CanBeNull] public readonly Room parent;
        public readonly Vector2 direction;

        public Branch(float x, float y, Room parent, Vector2 direction)
        {
            this.x = x;
            this.y = y;
            this.parent = parent;
            this.direction = direction;
        }
    }
    
    private static References refs;
    
    [SerializeField] private GameObject wallPrefab;
    [SerializeField] private GameObject openWallPrefab;
    [SerializeField] private GameObject doorPrefab;
    
    /// Distance from the center of a challenge room to the wall of a challenge room
    private const float CenterToWall = 15.5f;
    /// Distance from the center of a challenge room to the door of a challenge room
    private const float CenterToDoor = 16.5f;
    /// Distance from the center of a challenge room to the center of another challenge room
    private const float CenterToCenter = CenterToDoor * 2;
    /// Left, Right, Up, Down
    private static readonly Vector2[] Directions = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };
    
    /// All the challenge rooms that have been created
    private List<Room> rooms;
    
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
    
    private void Start()
    {
        refs = References.Refs;
        
        Room.DoorPrefab = doorPrefab;
        Room.OpenWallPrefab = openWallPrefab;
        
        GenerateMap();
    }
    
    private void GenerateMap()
    {
        //Create the initial rooms
        rooms = new Room[]
        {
            new (-43, 37, depth: 1, entranceSide: new Vector2(1, 0)),
            new (43, 37, depth: 1, entranceSide: new Vector2(-1, 0)),
            new (0, 80, depth: 1, entranceSide: new Vector2(0, -1))
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
        var possibleBranches = new List<Branch>();
        
        //Check every room in every direction to see if a new room can generate there
        foreach (var room in rooms)
        {
            foreach (var direction in Directions)
            {
                //Create a branch from the room and add it to the possible branches
                var newX = room.x + CenterToCenter * direction.x;
                var newY = room.y + CenterToCenter * direction.y;
                possibleBranches.Add(new Branch(newX, newY, room, direction));
            }
        }

        //Choose a random possible branch and generate a room in its place
        var branch = possibleBranches[Random.Range(0, possibleBranches.Count)];
        possibleBranches.Clear();
        GenerateRoomFromBranch(branch);
    }

    private void GenerateRoomFromBranch(Branch branch)
    {
        //Create a room with properties from the branch
        var x = branch.x;
        var y = branch.y;
        var depth = branch.parent!.depth + 1;
        var parent = branch.parent;
        var room = new Room(x, y, depth);

        //Create an open wall and a door in the parent of the branch going into the new room
        parent.CreateOpenWall(branch.direction, createDoor: true);
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
            //If there is no wall, create one
            if (wall == null)
            {
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
    }
}
