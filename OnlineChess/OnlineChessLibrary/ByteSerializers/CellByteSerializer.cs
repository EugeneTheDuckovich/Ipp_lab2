using OnlineChessLibrary.BoardElements;
using OnlineChessLibrary.Memento;
using System;
using System.IO;
using System.Text;
using System.Text.Json;

namespace OnlineChessLibrary.ByteSerializers
{
    public static class CellByteSerializer
    {
        public static byte[] Serialize(Cell cell)
        {
            try
            {
                var cellDTO = new CellDTO { X= cell.Coordinates.X,Y=cell.Coordinates.Y,State = cell.State };
                var buffer = JsonSerializer.SerializeToUtf8Bytes<CellDTO>(cellDTO);
                var jsonString = Encoding.UTF8.GetString(buffer);
                return buffer;
            }
            catch 
            {
                return new byte[0];
            }
        }

        public static Cell? Deserialize(byte[] bytes)
        {
            Cell? cell = null;

            try
            {
                using (var stream = new MemoryStream(bytes))
                {
                    var cellDTO = JsonSerializer.Deserialize<CellDTO>(stream);
                    if (cellDTO is null) throw new ArgumentException();
                    cell = new Cell(new System.Drawing.Point(cellDTO.X,cellDTO.Y));
                    cell.State = cellDTO.State;
                }
            }
            catch { }

            return cell;
        }
    }
}