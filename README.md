# DictionaryIndexCompareJP
日本語版辞書よみ順ソート用比較クラス
======================
辞書・辞典の目次のような「読み順ソート」を行うための文字列比較クラス(DictionaryIndexCompareJP.cs)です。  
JIS X 4061に従っているつもりです。

言語・開発環境
------
C#
.NET Framework 4.0
VisualStudio2015

使い方
------
１、プロジェクトへの登録
DictionaryIndexCompareJP.cs を目的のプロジェクトなどへコピー・登録してください。

２、コードの利用
list.Sort(new DictionaryIndexCompareJP()); のようにソーターに比較クラスのインスタンスを渡します。  
※TestForm.OnLoad() を参照のこと

関連情報
------
１、よみがな（平仮名や片仮名）をソートする前提になっています  
２、平仮名や片仮名以外の文字が混ざっている場合の結果は不定です  
３、実行速度の最適化は行っていません

ライセンス
------
ライセンスを適用するファイルは DictionaryIndexCompareJP.cs のみです。  
他のソース・プロジェクトファイルへは著作権を主張しません。  
The MIT License (MIT)  
Copyright (c) 2016 ZZO  
see also 'LICENSE' file
