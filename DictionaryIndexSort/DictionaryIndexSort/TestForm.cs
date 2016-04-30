using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DictionaryIndexCompares
{
	public partial class TestForm : Form
	{
		public TestForm()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (this.DesignMode) {
				return;
			}

			// ソートデータ
			List<string> list = new List<string>() {
				"アロエ",
				"あろえ",
				"でーたー",
				"でいたん",
				"でんおん",
				"データ",
				"ｱがパヺヿ",
				"あゞ",
				"ラジヱーター",
				"ぼんど",
				"ホント",
			};

			// ソート実行
			list.Sort(new DictionaryIndexCompareJP());

			// データダンプ
			DictionaryIndexCompareJP cmp = new DictionaryIndexCompareJP();
			this.lstResult.Items.Clear();
			for (int i = 0; i < list.Count; i++) {
				string[] items = new string[] { i.ToString(), list[i], cmp.GetCompareString(list[i]) };
				this.lstResult.Items.Add(new ListViewItem(items));
			}
		}
	}
}
