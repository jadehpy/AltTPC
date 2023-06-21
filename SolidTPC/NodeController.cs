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
    internal class NodeController {


        // ノードの管理に関係する関数群



        // ======================================================================================================================================================

        // ノードの解決
        // 後述のノードの子要素チェックを各ノードについて呼び出す

        /**
            <summary>
            渡したノードを解決する
            ノード中の子も再帰で解決する
            allにはtrueを渡し、anyにはfalseを渡しておく
            allがtrueならばそのノードは全て解決したことを、anyがtrueならば何かしらは解決したことを指す
            </summary> 
        */
        public static void ResolveNode(Node node, ref bool all, ref bool any) {

            foreach (var n in node.child) {

                if (n.returnedType == NodeType.PENDING) {

                    if (node.type != NodeType.BLOCK || n.type != NodeType.BLOCK) {  // 親と子が両方ブロックなら呼び出しは無駄なので回避

                        ResolveNode(n, ref all, ref any);

                    }
                }
            }

            if (CheckNodeClose(node, null)) {

                // このノードが解決された場合
                any = true;

            } else {

                // このノードが解決されなかった場合
                all = false;

            }
        }



        // ======================================================================================================================================================

        // ノードの子要素チェック

        /**
            <summary>
            ノードの子要素の型をチェックし、有効な型でないならばエラーを返す<br />
            </summary>
        */
        public static bool CheckNodeClose(Node node, char? endToken) {



            foreach (var n in node.child) {

                if (n.type == NodeType.NAME && n.parent.type == NodeType.OPERATOR_PERIOD) {
                    continue;
                }

                if (n.returnedType == NodeType.PENDING) {
                    return false;
                }
            }

            if (node.isBracket && node.word.Length != 0) {
                if (node.endToken != endToken) {
                    if (endToken == null) {
                        throw Error.Call(Error.Close_Token_Does_Not_Exist, node);
                    } else {
                        throw Error.Call(Error.Invalid_Close_Token + $"({endToken})", node);
                    }
                }
            }

            switch (node.type) {


                // 波括弧（ブロック）の場合

                case NodeType.BLOCK:

                    node.returnedType = NodeType.BLOCK;

                    foreach (var n in node.child) {

                        if (n.returnedType == NodeType.VOID) continue;

                        throw Error.Call(Error.Invalid_Token_In_Block, n);
                    }

                    break;

                // ──────────────────────────────────────────────────────────────────────

                // 丸括弧の場合 

                case NodeType.BRACKET:


                    if (node.parent.type == NodeType.NAME) {

                        node.returnedType = NodeType.BRACKET;
                        break;

                    }

                    if (node.child.Count == 0) {

                        node.returnedType = NodeType.VOID;
                        break;

                    }


                    if (node.child.Count == 1) {

                        if (node.returnedType == NodeType.NUMBER) {

                            node.child[0].ReturnNumber(node);

                        }

                        break;

                    }

                    node.returnedType = NodeType.BRACKET;
                    break;


                // ──────────────────────────────────────────────────────────────────────




                // ──────────────────────────────────────────────────────────────────────

                // 加減算演算子の場合

                case NodeType.OPERATOR_PM:

                    if (node.oprArgumentNum != node.child.Count) {  // 子の数が要求される数と一致しない場合はエラー

                        throw Error.Call(Error.Invalid_Operator, node);

                    }

                    if (node.oprArgumentNum == 1) {  // 単項演算子の場合
                        
                        var a = node.child[0];

                        if (a.returnedType == NodeType.NUMBER) {
                            
                            node.returnedType = NodeType.NUMBER;

                            if (a.isFloat) {

                                node.isFloat = true;
                                node.value_f = a.value_f;

                                if (node.word[0] == '-') {

                                    node.value_f *= -1;                               

                                } 

                            } else {

                                node.value = a.value;

                                if (node.word[0] == '-') {

                                    node.value *= -1;

                                }
                            }

                        } else if (Node.isSwitchVariable(a) || Node.isNumberVariable(a)) {

                            node.returnedType = a.returnedType;

                        } else {

                            throw Error.Call(Error.Opr_PM_Has_Invalid_Value, node);

                        }

                        break;

                    } else {  // 二項演算子の場合

                        var a = node.child[0];
                        var b = node.child[1];

                        if (node.word[0] == '+') {

                            if (a.returnedType == NodeType.NUMBER && b.returnedType == NodeType.NUMBER) {  // どちらも定数数値なら定数数値を返す

                                node.returnedType = NodeType.NUMBER;

                                if (a.isFloat || b.isFloat) {

                                    node.isFloat = true;
                                    node.value_f = (a.isFloat ? a.value_f : a.value) + (b.isFloat ? b.value_f : b.value);

                                } else {

                                    node.value = a.value + b.value;

                                }

                            } else if (a.returnedType == NodeType.STRING && b.returnedType == NodeType.STRING) {  // どちらも定数文字列なら以下略

                                node.returnedType = NodeType.STRING;
                                node.word = new StringBuilder(a.word).Append(b.word).ToString();

                            } else if (a.returnedType == NodeType.EXPRESSION || b.returnedType == NodeType.EXPRESSION) {

                                node.returnedType = NodeType.EXPRESSION;

                            } else if ((Node.isNumberVariable(a) || Node.isSwitchVariable(a) || a.returnedType == NodeType.NUMBER) && 
                                (Node.isNumberVariable(b) || Node.isSwitchVariable(b)) || b.returnedType == NodeType.NUMBER) {

                                node.returnedType = NodeType.EXPRESSION;

                            }

                        } else {



                        }
                    }

                    break;


                // ──────────────────────────────────────────────────────────────────────

                // 文字列の場合

                case NodeType.STRING:

                    node.returnedType = NodeType.STRING;

                    StringBuilder word = new("");

                    foreach (var n in node.child) {

                        if (n.returnedType == NodeType.STRING) {
                            word.Append(n.word);
                            continue;
                        }

                        throw Error.Call(Error.Invalid_Inextractable_Token_In_String, node);
                    }

                    node.word = word.ToString();

                    break;


                // ──────────────────────────────────────────────────────────────────────

                // @ の場合

                case NodeType.AT:

                    if (node.child.Count == 0) {
                        throw Error.Call(Error.Invalid_Token, node);
                    }

                    node.returnedType = NodeType.VOID;

                    if (node.child[0].returnedType == NodeType.COMMAND_MAIN) {  // 直下にメインコマンドがある (サブコマンドがない) 場合

                        CheckArguments(node, true);


                    } else if (node.child[0].returnedType != NodeType.COMMAND) {  // 直下に有効なMain.Subがない

                        // 無効なコマンド名としてエラー
                        throw Error.Call(Error.Invalid_Command_Name_After_Atmark, node);
                    }

                    break;


                // ──────────────────────────────────────────────────────────────────────

                // ピリオドの場合

                case NodeType.OPERATOR_PERIOD:

                    if (node.child.Count != 2) {
                        throw Error.Call(Error.Invalid_Token, node);

                    } else {

                        var a = node.child[0];
                        var b = node.child[1];

                        if (a.returnedType == NodeType.COMMAND_MAIN && b.returnedType == NodeType.COMMAND_SUB) {

                            // イベントコマンドの場合

                            node.returnedType = NodeType.COMMAND;

                        } else if (b.type == NodeType.NAME) {

                            // 関数の場合

                            Methods.Execute(node);

                        }
                    }


                    break;


                // ──────────────────────────────────────────────────────────────────────

                // サブコマンドの場合

                case NodeType.COMMAND_SUB:

                    node.returnedType = NodeType.COMMAND_SUB;

                    CheckArguments(node.parent, false);

                    break;


                // ──────────────────────────────────────────────────────────────────────

                // 数値の場合

                case NodeType.NUMBER:

                    node.returnedType = NodeType.NUMBER;


                    break;





                default:
                    break;
            }

            return node.returnedType != NodeType.PENDING;  // 解決
        }



        // ======================================================================================================================================================

        // イベントコマンドの引数チェック

        static int CheckArguments(Node node, bool isOnlyMainCommand) {

            Node n;
            int startIndex;
            string main;
            string sub;


            if (isOnlyMainCommand) {

                // NodeType.ATが渡される

                n = node;
                startIndex = 1;
                main = node.child[0].word;
                sub = "";

            } else {

                // NodeType.PERIODが渡される

                n = node.child[1];
                startIndex = 0;
                main = node.child[0].word;
                sub = node.child[1].word;

            }

            for (int i = startIndex; i < n.child.Count; i++) {



            }

            return 0;

        }


        // ======================================================================================================================================================

        // PENDING中のノードを取得する

        public static Node? GetPendingToken(Node node) {

            foreach (var n in node.child) {

                Node? nd = GetPendingToken(n);

                if (nd != null) {

                    return nd;

                }
            }

            if (node.returnedType == NodeType.PENDING) {

                return node;

            }

            return null;
        }


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

        /**
            <summary>
            wordを元にトークンの種類を判別して保存する    
            </summary>    
        */
        public static void SetNodeType(Node node) {

            char w = node.word[0];
            string ws = node.word;

            if (w == '@') {  // コマンド・CODE型

                node.type = NodeType.AT;
                node.hasChild = true;


            } else if (ws == "//") {  // 行コメント

                node.type = NodeType.COM_LINE;
                node.hasChild = true;


            } else if (ws == "/*") {  // 範囲コメント

                node.type = NodeType.COM_RANGE;
                node.hasChild = true;


            } else if (w == '{') {  // ブロック

                node.type = NodeType.BLOCK;
                node.hasChild = true;
                node.endToken = '}';
                node.isBracket = true;


            } else if (w == '(') {  // 括弧

                node.type = NodeType.BRACKET;
                node.hasChild = true;
                node.endToken = ')';
                node.isBracket = true;


            } else if (w == '[') {  // 鍵括弧

                node.type = NodeType.BRACKET;
                node.hasChild = true;
                node.endToken = ']';
                node.isBracket = true;

            } else if (ws == "..") {  // 連番範囲演算子

                node.type = NodeType.OPERATOR_RANGE;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (ws == "==" || ws == "!=") {  // 等価比較演算子

                node.type = NodeType.OPERATOR_EQUIV;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (w == '=' ||   // 代入演算子
                ws.Length == 2 && ws[1] == '=' && (w == '+' || w == '-' || w == '*' || w == '/' || w == '|' || w == '&' || w == '^') ||
                ws.Length == 3 && ws[2] == '=' && (ws[..1] == "<<" || ws[..1] == ">>")) {

                node.type = NodeType.OPERATOR_ASG;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (ws == "<<" || ws == ">>") {  // ビットシフト演算子

                node.type = NodeType.OPERATOR_SHIFT;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (w == '>' || w == '<' || ws == ">=" || ws == "<=") {  // 比較演算子

                node.type = NodeType.OPERATOR_COMPARE;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (w == '&') {

                node.type = NodeType.OPERATOR_AND;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (w == '^') {

                node.type = NodeType.OPERATOR_XOR;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (w == '|') {

                node.type = NodeType.OPERATOR_OR;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (w == '+' || w == '-') {  // 加減算

                node.type = NodeType.OPERATOR_PM;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (w == '?' || w == ':') {  // 三項演算

                node.type = NodeType.OPERATOR_TERNARY;
                node.hasChild = true;
                node.oprArgumentNum = 3;


            } else if (w == '*' || w == '/' || w == '%') {  // 乗除剰余

                node.type = NodeType.OPERATOR_MDM;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (w == '!' || node.word.ToLower() == "not") {  // 単項演算子

                node.type = NodeType.OPERATOR_SINGLE;
                node.hasChild = true;
                node.oprArgumentNum = 1;


            } else if (w == '.') {  // ピリオド

                node.type = NodeType.OPERATOR_PERIOD;
                node.hasChild = true;
                node.oprArgumentNum = 2;


            } else if (node.word.Length >= 2 && node.word[1] == '[') {  // ツクール変数

                var c = node.word[0];

                if (c == 'v' || c == 'V') {
                    node.type = NodeType.TKV_V;
                } else if (c == 's' || c == 'S') {
                    node.type = NodeType.TKV_S;
                } else {
                    node.type = NodeType.TKV_T;
                }

                node.hasChild = true;
                node.endToken = ']';
                node.isBracket = true;


            } else if (node.word.Length >= 3 && node.word[2] == '[') {  // 共有セーブ

                var c = node.word[1];

                if (c == 'v' || c == 'V') {
                    node.type = NodeType.TKV_GV;
                } else if (c == 's' || c == 'S') {
                    node.type = NodeType.TKV_GS;
                } else {
                    node.type = NodeType.TKV_GT;
                }

                node.hasChild = true;
                node.endToken = ']';
                node.isBracket = true;


            } else if (node.word[0] == '\"') {  // 文字列

                node.type = NodeType.STRING;
                node.hasChild = true;
                node.endToken = '\"';
                node.isBracket = true;
                Parser.escaping = false;


            } else if (number.Contains(node.word[0])) {  // 数値

                node.type = NodeType.NUMBER;
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

                String wl = node.word.ToLower();

                if (Commands.DB.ContainsKey(wl)) {  // メインコマンド

                    node.word = node.word.ToLower();
                    node.type = NodeType.COMMAND_MAIN;
                    node.returnedType = NodeType.COMMAND_MAIN;
                    Parser.MainCommand = node.word.ToLower();


                } else if (Commands.ContainsSubCommand(wl)) {  // サブコマンド

                    node.type = NodeType.COMMAND_SUB;
                    Parser.SubCommand = node.word.ToLower();
                    node.hasChild = true;


                } else {  // 何らかの定義名 (固有引数・組み込みメソッド・クラス名・クラスメソッド)

                    node.type = NodeType.NAME;

                }
            }

            //} else if (Commands.ContainsSubCommand(node.word.ToLower())) {  // サブコマンド

            //    node.type = NodeType.COMMAND_SUB;
            //    Interpreter.SubCommand = node.word.ToLower();
            //    //node.hasChild = Commands.GetArgumentList().Length > 0;
            //    node.hasChild = true;


            //} else if (Commands.ContainsArgument(node.word)) {  // サブコマンドの引数

            //    node.type = NodeType.COMMAND_ARG;
            //    node.hasChild = Commands.HasArgument(node.word);

            //}
        }
    }
}
