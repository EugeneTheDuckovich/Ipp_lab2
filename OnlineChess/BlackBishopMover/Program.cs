using OnlineChessLibrary.ByteSerializers;
using OnlineChessLibrary.BoardElements;
using System.Net.Sockets;

var tcpClient = new TcpClient();
await tcpClient.ConnectAsync("127.0.0.1", 8888);
var stream = tcpClient.GetStream();
try
{
    while (true)
    {
        await Task.Delay(3000);
        var boardSizeBuffer = new byte[4];
        await stream.ReadAsync(boardSizeBuffer);
        var boardBytesSize = BitConverter.ToInt32(boardSizeBuffer);
        if (boardBytesSize == 0) throw new IOException();

        var boardBuffer = new byte[BitConverter.ToInt32(boardSizeBuffer)];
        await stream.ReadAsync(boardBuffer);
        var board = BoardByteSerializer.Deserialize(boardBuffer);
        if (board is null) continue;

        var blackBishop = board.FirstOrDefault(c => c.State == State.BlackBishop);
        if (blackBishop is null) continue;

        board.ActivateCells(blackBishop);
        var activeCells = board.Where(c => c.Active).ToArray();
        var endPointCell = activeCells[(new Random()).Next() % activeCells.Length];
        endPointCell.State = blackBishop.State;

        var cellBuffer = CellByteSerializer.Serialize(endPointCell);
        await stream.WriteAsync(BitConverter.GetBytes(cellBuffer.Length));
        await stream.WriteAsync(cellBuffer);

        Console.WriteLine($"Black bishop was moved from " +
            $"[{blackBishop.Coordinates.X},{blackBishop.Coordinates.Y}]" +
            $"to [{endPointCell.Coordinates.X},{endPointCell.Coordinates.Y}]");
    }
}
catch (IOException)
{
    Console.WriteLine("Connection lost");
}