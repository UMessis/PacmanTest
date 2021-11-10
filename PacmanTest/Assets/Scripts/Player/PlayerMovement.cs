using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float playerSpeed = 5;

    Vector2 currentDirection, nextDirection;
    Node currentNode, previousNode, targetNode;

    void Start()
    {
        currentNode = GetNodeAtPosition(transform.position);
        ChangeDirection(Vector2.left);
    }

    void Update()
    {
        CheckInput();
        MovePlayer();
    }

    void CheckInput()
    {
        if (Input.GetButtonDown("Left"))
        {
            ChangeDirection(Vector2.left);
        }
        if (Input.GetButtonDown("Right"))
        {
            ChangeDirection(Vector2.right);
        }
        if (Input.GetButtonDown("Up"))
        {
            ChangeDirection(Vector2.up);
        }
        if (Input.GetButtonDown("Down"))
        {
            ChangeDirection(Vector2.down);
        }
    }
    
    /// <summary>
    /// Move the player towards it's target node
    /// </summary>
    void MovePlayer()
    {
        if (targetNode != currentNode && targetNode != null)
        {
            if (OverShotTarget())
            {
                currentNode = this.targetNode;
                transform.position = currentNode.transform.position;

                //Check for target of pre-move
                Node targetNode = CanMove(nextDirection);

                //If not possible, check if can keep going straight
                if (targetNode == null)
                {
                    targetNode = CanMove(currentDirection);
                }

                //If possible, move that direction
                if (targetNode != null)
                {
                    currentDirection = nextDirection;
                    this.targetNode = targetNode;
                    previousNode = currentNode;
                    currentNode = null;
                }

                //Otherwise stop moving
                else
                {
                    currentDirection = Vector2.zero;
                }
            }
            else
            {
                transform.localPosition += (Vector3)currentDirection * playerSpeed * Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// Change the direction of the player
    /// </summary>
    /// <param name="dir"></param>
    void ChangeDirection(Vector2 dir)
    {
        if (dir != currentDirection)
        {
            nextDirection = dir;
        }

        if (currentNode != null)
        {
            Node targetNode = CanMove(dir);

            if (targetNode != null)
            {
                currentDirection = dir;
                this.targetNode = targetNode;
                previousNode = currentNode;
                currentNode = null;
            }
        }
    }

    /// <summary>
    /// Returns the target node if the move is possible, otherwise returns null
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Returns the node at the position of the player
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    Node GetNodeAtPosition(Vector2 pos)
    {
        GameObject tile = GameObject.Find("GameManager").GetComponent<GameBoard>().board[(int)pos.x, (int)pos.y];
        
        if (tile != null)
        {
            return tile.GetComponent<Node>();
        }

        return null;
    }

    /// <summary>
    /// Returns whether or not the player has reached it's destination
    /// </summary>
    /// <returns></returns>
    bool OverShotTarget()
    {
        float nodeToTarget = DistanceFromNode(targetNode.transform.position);
        float nodeToSelf = DistanceFromNode(transform.localPosition);

        return nodeToSelf > nodeToTarget;
    }

    /// <summary>
    /// Returns the distance from a point to the previous node
    /// </summary>
    /// <param name="targetPos"></param>
    /// <returns></returns>
    float DistanceFromNode(Vector2 targetPos)
    {
        Vector2 temp = targetPos - (Vector2)previousNode.transform.position;
        return temp.sqrMagnitude;
    }
}