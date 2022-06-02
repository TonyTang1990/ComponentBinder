/*
 * Description:             WindowBindPrefabBinder.cs
 * Author:                  TONYTANG
 * Create Date:             2022//06/02
 */

using UnityEngine;
using UnityEngine.UI;
using TH.Modules.UI;

namespace Game.Modules.UI
{
	/// <summary>
	/// WindowBindPrefab窗口的组件绑定
	/// </summary>
    public partial class WindowBindPrefab : BaseWindow
    {
		 
        /// <summary> 根GameObject /// </summary>
        private GameObject _rootGo;
        /// <summary> 根RectTrasform /// </summary>
        private RectTransform _rootRectranform;
        /// <summary> 背景图 /// </summary>
        private Image imgBg;
        /// <summary> 左侧按钮 /// </summary>
        private Button btnLeftSwitch;
        /// <summary> 右侧按钮 /// </summary>
        private Button btnRightSwitch;
		   
		/// <summary>
		/// 缓存组件
		/// </summary>
		protected override void cacheComponents()
		{
			base.cacheComponents();
            
            _rootGo = mComponentBinder.NodeDatas[0].NodeTarget as GameObject;
            _rootRectranform = mComponentBinder.NodeDatas[1].NodeTarget as RectTransform;
            imgBg = mComponentBinder.NodeDatas[2].NodeTarget as Image;
            btnLeftSwitch = mComponentBinder.NodeDatas[3].NodeTarget as Button;
            btnRightSwitch = mComponentBinder.NodeDatas[4].NodeTarget as Button;
        }

		/// <summary>
		/// 释放组件
		/// </summary>
		protected override void disposeComponents()
		{
			base.disposeComponents();
			
            _rootGo = null;
            _rootRectranform = null;
            imgBg = null;
            btnLeftSwitch = null;
            btnRightSwitch = null;
		}
    }
}
