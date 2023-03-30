using OnlineChessLibrary.Utilities;
using System.Drawing;

namespace OnlineChessLibrary.BoardElements;

public enum State
{
    Empty,
    BlackBishop,
    WhiteBishop,
}
public class Cell : NotifyPropertyChanged
{
    private State _state;
    private bool _active;
    public readonly Point Coordinates;

    public State State
    {
        get=> _state; 
        set 
        {
            _state = value;
            OnPropertyChanged(nameof(State));
        }
    }

    public bool Active
    {
        get => _active;
        set
        {
            _active = value;
            OnPropertyChanged(nameof(Active));
        }
    }

    public Cell() { }

    public Cell(Point coordinates)
    {
        this.Coordinates = coordinates;
        State= State.Empty;
    }
}
