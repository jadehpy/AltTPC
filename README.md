# SolidTPC
<br>
TPCの方言です。<br>
従来の文法のゆるくてふわふわした部分を刈り取ってより厳密にし、かつ便利にする目的で作られています。
<br><br>
細かい解説は<a href="https://github.com/jadehpy/SolidTPC/wiki">リポジトリのWikiページ</a> へ。
<br><br><br><br>

## 変更履歴
<br>

```
2023.06.19
  リポジトリ作成。数式などごく一部の構文解釈ができるだけのバージョン。
```
<br><br><br>

<!--

## 特徴

<br>

### ステートメント区切りにセミコロン必須
必須です。サボるな。

<br><br><br>
  
### 静的な型の導入
メタ変数の宣言に型名が必要になりました。
基本型は以下の通り。
<br>
number : 整数または小数。小数が混ざった計算は小数を返す。ただし.floor()で整数に戻せる。<br>
string : 文字列。+で結合もできる。テンプレート構文がデフォルトで、文字列内で{}で値を（暗黙の型変換をしつつ）展開する。<br>
v[] : v[何らかの内容]を受け入れる。 
block : {}で括られた一連の処理。+で連結もできる。代入や連結がされていないblock型は中身を展開する。<br>
bool : 真偽値。ちなみにメタ分岐では真偽値以外は受け付けなくなりました。
```
v[] a = v[1]; 
const string b = "c" + "d";  // 接頭辞constで再代入を不可能にする
number c = 4.5;
bool d = c.isFloat()  // .isFloat()は小数かどうかの真偽値を返す。これはtrue
```
<br><br><br>

### コマンドと非コマンドの明確化
「@msg.show」などのコマンドに@が付いているのに対し「v[1] = 3」 のように変数操作などには付いておらず、<br>
これによってメタ変数への代入とツクール変数への代入が混ざったりするのをコマンドに全て@を付けることで防ぎます。
```
@v[1] = 5;
v[] a = v[2];  // v[何らかの値]が入る変数としてaを宣言
a = 3;  // コンパイルエラー。aにnumber型は代入できない
@a = 3;  // これは@v[2] = 3でコマンド扱いで通る
a = v[4];  // これはaにv[]型を代入なので通る
@if v[1] == v[2] + v[3];  // バッククォート不要に
@else;  // これも単一のコマンドに。メタ分岐の.elseと混ざってややこしい & 自由なネストを作れないため
@end;  // @if - @endと囲わないとエラーを吐きます
@t[1] = "asd";  // 文字列変数に対して.asgや.eqなどは使わなくなりました
```
<br><br><br>

### コマンド固有引数の記法の変更
ピリオドにメンバ参照演算子の意味を付加したため、従来のようにピリオドで始まる引数には$を付けるか、<br>
ピリオドは付けずに記述するようになりました（コマンドの引数にはコンマを使わないせいでメンバと解釈されてしまうため）。<br>
（ただしピリオドを付けない記法の場合はabcというメタ変数を作っていた場合に隠蔽されます）。
```
@ev.setAction $.player 
  $.act(
    moveLeft
    moveRight
  );
```
<br><br><br>

### ツクール変数の重複宣言防止
同じ変数を重複して宣言すると怒られます。エディタのような変数名の入力欄がないのでその代わり。
```
tkv {
  a = v[1..5],
  b = v[3]  // エラー
};
const v[] c = v[2]  // 書き換え不可のメタ変数として宣言すれば重複していても怒られない
```

<br><br><br>

### 一時変数の予約機能
ツクール変数を予約し、それをヒープ的な感じで一時変数として扱えるようにします。<br>

足りなくなるとコンパイル中にエラーを吐きます。また、tkvで宣言したツクール変数と番号が被っていると怒られます。<br>
```
temp v[1 .. 100];
temp v[201 .. 300];
{
  temp_v qwe = 3;  // v[1..100]およびv[201..300]のうちどれかが予約され、スコープ内でqweという名前で使える
  @qwe = 4;
};  // スコープを出ると勝手に解放されるほか、qwe.free()で明示的に解放も可能
```
ちなみに宣言時に初期化しないと何が入ってるか分かりません。恐ろしいことに。

<br><br><br>

### メタ関数の機能強化
式中で関数が値を返せないのは不便なので返せるようになりました。<br>
当然ながら戻り値の型指定をする必要があります。<br>
戻り値とは関係なく、関数内にあるイベントコマンドも関数呼び出しが掛かった行の**直前**に出力されます。<br>
ちなみに、要求された値と関数の返り値を一致させるために先述の一時変数が使われます。
```
func number TestFunction(number a, number b) {
  number c = a + b;
  @msg.show c;
  return c;
};

// この位置に@msg.show "7"が発生する
v[1] = TestFunction(3, 4);  // v[1] = 7


temp v[1];
func v[] ccc(v[] a, number b) {
  return v[a + b]
};

// この位置に v[1] = v[v[5] + 2] が発生する
@party.money $.add( ccc(v[5], 2) );  // 実際は @party.money .add(v[1]) となる
```

<br><br><br>

### クラスの定義
クラスを定義し、それを実行中にインスタンス(実体)として生成できます。<br>
クラスにはメンバ(実体がそれぞれ持つ変数)・メソッド(関数)を持たせることができます。<br>
ちなみにクラスではない基本型にも組み込みメソッドが付いています（先述のnumber.isFloat()とか）。
```
class Harpy {
  v[] address;  // メンバ

  new(v[] adr) {  // コンストラクタ。生成時に使う処理で、これはv[]を引数に取る
    this.address = adr;  // thisでメンバアクセスができる
    @this.HP() = 40;
  };

  v[] HP() { 
    return v[this.address.ext() + 1];  // v[].ext()はv[]の中身を出す。つまりaddressの変数の次の番号の変数を返す
  };
};

Harpy imouto = new( v[1] );  // // 宣言。同時にv[2] = 40というコマンドも出力
@imouto.HP -= 5;  // v[2] -= 5。引数を持たないメソッドは()を省略できる
imouto.address = v[3];  // メンバ書き換えもできる
```

<br>
たぶん継承もできるようにします。
<br><br><br>

### バニラTPCの記述
文法の変更によって一切のバニラのコードを受け付けなくなったので最低限の後方互換措置です。<br>
処理としては中身のコードをそのまま返すだけです。AltTPC側で使っているメタ変数なども一切使えません。
```
#include(".\legacy_code.txt", vanilla);
vanilla {
  v[1] = 2
  t[2] .asg "asd"
};
```
<br><br><br>

### 未変更箇所の更新スキップ
前回のコンパイル時とソースコードを比較し、変更されていない部分は出力しないようにして<br>
大規模プロジェクトにおけるコンパイル速度を向上させます。

<br><br><br>

### メッセージ・エラー
mes(string)でコンソールにメッセージを出力します。BTLにもあったやつ。<br>
err(string)でコンパイルを停止してエラーを吐かせます。コード内の位置も言ってくれます。

<br><br><br>

### その他いろいろ
ArrayとかDictionaryとかRegexとか予定中だけど未定です。

-->

<br><br><br>
