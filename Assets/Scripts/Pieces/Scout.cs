using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scout : Pieces
{

    public override List<Vector2Int> GetMoves(ref Pieces[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        //Down direction
        for(int i = currentY - 1; i >= 0; i--)
        {
            if(Board.tiles[currentX,i].layer == 4)
            {
                break;
            }

            if(board[currentX,i] == null)
            {
                r.Add(new Vector2Int(currentX,i));
            }

            if(board[currentX,i] != null)
            {
                if(board[currentX,i].team != team)      
                {
                    r.Add(new Vector2Int(currentX,i));
                }
                break;
            }
        }

        //Up direction
        for(int i = currentY + 1; i < tileCountY; i++)
        {
            if(Board.tiles[currentX,i].layer == 4)
            {
                break;
            }

            if(board[currentX,i] == null)
            {
                r.Add(new Vector2Int(currentX,i));
            }

            if(board[currentX,i] != null)
            {
                if(board[currentX,i].team != team)      
                {
                    r.Add(new Vector2Int(currentX,i));
                }
                break;
            }
        }

        //Right direction
        for(int i = currentX + 1; i < tileCountX; i++)
        {
            if(Board.tiles[i,currentY].layer == 4)
            {
                break;
            }

            if(board[i,currentY] == null)
            {
                r.Add(new Vector2Int(i,currentY));
            }

            if(board[i,currentY] != null)
            {
                if(board[i,currentY].team != team)      
                {
                    r.Add(new Vector2Int(i,currentY));
                }
                break;
            }
        }

        //Left direction
        for(int i = currentX - 1; i >= 0; i--)
        {
            if(Board.tiles[i,currentY].layer == 4)
            {
                break;
            }
            
            if(board[i,currentY] == null)
            {
                r.Add(new Vector2Int(i,currentY));
            }

            if(board[i,currentY] != null)
            {
                if(board[i,currentY].team != team)      
                {
                    r.Add(new Vector2Int(i,currentY));
                }
                break;
            }
        }

        return r;
    }

    

    public override List<Vector4> GetMovesAI(ref Pieces[,] board, int tileCountX, int tileCountY)
    {
        List<Vector4> r = new List<Vector4>();

        //Down direction
        for(int i = currentY - 1; i >= 0; i--)
        {
            if(Board.tiles[currentX,i].layer == 4)
            {
                break;
            }

            if(board[currentX,i] == null)
            {
                r.Add(new Vector4(currentX,i,currentX,currentY));
            }

            if(board[currentX,i] != null)
            {
                if(board[currentX,i].team != team)      
                {
                    r.Add(new Vector4(currentX,i,currentX,currentY));
                }
                break;
            }
        }

        //Up direction
        for(int i = currentY + 1; i < tileCountY; i++)
        {
            if(Board.tiles[currentX,i].layer == 4)
            {
                break;
            }

            if(board[currentX,i] == null)
            {
                r.Add(new Vector4(currentX,i,currentX,currentY));
            }

            if(board[currentX,i] != null)
            {
                if(board[currentX,i].team != team)      
                {
                    r.Add(new Vector4(currentX,i,currentX,currentY));
                }
                break;
            }
        }

        //Right direction
        for(int i = currentX + 1; i < tileCountX; i++)
        {
            if(Board.tiles[i,currentY].layer == 4)
            {
                break;
            }

            if(board[i,currentY] == null)
            {
                r.Add(new Vector4(i,currentY,currentX,currentY));
            }

            if(board[i,currentY] != null)
            {
                if(board[i,currentY].team != team)      
                {
                    r.Add(new Vector4(i,currentY,currentX,currentY));
                }
                break;
            }
        }

        //Left direction
        for(int i = currentX - 1; i >= 0; i--)
        {
            if(Board.tiles[i,currentY].layer == 4)
            {
                break;
            }
            
            if(board[i,currentY] == null)
            {
                r.Add(new Vector4(i,currentY,currentX,currentY));
            }

            if(board[i,currentY] != null)
            {
                if(board[i,currentY].team != team)      
                {
                    r.Add(new Vector4(i,currentY,currentX,currentY));
                }
                break;
            }
        }

        return r;
    }

}
