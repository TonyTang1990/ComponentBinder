/*
 * Description:             GameLauncher.cs
 * Author:                  TONYTANG
 * Create Date:             2022/05/26
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameLauncher.cs
/// 游戏启动脚本
/// </summary>
public class GameLauncher : MonoBehaviour
{
    /// <summary>
    /// UI挂载节点
    /// </summary>
    [Header("UI挂载节点")]
    public Transform UICanvasNode;

    /// <summary>
    /// 窗口组件绑定预制件
    /// </summary>
    [Header("窗口组件绑定预制件")]
    public GameObject WindowBindPrefab;

    /// <summary>
    /// 单元格组件绑定预制件
    /// </summary>
    [Header("单元格组件绑定预制件")]
    public GameObject CellBindPrefab;

    /// <summary>
    /// GameObject组件绑定预制件
    /// </summary>
    [Header("GameObject组件绑定预制件")]
    public GameObject GameObjectBindPrefab;


}