/*
 * Description:             WindowBindPrefab.cs
 * Author:                  TONYTANG
 * Create Date:             2022//05/29
 */

using UnityEngine;
using UnityEngine.UI;
using TH.Modules.UI;

namespace Game.Modules.UI
{	
	/// <summary>
	/// WindowBindPrefab窗口
	/// </summary>
    public class WindowBindPrefab : BaseWindow
    {
		public WindowBindPrefab()
		{

		}

		/// <summary>
		/// 添加监听
		/// </summary>
		protected override void addListeners()
		{
			base.addListeners();
		}

		/// <summary>
		/// 窗口显示
		/// </summary>
		protected override void onShow()
		{
			base.onShow();
		}
		
		/// <summary>
		/// 移除监听
		/// </summary>
		protected override void removeListeners()
		{
			base.removeListeners();
		}
		
		/// <summary>
		/// 窗口销毁
		/// </summary>
		protected override void onDestroy()
		{
			base.onDestroy();
		}
    }
}
