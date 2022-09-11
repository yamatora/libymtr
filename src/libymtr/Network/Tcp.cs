#pragma warning disable CS0168 // 変数は宣言されていますが、使用されていません
#pragma warning disable CS0649 //
#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace libymtr.Network {
    public class TcpServer {
        public event EventHandler TcpConnected;

        private readonly TcpListener m_server;
        private NetworkStream m_stream;

        public TcpServer(int port) {
            IPAddress? addr = IP.GetLocalIP();
            if (addr == null) {
                return;
            }
            m_server = new TcpListener(addr, port);

            WaitConnection().Start();
        }

        public async Task<bool> WaitConnection() {
            var client = await m_server.AcceptTcpClientAsync();

            if (!client.Connected) {
                return false;
            }
            TcpConnected(this, new EventArgs());
            return true;
        }
        public async Task<bool> Send(byte[] data, CancellationToken token = default) {
            if (!m_stream.CanWrite) {
                return false;
            }
            await m_stream.WriteAsync(data, token);
            if (token.IsCancellationRequested) {
                return false;
            }
            return true;
        }
    }
    public class TcpReceiver {
        private readonly TcpClient m_client;
        private NetworkStream m_stream;

        public TcpReceiver() {
            m_client = new TcpClient();
        }

        public async Task<bool> Connect(string dst, int port) {
            try {
                await m_client.ConnectAsync(dst, port);
                m_stream = m_client.GetStream();
            } catch (Exception e) {
                return false;
            }
            return true;
        }
        public async Task<bool> Send(byte[] data, CancellationToken token = default) {
            if (!m_stream.CanWrite) {
                return false;
            }
            await m_stream.WriteAsync(data, token);
            if (token.IsCancellationRequested) {
                return false;
            }
            return true;
        }
    }
}
