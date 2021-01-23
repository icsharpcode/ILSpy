using System.Windows.Forms;
namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	internal class Issue2260
	{
		private void dgvItemList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			string text = default(string);
			string s = default(string);
			switch (text)
			{
				case "rowno":
					break;
				case "item_no":
					new object();
					break;
				case "stock_qty":
					{
						decimal result2 = default(decimal);
						if (!decimal.TryParse(s, out result2))
						{
							new object();
						}
						else if (result2 < 1m)
						{
							new object();
						}
						break;
					}
				case "new_price":
					{
						decimal num = default(decimal);
						break;
					}
				case "new_price4":
					{
						decimal result4 = default(decimal);
						if (decimal.TryParse(s, out result4) && !(result4 < 0m))
						{
						}
						break;
					}
				case "new_price1":
					{
						decimal result3 = default(decimal);
						if (!decimal.TryParse(s, out result3))
						{
							new object();
							break;
						}
						if (result3 < 0m)
						{
							new object();
						}
						new object();
						break;
					}
				case "new_price2":
					{
						decimal result = default(decimal);
						if (!decimal.TryParse(s, out result))
						{
							new object();
						}
						else if (result < 0m)
						{
							new object();
						}
						break;
					}
			}
		}
	}
}
