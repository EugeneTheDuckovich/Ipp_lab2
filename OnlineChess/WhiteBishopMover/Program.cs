using OnlineChessLibrary.ByteSerializers;
using OnlineChessLibrary.BoardElements;
using System.Net.Sockets;

var tcpClient = new TcpClient();
await tcpClient.ConnectAsync("127.0.0.1", 8888);
var stream = tcpClient.GetStream();

while (tcpClient.Connected)
{
    var boardSizeBuffer = new byte[4];
    await stream.ReadAsync(boardSizeBuffer);
    var boardBytesSize = BitConverter.ToInt32(boardSizeBuffer);
    if (boardBytesSize == 0)
    {
        Console.WriteLine("Connection lost");
        Environment.Exit(0);
    }

    var boardBuffer = new byte[BitConverter.ToInt32(boardSizeBuffer)];
    await stream.ReadAsync(boardBuffer);
    var board = BoardByteSerializer.Deserialize(boardBuffer);
    if (board is null) continue;

    var whiteBishopCell = board.FirstOrDefault(c => c.State == State.WhiteBishop);
    if (whiteBishopCell is null) continue;

    board.ActivateCells(whiteBishopCell);
    var activeCells = board.Where(c => c.Active).ToArray();
    var endPointCell = activeCells[(new Random()).Next() % activeCells.Length];
    board.Move(whiteBishopCell, endPointCell);

    var responseBoardBuffer = BoardByteSerializer.Serialize(board);
    await stream.WriteAsync(BitConverter.GetBytes(responseBoardBuffer.Length));
    await stream.WriteAsync(responseBoardBuffer);

    Console.WriteLine($"White bishop was moved from " +
        $"[{whiteBishopCell.Coordinates.X},{whiteBishopCell.Coordinates.Y}]" +
        $"to [{endPointCell.Coordinates.X},{endPointCell.Coordinates.Y}]");
}