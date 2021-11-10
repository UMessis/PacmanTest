using UnityEngine;

public class Node : MonoBehaviour
{
    public Node[] neighbors;
    public Vector2[] validDirections;

    void Start()
    {
        validDirections = new Vector2[neighbors.Length];

        //Create list of valid directions from node
        for (int i = 0; i < neighbors.Length; i++)
        {
            Node neighbor = neighbors[i];
            Vector2 temp = neighbor.transform.localPosition - transform.localPosition;
            validDirections[i] = temp.normalized;
        }
    }
}