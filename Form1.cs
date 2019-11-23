using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace lambda
{
    public partial class Form1 : Form
    {
        private static void Log(object msg)
        {
            System.Diagnostics.Trace.WriteLine(msg.ToString());
        }

        public Form1()
        {
            InitializeComponent();
        }

        private async void cmdTest_Click(object sender, EventArgs e)
        {
            cmdTest.Enabled = false;
            await test4().ConfigureAwait(true);
            // test5();
            cmdTest.Enabled = true;
        }

        private void test5()
        {
            Dictionary<int, string> d = new Dictionary<int, string>
            {
                { 1, "one" },
                { 2, "two" },
                { 3, "three" }
            };

            foreach(var key in d.Keys.OrderByDescending(k => k).Skip(1).Take(2))
            {
                Log($"{key} => {d[key]}");
            }
        }

        private void test1()
        {
            //List<int> nums = Enumerable.Range(1, 10).ToList();
            IEnumerable<int> nums = Enumerable.Range(1, 10);

            IEnumerable<int> result1 = nums.Where(num => num < 5);

            IEnumerable<decimal> result2 = result1.Select(num => (decimal)num + 0.01M);

            foreach (var d in result2)
            {
                Log($"{d:0.00}");
            }

            IEnumerable<decimal> result3 = result1.Select(num => (decimal)num + 0.01M);

            foreach (var d in result3)
            {
                Log($"{d:0.00}");
            }

        }

        private void test2()
        {
            IEnumerable<int> nums = Enumerable.Range(1, 10);

            IEnumerable<decimal> result = nums
                .Where(num => num < 5)
                .Select(num => (decimal)num + 0.01M)
                .OrderByDescending(num => num)
                .Skip(2)
                .Take(2);

            foreach (var d in result)
            {
                Log($"{d:0.00}");
            }
        }

        private void test3()
        {
            // Log("abcdef".Reverse());

            IEnumerable<char> chars = "abcdef".ReverseEnumerable();
            //foreach(char c in chars)
            //{
            //    Log(c);
            //}
            chars.ToList().ForEach(c => Log(c));
        }

        private async Task test4()
        {
            List<Uri> urls = new List<Uri>()
            {
                new Uri("https://www.kenneth-truyers.net/2016/05/12/yield-return-in-c/"),
                new Uri("https://dailycaller.com/2019/11/02/nancy-pelosi-democratic-party-left-wing-election/"),
                new Uri("https://finance.yahoo.com/quote/SCOR?p=SCOR&.tsrc=fin-srch"),
                new Uri("https://weather.com/weather/hourbyhour/l/70db2a5645ebed723839fc9c8c84f0987938b3fa739ade5fd47d8ef3bfda4b0e")
            };

            List<Task<(string, int, long)>> Tasks = new List<Task<(string, int, long)>>();
            int index = 0;
            urls.ForEach(url => Tasks.Add(GetAsync(url, index++)));

            while (Tasks.Any())
            {
                Task<(string content, int index, long runtime)> completedTask = await Task.WhenAny(Tasks).ConfigureAwait(false);

                Log("---------------------------------------------------------------------------");
                Log($"[index: {completedTask.Result.index}][{urls[completedTask.Result.index]}] => [length: {completedTask.Result.content.Length:#,##0}] [runtime: {completedTask.Result.runtime:#,##0} ms]");
                //Log($"{completedTask.Result.content}");
                parseHREFs(completedTask.Result.content);
                Log("---------------------------------------------------------------------------");

                Tasks.Remove(completedTask);
            }

        }

        private static void parseHREFs(string content)
        {
            // href="https://dailycallermerchandise.com/" 

            int startIndex = 0;
            int endIndex = 0;
            while (true)
            {
                startIndex = content.IndexOf("href=\"http", startIndex, StringComparison.Ordinal);
                if (startIndex == -1) break;
                startIndex += 6;
                endIndex = content.IndexOf("\"", startIndex, StringComparison.Ordinal);
                string url = content.Substring(startIndex, endIndex - startIndex);
                Log($"   {url}");
            }
        }

        private static async Task<(string, int, long)> GetAsync(Uri url, int index)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "lambda");

                HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    sw.Stop();
                    return (content, index, sw.ElapsedMilliseconds);
                }
                else
                {
                    Log(response.StatusCode.ToString());
                    return (response.StatusCode.ToString(), index, -1L);
                }
            }
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void cmdTest2_Click(object sender, EventArgs e)
        {
            Log("IsNullOrEmpty => " + "".IsNullOrEmpty());
            Log("IsNotNullOrEmpty => " + "".IsNotNullOrEmpty());
        }
    }
}
