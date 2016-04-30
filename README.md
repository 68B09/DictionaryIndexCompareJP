# DictionaryIndexCompareJP
日本語版辞書よみ順ソート用比較クラス

辞書・辞典の目次のようなソートを行うための文字列比較クラスです。
JIS X 4061に従っているつもりです。
list.Sort(new DictionaryIndexCompareJP()) のようにソーターに比較クラスのインスタンスを渡して使ってください。
※TestForm.OnLoad() を参照

・よみがな（平仮名や片仮名）をソートする前提になっています
・平仮名や片仮名以外の文字が混ざっている場合の結果は不定です
