/*
 * Description:             CellBindPrefabBinder.cs
 * Author:                  TONYTANG
 * Create Date:             2022//05/29
 */

using UnityEngine;
using UnityEngine.UI;
using TH.Modules.UI;

namespace Game.Modules.UI
{
	/// <summary>
	/// CellBindPrefab单元格的组件绑定
	/// </summary>
	public partial class CellBindPrefab
	{
		
		/// <summary> 根RectTransform /// </summary>
		private RectTransform CellBindPrefab;
		/// <summary> 单元格背景 /// </summary>
		private Image imgCellBG;
		/// <summary> 单元格文本 /// </summary>
		private Text txtCell;
		/// <summary> 单元格按钮 /// </summary>
		private Button btnCell;

		/// <summary>
		/// 缓存组件
		/// </summary>
		protected override void cacheComponent()
		{
			base.cacheComponent();
			
			CellBindPrefab = mComponentBinder.NodeDatas[0].NodeTarget as RectTransform;
			imgCellBG = mComponentBinder.NodeDatas[1].NodeTarget as Image;
			txtCell = mComponentBinder.NodeDatas[2].NodeTarget as Text;
			btnCell = mComponentBinder.NodeDatas[3].NodeTarget as Button;
		}

		/// <summary>
		/// 清除组件
		/// </summary>
		protected override void clearComponent()
		{
			base.clearComponent();
			
			CellBindPrefab = null;
			imgCellBG = null;
			txtCell = null;
			btnCell = null;
		}
	}
}
