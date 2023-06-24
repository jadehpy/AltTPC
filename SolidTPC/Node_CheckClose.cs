using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。
#pragma warning disable CS8604 // Null 参照引数の可能性があります。

namespace SolidTPC {
    partial class Node_CheckType {

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

                    if (node.baseType != NodeType.BLOCK || n.baseType != NodeType.BLOCK) {  // 親と子が両方ブロックなら呼び出しは無駄なので回避

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

        // ノードの子要素の型チェック

        /**
            <summary>
            ノードの子要素の型をチェックし、有効な型でないならばエラーを返す<br />
            </summary>
        */
        public static bool CheckNodeClose(Node node, char? endToken) {

            foreach (var n in node.child) {

                //// <定義名>.
                //if (n.returnedType == NodeType.NAME && n.parent.baseType == NodeType.OPERATOR_PERIOD) {
                //    continue;
                //}

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

            switch (node.baseType) {

                // ──────────────────────────────────────────────────────────────────────

                // 定義名の場合

                case NodeType.NAME:


                    if (node.parent.baseType == NodeType.TYPE) {  // 宣言の場合

                        node.returnedType = NodeType.NAME;

                        node.name = new(node);
                        

                    } else {  // 展開時



                    }

                    break;

                // ──────────────────────────────────────────────────────────────────────

                // 型名の場合

                case NodeType.TYPE:

                    // 型の判別
                    
                    if (node.word == Node.KEYWORD_NUMBER) node.value = (int)NodeType.NUMBER;
                    if (node.word == Node.KEYWORD_STRING) node.value = (int)NodeType.STRING;
                    if (node.word == Node.KEYWORD_ARRAY) node.value = (int)NodeType.ARRAY;
                    if (node.word == Node.KEYWORD_DICTIONARY) node.value = (int)NodeType.DICTIONARY;
                    


                    if (node.child.Count == 0) {  // 子がない場合は型名として扱う
                        node.returnedType = NodeType.TYPENAME;
                        break;
                    }

                    if (node.child[0].returnedType != NodeType.NAME) {  // 型名の後に有効な定義名がない
                        throw Error.Call(Error.Invalid_Token_After_Type_Declaration, node.child[0]); 
                    }

                    if (node.child.Count == 1) {

                        if (node.child[0].returnedType == NodeType.NAME) {  // 変数宣言
                            node.returnedType = NodeType.TYPE;
                            break;
                        }

                        throw Error.Call(Error.Invalid_Token_After_Type_Declaration, node.child[0]);
                    }

                    if (node.child.Count == 2) {

                        if (node.child[0].returnedType == NodeType.NAME && node.child[1].baseType == NodeType.BLOCK) {  // 関数宣言

                            node.returnedType = NodeType.TYPE;
                            break;

                        }

                        throw Error.Call(Error.Invalid_Declaration, node);
                    }

                    throw Error.Call(Error.Too_Many_Arguments_After_Type_Declaration, node);


                // ──────────────────────────────────────────────────────────────────────

                // 修飾子の場合

                case NodeType.MODIFIER:

                    if (node.child.Count == 1) {

                        if (node.child[0].returnedType == NodeType.MODIFIER ||
                            node.child[0].returnedType == NodeType.TYPE) {

                            node.returnedType = NodeType.MODIFIER;
                            break;
                        }

                        throw Error.Call(Error.Invalid_Token_After_Modifier, node.child[0]);
                    }

                    throw Error.Call("too many arguments after modifier", node);

                // ──────────────────────────────────────────────────────────────────────

                // クラス宣言子の場合

                case NodeType.CLASS:

                    if (node.child.Count != 2) {

                        throw Error.Call(Error.Invalid_Class_Declaration, node);

                    }

                    if (node.child[0].returnedType != NodeType.NAME || node.child[1].returnedType != NodeType.BLOCK) {

                        throw Error.Call(Error.Invalid_Class_Declaration, node);

                    }

                    if (node.child[0].child.Count != 0) {

                        throw Error.Call(Error.No_Bracket_Is_Needed_In_Class_Declaration, node.child[0]);

                    }

                    node.returnedType = NodeType.VOID;

                    break;

                // ──────────────────────────────────────────────────────────────────────

                // 関数宣言子の場合

                case NodeType.FUNCTION:

                    if (node.child.Count != 1) {

                        throw Error.Call(Error.Invalid_Function_Declaration, node);

                    }

                    var type = node.child[0];

                    if (type.returnedType != NodeType.TYPE) {  // 型が違う場合

                        throw Error.Call(Error.Type_Must_Exist_After_Function_Declaration, node);

                    }

                    if (type.child.Count != 2) {

                        throw Error.Call(Error.Invalid_Function_Declaration, node);

                    }

                    if (type.child[0].returnedType != NodeType.NAME || type.child[1].returnedType != NodeType.BLOCK) {  // 型名は名前・ブロックを持たないといけない

                        throw Error.Call(Error.Bracket_Is_Needed_In_Function_Declaration, node.child[0]);

                    }

                    var name = type.child[0];

                    if (name.child.Count != 1 || name.child[0].returnedType != NodeType.TYPE)

                    node.returnedType = NodeType.VOID;

                    break;


                // ──────────────────────────────────────────────────────────────────────

                // 波括弧（ブロック）の場合

                case NodeType.BLOCK:

                    node.returnedType = NodeType.BLOCK;

                    foreach (var n in node.child) {

                        if (n.returnedType == NodeType.VOID) continue;

                        throw Error.Call(Error.Invalid_Statement, n);
                    }

                    break;

                // ──────────────────────────────────────────────────────────────────────

                // 丸括弧の場合 

                case NodeType.BRACKET:

                    if (node.parent.baseType == NodeType.NAME) {  // 定義名() のとき

                        if (node.child.Count > 0 && node.child[0].returnedType == NodeType.TYPE) {

                            node.returnedType = NodeType.BRACKET_TYPE;

                            foreach (var n in node.child) {

                                if (n.returnedType != NodeType.TYPE) {

                                    node.returnedType = NodeType.BRACKET;  // 1つでもTYPE以外が混ざれば関数宣言ではない括弧と見做す
                                    break;
                                }
                            }

                            break;
                        }

                        node.returnedType = NodeType.BRACKET;
                        break;

                    }

                    if (node.child.Count == 0) {

                        node.returnedType = NodeType.VOID;
                        break;

                    }


                    if (node.child.Count == 1) {

                        if (node.child[0].returnedType == NodeType.TYPE) {  // 宣言時の型名は括弧で括れない

                            Error.Call(Error.Type_Cannot_Be_Enclosed_By_Bracket, node);

                        }

                        if (node.child[0].baseType == NodeType.CLASS || node.child[0].baseType == NodeType.FUNCTION) {  // クラス・関数宣言子は括弧で括れない

                            Error.Call(Error.Declaration_Keyword_Cannot_Be_Enclosed_By_Bracket, node);

                        }

                        // そのまま受け渡し
                        int i = node.parent.child.IndexOf(node);
                        node.parent.child.RemoveAt(i);
                        node.parent.child.Insert(i, node.child[0]);

                        break;

                    }

                    node.returnedType = NodeType.BRACKET;
                    break;

                // ──────────────────────────────────────────────────────────────────────

                // ツクール変数の場合

                case NodeType.TKV_V:
                case NodeType.TKV_S:
                case NodeType.TKV_T:

                    if (node.child.Count == 0) {

                        node.returnedType = NodeType.TYPE;
                        break;

                    } else if (node.child.Count == 1 && node.child[0].returnedType != NodeType.OPERATOR_RANGE) {

                        if (Node.isAnyExpression(node.child[0])) {  // 何らかの式
                            node.returnedType = node.baseType;
                            break;
                        }

                        // それ以外ならエラー
                        throw Error.Call(Error.Invalid_Token_In_TKV, node.child[0]);

                    } else {  // 複数の値を持つ or RANGEを持つ場合

                        node.returnedType = (NodeType)((int)node.baseType + 3);  // RANGEに変更

                        foreach (var n in node.child) {

                            if (n.returnedType == NodeType.OPERATOR_RANGE) continue;

                            if (Node.isAnyExpression(n)) continue;

                            // それ以外ならエラー
                            throw Error.Call(Error.Invalid_Token_In_TKV, n);
                        }

                        break;
                    }


                // ──────────────────────────────────────────────────────────────────────

                // 範囲指定演算子の場合

                case NodeType.OPERATOR_RANGE:

                    if (node.child.Count == 1) {  // 「x..」および「..x」　前者はoprArgNumが2、後者はoprArgNumが1

                        if (node.child[0].returnedType != NodeType.NUMBER) {

                            throw Error.Call(Error.Operator_Range_With_1_Argument_Must_Have_Number, node.child[0]);

                        }

                        node.returnedType = NodeType.OPERATOR_RANGE;
                    }

                    if (node.child.Count == 2) {

                        foreach (var n in node.child) {

                            if (Node.isAnyExpression(n)) continue;

                            throw Error.Call(Error.Invalid_Value_In_Range_Operator, n);
                        }
                        node.returnedType = NodeType.OPERATOR_RANGE;
                    }

                    throw Error.Call(Error.Invalid_Value_In_Range_Operator, node);


                // ──────────────────────────────────────────────────────────────────────

                // 鍵括弧（配列）の場合

                case NodeType.BRACKET_SQUARE_ARR:

                    node.returnedType = NodeType.ARRAY;

                    if (node.child.Count == 0) {

                        throw Error.Call(Error.No_Element_In_Array, node);

                    }

                    if (node.child[0].returnedType != NodeType.ARRAY && node.child[0].returnedType != NodeType.DICTIONARY &&
                        node.child[0].returnedType != NodeType.TYPE) {

                        // 通常の型の場合

                        NodeType nt = node.child[0].returnedType;

                        foreach (var n in node.child) {
                            if (n.returnedType != nt) {
                                throw Error.Call(Error.Mixed_Type_In_Array, node);
                            }
                        }
                        node.returnedType = nt;

                        break;



                    } else {

                        // 配列の中に配列が入っている場合


                    }

                    break;


                // ──────────────────────────────────────────────────────────────────────

                // 鍵括弧（インデックスアクセス）の場合

                case NodeType.BRACKET_SQUARE_IDX:

                    if (node.child.Count == 1) {

                        throw Error.Call(Error.No_Index, node);

                    }

                    if (node.child.Count == 2) {  // 通常のインデックスアクセス

                        var arr = node.child[0];
                        var idx = node.child[1];

                        if (idx.returnedType == NodeType.NUMBER) {

                            if (arr.returnedType == NodeType.NUMBER) {
                                node.returnedType = NodeType.NUMBER;
                                break;
                            }

                            // 文字列の場合、インデックスの位置の文字を返す
                            if (arr.returnedType == NodeType.STRING) {
                                node.returnedType = NodeType.STRING;
                                break;
                            }

                            // v[x]の場合、v[x + idx]を返す
                            if (arr.returnedType == NodeType.TKV_V || arr.returnedType == NodeType.TKV_S || arr.returnedType == NodeType.TKV_T) {
                                node.returnedType = arr.returnedType;
                                break;
                            }

                            // v[a..b] や v[a, b..] などのTKV_X_RANGEの場合、TKV_Xを返す
                            if (arr.returnedType == NodeType.TKV_V_RANGE || arr.returnedType == NodeType.TKV_S_RANGE || arr.returnedType == NodeType.TKV_T_RANGE) {
                                node.returnedType = (NodeType)((int)arr.returnedType - 3);
                                break;
                            }

                            // 配列の場合、配列の要素を返す
                            if (arr.returnedType == NodeType.ARRAY) {

                                node.returnedType = arr.returnedType;


                            }

                            // 
                            if (arr.returnedType == NodeType.DICTIONARY) {

                            }

                            // 配列として与えられた値がインデックスアクセスできない型だった場合
                            throw Error.Call(Error.Token_Cannot_Be_Accessed_By_Index, arr);
                        }

                        if (Node.isAnyExpression(idx)) {  // インデックスが式の場合はまとめてEXPRESSIONとする

                            node.returnedType = NodeType.EXPRESSION;
                            break;

                        }

                        if (idx.returnedType == NodeType.OPERATOR_RANGE) {  // 範囲指定演算子が入っている場合、NUMBER以外は弾く

                            foreach (var n in idx.child) {
                                if (n.returnedType != NodeType.NUMBER) {
                                    throw Error.Call(Error.Operator_Range_In_Index_Access_Must_Have_Number, n);
                                }
                            }
                            break;
                        }

                        // インデックスとして利用不能な値だった場合は弾く
                        throw Error.Call(Error.Invalid_Index, idx);
                    }

                    throw Error.Call(Error.Too_Many_Tokens_In_Index_Bracket, node);


                // ──────────────────────────────────────────────────────────────────────

                // イコールの場合

                case NodeType.OPERATOR_ASG:

                    var c1 = node.child[0];
                    var c2 = node.child[1];

                    node.returnedType = NodeType.VOID;

                    if (node.word.Length == 1) {

                        // = の場合

                        if (c1.returnedType == NodeType.TKV_V) {

                        }

                    } else if (node.word.Length == 2) {


                    } else {



                    }

                    break;


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

                        } else if (b.baseType == NodeType.NAME) {

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


    }
}
