using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Globalization;

#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8604 // Null 参照引数の可能性があります。

namespace SolidTPC {
    partial class NodeController {


        // ======================================================================================================================================================

        // ノードの種類判別

        static Regex regNum_dec = new(@"^\d+$");
        static Regex regNum_hex = new(@"^0x[a-fA-F0-9]+$");
        static Regex regNum_bin = new(@"^0b[01]+$");
        static char[] number = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        static string[] opr_single = new[] { "!" };
        static char[] opr_mdm = new[] { '*', '/', '%' };
        static char[] opr_pm = new[] { '+', '-' };
        static string[] opr_shift = new[] { "<<", ">>" };
        static string[] opr_compare = new[] { ">", "<", ">=", "<=" };
        static string[] opr_asg = new[] { "=", "+=", "-=", "*=", "/=", "%=", "|=", "&=", "^=", "<<=", ">>=" };
        static string[] types = new[] { Node.KEYWORD_NUMBER, Node.KEYWORD_STRING, Node.KEYWORD_ARRAY, Node.KEYWORD_DICTIONARY };
        static string[] modifiers = new[] { Node.KEYWORD_CONST, Node.KEYWORD_STATIC, Node.KEYWORD_PRIVATE, Node.KEYWORD_GLOBAL };

        /**
            <summary>
            wordを元にトークンの種類を判別して保存する    
            </summary>    
        */
        public static void SetNodeType(Node node) {

            char w = node.word[0];
            string ws = node.word;

            if (w == '@') {  // コマンド・CODE型

                node.baseType = NodeType.AT;
                node.hasChild = true;


            } else if (ws == "//") {  // 行コメント

                node.baseType = NodeType.COM_LINE;
                node.hasChild = true;


            } else if (ws == "/*") {  // 範囲コメント

                node.baseType = NodeType.COM_RANGE;
                node.hasChild = true;


            } else if (w == '{') {  // ブロック

                node.baseType = NodeType.BLOCK;
                node.hasChild = true;
                node.endToken = '}';
                node.isBracket = true;


            } else if (w == '(') {  // 括弧

                node.baseType = NodeType.BRACKET;
                node.hasChild = true;
                node.endToken = ')';
                node.isBracket = true;


            } else if (w == '[') {  // 鍵括弧

                node.baseType = NodeType.BRACKET_SQUARE_ARR;
                node.hasChild = true;
                node.endToken = ']';
                node.isBracket = true;

            } else if (ws == "..") {  // 連番範囲演算子

                node.baseType = NodeType.OPERATOR_RANGE;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (ws == "==" || ws == "!=") {  // 等価比較演算子

                node.baseType = NodeType.OPERATOR_EQUIV;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (w == '=' ||   // 代入演算子
                ws.Length == 2 && ws[1] == '=' && (w == '+' || w == '-' || w == '*' || w == '/' || w == '|' || w == '&' || w == '^') ||
                ws.Length == 3 && ws[2] == '=' && (ws[..1] == "<<" || ws[..1] == ">>")) {

                node.baseType = NodeType.OPERATOR_ASG;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (ws == "<<" || ws == ">>") {  // ビットシフト演算子

                node.baseType = NodeType.OPERATOR_SHIFT;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (w == '>' || w == '<' || ws == ">=" || ws == "<=") {  // 比較演算子

                node.baseType = NodeType.OPERATOR_COMPARE;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (w == '&') {

                node.baseType = NodeType.OPERATOR_AND;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (w == '^') {

                node.baseType = NodeType.OPERATOR_XOR;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (w == '|') {

                node.baseType = NodeType.OPERATOR_OR;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (w == '+' || w == '-') {  // 加減算

                node.baseType = NodeType.OPERATOR_PM;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (w == '?' || w == ':') {  // 三項演算

                node.baseType = NodeType.OPERATOR_TERNARY;
                node.hasChild = true;
                node.oprArgumentNum = 3;


            } else if (w == '*' || w == '/' || w == '%') {  // 乗除剰余

                node.baseType = NodeType.OPERATOR_MDM;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (w == '!') {  // 単項演算子

                node.baseType = NodeType.OPERATOR_SINGLE;
                node.hasChild = true;
                node.oprArgumentNum = 1;


            } else if (w == '.') {  // ピリオド

                node.baseType = NodeType.OPERATOR_PERIOD;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (node.word.Length == 2 && node.word[1] == '[') {  // ツクール変数

                var c = node.word[0];

                if (c == 'v' || c == 'V') {
                    node.baseType = NodeType.TKV_V;
                } else if (c == 's' || c == 'S') {
                    node.baseType = NodeType.TKV_S;
                } else {
                    node.baseType = NodeType.TKV_T;
                }

                node.hasChild = true;
                node.endToken = ']';
                node.isBracket = true;


            } else if (node.word.Length == 3 && node.word[2] == '[') {  // 共有セーブ

                var c = node.word[1];

                if (c == 'v' || c == 'V') {
                    node.baseType = NodeType.TKV_GV;
                } else if (c == 's' || c == 'S') {
                    node.baseType = NodeType.TKV_GS;
                } else {
                    node.baseType = NodeType.TKV_GT;
                }

                node.hasChild = true;
                node.endToken = ']';
                node.isBracket = true;


            } else if (node.word[0] == '\"') {  // 文字列

                node.baseType = NodeType.STRING;
                node.hasChild = true;
                node.endToken = '\"';
                node.isBracket = true;
                Parser.escaping = false;


            } else if (number.Contains(node.word[0])) {  // 数値

                node.baseType = NodeType.NUMBER;
                node.returnedType = NodeType.NUMBER;

                if (regNum_dec.IsMatch(node.word)) {  // 10進数

                    node.value = Convert.ToInt32(node.word, 10);


                } else if (regNum_hex.IsMatch(node.word)) {  // 16進数

                    node.value = Convert.ToInt32(node.word[2..], 16);


                } else if (regNum_bin.IsMatch(node.word)) {  // 2進数

                    node.value = Convert.ToInt32(node.word[2..], 2);


                } else {  // 1文字目が数値であり、かつ数値のパターンに合致しない

                    if (true) {  // 宣言中である場合

                        throw Error.Call(Error.Defined_Name_Begins_With_Number, node);

                    } else {  // 不正な数値リテラル

                        throw Error.Call(Error.Invalid_Number_Literal, node);

                    }
                }


            } else {

                string wl = node.word.ToLower();

                if (Commands.DB.ContainsKey(wl)) {  // メインコマンド

                    node.word = wl;
                    node.baseType = NodeType.COMMAND_MAIN;
                    node.returnedType = NodeType.COMMAND_MAIN;
                    Parser.MainCommand = node.word.ToLower();


                } else if (Commands.ContainsSubCommand(wl)) {  // サブコマンド

                    node.baseType = NodeType.COMMAND_SUB;
                    node.word = wl;
                    Parser.SubCommand = node.word.ToLower();
                    node.hasChild = true;

                } else if (wl == Node.KEYWORD_CLASS) {  // クラス

                    node.baseType = NodeType.CLASS;
                    node.hasChild = true;

                } else if (wl == Node.KEYWORD_FUNCTION) {  // 関数

                    node.baseType = NodeType.FUNCTION;
                    node.hasChild = true;

                } else if (types.Contains(wl)) {  // 型

                    node.baseType = NodeType.TYPE;
                    node.hasChild = true;

                } else if (modifiers.Contains(wl)) {  // 宣言時の修飾子

                    node.baseType = NodeType.MODIFIER;
                    node.word = wl;
                    node.hasChild = true;

                } else {  // 何らかの定義名 (固有引数・組み込みメソッド・クラス名・クラスメソッド)

                    node.baseType = NodeType.NAME;

                }
            }

            //} else if (Commands.ContainsSubCommand(node.word.ToLower())) {  // サブコマンド

            //    node.baseType = NodeType.COMMAND_SUB;
            //    Interpreter.SubCommand = node.word.ToLower();
            //    //node.hasChild = Commands.GetArgumentList().Length > 0;
            //    node.hasChild = true;


            //} else if (Commands.ContainsArgument(node.word)) {  // サブコマンドの引数

            //    node.baseType = NodeType.COMMAND_ARG;
            //    node.hasChild = Commands.HasArgument(node.word);

            //}
        }
    }
}
