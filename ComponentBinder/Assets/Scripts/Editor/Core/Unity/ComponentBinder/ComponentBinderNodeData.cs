/*
 * Description:             ComponentBinderNodeData.cs
 * Author:                  TONYTANG
 * Create Date:             2019//04/23
 */

using System;

/// <summary>
/// ComponentBinderNodeData节点绑定信息
/// </summary>
public class ComponentBinderNodeData
{
    /// <summary>
    /// 节点索引号
    /// </summary>
    public int NodeIndex
    {
        get;
        set;
    }

    /// <summary>
    /// 节点数据
    /// </summary>
    public ComponentBinderData NodeData
    {
        get;
        set;
    }
        
    public ComponentBinderNodeData(int nodeindex, ComponentBinderData nodedata)
    {
        NodeIndex = nodeindex;
        NodeData = nodedata;
    }
}