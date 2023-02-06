using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LabelService3.Models
{
    class Http
    {
        public static async void updateNova(Label data)
        {
            Console.WriteLine(String.Format("{0}/labels/{1}/printed", Globals.NovaUrl, data.order_id));

            Uri Address = new Uri(String.Format("{0}/labels/{1}/printed", Globals.NovaUrl, data.order_id));
            HttpClient client = new HttpClient();
            client.BaseAddress = Address;

            var response = await client.GetAsync(Address);
        }

        public async static void updateCore(Label data)
        {
            var url = String.Format("{0}/labels/{1}/printed", Globals.CoreUrl, data.order_id);

            async Task<string> GetResponseString(Label label)
            {
                var httpClient = new HttpClient();

                var parameters = new Dictionary<string, string>();
                parameters["computer_name"] = System.Environment.MachineName;
                parameters["pdf_base64"] = label.pdf_base64;
                parameters["pdf_path"] = label.pdf_url;
                parameters["printed"] = label.printed;
                parameters["printed_at"] = DateTime.Now.ToString("");

                var response = await httpClient.PostAsync(url, new FormUrlEncodedContent(parameters));
                var contents = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(contents);
                return contents;
            }

            await GetResponseString(data);
        }

        public static void PingAlive()
        {
            async void GetResponseString()
            {
                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(Globals.PingUrl);
                Console.WriteLine(response);
            }

            GetResponseString();
        }
    }
}
