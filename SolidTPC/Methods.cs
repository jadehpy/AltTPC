using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable CS8602 // null 参照の可能性があるものの逆参照です。

namespace SolidTPC {
    internal class Methods {

        static int argNum = 0;

        public static void Execute(Node node) {

            var a = node.child[0];
            var b = node.child[1];

            int ArgNum(int min, int max) {

                if (b.child.Count == 0) {
                    if (min > 0) {
                        throw Error.Call(Error.Invalid_Number_Of_Arguments_In_Method, b);
                    }
                }

                if (b.child[0].child.Count >= min && b.child[0].child.Count <= max) {
                    return b.child[0].child.Count;
                }
                throw Error.Call(Error.Invalid_Number_Of_Arguments_In_Method, b);
            }

            List<Node>? args = b.child[0].child;

            void CheckArgTypeAndThrow(int idx, NodeType nt) {
                if (b.child[0].child[idx].returnedType != nt) throw Error.Call(Error.Invalid_Type, b.child[0].child[idx]);
            }

            bool CheckArgType(int idx, NodeType nt) {
                return b.child[0].child[idx].returnedType == nt;
            }




            if (a.returnedType == NodeType.NUMBER) {

                // ========================================================================================================================

                // number型の組み込みメソッド

                // ========================================================================================================================


                if (b.word == "isFloat") {

                    ArgNum(0, 0);

                    node.returnedType = NodeType.BOOL;
                    node.value = a.isFloat ? 1 : 0;

                } else if (b.word == "toStr") {

                    ArgNum(0, 0);

                    node.returnedType = NodeType.STRING;
                    node.word = a.isFloat ? a.value_f.ToString() : a.value.ToString();

                } else if (b.word == "to0x") {

                    ArgNum(0, 0);

                    node.returnedType = NodeType.STRING;
                    node.word = a.isFloat ? Convert.ToString((int)a.value_f, 16) : Convert.ToString(a.value, 16);

                } else if (b.word == "to0b") {

                    ArgNum(0, 0);

                    node.returnedType = NodeType.STRING;
                    node.word = a.isFloat ? Convert.ToString((int)a.value_f, 2) : Convert.ToString(a.value, 2);

                } else if (b.word == "floor") {

                    ArgNum(0, 0);

                    node.returnedType = NodeType.NUMBER;
                    node.value = a.isFloat ? (int)a.value_f : a.value;

                } else {

                    throw Error.Call(Error.Invalid_Method_In_Number, b);

                }






            } else if (a.returnedType == NodeType.STRING) {

                // ========================================================================================================================

                // string型の組み込みメソッド

                // ========================================================================================================================

                b.returnedType = NodeType.NAME;

                if (b.word == "length") {

                    ArgNum(0, 0);
                    CheckArgTypeAndThrow(0, NodeType.NUMBER);

                    node.returnedType = NodeType.NUMBER;
                    node.value = a.word.Length;

                } else if (b.word == "at") {

                    ArgNum(1, 1);
                    CheckArgTypeAndThrow(0, NodeType.NUMBER);

                    int idx = Math.Clamp(args[0].Value, 0, a.word.Length - 1);

                    node.returnedType = NodeType.STRING;
                    node.word = a.word[idx].ToString();

                } else if (b.word == "substr" || b.word == "substr_rev") {

                    node.returnedType = NodeType.STRING;

                    argNum = ArgNum(1, 2);
                    CheckArgTypeAndThrow(0, NodeType.NUMBER);

                    if (argNum == 1) {

                        var start = args[0].isFloat ? (int)args[0].value_f : args[0].value;
                        start = Math.Clamp(start, 0, a.word.Length - 1);

                        if (b.word.Length == 6) {  // substr
                            node.word = a.word.Substring(start);
                        } else {  // substr_rev
                            node.word = a.word.Substring(0, a.word.Length - start);
                        }

                    } else {

                        CheckArgTypeAndThrow(1, NodeType.NUMBER);

                        var start = args[0].isFloat ? (int)args[0].value_f : args[0].value;
                        var length = args[1].isFloat ? (int)args[1].value_f : args[1].value;

                        start = Math.Clamp(start, 0, a.word.Length);
                        length = Math.Clamp(length, 0, a.word.Length - start);

                        if (b.word.Length == 6) {  // substr

                            node.word = a.word.Substring(start, length);

                        } else {  // substr_rev

                            node.word = a.word.Substring(a.word.Length - start - length, length);

                        }
                    }

                } else {

                    throw Error.Call(Error.Invalid_Method_In_String, b);

                }

            }

        }
    }
}
