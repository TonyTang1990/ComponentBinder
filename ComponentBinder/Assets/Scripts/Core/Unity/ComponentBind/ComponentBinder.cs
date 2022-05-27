/*
 * Description:             ComponentBinder.cs
 * Author:                  TONYTANG
 * Create Date:             2019//04/23
 */

using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 节点绑定类
/// </summary>
[DisallowMultipleComponent]
public class ComponentBinder : MonoBehaviour
{
    /// <summary>
    /// 绑定类型索引值
    /// </summary>
    public int BindCodeTypeIndex;

    /// <summary>
    /// UI节点数据
    /// </summary>
    public List<ComponentBindData> NodeDatas = new List<ComponentBindData>();
}