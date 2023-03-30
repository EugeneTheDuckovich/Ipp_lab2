using OnlineChessLibrary.BoardElements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineChessLibrary.Memento;

public class CellDTO
{
    public int X { get; set; }
    public int Y { get; set; }
    public State State { get; set; }
}
