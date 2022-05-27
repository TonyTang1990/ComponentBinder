/*
 * Description:             ComponentBindData.cs
 * Author:                  TONYTANG
 * Create Date:             2019//04/23
 */

using System;
using UnityEngine;

/// <summary>
/// 组件绑定节点信息
/// </summary>
[Serializable]
public class ComponentBindData
{
    /// <summary>
    /// 节点对象
    /// </summary>
    [SerializeField]
    public UnityEngine.Object NodeTarget;

    /// <summary>
    /// 变量别名(用于生成代码)
    /// </summary>
    [SerializeField]
    public string VariableAlias;

    /// <summary>
    /// 节点描述
    /// </summary>
    [SerializeField]
    public string NodeDes;
    
    public ComponentBindData(UnityEngine.Object target)
    {
        NodeTarget = target;
        NodeDes = string.Empty;
    }
}