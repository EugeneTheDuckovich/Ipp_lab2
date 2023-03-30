using OnlineChessLibrary.BoardElements;
using OnlineChessLibrary.Memento;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace OnlineChessLibrary.ByteSerializers;

public static class BoardByteSerializer
{
    public static byte[] Serialize(Board board)
    {
        try
        {
            var boardMemento = board.Select(c => c.State).ToArray();
            return JsonSerializer.SerializeToUtf8Bytes<State[]>(boardMemento);
        }
        catch
        {
            return new byte[0];
        }
    }

    public static Board? Deserialize(byte[] bytes)
    {
        try
        {
            Board board = new Board(false);
            State[] stateMap;

            using (var stream = new MemoryStream(bytes))
            {
                stateMap = JsonSerializer.Deserialize<State[]>(stream);                
            }

            for(int i = 0; i < 8; i ++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j].State = stateMap[i*8+j];
                }
            }

            return board;
        }
        catch 
        {
            return null;
        }
    }
}
