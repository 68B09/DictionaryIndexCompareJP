/*
The MIT License (MIT)

Copyright (c) 2016 ZZO.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DictionaryIndexCompares
{
	/// <summary>
	/// 辞書読み順ソート(日本語版)
	/// </summary>
	public class DictionaryIndexCompareJP : IComparer<string>
	{
		/// <summary>
		/// 文字列大小判定(辞書読み順)
		/// </summary>
		/// <param name="pString1">よみ1</param>
		/// <param name="pString2">よみ2</param>
		/// <returns>負=pString2が大きい、0=等しい、正=pString1が大きい</returns>
		public int Compare(string pString1, string pString2)
		{
			CharReader reader1 = new CharReader(pString1);
			List<CharReader.CharFlag> flagList1 = new List<CharReader.CharFlag>(pString1.Length * 2);

			CharReader reader2 = new CharReader(pString2);
			List<CharReader.CharFlag> flagList2 = new List<CharReader.CharFlag>(pString2.Length * 2);

			bool blValid1, blValid2;
			char c1, c2;
			CharReader.CharFlag f1, f2;

			// かな順で判定
			while (true) {
				// よみ1,2から「よみ」を1文字取得
				blValid1 = reader1.GetCharData(out c1, out f1);
				blValid2 = reader2.GetCharData(out c2, out f2);

				// 文字列長で判定
				if ((blValid1 == false) && (blValid2 == false)) {
					break;
				}

				if ((blValid1 == true) && (blValid2 == false)) {
					return 1;
				}

				if ((blValid1 == false) && (blValid2 == true)) {
					return -1;
				}

				// UNICODE順による大小判定
				int diff = c1 - c2;
				if (diff != 0) {
					return diff;
				}

				// 付加情報を記録
				flagList1.Add(f1);
				flagList2.Add(f2);
			}

			// 濁音などの付加情報で判定
			int i = 0;
			while (true) {
				// よみ1,2から付加情報を1つ取得
				blValid1 = i < flagList1.Count;
				if (blValid1) {
					f1 = flagList1[i];
				}

				blValid2 = i < flagList2.Count;
				if (blValid2) {
					f2 = flagList2[i];
				}

				// 文字列長で判定
				if ((blValid1 == false) && (blValid2 == false)) {
					break;
				}

				if ((blValid1 == true) && (blValid2 == false)) {
					return 1;
				}

				if ((blValid1 == false) && (blValid2 == true)) {
					return -1;
				}

				// 付加情報で判定
				int diff = f1 - f2;
				if (diff != 0) {
					return diff;
				}

				i++;
			}

			// 同一と判定
			return 0;
		}

		/// <summary>
		/// 大小判定用文字列取得
		/// </summary>
		/// <param name="pString">よみ文字列</param>
		/// <returns>判定用文字列</returns>
		public string GetCompareString(string pString)
		{
			CharReader data = new CharReader(pString);
			StringBuilder sb = new StringBuilder(pString.Length * 2);

			char c;
			CharReader.CharFlag flag;

			while (data.GetCharData(out c, out flag)) {
				sb.Append(c);
			}

			return sb.ToString();
		}

		#region CharReader
		/// <summary>
		/// よみ解析・取得クラス
		/// </summary>
		public class CharReader
		{
			#region 定数類
			/// <summary>
			/// 付加情報
			/// </summary>
			[Flags]
			public enum CharFlag : int
			{
				Normal = 0,
				Dakuon = 1,
				Handakuon = 2,
				Tyouon = 4,
				Kogaki = 8,
				Kurikaesi = 16,
				Katakana = 32,
			}

			/// <summary>
			/// 小書き変換用データテーブル
			/// </summary>
			static readonly Dictionary<char, char> mKogakiTbl = new Dictionary<char, char>() {
				{ 'ぁ', 'あ' }, { 'ぃ', 'い' }, { 'ぅ', 'う' }, { 'ぇ', 'え' }, { 'ぉ', 'お' },
				{ 'ゃ', 'や' }, { 'ゅ', 'ゆ' }, { 'ょ', 'よ' }, { 'っ', 'つ' },
				{ 'ゕ', 'か' }, { 'ゖ', 'け' }, { 'ゎ', 'わ' },
				{ 'ァ', 'ア' }, { 'ィ', 'イ' }, { 'ゥ', 'ウ' }, { 'ェ', 'エ' }, { 'ォ', 'オ' },
				{ 'ャ', 'ヤ' }, { 'ュ', 'ユ' }, { 'ョ', 'ヨ' }, { 'ッ', 'ツ' },
				{ 'ヵ', 'カ' }, { 'ヶ', 'ケ' }, { 'ヮ', 'ワ' },
			};

			/// <summary>
			/// 母音取得用データテーブル
			/// </summary>
			static readonly Dictionary<char, char> mTyouonTbl = new Dictionary<char, char>() {
				{'あ', 'あ'}, {'か', 'あ'}, {'さ', 'あ'}, {'た', 'あ'}, {'な', 'あ'}, {'は', 'あ'}, {'ま', 'あ'}, {'や', 'あ'}, {'ら', 'あ'}, {'わ', 'あ'},
				{'い', 'い'}, {'き', 'い'}, {'し', 'い'}, {'ち', 'い'}, {'に', 'い'}, {'ひ', 'い'}, {'み', 'い'}, {'り', 'い'}, {'ゐ', 'い'},
				{'う', 'う'}, {'く', 'う'}, {'す', 'う'}, {'つ', 'う'}, {'ぬ', 'う'}, {'ふ', 'う'}, {'む', 'う'}, {'ゆ', 'う'}, {'る', 'う'},
				{'え', 'え'}, {'け', 'え'}, {'せ', 'え'}, {'て', 'え'}, {'ね', 'え'}, {'へ', 'え'}, {'め', 'え'}, {'れ', 'え'}, {'ゑ', 'え'},
				{'お', 'お'}, {'こ', 'お'}, {'そ', 'お'}, {'と', 'お'}, {'の', 'お'}, {'ほ', 'お'}, {'も', 'お'}, {'よ', 'お'}, {'ろ', 'お'}, {'を', 'お'},
				{'ん', 'ん'},
			};

			/// <summary>
			/// 半角全角変換等用データテーブル
			/// </summary>
			static readonly Dictionary<char, char> mSingleReplaceTbl = new Dictionary<char, char>() {
				{ 'ｦ', 'ヲ' },
				{ 'ｧ', 'ァ' }, { 'ｨ', 'ィ' }, { 'ｩ', 'ゥ' }, { 'ｪ', 'ェ' }, { 'ｫ', 'ォ' },
				{ 'ｬ', 'ャ' }, { 'ｭ', 'ュ' }, { 'ｮ', 'ョ' }, { 'ｯ', 'ッ' },
				{ 'ｰ', 'ー' },
				{ 'ｱ', 'ア' }, { 'ｲ', 'イ' }, { 'ｳ', 'ウ' }, { 'ｴ', 'エ' }, { 'ｵ', 'オ' },
				{ 'ｶ', 'カ' }, { 'ｷ', 'キ' }, { 'ｸ', 'ク' }, { 'ｹ', 'ケ' }, { 'ｺ', 'コ' },
				{ 'ｻ', 'サ' }, { 'ｼ', 'シ' }, { 'ｽ', 'ス' }, { 'ｾ', 'セ' }, { 'ｿ', 'ソ' },
				{ 'ﾀ', 'タ' }, { 'ﾁ', 'チ' }, { 'ﾂ', 'ツ' }, { 'ﾃ', 'テ' }, { 'ﾄ', 'ト' },
				{ 'ﾅ', 'ナ' }, { 'ﾆ', 'ニ' }, { 'ﾇ', 'ヌ' }, { 'ﾈ', 'ネ' }, { 'ﾉ', 'ノ' },
				{ 'ﾊ', 'ハ' }, { 'ﾋ', 'ヒ' }, { 'ﾌ', 'フ' }, { 'ﾍ', 'ヘ' }, { 'ﾎ', 'ホ' },
				{ 'ﾏ', 'マ' }, { 'ﾐ', 'ミ' }, { 'ﾑ', 'ム' }, { 'ﾒ', 'メ' }, { 'ﾓ', 'モ' },
				{ 'ﾔ', 'ヤ' }, { 'ﾕ', 'ユ' }, { 'ﾖ', 'ヨ' },
				{ 'ﾗ', 'ラ' }, { 'ﾘ', 'リ' }, { 'ﾙ', 'ル' }, { 'ﾚ', 'レ' }, { 'ﾛ', 'ロ' },
				{ 'ﾜ', 'ワ' }, { 'ﾝ', 'ン' },
				{ 'ﾞ', '゛' }, { 'ﾟ', '゜' },
				{ 'ゞ', 'ゝ' }, { 'ヽ', 'ゝ' }, { 'ヾ', 'ゝ' },
				{ '－', 'ー' },
			};

			/// <summary>
			/// 濁音変換用
			/// </summary>
			static readonly Dictionary<char, char> mDakuonTbl = new Dictionary<char, char>() {
				{'が','か'}, {'ぎ','き'}, {'ぐ','く'}, {'げ','け'}, {'ご','こ'},
				{'ざ','さ'}, {'じ','し'}, {'ず','す'}, {'ぜ','せ'}, {'ぞ','そ'},
				{'だ','た'}, {'ぢ','ち'}, {'づ','つ'}, {'で','て'}, {'ど','と'},
				{'ば','は'}, {'び','ひ'}, {'ぶ','ふ'}, {'べ','へ'}, {'ぼ','ほ'},
				{'ゔ','う'},
				{'ガ','カ'}, {'ギ','キ'}, {'グ','ク'}, {'ゲ','ケ'}, {'ゴ','コ'},
				{'ザ','サ'}, {'ジ','シ'}, {'ズ','ス'}, {'ゼ','セ'}, {'ゾ','ソ'},
				{'ダ','タ'}, {'ヂ','チ'}, {'ヅ','ツ'}, {'デ','テ'}, {'ド','ト'},
				{'バ','ハ'}, {'ビ','ヒ'}, {'ブ','フ'}, {'ベ','ヘ'}, {'ボ','ホ'},
				{'ヴ','ウ'},
			};

			/// <summary>
			/// 半濁音変換用
			/// </summary>
			static readonly Dictionary<char, char> mHandakuonTbl = new Dictionary<char, char>() {
				{'ぱ','は'}, {'ぴ','ひ'}, {'ぷ','ふ'}, {'ぺ','へ'}, {'ぽ','ほ'},
				{'パ','ハ'}, {'ピ','ヒ'}, {'プ','フ'}, {'ペ','ヘ'}, {'ポ','ホ'},
			};
			#endregion

			#region 変数類
			/// <summary>
			///  解析用文字リスト
			/// </summary>
			private List<char> mString = new List<char>();

			/// <summary>
			/// 解析用文字列設定
			/// </summary>
			public String Yomi
			{
				set {
					this.Initialize(value);
				}
			}

			/// <summary>
			/// 解析位置 0～
			/// </summary>
			private int mIndex;

			/// <summary>
			/// 直前の読み文字
			/// </summary>
			private char mPrevChar;
			#endregion

			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public CharReader()
			{
			}

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="pString">よみ文字列</param>
			public CharReader(string pString) : this()
			{
				this.Yomi = pString;
			}
			#endregion

			#region 初期化
			/// <summary>
			/// 初期化
			/// </summary>
			/// <param name="pString">よみ文字列</param>
			private void Initialize(string pString)
			{
				this.mIndex = 0;
				this.mPrevChar = char.MaxValue;

				// 合成文字などを合成後、文字単位に分解
				string normalized = pString.Normalize();
				this.mString.Clear();
				if (this.mString.Capacity < normalized.Length * 2) {
					this.mString.Capacity = normalized.Length * 2;
				}
				this.mString.AddRange(normalized.ToArray());
			}
			#endregion

			/// <summary>
			/// よみ文字取得
			/// </summary>
			/// <param name="pChar">よみ文字</param>
			/// <param name="pCharFlag">付加情報</param>
			/// <returns>true=処理続行、false=処理中止(末尾に達した)</returns>
			public bool GetCharData(out char pChar, out CharFlag pCharFlag)
			{
				char cc;

				pCharFlag = CharFlag.Normal;

				if (mIndex >= this.mString.Count) {
					pChar = char.MaxValue;
					return false;
				}

				int prevIndex = this.mIndex - 1;
				int nextIndex = this.mIndex + 1;

				#region 現在位置の文字を取得
				pChar = mString[mIndex];

				// 置き換え可能な文字なら置き換える
				if (pChar == 'ゟ') {
					pChar = 'よ';
					this.mString[mIndex] = pChar;
					this.mString.Insert(nextIndex, 'り');
				} else if (pChar == 'ヷ') {
					pChar = 'ワ';
					this.mString[mIndex] = pChar;
					this.mString.Insert(nextIndex, '゛');
				} else if (pChar == 'ヸ') {
					pChar = 'ヰ';
					this.mString[mIndex] = pChar;
					this.mString.Insert(nextIndex, '゛');
				} else if (pChar == 'ヹ') {
					pChar = 'ヱ';
					this.mString[mIndex] = pChar;
					this.mString.Insert(nextIndex, '゛');
				} else if (pChar == 'ヺ') {
					pChar = 'ヲ';
					this.mString[mIndex] = pChar;
					this.mString.Insert(nextIndex, '゛');
				} else if (pChar == 'ヿ') {
					pChar = 'コ';
					this.mString[mIndex] = pChar;
					this.mString.Insert(nextIndex, 'ト');
				} else if (mSingleReplaceTbl.TryGetValue(pChar, out cc)) {
					// 文字置換
					pChar = cc;
					this.mString[mIndex] = pChar;
				} else {
					// 濁音・半濁音を分離
					if (mDakuonTbl.TryGetValue(pChar, out cc)) {
						pChar = cc;
						this.mString[mIndex] = pChar;
						this.mString.Insert(nextIndex, '゛');
					} else if (mHandakuonTbl.TryGetValue(pChar, out cc)) {
						pChar = cc;
						this.mString[mIndex] = pChar;
						this.mString.Insert(nextIndex, '゜');
					}
				}

				mIndex++;
				#endregion

				#region 長音
				if (pChar == 'ー') {
					if (mPrevChar != char.MaxValue) {
						if (mTyouonTbl.TryGetValue(this.mPrevChar, out cc)) {
							pChar = cc;
						}
					}
					pCharFlag = CharFlag.Tyouon;
					return true;
				}
				#endregion

				#region 繰り返し
				if (pChar == 'ゝ') {
					if (mPrevChar != char.MaxValue) {
						pChar = this.mPrevChar;
					}
					pCharFlag = CharFlag.Kurikaesi;
					return true;
				}
				#endregion

				bool validNextChar = nextIndex < this.mString.Count;
				char nextChar = validNextChar ? this.mString[nextIndex] : char.MaxValue;

				if (mKogakiTbl.TryGetValue(pChar, out cc)) {
					// 小書き
					pChar = cc;
					pCharFlag |= CharFlag.Kogaki;
				}

				if ((pChar >= 'ァ') && (pChar <= 'ヶ')) {
					// 片仮名
					pChar = (char)(pChar - ('ァ' - 'ぁ'));
					pCharFlag |= CharFlag.Katakana;
				}

				if (nextChar == '゛') {
					// 濁音
					pCharFlag |= CharFlag.Dakuon;
					this.mString.RemoveAt(this.mIndex);
				} else if (nextChar == '゜') {
					// 半濁音
					pCharFlag |= CharFlag.Handakuon;
					this.mString.RemoveAt(this.mIndex);
				}

				if ((pChar >= 'あ') && (pChar <= 'ん')) {
					this.mPrevChar = pChar;
				} else {
					this.mPrevChar = char.MaxValue;
				}

				return true;
			}
		}
		#endregion
	}
}
