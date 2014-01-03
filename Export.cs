using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.IO;

namespace CC2Amazon
{
	public partial class Export : Form
	{
		public Export()
		{
			InitializeComponent();

			comboUpdateDelete.SelectedIndex = 0;
			textLaunchDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
		}

		private void buttonExport_Click(object sender, EventArgs e)
		{
			bool delete = (comboUpdateDelete.Text == "Delete");

			#region Options SQL

			StringBuilder sbOptions = new StringBuilder();
			sbOptions.AppendLine("SELECT productCode AS SKU, t.amazon_label as OptionName, m.value_name as OptionValue ");
			sbOptions.AppendLine("FROM cubecart_inventory i ");
			sbOptions.AppendLine("INNER JOIN cubecart_category ON i.cat_id = cubecart_category.cat_id ");
			sbOptions.AppendLine("left join cubecart_options_bot b on i.productId = b.product ");
			sbOptions.AppendLine("inner join cubecart_options_mid m on b.value_id = m.value_id ");
			sbOptions.AppendLine("inner join cubecart_options_top t on m.father_id = t.option_id ");
			sbOptions.AppendLine("WHERE cubecart_category.cat_metadesc <> '' ");
			sbOptions.AppendLine("AND t.amazon_label <> '' ");
			sbOptions.AppendLine("AND b.option_price = 0 ");
			sbOptions.AppendLine("ORDER BY SKU, OptionName, OptionValue");

			#endregion

			#region Inventory SQL

			StringBuilder sbInventory = new StringBuilder();
			sbInventory.AppendLine("SELECT ");
			sbInventory.AppendLine("productCode AS SKU, ");
			sbInventory.AppendLine("' ' as ParentSKU, ");
			sbInventory.AppendLine("prod_metakeywords, ");
			sbInventory.AppendLine("name AS ProductName, ");
			sbInventory.AppendLine("'Riata Leather' AS Brand, ");
			sbInventory.AppendLine("'Riata Leather' AS Manufacturer, ");
			sbInventory.AppendLine("cat_metadesc AS ItemType, ");
			sbInventory.AppendLine("description AS ProductDescription, ");
			sbInventory.AppendLine("concat( 'http://www.riataleather.com/images/uploads/', image ) AS MainImageURL, ");
			sbInventory.AppendLine("prodWeight AS ShippingWeight, ");
			sbInventory.AppendLine("'LB' AS ShippingWeightUnitOfMeasure, ");
			sbInventory.AppendLine("' ' AS ItemPrice, ");
			sbInventory.AppendLine("'parent' as Parentage, ");
			sbInventory.AppendLine("'' as RelationshipType, ");
			sbInventory.AppendLine("'Style' as VariationTheme, ");
			sbInventory.AppendFormat("'{0}' as LaunchDate, ", textLaunchDate.Text);
			sbInventory.AppendLine("' ' as Quantity, ");
			sbInventory.AppendFormat("'{0}' as UpdateDelete, ", comboUpdateDelete.Text);
			sbInventory.AppendLine("'PrivateLabel' as registeredparameter ");
			sbInventory.AppendLine("FROM cubecart_inventory i ");
			sbInventory.AppendLine("INNER JOIN cubecart_category ON i.cat_id = cubecart_category.cat_id ");
			sbInventory.AppendLine("left join cubecart_options_bot b ");
			sbInventory.AppendLine("on i.productId = b.product ");
			sbInventory.AppendLine("inner join cubecart_options_mid m ");
			sbInventory.AppendLine("on b.value_id = m.value_id ");
			sbInventory.AppendLine("inner join cubecart_options_top t ");
			sbInventory.AppendLine("on m.father_id = t.option_id ");
			sbInventory.AppendLine("WHERE cubecart_category.cat_metadesc <> '' ");

			sbInventory.AppendLine("");

			sbInventory.AppendLine("UNION SELECT ");
			sbInventory.AppendLine("productCode AS SKU, ");
			sbInventory.AppendLine("productCode AS ParentSKU, ");
			sbInventory.AppendLine("prod_metakeywords, ");
			sbInventory.AppendLine("name AS ProductName, ");
			sbInventory.AppendLine("'Riata Leather' AS Brand, ");
			sbInventory.AppendLine("'Riata Leather' AS Manufacturer, ");
			sbInventory.AppendLine("cat_metadesc AS ItemType, ");
			sbInventory.AppendLine("description AS ProductDescription, ");
			sbInventory.AppendLine("' ' AS MainImageURL, ");
			sbInventory.AppendLine("' ' AS ShippingWeight, ");
			sbInventory.AppendLine("' ' AS ShippingWeightUnitOfMeasure, ");
			sbInventory.AppendLine("price AS ItemPrice, ");
			sbInventory.AppendLine("'child' as Parentage, ");
			sbInventory.AppendLine("'Variation' as RelationshipType, ");
			sbInventory.AppendLine("'Style' as VariationTheme, ");
			sbInventory.AppendFormat("'{0}' as LaunchDate, ", textLaunchDate.Text);
			sbInventory.AppendLine("'20' as Quantity, ");
			sbInventory.AppendFormat("'{0}' as UpdateDelete, ", comboUpdateDelete.Text);
			sbInventory.AppendLine("'PrivateLabel' as registeredparameter ");
			sbInventory.AppendLine("FROM cubecart_inventory i ");
			sbInventory.AppendLine("INNER JOIN cubecart_category ON i.cat_id = cubecart_category.cat_id ");
			sbInventory.AppendLine("left join cubecart_options_bot b ");
			sbInventory.AppendLine("on i.productId = b.product ");
			sbInventory.AppendLine("inner join cubecart_options_mid m ");
			sbInventory.AppendLine("on b.value_id = m.value_id ");
			sbInventory.AppendLine("inner join cubecart_options_top t ");
			sbInventory.AppendLine("on m.father_id = t.option_id ");
			sbInventory.AppendLine("WHERE cubecart_category.cat_metadesc <> '' ");
			sbInventory.AppendLine("ORDER BY SKU, ParentSKU ");

			#endregion

			StreamWriter sw = new StreamWriter(textFile.Text, false);

			try
			{
				string myConString = "SERVER=127.0.0.1;" +
								"DATABASE=riatatest;" +
								"UID=root;" +
								"PASSWORD=mysql;";

				MySqlConnection connection = new MySqlConnection(myConString);
				connection.Open();

				#region Load options

				MySqlCommand optionsCommand = connection.CreateCommand();
				optionsCommand.CommandText = sbOptions.ToString();

				MySqlDataReader optionsReader = optionsCommand.ExecuteReader();

				//dictionary of skus keying a dictionary of amazon labels keying a list of values
				Dictionary<string /*sku*/, Dictionary<string /*amazon label*/, List<string /*values*/>>> skus = new Dictionary<string, Dictionary<string, List<string>>>();

				while (optionsReader.Read())
				{
					string sku = optionsReader.GetString("SKU");
					string option = optionsReader.GetString("OptionName");
					string optionVal = optionsReader.GetString("OptionValue");

					Dictionary<string /*amazon label*/, List<string /*values*/>> options = null;
					List<string /*values*/> optionVals = null;

					if (!skus.TryGetValue(sku, out options))
					{
						options = new Dictionary<string, List<string>>();
						skus.Add(sku, options);
					}

					if (!options.TryGetValue(option, out optionVals))
					{
						optionVals = new List<string>();
						options.Add(option, optionVals);
					}

					optionVals.Add(string.Format("{0}: {1}", option, optionVal));
				}

				optionsReader.Close();

				#endregion

				MySqlCommand inventoryCommand = connection.CreateCommand();
				inventoryCommand.CommandText = sbInventory.ToString();

				MySqlDataReader inventoryReader = inventoryCommand.ExecuteReader();

				#region File Header

				sw.Write("TemplateType=Sports\t");
				sw.Write("Version=1.1/1.0.14\t");
				sw.Write("This row for Amazon.com use only.  Do not modify or delete.\t\t\t\t\t\t\t");
				sw.Write("Macros:Active\n");

				sw.Write("SKU\t");

				if (!delete)
				{
					sw.Write("ParentSKU\t");
					sw.Write("SearchTerm1\t");
					sw.Write("SearchTerm2\t");
					sw.Write("SearchTerm3\t");
					sw.Write("SearchTerm4\t");
					sw.Write("SearchTerm5\t");
					sw.Write("TargetAudience1\t");
					sw.Write("Material\t");
					sw.Write("ProductName\t");
					sw.Write("Brand\t");
					sw.Write("Manufacturer\t");
					sw.Write("ItemType\t");
					sw.Write("ProductDescription\t");
					sw.Write("MainImageURL\t");
					sw.Write("ShippingWeight\t");
					sw.Write("ShippingWeightUnitOfMeasure\t");
					sw.Write("ItemPrice\t");
					sw.Write("Parentage\t");
					sw.Write("RelationshipType\t");
					sw.Write("VariationTheme\t");
					sw.Write("LaunchDate\t");
					sw.Write("Quantity\t");
					sw.Write("UpdateDelete\t");
					sw.Write("registered-parameter\t");
					sw.Write("Style\n");
				}
				else
				{
					sw.Write("UpdateDelete\t");
					sw.Write("Style\n");
				}

				#endregion

				int childNum = 0;
				string lastParentSKU = "this is not it";
				string subSKU = "abcdefghijklmnopqrstuvwxyz";

				while (inventoryReader.Read())
				{
					string SKU = inventoryReader.GetString(0).Trim();
					string parentSKU = inventoryReader.GetString(1).Trim();
					string[] keyWords = inventoryReader.GetString(2).ToLower().Trim().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
					bool isParent = true;

					List<string> line = new List<string>();

					if (string.IsNullOrEmpty(parentSKU))
					{
						line.Add(SKU);
					}
					else
					{
						if (parentSKU != lastParentSKU)
						{
							childNum = 0;
						}

						line.Add(string.Format("{0}{1}", SKU, subSKU[childNum]));
						childNum++;
						lastParentSKU = parentSKU;
						isParent = false;
					}

					if (delete)
					{
						line.Add("Delete");
					}
					else
					{
						line.Add(parentSKU);

						if (isParent)
						{
							for (int i = 0; i < 5; i++)
							{
								if (i < keyWords.Length)
								{
									line.Add(keyWords[i].Trim());
								}
								else
								{
									line.Add("");
								}
							}

							//other static attributes
							line.Add("unisex-adult");
							if (SKU.StartsWith("SB") || SKU.StartsWith("PB"))
							{
								line.Add("wool");
							}
							else if (SKU.StartsWith("694"))
							{
								line.Add("mohair");
							}
							else
							{
								line.Add("leather");
							}
						}
						else
						{
							//for child items, just fill up with tabs
							line.Add("");
							line.Add("");
							line.Add("");
							line.Add("");
							line.Add("");
							line.Add("");
							line.Add("");
						}

						for (int i = 3; i < inventoryReader.FieldCount - 1; i++)
						{
							line.Add(inventoryReader.GetString(i).Trim().Replace("\n", "").Replace("\r", ""));
						}

						line.Add(inventoryReader.GetString(inventoryReader.FieldCount - 1));
					}

					if (isParent)
					{
						line.Add("");
						sw.WriteLine(string.Join("\t", line.ToArray()));
					}
					else
					{
						//dictionary of skus keying a dictionary of amazon labels keying a list of values
						//Dictionary<string /*sku*/, Dictionary<string /*amazon label*/, List<string /*values*/>>> skus = new Dictionary<string, Dictionary<string, List<string>>>();

						Dictionary<string /*amazon label*/, List<string /*values*/>> options = null;

						if (skus.TryGetValue(SKU, out options))
						{
							var list1 = new List<string>();
							var list2 = new List<string>();
							var list3 = new List<string>();
							var list4 = new List<string>();
							var list5 = new List<string>();
							var list6 = new List<string>();

							foreach (string option in options.Keys)
							{
								List<string /*values*/> optionVals = null;

								if (options.TryGetValue(option, out optionVals))
								{
									if (list1.Count == 0)
									{
										list1 = optionVals;
									}
									else if (list2.Count == 0)
									{
										list2 = optionVals;
									}
									else if (list3.Count == 0)
									{
										list3 = optionVals;
									}
									else if (list4.Count == 0)
									{
										list4 = optionVals;
									}
									else if (list5.Count == 0)
									{
										list5 = optionVals;
									}
									else if (list6.Count == 0)
									{
										list6 = optionVals;
									}
								}
							}

							int i = 0;

							if (list6.Count > 0)
							{
								var products = from l1 in list1
											   from l2 in list2
											   from l3 in list3
											   from l4 in list4
											   from l5 in list5
											   from l6 in list6
											   select new { option1 = l1, option2 = l2, option3 = l3, option4 = l4, option5 = l5, option6 = l6 };

								foreach (var p in products)
								{
									line[0] = string.Format("{0}{1}", SKU, subSKU[i]);
									i++;
									sw.WriteLine("{0}\t{1}; {2}; {3}; {4}; {5}; {6}", string.Join("\t", line.ToArray()), p.option1, p.option2, p.option3, p.option4, p.option5, p.option6);
								}
							}
							else if (list6.Count > 0)
							{
								var products = from l1 in list1
											   from l2 in list2
											   from l3 in list3
											   from l4 in list4
											   from l5 in list5
											   from l6 in list6
											   select new { option1 = l1, option2 = l2, option3 = l3, option4 = l4, option5 = l5, option6 = l6 };

								foreach (var p in products)
								{
									line[0] = string.Format("{0}{1}", SKU, subSKU[i]);
									i++;
									sw.WriteLine("{0}\t{1}; {2}; {3}; {4}; {5}; {6}", string.Join("\t", line.ToArray()), p.option1, p.option2, p.option3, p.option4, p.option5, p.option6);
								}
							}
							else if (list6.Count > 0)
							{
								var products = from l1 in list1
											   from l2 in list2
											   from l3 in list3
											   from l4 in list4
											   from l5 in list5
											   from l6 in list6
											   select new { option1 = l1, option2 = l2, option3 = l3, option4 = l4, option5 = l5, option6 = l6 };

								foreach (var p in products)
								{
									line[0] = string.Format("{0}{1}", SKU, subSKU[i]);
									i++;
									sw.WriteLine("{0}\t{1}; {2}; {3}; {4}; {5}; {6}", string.Join("\t", line.ToArray()), p.option1, p.option2, p.option3, p.option4, p.option5, p.option6);
								}
							}
							else if (list5.Count > 0)
							{
								var products = from l1 in list1
											   from l2 in list2
											   from l3 in list3
											   from l4 in list4
											   from l5 in list5
											   select new { option1 = l1, option2 = l2, option3 = l3, option4 = l4, option5 = l5 };

								foreach (var p in products)
								{
									line[0] = string.Format("{0}{1}", SKU, subSKU[i]);
									i++;
									sw.WriteLine("{0}\t{1}; {2}; {3}; {4}; {5}", string.Join("\t", line.ToArray()), p.option1, p.option2, p.option3, p.option4, p.option5);
								}
							}
							else if (list4.Count > 0)
							{
								var products = from l1 in list1
											   from l2 in list2
											   from l3 in list3
											   from l4 in list4
											   select new { option1 = l1, option2 = l2, option3 = l3, option4 = l4 };

								foreach (var p in products)
								{
									line[0] = string.Format("{0}{1}", SKU, subSKU[i]);
									i++;
									sw.WriteLine("{0}\t{1}; {2}; {3}; {4}", string.Join("\t", line.ToArray()), p.option1, p.option2, p.option3, p.option4);
								}
							}
							else if (list3.Count > 0)
							{
								var products = from l1 in list1
											   from l2 in list2
											   from l3 in list3
											   select new { option1 = l1, option2 = l2, option3 = l3 };

								foreach (var p in products)
								{
									line[0] = string.Format("{0}{1}", SKU, subSKU[i]);
									i++;
									sw.WriteLine("{0}\t{1}; {2}; {3}", string.Join("\t", line.ToArray()), p.option1, p.option2, p.option3);
								}
							}
							else if (list2.Count > 0)
							{
								var products = from l1 in list1
											   from l2 in list2
											   select new { option1 = l1, option2 = l2 };

								foreach (var p in products)
								{
									line[0] = string.Format("{0}{1}", SKU, subSKU[i]);
									i++;
									sw.WriteLine("{0}\t{1}; {2}", string.Join("\t", line.ToArray()), p.option1, p.option2);
								}
							}
							else if (list1.Count > 0)
							{
								var products = from l1 in list1
											   select new { option1 = l1 };

								foreach (var p in products)
								{
									line[0] = string.Format("{0}{1}", SKU, subSKU[i]);
									i++;
									sw.WriteLine("{0}\t{1}", string.Join("\t", line.ToArray()), p.option1);
								}
							}
						}
					}
				}

				connection.Close();
				inventoryReader.Close();
			}
			catch (Exception ex)
			{
				sw.WriteLine(sbOptions.ToString());
				sw.WriteLine(sbInventory.ToString());
			}

			sw.Close();
		}
	}
}
