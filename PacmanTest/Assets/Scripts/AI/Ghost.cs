using UnityEngine;

public class Ghost : MonoBehaviour
{
    [SerializeField] float ghostSpeed = 6;
    [SerializeField] Node startNode;

    Vector2 currentDirection, nextDirection;
    Node currentNode, previousNode, targetNode;

    GameBoard gameBoard;

    void Start()
    {
        currentNode = startNode;
        transform.position = startNode.transform.position;
        gameBoard = GameObject.Find("GameManager").GetComponent<GameBoard>();
    }

    void Update()
    {
        ChooseNextDirection();
        MoveAI();
    }

    void MoveAI()
    {
        if (targetNode != currentNode && targetNode != null)
        {
            if (HasReachedDest())
            {
                currentNode = this.targetNode;
                transform.position = currentNode.transform.position;

                Node targetNode = CanMove(nextDirection);

                if (targetNode != null)
                {
                    currentDirection = nextDirection;
                    nextDirection = Vector2.zero;
                    this.targetNode = targetNode;
                    previousNode = currentNode;
                    currentNode = null;
                }
            }
            else
            {
                transform.localPosition += (Vector3)currentDirection * ghostSpeed * Time.deltaTime;
            }
        }
    }

    void ChooseNextDirection()
    {
        //If currently moving towards target node, find next move
        if (nextDirection == Vector2.zero && targetNode != null)
        {
            do
            {
                int rand = Random.Range(0, targetNode.validDirections.Length);
                nextDirection = targetNode.validDirections[rand];

                //Check if next target is a portal
                if (targetNode.neighbors[rand].GetComponent<Tile>() != null)
                {
                    if (targetNode.neighbors[rand].GetComponent<Tile>().isPortal)
                    {
                        nextDirection = Vector2.zero;
                    }
                }
            } //Keep looping if next move is backwards or a portal
            while (nextDirection == (currentDirection * -1) || nextDirection == Vector2.zero);
        }

        //If there is no current target node, find an available one from current node
        if (targetNode == null && currentNode != null)
        {
            int rand = Random.Range(0, currentNode.neighbors.Length);
            targetNode = currentNode.neighbors[rand];
            currentDirection = currentNode.validDirections[rand];

            previousNode = currentNode;
            currentNode = null;
        }
    }

    public void ResetGhost()
    {
        transform.position = startNode.transform.position;
        currentNode = startNode;
        currentDirection = Vector2.zero;
        nextDirection = Vector2.zero;
        targetNode = null;
        previousNode = null;
    }

    public void GhostDeath()
    {
        transform.position = gameBoard.ghostRespawnNode.transform.position;
        currentNode = gameBoard.ghostRespawnNode;
        currentDirection = Vector2.zero;
        nextDirection = Vector2.zero;
        targetNode = null;
        previousNode = null;

        gameBoard.AddScore(gameBoard.pointsPerGhostKill);
    }

    Node CanMove(Vector2 dir)
    {
        Node targetNode = null;

        for (int i = 0; i < currentNode.neighbors.Length; i++)
        {
            if (currentNode.validDirections[i] == dir)
            {
                targetNode = currentNode.neighbors[i];
                break;
            }
        }

        return targetNode;
    }

    bool HasReachedDest()
    {
        float nodeToTarget = DistanceFromNode(targetNode.transform.position);
        float nodeToSelf = DistanceFromNode(transform.localPosition);

        return nodeToSelf > nodeToTarget;
    }

    float DistanceFromNode(Vector2 targetPos)
    {
        Vector2 dist = targetPos - (Vector2)previousNode.transform.position;
        return dist.sqrMagnitude;
    }
}