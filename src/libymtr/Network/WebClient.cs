using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

namespace libymtr.Network {
    public delegate void ResponseHandler(HttpResponseMessage message, bool isErro = false);

    public class WebClient {


        public event ResponseHandler OnResponse;

        private static HttpClient m_client = null;
        private CancellationTokenSource m_canceller = new CancellationTokenSource();

        public WebClient(int timeoutSec = 60) {
            OnResponse += _OnResponse;
        }
        ~WebClient() {
            m_client.Dispose();
        }

        public async void SendAsync(HttpRequestMessage request) {
            try {
                var response = await m_client.SendAsync(request, m_canceller.Token);
                OnResponse(response);
            } catch {
                Cancel();
                HttpResponseMessage msg = new HttpResponseMessage();
                msg.Content = new StringContent("Network Error");
                OnResponse(msg, true);
            }
        }
        public void Cancel() {
            m_canceller.Cancel();
            m_canceller = new CancellationTokenSource();
        }

        private void _OnResponse(HttpResponseMessage message, bool isError) {
            //  Empty event
        }
    }
}
