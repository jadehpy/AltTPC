using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolidTPC {
    internal class Error {

        // エラーメッセージ定数

        public const string

            TPC_Does_Not_Exist = "tpc.exeが同ディレクトリに存在しません",

            Defined_Name_Begins_With_Number = "定義名は数字以外で始まる必要があります",

            Invalid_Inextractable_Token_In_String = "文字列内に展開できない不正なトークンがあります",

            Invalid_Number_Literal = "不正な数値リテラルです。数値は10進数の整数もしくは小数、16進数、2進数のどれかである必要があります",

            Invalid_Token = "不正な記号です",

            Invalid_Token_In_Block = "不正なトークンがブロック内に存在します",

            Invalid_Main_Command = "存在しないメインコマンド名です",

            Close_Token_Does_Not_Exist = "閉じ括弧が存在しません",

            Invalid_Close_Token = "閉じ括弧が正しくありません",

            Float_Cannot_Used_In_Not_Decimal_Number = "小数点は十進数の数値にのみ利用可能です",

            Invalid_Operator = "不正な演算子です",

            Invalid_Command_Name_After_Atmark = "@ の後には有効なメインコマンドが必要です",

            Invalid_Arguments_In_Event_Command = "イベントコマンドの引数が不正です",

            Semicolon_Does_Not_Exist = "トークンの直前にセミコロンが存在しません",

            Opr_PM_Has_Invalid_Value = "符号演算子の持つ値が不正です",

            Invalid_Method_In_Number = "その名前のメソッドはnumber型には存在しません",

            Invalid_Method_In_String = "その名前のメソッドはstring型には存在しません",

            Invalid_Number_Of_Arguments_In_Method = "引数の数が不正です",

            Invalid_Type = "型が不正です",

            Argument_Is_Under_0 = "値は0以上でないといけません",

            Cannnot_Resolve_Token = "定義されていない識別子です",

            Source_Does_Not_Exist = "ソースファイルが存在しません",

            Invalid_Value_In_Range_Operator = "範囲指定演算子には数値か式を2つ指定する必要があります",

            Invalid_Token_In_TKV = "v[], s[], t[] の中には数値およびv[]が必要です",

            Invalid_Statement = "ステートメントはイベントコマンド、宣言、代入式、フロー制御文のどれかである必要があります",

            Operator_Range_With_1_Argument_Must_Have_Number = "範囲指定演算子に与えられる要素が一つの場合は要素は数値である必要があります",

            Operator_Range_In_Index_Access_Must_Have_Number = "インデックスアクセスに使用する範囲指定演算子に与えられる要素は数値である必要があります",

            Token_Cannot_Be_Accessed_By_Index = "インデックスアクセスが不可能なトークンです",

            No_Element_In_Array = "配列内に要素がありません",

            No_Index = "インデックスがありません",

            Invalid_Index = "配列へのアクセスに用いるインデックスには数値および数式が必要となります",

            Too_Many_Tokens_In_Index_Bracket = "インデックスアクセスに用いる[]内の要素が多すぎます",

            Mixed_Type_In_Array = "配列内に複数の型が混在しています",

            Invalid_Token_After_Type_Declaration = "宣言をする際は型名の後に有効な定義名が必要です",

            Invalid_Token_After_Modifier = "修飾子の後には修飾子もしくは型名が必要です",

            Block_Must_Exist_After_Function_Declaration = "関数宣言の後にはブロックが必要です",

            Bracket_Is_Not_Closed = "括弧が閉じていません",

            Too_Many_Arguments_After_Type_Declaration = "型宣言の後の引数が多すぎます",

            Invalid_Value_In_Args_Of_Function_Declaration = "関数宣言の引数が無効です",

            Invalid_Declaration = "無効な宣言です",

            Invalid_Class_Declaration = "無効なクラス宣言です",

            No_Bracket_Is_Needed_In_Class_Declaration = "クラスの宣言を行う際に定義名に括弧は不要です",

            Type_Cannot_Be_Enclosed_By_Bracket = "型名を括弧で括ることはできません",

            Declaration_Keyword_Cannot_Be_Enclosed_By_Bracket = "クラス・関数宣言子を括弧で括ることはできません",

            Invalid_Function_Declaration = "無効な関数宣言です",

            Type_Must_Exist_After_Function_Declaration = "関数宣言の後には型名が必要です",

            Bracket_Is_Needed_In_Function_Declaration = "関数の宣言を行う際に定義名には括弧を付ける必要があります",

            Class_Does_Not_Exist = "現在のスコープにそのクラスは存在しません",

            Name_Is_Not_Class = "型として指定された定義名はクラスではありません",

            Name_Is_Already_Declared_In_Current_Block = "その名前は現在のブロックで既に定義されています",

            Invalid_Place_To_Declare = "宣言する場所が正しくありません",

            Modifier_Private_Has_Effect_Only_In_Class_Member_Declaration = "private修飾子はクラスのメンバに対してのみ有効です",

            Modifier_Global_Is_Invalid_In_Class_Member_Declaration = "global修飾子は",

            Modifier_Static_Cannot_Be_Used_With_Modifier_Global = "static修飾子はglobal修飾子と同時に使うことはできません",

            Modifier_Prior_Cannot_Be_Used_With_Modifier_Global = "prior修飾子はglobal修飾子と同時に使うことはできません",

            Defined_Name_Is_Duplicated = "その変数の定義名は既に同ブロック内で使われています",

            Defined_Global_Name_Is_Duplicated = "その変数の定義名は既にグローバル空間で使われています",

            Defined_Name_Is_Duplicated_In_Upper_Block = "その変数の定義名は既に上位ブロック内で使われています（prior修飾子の利用を検討してください）",

            No_Argument_On_The_Right_Side_Of_Binary_Operator = "二項演算子の右側に値がありません",

            Type_Of_Assigned_Value_Is_Not_Suited_To_Variable = "代入される値の型と変数の型が一致しません",


            END = "";






        // エラー位置の特定

        public static string GetPositionInSource(Node node) {

            string src = Parser.sourceList[node.indexInSourceList].code;

            int line = 1;
            int idx = 1;

            for (int i = 0; i < node.positionInSource; i++) {
                idx++;
                if (src[i] == '\n') {
                    line++;
                    idx = 1;
                }
            }

            return $"{Parser.sourceList[node.indexInSourceList].name}, {line} 行目, {idx - node.word.Length} 文字目, \"{node.word}\"";

        }


        // エラー呼び出し

        public static Exception Call(string error, Node node) {
            return new Exception($"{error} : {GetPositionInSource(node)}");
        }

        public static Exception Call(string error, Node node1, Node node2) {
            return new Exception($"{error} : {GetPositionInSource(node1)}, {GetPositionInSource(node2)}");
        }

        public static Exception Call(string error) {
            return new Exception($"{error}");
        }

        public static Exception Call(string error, string detail) {
            return new Exception($"{error} : {detail}");
        }
    }
}
