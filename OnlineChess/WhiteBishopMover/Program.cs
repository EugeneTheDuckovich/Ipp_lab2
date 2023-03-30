using OnlineChessLibrary.ByteSerializers;
using OnlineChessLibrary.BoardElements;
using System.Net.Sockets;

var tcpClient = new TcpClient();
await tcpClient.ConnectAsync("127.0.0.1", 8888);
var stream = tcpClient.GetStream();

try
{
    while (tcpClient.Connected)
    {
        var boardSizeBuffer = new byte[4];
        await stream.ReadAsync(boardSizeBuffer);
        var boardBytesSize = BitConverter.ToInt32(boardSizeBuffer);
        if (boardBytesSize == 0) throw new IOException();

        var boardBuffer = new byte[BitConverter.ToInt32(boardSizeBuffer)];
        await stream.ReadAsync(boardBuffer);
        var board = BoardByteSerializer.Deserialize(boardBuffer);
        if (board is null) continue;

        var whiteBishop = board.FirstOrDefault(c => c.State == State.WhiteBishop);
        if (whiteBishop is null) continue;

        board.ActivateCells(whiteBishop);
        var activeCells = board.Where(c => c.Active).ToArray();
        var endPointCell = activeCells[(new Random()).Next() % activeCells.Length];
        endPointCell.State = whiteBishop.State;

        var cellBuffer = CellByteSerializer.Serialize(endPointCell);
        await stream.WriteAsync(BitConverter.GetBytes(cellBuffer.Length));
        await stream.WriteAsync(cellBuffer);

        Console.WriteLine($"White bishop was moved from " +
            $"[{whiteBishop.Coordinates.X},{whiteBishop.Coordinates.Y}]" +
            $"to [{endPointCell.Coordinates.X},{endPointCell.Coordinates.Y}]");
    }
}
catch(IOException)
{
    Console.WriteLine("Connection lost");
}