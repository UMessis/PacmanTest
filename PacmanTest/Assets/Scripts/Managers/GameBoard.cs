using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour
{
    [Header("Options")]
    public int pointsPerDot = 10;
    public int pointsPerGhostKill = 200;
    public Node ghostRespawnNode;
    public Node playerRespawnNode;

    [Header("UI")]
    public Text scoreText;
    public Text enragedText;
    public Text livesText;
    public Text gameOverText;

    [Header("Ghosts")]
    [SerializeField] Ghost[] ghosts;

    int totalDots = 0;
    int consumedDots = 0;
    int score = 0;

    public GameObject[,] board = new GameObject[boardWidth, boardHeight];
    public Ghost[,] ghostPositions = new Ghost[boardWidth, boardHeight];
    List<Tile> dots = new List<Tile>();

    static int boardWidth = 29;
    static int boardHeight = 32;

    void Start()
    {
        Object[] objs = FindObjectsOfType(typeof(GameObject));

        foreach (GameObject obj in objs)
        {
            //Ignore if object is Pacman or a Ghost or UI
            if (obj.tag == "Player" || obj.tag == "Ghost" || obj.layer == 5)
            {
                continue;
            }

            //Store object in board position
            Vector2 pos = obj.transform.position;
            board[(int)pos.x, (int)pos.y] = obj;

            Tile tile = obj.GetComponent<Tile>();

            if (tile != null)
            {
                if (tile.isDot || tile.isBigDot)
                {
                    totalDots++;
                    dots.Add(tile);
                }
            }
        }
    }

    void Update()
    {
        //Reset board
        ghostPositions = new Ghost[boardWidth, boardHeight];

        //Locate ghosts
        foreach (Ghost ghost in ghosts)
        {
            Vector3 pos = ghost.transform.position;

            ghostPositions[(int)pos.x, (int)pos.y] = ghost;
        }
    }

    public void ConsumedDot()
    {
        consumedDots++;
        AddScore(pointsPerDot);

        if (IsGameWon())
        {
            ResetDots();
            ResetGhosts();
            GameObject.Find("Pacman").GetComponent<Pacman>().ResetPlayer();
        }
    }

    bool IsGameWon()
    {
        if (consumedDots == totalDots)
        {
            return true;
        }

        return false;
    }

    public void GameOver()
    {
        GameObject.Find("Pacman").SetActive(false);

        foreach (Ghost ghost in ghosts)
        {
            ghost.gameObject.SetActive(false);
        }

        gameOverText.enabled = true;
    }

    public void AddScore(int amount)
    {
        if (amount > 0)
        {
            score += amount;
            scoreText.text = score.ToString();
        }
    }

    void ResetDots()
    {
        foreach (Tile dot in dots)
        {
            dot.isConsumed = false;
            dot.GetComponent<SpriteRenderer>().enabled = true;
        }

        consumedDots = 0;
    }

    void ResetGhosts()
    {
        foreach (Ghost ghost in ghosts)
        {
            ghost.ResetGhost();
        }
    }
}