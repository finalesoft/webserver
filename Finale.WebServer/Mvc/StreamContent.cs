using System.IO;

namespace Finale.WebServer.Mvc {
    class StreamContent : IRazorContent {
        Stream _stream;
        public StreamContent(Stream stream) { this._stream = stream; }

        void IRazorContent.Execute(System.IO.StreamWriter writer) {
            this._stream.Position = 0;
            this._stream.CopyTo(writer.BaseStream);
        }
    }
}