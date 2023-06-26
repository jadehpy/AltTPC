
using System;
using System.Text.RegularExpressions;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace SolidTPC {
    internal static class Program {


        [STAThread]
        static void Main(string[] args) {

            ApplicationConfiguration.Initialize();

            if (args.Length > 0) {
                var list = new List<Node>();              

                Parser.Run(new FileInfo(args[0]));

            } else {

                bool hhhh = Commands.DB.ContainsKey("msg");

                try {


                    FileInfo tpc = new FileInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\tpc.exe");
                    if (!tpc.Exists) {
                        throw Error.Call(Error.TPC_Does_Not_Exist);
                    }

                    var sw = new Stopwatch();
                    
                    FileInfo f;
                    f = new(@"...\testcode.txt");
                    sw.Start();
                    var ip = Parser.Run(f);
                    sw.Stop();

                    Application.Run(new Form2($"Elapsed : {sw.ElapsedMilliseconds} ms\r\n\r\nInput :\r\n\r\n{TestCode.GetCode()}\r\n\r\n\r\n\r\nParsed : \r\n\r\n{Parser.NodesToString(ip)}\r\n\r\n{m.log}"));


                } catch (Exception e) {

                    m.log = new(Parser.NodesToString(Parser.tree).ToString() + m.log.ToString());

                    Application.Run(new Form2(e.Message + "\r\n\r\n" + e.StackTrace + "\r\n\r\n" + m.log));

                }
            }
        }
    }



    class m {

        public static StringBuilder log = new("log : \r\n\r\n");
        const bool showMessage = false;

        // デバッグ用
        public static void s<T>(params T[] s) {
            StringBuilder sb = new();
            foreach (T t in s) {
                if (t is not null) {
                    sb.Append(t.ToString());
                    sb.Append("\r\n");
                }
            }
            if (showMessage) {
                MessageBox.Show(sb.ToString());
            }
            log.Append(sb.ToString() + "\r\n");
            Console.WriteLine(sb.ToString());
        }

        public static void console(string s) {
            Console.WriteLine(s);
        }

        public static bool Between(int x, int min, int max) {
            return (x >= min && x <= max);
        }
    }
}