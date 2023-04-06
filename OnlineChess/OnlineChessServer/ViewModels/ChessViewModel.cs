using OnlineChessLibrary.BoardElements;
using OnlineChessLibrary.ByteSerializers;
using OnlineChessLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace OnlineChessServer.ViewModels
{
    public class ChessViewModel : NotifyPropertyChanged
    {
        private Board _board;
        private TcpListener _tcpListener;
        private List<TcpClient> _tcpClients;
        private List<CancellationTokenSource> _cancellationTokens;
        private Mutex _mutex;

        public IEnumerable<char> Numbers { get; }
        public IEnumerable<char> Letters { get; }

        public Board Board
        {
            get => _board;
            set
            {
                _board = value;
                OnPropertyChanged(nameof(Board));
            }
        }

        public ChessViewModel()
        {
            Board= new Board(true);
            Numbers = "87654321";
            Letters = "ABCDEFGH";
            _tcpClients= new List<TcpClient>();
            _cancellationTokens = new List<CancellationTokenSource>();
            _mutex = new Mutex();

            _tcpListener = new TcpListener(IPAddress.Loopback,8888);
            _tcpListener.Start();
            Task.Run(async () => await AcceptClientsAsync());
        }

        private async Task AcceptClientsAsync()
        {
            while (true)
            {
                Task.Delay(1000).Wait();
                var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                _mutex.WaitOne();
                _tcpClients.Add(tcpClient);

                var tokenSource = new CancellationTokenSource();
                _cancellationTokens.Add(tokenSource);
                var processClientTask = Task.Run(
                    async () => await ProcessClient(tcpClient),
                    tokenSource.Token);
                _mutex.ReleaseMutex();
            }
        }

        private async Task ProcessClient(TcpClient tcpClient)
        {
            while (true)
            {
                await Task.Delay(3000);
                var stream = tcpClient.GetStream();
                var boardBuffer = BoardByteSerializer.Serialize(_board);

                await stream.WriteAsync(BitConverter.GetBytes(boardBuffer.Length));
                await stream.WriteAsync(boardBuffer);

                var cellSizeBuffer = new byte[4];
                await stream.ReadAsync(cellSizeBuffer);
                var cellBuffer = new byte[BitConverter.ToInt32(cellSizeBuffer,0)];
                await stream.ReadAsync(cellBuffer);
                var responseCell = CellByteSerializer.Deserialize(cellBuffer);

                if (responseCell is null) continue;

                _mutex.WaitOne();
                var oldPosition = _board.FirstOrDefault(c => c.State == responseCell.State);
                if (oldPosition is null) continue;

                oldPosition.State = State.Empty;

                Board[responseCell.Coordinates.X, responseCell.Coordinates.Y].State = responseCell.State;
                _mutex.ReleaseMutex();
            }
        }

        public void CloseConnections()
        {
            _cancellationTokens.ForEach(c =>
            {
                c.Cancel();
            });
            _tcpClients.ForEach(t => t.Close());
            _tcpListener.Stop();
        }
    }
}
