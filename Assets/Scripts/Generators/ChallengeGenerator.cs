using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Generators
{
    public class ChallengeGenerator : MonoBehaviour
    {
        [SerializeField] private List<GameObject> challengePrefabs;

        public void GenerateChallenges(List<Room> rooms)
        {
            //Make sure there are as many challenge prefabs as rooms generated
            if (challengePrefabs.Count != rooms.Count)
            {
                Debug.LogError($"Not enough challenges, there needs to be {rooms.Count}");
                return;
            }
        
            var challengeAmount = challengePrefabs.Count;
        
            for (var i = 0; i < challengeAmount; i++)
            {
                //Choose a random challenge prefab to generate
                var challengeNum = Random.Range(0, challengeAmount - i);
                var challenge = Instantiate(challengePrefabs[challengeNum]);
                challengePrefabs.RemoveAt(challengeNum);

                //Get the script of the challenge
                var challengeScript = challenge.GetComponent<Challenge>();
                if (challengeScript == null)
                    Debug.LogError("The challenge prefab must have a challenge script");
            
                challenge.transform.position = new Vector3(rooms[i].x, 0, rooms[i].y);
                challenge.transform.rotation = RotationFromEntranceSide(rooms[i].entranceSide);
                challengeScript.openOnCompleted = 
                    rooms[i].doors.Values.Where(door => door != null)
                    .Select(door => door.GetComponent<DoorScript>()).ToArray();
            }
        }
    
        /// The rotation the challenge should have based on which side the entrance is on
        private static Quaternion RotationFromEntranceSide(Vector2 entranceSide)
        {
            var directions = MapGenerator.Directions;
            var yRotation = 
                entranceSide == directions[0] ? -90 :
                entranceSide == directions[1] ? 90 :
                entranceSide == directions[2] ? 180 :
                entranceSide == directions[3] ? 0 :
                throw new Exception("Invalid entrance side");
            return Quaternion.Euler(0, yRotation, 0);
        }
    }
}
