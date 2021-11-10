using UnityEngine;

public class GameBoard : MonoBehaviour
{
    static int boardWidth = 29;
    static int boardHeight = 32;

    public GameObject[,] board = new GameObject[boardWidth, boardHeight];

    void Start()
    {
        Object[] objects = GameObject.FindObjectsOfType(typeof(GameObject));

        foreach (GameObject obj in objects)
        {
            //Ignore if object is pacman or UI
            if (obj.tag == "Player" || obj.layer == 5)
            {
                continue;
            }

            //Store object in board position
            Vector2 pos = obj.transform.position;
            board[(int)pos.x, (int)pos.y] = obj;
        }
    }
}