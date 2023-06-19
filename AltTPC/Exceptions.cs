using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AltTPC {
    internal class Error {

        // エラーメッセージ定数

        public static string

            Defined_Name_Begins_With_Number = "定義名は数字以外で始まる必要があります",

            Invalid_Inextractable_Token_In_String = "文字列内に展開できない不正なトークンがあります",

            Invalid_Number_Literal = "不正な数値リテラルです。数値は10進数の整数もしくは小数、16進数、2進数のどれかである必要があります",

            Invalid_Token = "不正な記号です",

            Invalid_Token_In_Block = "不正なトークンがブロック内に存在します",

            Invalid_Main_Command = "存在しないメインコマンド名です",

            Invalid_Close_Token = "不正な閉じ括弧です",

            Float_Cannot_Used_In_Not_Decimal_Number = "小数点は十進数の数値にのみ利用可能です",

            Invalid_Operator = "不正な演算子です",

            Invalid_Command_Name_After_Atmark = "@ の後には有効なメインコマンドが必要です",

            Invalid_Arguments_In_Event_Command = "イベントコマンドの引数が不正です",

            Semicolon_Does_Not_Exist = "セミコロンが存在しません",

            Opr_PM_Has_Invalid_Value = "符号演算子の持つ値が不正です",


            END = "";






        // エラー位置の特定

        public static string GetPositionInSource(Node node) {

            string src = Interpreter.sourceList[node.indexInSourceList].code;

            int line = 1;
            int idx = 1;

            for (int i = 0; i < node.positionInSource; i++) {
                if (src[i] == '\n') {
                    line++;
                    idx = 0;
                }
                idx++;
            }

            return $"{Interpreter.sourceList[node.indexInSourceList].name}, {line} 行目, {idx} 文字目, \"{node.word}\"";

        }


        // エラー呼び出し

        public static Exception Call(string error, Node node) {
            return new Exception($"{error} : {GetPositionInSource(node)}");
        }
    }
}
