using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pacman : MonoBehaviour
{
    [SerializeField] float playerSpeed = 6;
    [SerializeField] int numLives = 3;
    [SerializeField] float enrageDuration = 5;
    [SerializeField] Node startNode;

    Vector2 currentDirection, nextDirection;
    Node currentNode, previousNode, targetNode;
    int currentLives;
    bool isPlayerEnraged = false;

    GameBoard gameBoard;

    void Start()
    {
        currentNode = startNode;
        transform.position = startNode.transform.position;
        ChangeDirection(Vector2.left);
        currentLives = numLives;
        gameBoard = GameObject.Find("GameManager").GetComponent<GameBoard>();
        gameBoard.livesText.text = numLives.ToString() + " lives left";
    }

    void Update()
    {
        CheckForCollision();
        CheckInput();
        MovePlayer();
        ConsumeDot();
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

    void MovePlayer()
    {
        if (targetNode != currentNode && targetNode != null)
        {
            //Check to move backwards
            if (nextDirection == currentDirection * -1)
            {
                currentDirection = nextDirection;

                Node temp = targetNode;
                targetNode = previousNode;
                previousNode = temp;
            }

            //If target reached, check for premove or stop the player
            if (HasReachedDest())
            {
                currentNode = this.targetNode;
                transform.position = currentNode.transform.position;

                //If reached node is a portal, teleport the player
                GameObject recievePortal = GetPortal(currentNode.transform.position);
                if (recievePortal != null)
                {
                    transform.position = recievePortal.transform.position;
                    currentNode = recievePortal.GetComponent<Node>();
                }

                Node targetNode = CanMove(nextDirection);

                if (targetNode == null)
                {
                    targetNode = CanMove(currentDirection);
                } 
                else if (targetNode != null)
                {
                    currentDirection = nextDirection;
                    this.targetNode = targetNode;
                    previousNode = currentNode;
                    currentNode = null;
                }
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

    void ChangeDirection(Vector2 dir)
    {
        //Set premove
        if (dir != currentDirection)
        {
            nextDirection = dir;
        }

        //If player is not currently moving
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

    void ConsumeDot()
    {
        Tile tile = GetTileAtPosition(transform.position);

        if (tile != null)
        {
            if (!tile.isConsumed && (tile.isDot || tile.isBigDot))
            {
                tile.GetComponent<SpriteRenderer>().enabled = false;
                tile.isConsumed = true;
                gameBoard.ConsumedDot();

                if (tile.isBigDot)
                {
                    StopCoroutine(EnrageTimer());
                    StartCoroutine(EnrageTimer());
                }
            }
        }
    }

    IEnumerator EnrageTimer()
    {
        isPlayerEnraged = true;
        gameBoard.enragedText.enabled = true;

        yield return new WaitForSeconds(enrageDuration);

        isPlayerEnraged = false;
        gameBoard.enragedText.enabled = false;
    }

    void CheckForCollision()
    {
        int posX = (int)transform.position.x;
        int posY = (int)transform.position.y;

        Ghost ghost = gameBoard.ghostPositions[posX, posY];

        if (ghost != null)
        {
            if (!isPlayerEnraged)
            {
                PlayerDeath();
            }
            else
            {
                ghost.GhostDeath();
            }
        }
    }

    void PlayerDeath()
    {
        currentLives--;
        gameBoard.livesText.text = currentLives.ToString() + " lives left";

        if (currentLives <= 0)
        {
            gameBoard.GameOver();
        }
        else
        {
            transform.position = gameBoard.playerRespawnNode.transform.position;
            currentNode = gameBoard.playerRespawnNode;
            targetNode = null;
            previousNode = null;
        }
    }

    public void ResetPlayer()
    {
        currentNode = startNode;
        transform.position = startNode.transform.position;
        targetNode = null;
        previousNode = null;
        ChangeDirection(Vector2.left);
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

    Tile GetTileAtPosition(Vector2 pos)
    {
        int tileX = (int)pos.x;
        int tileY = (int)pos.y;

        GameObject tile = gameBoard.board[tileX, tileY];

        if (tile != null)
        {
            return tile.GetComponent<Tile>();
        }

        return null;
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

    GameObject GetPortal (Vector2 pos)
    {
        GameObject tile = gameBoard.board[(int)pos.x, (int)pos.y];

        if (tile != null)
        {
            if (tile.GetComponent<Tile>() != null)
            {
                if (tile.GetComponent<Tile>().isPortal)
                {
                    GameObject recievePortal = tile.GetComponent<Tile>().portalReciever;
                    return recievePortal;
                }
            }
        }

        return null;
    }
}