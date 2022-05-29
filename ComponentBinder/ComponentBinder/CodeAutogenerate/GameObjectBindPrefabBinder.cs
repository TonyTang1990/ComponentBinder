/*
 * Description:             GameObjectBindPrefabBinder.cs
 * Author:                  TONYTANG
 * Create Date:             2022/05/29
 */

using UnityEngine;
using UnityEngine.UI;
using TH.Modules.UI;

namespace Game.Modules.UI
{
	/// <summary>
	/// GameObjectBindPrefab的组件绑定
	/// </summary>
	public partial class GameObjectBindPrefab
	{
		
		/// <summary> 根节点 /// </summary>
		private Transform GameObjectBindPrefab;
		/// <summary> 头顶挂点 /// </summary>
		private Transform HeadNode;
		/// <summary> 翅膀挂点 /// </summary>
		private Transform WingNode;

		/// <summary>
		/// 缓存组件
		/// </summary>
		protected void cacheComponent()
		{
			
			GameObjectBindPrefab = mComponentBinder.NodeDatas[0].NodeTarget as Transform;
			HeadNode = mComponentBinder.NodeDatas[1].NodeTarget as Transform;
			WingNode = mComponentBinder.NodeDatas[2].NodeTarget as Transform;
		}

		/// <summary>
		/// 清除组件
		/// </summary>
		protected void clearComponent()
		{
			
			GameObjectBindPrefab = null;
			HeadNode = null;
			WingNode = null;
		}
	}
}
