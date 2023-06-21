using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SolidTPC {

    enum NodeType {
        ROOT,
        PENDING,
        VOID,
        NAME,               // 定義名
        OPERATOR_PERIOD,    // .
        OPERATOR_SINGLE,    // !, not
        OPERATOR_MDM,       // *, /, %
        OPERATOR_PM,        // +, -
        OPERATOR_SHIFT,     // <<, >>
        OPERATOR_COMPARE,   // >, <, >=, <=
        OPERATOR_EQUIV,     // ==, !=
        OPERATOR_AND,       // &
        OPERATOR_XOR,       // ^
        OPERATOR_OR,        // |
        OPERATOR_BOOL_AND,  // &&
        OPERATOR_BOOL_OR,   // ||
        OPERATOR_RANGE,     // ..
        OPERATOR_TERNARY,   // ?, :
        OPERATOR_ASG,       // =, +=, -=, *=, /=, %=, |=, &=, ^=, <<=, >>=
        AT,
        BLOCK,
        BRACKET,
        COMMAND,            // ピリオドが COMMAND_MAIN.COMMAND_SUB を持つとき、有効な値にこれを返す
        COMMAND_MAIN,
        COMMAND_SUB,
        COMMAND_ARG,
        TKV_V,              // V[ NUMBER ]
        TKV_VV,             // V[V[ NUMBER ]]
        TKV_VV__,           // 内部にv[]もしくは数値しかない場合
        TKV_V_RANGE,        // V[(NUMBER) .. (NUMBER)] の形の範囲
        TKV_V_ANY,          // v[~]
        TKV_S,
        TKV_SV,
        TKV_SV__,
        TKV_S_RANGE,
        TKV_T,
        TKV_TV,
        TKV_TV__,
        TKV_T_RANGE,
        TKV_GV,
        TKV_GS,
        TKV_GT,
        EXPRESSION,         // ツクール上で何らかの有効な値を返す式
        STRING,
        STRING_EXT,         // 文字列内に展開される{}のノード
        NUMBER,
        BOOL,
        ARRAY,
        DICTIONARY,

        BR_MOVEROUTE,
        PRIMITIVE,
        FUNCTION,
        COM_LINE,           // 行コメント
        COM_RANGE           // 範囲コメント

    }


    // 各トークンの内容を示すNode

    internal class Node {

        public NodeType type;
        public NodeType returnedType;
        public string word;
        public char endToken = ' ';
        public bool isBracket = false;
        public List<Node> child = new();
        public bool hasChild = false;
        public string nameSpace = "";
        public int nest = -1;
        public int positionInSource = 0;
        public int indexInSourceList = 0;

        public int oprArgumentNum = -83;  // 演算子が持つべき引数の数
        public bool isClosed = false;  // このノードがもう引数を受け付けないかどうか

        // NUMBER用
        public int value = 0;
        public float value_f = 0;
        public bool isNotDecimal = false;
        public bool isLiteral = true;      // リテラルであるかどうか　!isFloat && isLiteral のノードが2連続した時、それは小数となる
        public bool isFloat = false;

        public int Value {
            get {
                return this.isFloat ? (int)this.value_f : this.value;
            }
        }

        public void ReturnNumber(Node n) {
            n.type = NodeType.NUMBER;
            if (isFloat) {
                n.isFloat = true;
                n.value_f = value_f;
            } else {
                n.value = value;
            }
        }

        public Node? parent;


        // Nodeのコンストラクタ


        public Node(string word) {
            this.word = word;
            returnedType = NodeType.PENDING;
            NodeController.SetNodeType(this);
            indexInSourceList = Parser.sources.Last().indexInSourceList;
            positionInSource = Parser.sources.Last().i;
        }

        public Node(string word, NodeType type) {
            this.word = word;
            this.type = type;
            returnedType = NodeType.PENDING;
            indexInSourceList = Parser.sources.Last().indexInSourceList;
            positionInSource = Parser.sources.Last().i;
        }

        public Node(string word, NodeType type, NodeType returnedType) {
            this.word = word;
            this.type = type;
            this.returnedType = returnedType;
            indexInSourceList = Parser.sources.Last().indexInSourceList;
            positionInSource = Parser.sources.Last().i;
        }


        public static bool isNumberVariable(Node n) {
            return n.returnedType == NodeType.TKV_V || n.returnedType == NodeType.TKV_VV || n.returnedType == NodeType.TKV_VV__;
        }

        public static bool isSwitchVariable(Node n) {
            return n.returnedType == NodeType.TKV_S || n.returnedType == NodeType.TKV_SV || n.returnedType == NodeType.TKV_SV__;
        }


        /** 
            <summary>
            数値配列でアドレスを指定してツリー内からノードを返す<br />
            [0] ならルートを返し、[0, 1] ならルート直下の2番目のノードを返す
            </summary>
        */
        public static Node GetNode(Node node, List<int> indices) {
            
            return gn(node, indices, 0);

            static Node gn(Node node, List<int> indices, int now) {

                now++;

                if (now < indices.Count) {
                    
                    try {
                        var a = indices[now];
                    } catch {
                        string asd = "";
                        foreach (var n in indices) {
                            if (asd != "") asd += ", ";
                            asd += n.ToString();
                        }
                        throw new Exception($"indices : count = {indices.Count}, index = {now}\r\n\r\n{asd}");
                    }

                    try {
                        var a = node.child[indices[now]];
                    } catch {
                        string asd = "";
                        foreach (var n in indices) {
                            if (asd != "") asd += ", ";
                            asd += n.ToString();
                        }
                        throw new Exception($"now : count = {node.child.Count}, index = {indices[now]}\r\n\r\n{asd}");
                    }

                    return gn(node.child[indices[now]], indices, now);

                } else {

                    return node;

                }
            }
        }
    }
}
