using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.VisualScripting;
using UnityEngine;

//Instead of spawning them all at once spawn them one by one everytime it gets triggerd 
public class DrunkardsWalk : MonoBehaviour
{
    CPCount cpCount;
    GameObject player;
    float playerPosX;
    float playerPosY;
    float playerPosZ;
    public GameObject CheckpointPrefab;
    Vector3 startPos;
    public float startPosOffsetX = 20f;
    int currentCheckpointValue;

    int currentCheckpoint;
    int nextCheckpoint;

    public int stepsToTake;
    
    public List<Vector3> walkPositions = new List<Vector3>();

    bool startPosSpawned;
    public bool drunkWalkStarted;

    List<Vector3> allPossibleDirections = new List<Vector3> {
            new Vector3(0f, 0f, 100f),
            new Vector3(0f, 0f, -100f),
            new Vector3(-100f, 0f, 0f),
            new Vector3(100f, 0f, 0f)
            };
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cpCount = GetComponent<CPCount>();
    }

    private void Update()
    {
        if (drunkWalkStarted)
        {
            currentCheckpointValue = cpCount.count;
            nextCheckpoint = currentCheckpointValue + 1;
            InstantiateCheckpoints();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            
            walkPositions = GenerateRandomWalk();
            drunkWalkStarted = true;
        }
       
    }
    void InstantiateCheckpoints()
    {
        //if (!startPosSpawned) 
        //{
        //    Instantiate(CheckpointPrefab, walkPositions[0], Quaternion.identity);
        //    startPosSpawned = true;
        //}

        

        if(currentCheckpoint != nextCheckpoint)
        {
           
            Instantiate(CheckpointPrefab, walkPositions[currentCheckpointValue], Quaternion.identity);
            
            currentCheckpoint = nextCheckpoint;
        }

    }
    public List<Vector3> GenerateRandomWalk()
    {
        playerPosX=  player.transform.position.x ;
        playerPosY = player.transform.position.y ;
        playerPosZ = player.transform.position.z ;

        Vector3 startPos = new Vector3(playerPosX + Random.Range(5, 15) , playerPosY, playerPosZ + Random.Range(5, 10));

        WalkNode currentNode = new WalkNode(startPos, null, new List<Vector3>(allPossibleDirections));

        int stepsSoFar = 0;

        List<Vector3> visitedNodes = new List<Vector3>();
        visitedNodes.Add(startPos);

        while (true)
        {
            if (stepsSoFar == stepsToTake)
            {
                break;
            }


            //If we there is a need to backtrack 
            while (currentNode.possibleDirections.Count == 0)
            {
                currentNode = currentNode.previousNode;
                stepsSoFar -= 1;
            }

            int randomDirPos = Random.Range(0, currentNode.possibleDirections.Count);
            Vector3 randomDir = currentNode.possibleDirections[randomDirPos];
            currentNode.possibleDirections.RemoveAt(randomDirPos);
            Vector3 nextNodePos = currentNode.pos + randomDir;

            //Have we visited this position before?
            if (!HasVisitedNode(nextNodePos, visitedNodes))
            {
                //Walk to this node
                currentNode = new WalkNode(nextNodePos, currentNode, new List<Vector3>(allPossibleDirections));

                visitedNodes.Add(nextNodePos);

                stepsSoFar++;
            }
        }
        //Generate final  path
        List<Vector3> randomWalkPositions = new List<Vector3>();
        while (currentNode.previousNode != null)
        {
            randomWalkPositions.Add(currentNode.pos);

            currentNode = currentNode.previousNode;
        }

        randomWalkPositions.Add(currentNode.pos);
        randomWalkPositions.Reverse();
        return randomWalkPositions;
    }

    bool HasVisitedNode(Vector3 pos, List<Vector3> listPos)
    {
        bool hasVisited = false;

        for (int i = 0; i < listPos.Count; i++)
        {
            float distSqr = Vector3.SqrMagnitude(pos - listPos[i]);

            //Cant compare exactly because of floating point precisions
            if (distSqr < 0.001f)
            {
                hasVisited = true;

                break;
            }
        }

        return hasVisited;
    }

    
}
public class WalkNode
{
    //The position of this node in the world
    public Vector3 pos;

    public WalkNode previousNode;

    //Which steps can we take from this node?
    public List<Vector3> possibleDirections;

    public WalkNode(Vector3 pos, WalkNode previousNode, List<Vector3> possibleDirections)
    {
        this.pos = pos;

        this.previousNode = previousNode;

        this.possibleDirections = possibleDirections;
    }
}