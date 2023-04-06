using OnlineChessLibrary.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace OnlineChessLibrary.BoardElements;

public class Board : IEnumerable<Cell>
{
    private readonly Cell[,] _area;

    public Cell this[int row, int column]
    {
        get => _area[row, column];
    }

    public Board(bool fill = true)
    {
        _area = new Cell[8, 8];

        for (int i = 0; i < _area.GetLength(0); i++)
        {
            for (int j = 0; j < _area.GetLength(1); j++)
            {
                _area[i, j] = new Cell(new Point(i,j));
            }
        }

        if (fill)
        {
            _area[7, 2].State = State.WhiteBishop;
            _area[0, 2].State = State.BlackBishop;
        }
    }

    private void Deactivate()
    {
        foreach(var cell in _area)
        {
            cell.Active = false;
        }
    }

    public void ActivateCells(Cell cell)
    {
        if (cell.State == State.Empty) return;

        if (cell.State == State.WhiteBishop || cell.State == State.BlackBishop)
        {
            CellActivatior.BishopActivateCells(this, cell);
        }
    }

    public void Move(Cell startingCell, Cell finishingCell)
    {
        if (startingCell.State == State.Empty) throw new InvalidOperationException();


        finishingCell.State = startingCell.State;
        startingCell.State = State.Empty;

        Deactivate();
    }

    public IEnumerable<Cell> Cast()
    {
        return _area.Cast<Cell>();
    }

    public IEnumerator<Cell> GetEnumerator()
        => _area.Cast<Cell>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _area.GetEnumerator();
}