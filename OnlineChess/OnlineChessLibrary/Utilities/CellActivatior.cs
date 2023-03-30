using OnlineChessLibrary.BoardElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineChessLibrary.Utilities;

public static class CellActivatior
{
    public static void BishopActivateCells(Board board, Cell cell)
    {
        if (board is null) return;
        if (cell is null) return;
        if (!board.Contains(cell)) return;
        if (cell.State != State.BlackBishop && cell.State != State.WhiteBishop) return;

        int X = cell.Coordinates.X;
        int Y = cell.Coordinates.Y;
        for (int i = 1; X + i < 8 && Y + i < 8; i++)
        {
            board[X + i, Y + i].Active = true;
            if (board[X + i, Y + i].State != State.Empty) break;
        }
        for (int i = 1; X + i < 8 && Y >= i; i++)
        {
            board[X + i, Y - i].Active = true;
            if (board[X + i, Y - i].State != State.Empty) break;
        }
        for (int i = 1; X >= i && Y + i < 8; i++)
        {
            board[X - i, Y + i].Active = true;
            if (board[X - i, Y + i].State != State.Empty) break;
        }
        for (int i = 1; X >= i && Y >= i; i++)
        {
            board[X - i, Y - i].Active = true;
            if (board[X - i, Y - i].State != State.Empty) break;
        }
    }
}