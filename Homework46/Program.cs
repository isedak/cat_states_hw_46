using System;
using System.IO;
using System.Text;

namespace Homework46
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            string path = Directory.GetCurrentDirectory();
            MyHttpServer server = new MyHttpServer(path, 8000);
        }
    }
}