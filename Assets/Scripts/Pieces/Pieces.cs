using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PieceType 
{
    
    None = 0,
    Spy = 1,
    Scout = 2,
    Miner = 3,
    Sergeant = 4,
    Lieutenant = 5,
    Captain = 6,
    Major = 7,
    Colonel = 8,
    General = 9,
    Marshall = 10,
    Bomb = 11,
    Flag = 12

}

public class Pieces : MonoBehaviour
{
    
    public int team;
    public PieceType type;
    public int currentX;
    public int currentY;

    private Vector3 desiredPosition;

    public void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 8);
    }

    public virtual List<Vector2Int> GetMoves(ref Pieces[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        return r;
    }

    public virtual List<Vector4> GetMovesAI(ref Pieces[,] board, int tileCountX, int tileCountY)
    {
        List<Vector4> r = new List<Vector4>();

        return r;
    }  

    public virtual void SetPosition(Vector3 position, bool force = false)
    {
        desiredPosition = position;
        if (force)
        {
            transform.position = desiredPosition;
        }
    }

}
