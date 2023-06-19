
using System;
using System.Text.RegularExpressions;

namespace AltTPC {
    internal static class Program {


        [STAThread]
        static void Main(string[] args) {

            ApplicationConfiguration.Initialize();

            if (args.Length > 0) {
                var list = new List<Node>();              

                Interpreter.Run(new FileInfo(args[0]));

            } else {

                bool a = Commands.DB.ContainsKey("msg");


                try {
                    var sw = new System.Diagnostics.Stopwatch();
                    
                    FileInfo f;
                    f = new(@"C:\Users\Jade\OneDrive\AltTPC\testcode.txt");
                    if (!f.Exists) {
                        f = new(@"C:\Users\Jade_\OneDrive\AltTPC\testcode.txt");
                    }
                    sw.Start();
                    var ip = Interpreter.Run(f);
                    sw.Stop();

                    Application.Run(new Form2($"Elapsed : {sw.ElapsedMilliseconds} ms\r\n\r\nInput :\r\n\r\n{TestCode.GetCode()}\r\n\r\n\r\n\r\nParsed : \r\n\r\n{Interpreter.NodesToString(ip)}\r\n\r\n{m.log}"));


                } catch (Exception e) {

                    Application.Run(new Form2(e.Message + "\r\n\r\n" + e.StackTrace + "\r\n\r\n" + m.log));

                }
            }
        }
    }
}