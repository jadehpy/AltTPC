# AltTPC
<br>
TPCの方言です。<br>
文法のふわふわとした部分を削減してより厳密にし、かつ便利にする目的で作られています。
<br><br><br><br>

## 特徴

<br>

### ステートメント区切りにセミコロン必須
必須です。サボるな。

<br><br><br>
  
### 静的な型の導入
メタ変数の宣言に型名が必要になりました。
```
v[] a = v[1];  // v[定数数値] しか受け付けない型
const string b = "c" + "d";
number c = 4.5;  // 整数もしくは小数の数値　内部的にはint/float
bool d = c.isFloat()  // true
```
<br><br><br>

### コマンドと非コマンドの明確化
「@msg.show」などのコマンドに@が付いているのに対し「v[1] = 3」 のように変数操作などには付いておらず、<br>
これによってメタ変数への代入とツクール変数への代入が混ざったりするのをコマンドに全て@を付けることで防ぎます。
```
@v[1] = 5;
v[] a = v[2];
a = 3;  // コンパイルエラー
@a = 3;  // これは@v[2] = 3で通る
```
<br><br><br>

### コマンド固有引数の記法の変更
ピリオドにメンバアクセスする演算子の意味を付加したため、従来のようにピリオドで始まる引数には$を付けるか、<br>
ピリオドは付けずに記述するようになりました（ただしピリオドを付けない記法の場合はabcというメタ変数を作っていた場合に隠蔽されます）。
```
@ev.setAction $.player 
  $.act(
    moveLeft
    moveRight
  );
```
<br><br><br>

### ツクール変数の重複宣言防止
同じ変数を重複して宣言すると怒られます。エディタのように変数名の入力欄がないのでその代わり。
```
tkv {
  a = v[1..5],
  b = v[3]  // エラー
};
const v[] c = v[2]  // 書き換え不可のメタ変数として宣言すれば重複していても怒られない
```

<br><br><br>

### 一時変数の予約機能
ツクール変数を予約し、それをヒープ的な感じで一時変数として扱えるようにします。
足りなくなるとコンパイル中にエラーを吐きます。また、tkvで宣言したツクール変数と番号が被っていると怒られます。
```
temp v[1 .. 100];
{
  temp_v qwe = 3;  // v[1..100]のうちどれかが予約され、スコープ内でqweという名前で使える
  @qwe = 4;
}  // スコープを出ると勝手に解放されるほか、qwe.free()で明示的に解放も可能
```
ちなみに宣言時に初期化しないと何が入ってるか分かりません。恐ろしいことに。

<br><br><br>

### メタ関数の機能強化
式中で関数が値を返せないのは不便なので返せるようになりました。
当然ながら戻り値の型指定をする必要があります。
戻り値とは関係なく、関数内にあるイベントコマンドも関数呼び出しが掛かった行の**直前**に出力されます。
```
func number TestFunction(number a, number b) {
  number c = a + b;
  @msg.show c;
  return c;
}

// ここに@msg.show "7"が発生する
v[1] = TestFunction(3, 4);  // v[1] = 7
```
