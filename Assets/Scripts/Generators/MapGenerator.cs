using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Generators
{
    public class MapGenerator : MonoBehaviour
    {
        private References refs;
    
        [SerializeField] private ChallengeGenerator challengeGenerator;
        [SerializeField] private GameObject wallPrefab;
        [SerializeField] private GameObject openWallPrefab;
        [SerializeField] private GameObject doorPrefab;

        private float CenterToWall;
        private float CenterToDoor;
        private float CenterToCenter;
    
        /// Left, Right, Up, Down
        public static readonly Vector2[] Directions = { new(1, 0), new(-1, 0), new(0, 1), new(0, -1) };
    
        /// All the challenge rooms that have been created
        private List<Room> rooms = new ();
    
        /// Rectangles that cover the space that is occupied by rooms
        private List<Rectangle> occupiedSpace = new ();
    
        private void Start()
        {
            refs = References.Refs;

            CenterToWall = refs.gameData.CenterToWall;
            CenterToDoor = refs.gameData.CenterToDoor;
            CenterToCenter = refs.gameData.CenterToCenter;
        
            GenerateMap();

            challengeGenerator.GenerateChallenges(rooms);
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
            rooms = new []
            {
                CreateRoom(-48, 37, depth: 1, entranceSide: new Vector2(1, 0)),
                CreateRoom(48, 37, depth: 1, entranceSide: new Vector2(-1, 0)),
                CreateRoom(0, 85, depth: 1, entranceSide: new Vector2(0, -1))
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
        
        /// Creates a new Room object, adds and open wall and adds it to the occupied space
        private Room CreateRoom(float x, float y, int depth, Vector2 entranceSide)
        {
            var room = new Room(x, y, depth, entranceSide);
            
            //Create an open wall in the new room where the door going into the room is
            CreateOpenWall(room, room.entranceSide, createDoor: false);
            //Add the room to the occupied space so nothing generates inside the room
            occupiedSpace.Add(new Rectangle(room.x, room.y));
            
            return room;
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
            var room = CreateRoom(branch.x, branch.y, branch.parent.depth + 1, branch.direction * -1);

            //Create an open wall and a door in the parent of the branch going into the new room
            CreateOpenWall(branch.parent, branch.direction, createDoor: true);
        
            rooms.Add(room);
        }
        
        /// Create a wall with a gap in the middle on the <c>direction</c> side of the room.
        /// Also create a door if <c>createDoor</c> is true
        private void CreateOpenWall(Room room, Vector2 direction, bool createDoor)
        {
            //Create the open wall
            var wallX = room.x + CenterToWall * direction.x;
            var wallZ = room.y + CenterToWall * direction.y;
            var rotation = RotationFromDirection(direction);
        
            room.walls[direction] = Instantiate(
                openWallPrefab, 
                new Vector3(wallX, refs.gameData.wallY, wallZ), 
                Quaternion.Euler(0, rotation, 0));

            if (createDoor)
            {
                //Create the door
                var doorX = room.x + CenterToDoor * direction.x;
                var doorZ = room.y + CenterToDoor * direction.y;
            
                room.doors[direction] = Instantiate(
                    doorPrefab,
                    new Vector3(doorX, refs.gameData.wallY, doorZ),
                    Quaternion.Euler(0, rotation, 0));
            }
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
    }
}
