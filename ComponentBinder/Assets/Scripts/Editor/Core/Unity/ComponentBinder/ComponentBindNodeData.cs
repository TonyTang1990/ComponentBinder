/*
 * Description:             ComponentBindNodeData.cs
 * Author:                  TONYTANG
 * Create Date:             2019//04/23
 */

using System;

/// <summary>
/// ComponentBindNodeData节点绑定信息
/// </summary>
public class ComponentBindNodeData
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
    public ComponentBindData NodeData
    {
        get;
        set;
    }
        
    public ComponentBindNodeData(int nodeindex, ComponentBindData nodedata)
    {
        NodeIndex = nodeindex;
        NodeData = nodedata;
    }
}