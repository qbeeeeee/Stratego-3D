using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spy : Pieces
{
    public override List<Vector2Int> GetMoves(ref Pieces[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        //Right direction
        if(currentX + 1 < tileCountX){
            if(Board.tiles[currentX + 1, currentY].layer != 4)
            {
                if(board[currentX + 1, currentY] == null)
                {
                    r.Add(new Vector2Int(currentX + 1, currentY));
                }
                else if(board[currentX + 1, currentY].team != team)
                {
                    r.Add(new Vector2Int(currentX + 1, currentY));
                }
            }
        }

        //Left direction
        if(currentX - 1 >= 0){
            if(Board.tiles[currentX - 1, currentY].layer != 4)
            {
                if(board[currentX - 1, currentY] == null)
                {
                    r.Add(new Vector2Int(currentX - 1, currentY));
                }
                else if(board[currentX - 1, currentY].team != team)
                {
                    r.Add(new Vector2Int(currentX - 1, currentY));
                }
            }
        }

        //Up direction
        if(currentY + 1 < tileCountY){
            if(Board.tiles[currentX, currentY + 1].layer != 4)
            {
                if(board[currentX, currentY + 1] == null)
                {
                    r.Add(new Vector2Int(currentX, currentY + 1));
                }
                else if(board[currentX, currentY + 1].team != team)
                {
                    r.Add(new Vector2Int(currentX, currentY + 1));
                }
            }
        }

        //Down direction
        if(currentY - 1 >= 0){
            if(Board.tiles[currentX, currentY - 1].layer != 4)
            {
                if(board[currentX, currentY - 1] == null)
                {
                    r.Add(new Vector2Int(currentX, currentY - 1));
                }
                else if(board[currentX, currentY - 1].team != team)
                {
                    r.Add(new Vector2Int(currentX, currentY - 1));
                }
            }
        }

        return r;
    }

    public override List<Vector4> GetMovesAI(ref Pieces[,] board, int tileCountX, int tileCountY)
    {
        List<Vector4> r = new List<Vector4>();

        //Right direction
        if(currentX + 1 < tileCountX){
            if(Board.tiles[currentX + 1, currentY].layer != 4)
            {
                if(board[currentX + 1, currentY] == null)
                {
                    r.Add(new Vector4(currentX + 1, currentY,currentX,currentY));
                }
                else if(board[currentX + 1, currentY].team != team)
                {
                    r.Add(new Vector4(currentX + 1, currentY,currentX,currentY));
                }
            }
        }

        //Left direction
        if(currentX - 1 >= 0){
            if(Board.tiles[currentX - 1, currentY].layer != 4)
            {
                if(board[currentX - 1, currentY] == null)
                {
                    r.Add(new Vector4(currentX - 1, currentY,currentX,currentY));
                }
                else if(board[currentX - 1, currentY].team != team)
                {
                    r.Add(new Vector4(currentX - 1, currentY,currentX,currentY));
                }
            }
        }

        //Up direction
        if(currentY + 1 < tileCountY){
            if(Board.tiles[currentX, currentY + 1].layer != 4)
            {
                if(board[currentX, currentY + 1] == null)
                {
                    r.Add(new Vector4(currentX, currentY + 1,currentX,currentY));
                }
                else if(board[currentX, currentY + 1].team != team)
                {
                    r.Add(new Vector4(currentX, currentY + 1,currentX,currentY));
                }
            }
        }

        //Down direction
        if(currentY - 1 >= 0){
            if(Board.tiles[currentX, currentY - 1].layer != 4)
            {
                if(board[currentX, currentY - 1] == null)
                {
                    r.Add(new Vector4(currentX, currentY - 1,currentX,currentY));
                }
                else if(board[currentX, currentY - 1].team != team)
                {
                    r.Add(new Vector4(currentX, currentY - 1,currentX,currentY));
                }
            }
        }

        return r;
    }
}
