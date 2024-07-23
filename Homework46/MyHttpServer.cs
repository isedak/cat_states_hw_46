using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using Homework46.State;

namespace Homework46
{
    public class MyHttpServer
    {
        private Thread _serverThread;
        private string _siteDirectory;
        private HttpListener _listener;
        private int _port;
        private Random _random = new Random();
        protected static Cat Kitty { get; set; }

        public MyHttpServer(string siteDirectory, int port)
        {
            Initialize(siteDirectory, port);
        }

        private void Initialize(string path, int port)
        {
            _siteDirectory = path;
            _port = port;
            _serverThread = new Thread(Listen);
            _serverThread.Start();
            Console.WriteLine($"Server started at port {port}");
        }

        public void Stop()
        {
            _serverThread.Abort();
            _listener.Stop();
        }

        private void Listen()
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://localhost:{_port}/");
            _listener.Start();

            while (true)
            {
                try
                {
                    HttpListenerContext context = _listener.GetContext();
                    Process(context);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void Process(HttpListenerContext context)
        {
            string request = context.Request.Url.AbsolutePath;
            string filePath;
            if (context.Request.HasEntityBody)
            {
                filePath = ManagePostRequest(context);
            }
            else
            {
                filePath = BuildResponseContent(context, request);
            }

            SendResponsePage(context, filePath);
        }

        private void BuildPageHtml(HttpListenerContext context)
        {
            string template = File.ReadAllText(_siteDirectory + "/www/layout_cat.html");
            template = template.Replace("@Name", $"{Kitty.Name}");
            template = template.Replace("@Age", $"{Kitty.Age} years old");
            template = template.Replace("@AvatarPath", $"{Kitty.AvatarPath}");
            template = template.Replace("@AvatarNote", $"{Kitty.AvatarNote}");
            template = template.Replace("@Note", $"{Kitty.Note}");
            template = template.Replace("@Mood", $"{Kitty.Mood}");
            template = template.Replace("@Satiety", $"{Kitty.Satiety}");
            string htmlText = template;
            File.WriteAllText(_siteDirectory + "/www/cat_stats.html", htmlText, Encoding.UTF8);
        }

        private string BuildResponseContent(HttpListenerContext context, string request)
        {
            if (context.Request.QueryString.HasKeys())
            {
                Cat cat = DataLoader.GetCat("../../../cat.json");
                Kitty = cat;
                NameValueCollection query = context.Request.QueryString;
                string action = query["catAction"];
                if (Kitty.Mood < -5 && Kitty.Satiety < -10 || Kitty.Satiety < -15 || Kitty.Mood < -15)
                {
                    Kitty.ShowCatStateAvatarPath();
                    Kitty.State = new SleepingState();
                    Kitty.Note = $"It doesn't want anything!";
                    DataLoader.SaveFile(Kitty, "../../../cat.json");
                    BuildPageHtml(context);
                    return $"/www/cat_stats.html";
                }

                switch (action)
                {
                    case "feed":
                        if (Kitty.Satiety >= 100)
                        {
                            Kitty.Note = $"It doesn't want to eat!";
                            DataLoader.SaveFile(Kitty, "../../../cat.json");
                            BuildPageHtml(context);
                            return $"/www/cat_stats.html";
                        }
                        else
                        {
                            Kitty.Feed();
                            DataLoader.SaveFile(Kitty, "../../../cat.json");
                            BuildPageHtml(context);
                            return $"/www/cat_stats.html";
                        }

                    case "play":
                        if (Kitty.Satiety <= 0 && Kitty.StateName != "slept")
                        {
                            Kitty.Note = $"It doesn't want to play!";
                            DataLoader.SaveFile(Kitty, "../../../cat.json");
                            BuildPageHtml(context);
                            return $"/www/cat_stats.html";
                        }
                        else
                        {
                            if (Kitty.Mood >= 100 && Kitty.StateName != "slept")
                            {
                                Kitty.Note = $"It can't be happier than now!";
                                DataLoader.SaveFile(Kitty, "../../../cat.json");
                                BuildPageHtml(context);
                                return $"/www/cat_stats.html";
                            }

                            Kitty.Play();
                            DataLoader.SaveFile(Kitty, "../../../cat.json");
                            BuildPageHtml(context);
                            return $"/www/cat_stats.html";
                        }
                    case "sleep":
                        Kitty.PutToSleep();
                        DataLoader.SaveFile(Kitty, "../../../cat.json");
                        BuildPageHtml(context);
                        return $"/www/cat_stats.html";
                    default:
                        return $"/www{request}";
                }
            }

            return $"/www{request}";
        }

        private string ManagePostRequest(HttpListenerContext context)
        {
            var stream = context.Request.InputStream;
            var encoding = context.Request.ContentEncoding;
            var reader = new StreamReader(stream, encoding);
            string data = reader.ReadToEnd();
            var queryValues = HttpUtility.ParseQueryString(data);
            string value = queryValues.Get("nameOfCat");
            if (!string.IsNullOrEmpty(value))
            {
                if (value.Length < 16)
                {
                    value = char.ToUpper(value[0]) + value.Substring(1).ToLower();
                    Kitty = CreateNewCat(value);
                    DataLoader.SaveFile(Kitty, "../../../cat.json");
                    BuildPageHtml(context);
                    return "/www/cat_stats.html";
                }

                return "/www/index.html";
            }

            return "/www/index.html";
        }

        private Cat CreateNewCat(string name)
        {
            int step = 5;
            int min = 5;
            int max = 95;
            int number = _random.Next(min, max);
            int satiety = number / step * step;
            number = _random.Next(min, max);
            int mood = number / step * step;
            name = char.ToUpper(name[0]) + name.Substring(1).ToLower();
            int randomState = _random.Next(1, 3);
            switch (randomState)
            {
                case 1:
                {
                    Cat cat = new Cat(name, satiety, mood, new FedState());
                    return cat;
                }
                default:
                {
                    Cat cat = new Cat(name, satiety, mood, new SleepingState());
                    return cat;
                }
            }
        }

        private void SendResponsePage(HttpListenerContext context, string filename)
        {
            string time = DateTime.Now.ToString("dd.MM.yyyy, HH:mm:ss");
            Console.WriteLine($"({time})\n" +
                              $"HttpMethod: {context.Request.HttpMethod},\n" +
                              $"Request: {filename}\n");
            filename = filename.Substring(1);
            filename = Path.Combine(_siteDirectory, filename);

            if (File.Exists(filename))
            {
                try
                {
                    Stream fileStream = new FileStream(filename, FileMode.Open);
                    context.Response.ContentType = GetContentType(filename);
                    context.Response.ContentLength64 = fileStream.Length;
                    byte[] buffer = new byte[16 * 1024];
                    int dataLength;
                    do
                    {
                        dataLength = fileStream.Read(buffer, 0, buffer.Length);
                        context.Response.OutputStream.Write(buffer, 0, dataLength);
                    } while (dataLength > 0);

                    fileStream.Close();
                    context.Response.StatusCode = (int) HttpStatusCode.OK;
                    context.Response.OutputStream.Flush();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                }
            }
            else
            {
                context.Response.StatusCode = (int) HttpStatusCode.NotFound;
            }

            context.Response.OutputStream.Close();
        }

        private string GetContentType(string filename)
        {
            var dictionary = new Dictionary<string, string>
            {
                {".css", "text/css"},
                {".html", "text/html"},
                {".ico", "image/x-icon"},
                {".js", "application/x-javascript"},
                {".json", "application/json"},
                {".png", "image/png"},
                {".svg", "image/svg+xml"}
            };

            string fileExtension = Path.GetExtension(filename);
            dictionary.TryGetValue(fileExtension, out string contentType);
            return contentType;
        }
    }
}