using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

#pragma warning disable CS8618 // null 非許容のフィールドには、コンストラクターの終了時に null 以外の値が入っていなければなりません。Null 許容として宣言することをご検討ください。
#pragma warning disable CS8604 // Null 参照引数の可能性があります。


namespace SolidTPC {
    internal class Generator {


        /*
            渡されたノードからコードの生成を行う   
            コードはイベント単位で分割されて./generated/に一定の命名規則で出力される
            ./generated/__lastGenerated.txtにあるハッシュと今のファイルのハッシュを比較し、
            一致していないor新規に存在するもののみを選びルートファイルにincludeしていく
        */

        public enum FileType {
            Root,
            CommonEventFile,
            MapEventFile
        }

        public class GeneratedFile {

            FileType filetype;
            public StringBuilder code;
            string hash;


            public GeneratedFile(FileType ft, string code) {

                filetype = ft;
                this.code = new();
                this.code.Append(code);

                var sha = SHA256.Create();
                var arr1 = Encoding.UTF8.GetBytes(code);
                var arr2 = sha.ComputeHash(arr1);

                StringBuilder sb = new();
                for (int i = 0; i < arr2.Length; i++) {
                    sb.Append(arr2[i]);
                    hash = sb.ToString();
                }
            }
        }


        public static void Export(List<GeneratedFile> files) {

            DirectoryInfo cdir = new(Path.GetDirectoryName(Application.ExecutablePath));

            // generatedフォルダをクリーンに（なければ作成）

            // __files.txtとハッシュを照合

            // __main.txtファイルを作成

            // __files.txtにハッシュを保存

        }



        public static List<GeneratedFile> Run(Node node) {

            List<GeneratedFile> files = new();

            GeneratedFile root = new(FileType.Root, "");


            return files;
        }
    }
}
