using OnlineChessLibrary.BoardElements;
using OnlineChessLibrary.ByteSerializers;
using OnlineChessLibrary.Utilities;
using System;
using System.Collections.Generic;
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
            _mutex = new Mutex();

            _tcpListener = new TcpListener(IPAddress.Loopback,8888);
            _tcpListener.Start();
            Task.Run(async () => await AcceptClientsAsync());
        }

        private async Task AcceptClientsAsync()
        {
            while (_tcpClients.Count < 2)
            {
                var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                _tcpClients.Add(tcpClient);

                var tokenSource = new CancellationTokenSource();
                
                var processClientTask = Task.Run(
                    async () => await ProcessClient(tcpClient));
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

                var responseBoard = BoardByteSerializer.Deserialize(cellBuffer);
                if (responseBoard is null) continue; 
                
                _mutex.WaitOne();
                Board = responseBoard;
                _mutex.ReleaseMutex();
            }
        }

        public void CloseConnections()
        {
            _tcpClients.ForEach(t => t.Close());
            _tcpListener.Stop();
        }
    }
}
