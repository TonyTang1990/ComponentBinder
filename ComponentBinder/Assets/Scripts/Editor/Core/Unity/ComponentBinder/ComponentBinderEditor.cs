/*
 * Description:             ComponentBinderEditor.cs
 * Author:                  TONYTANG
 * Create Date:             2019//04/23
 */

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// 组件绑定自定义Editor
/// </summary>
[CustomEditor(typeof(ComponentBinder))]
[CanEditMultipleObjects]
public class ComponentBinderEditor : Editor
{
    /// <summary>
    /// 自定义GUI Skin
    /// </summary>
    private GUISkin mCustomGUISkin;

    /// <summary>
    /// 目标组件绑定组件
    /// </summary>
    private ComponentBinder mTargetComponentBinder;

    /// <summary>
    /// UI节点UI显示数据
    /// </summary>
    private List<ComponentBinderNodeData> mComponentBindNodeDatas;

    /// <summary>
    /// 当前选择的节点对象
    /// </summary>
    private UnityEngine.Transform mSelectedTarget;

    /// <summary>
    /// 可选的节点类型
    /// </summary>
    private string[] mAvalibleSelectedComponentArray;

    /// <summary>
    /// 可选的节点类型(和mAvalibleSelectedComponentArray一一对应)
    /// </summary>
    private UnityEngine.Object[] mAvalibleSelectedComponents;

    /// <summary>
    /// 当前选择的需要绑定的节点类型(e.g. GameObject或者Component)
    /// </summary>
    private UnityEngine.Object mCurrentSelectedComponent;

    /// <summary>
    /// 当前选择绑定的节点类型索引
    /// </summary>
    private int mCurrentSelectedComponentIndex;

    /// <summary>
    /// 是否折叠
    /// </summary>
    private bool mIsFoldOut = true;

    /// <summary>
    /// 当前可选的绑定类型数组
    /// </summary>
    private string[] mAvalaibleBindCodeTypes = Enum.GetNames(typeof(ComponentBinderCodeGenerator.BinderCodeType));

    /// <summary>
    /// 是否处于Prefab编辑模式
    /// </summary>
    private bool IsUnderPrefabMode;


    private void Awake()
    {
        if (mCustomGUISkin == null)
        {
            mCustomGUISkin = EditorGUIUtility.Load("ComponentBinder/TCustomGUISkin.guiskin") as GUISkin;
        }
        IsUnderPrefabMode = PrefabStageUtility.GetCurrentPrefabStage() != null;
    }

    private void OnEnable()
    {
        
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical(GUILayout.MinWidth(400.0f));
        initUIData();
        displayUITitle();
        EditorGUI.BeginChangeCheck();
        displayComponentBindNodes();
        displayComponentBindNodesTitle();
        mIsFoldOut = EditorGUILayout.Foldout(mIsFoldOut, mIsFoldOut ? "收起" : "展开", true);
        if (mIsFoldOut)
        {
            displayAllComponentBindNodes();
            displayUIInteraction();
        }
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(mTargetComponentBinder);
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 初始化UI显示数据
    /// </summary>
    private void initUIData()
    {
        if (mComponentBindNodeDatas == null)
        {
            mComponentBindNodeDatas = new List<ComponentBinderNodeData>();
        }

        if (mAvalibleSelectedComponentArray == null)
        {
            mSelectedTarget = null;
            mAvalibleSelectedComponentArray = new string[2] { "Invalide", "GameObject" };
            mCurrentSelectedComponent = null;
            mCurrentSelectedComponentIndex = 0;
        }

        mComponentBindNodeDatas.Clear();
        mTargetComponentBinder = target as ComponentBinder;
        var uinodedataslength = mTargetComponentBinder.NodeDatas.Count;
        for (int i = 0; i < uinodedataslength; i++)
        {
            var uinodeuidata = new ComponentBinderNodeData(i, mTargetComponentBinder.NodeDatas[i]);
            mComponentBindNodeDatas.Add(uinodeuidata);
        }
    }

    /// <summary>
    /// 显示标题
    /// </summary>
    private void displayUITitle()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("UI节点绑定", mCustomGUISkin.customStyles[0]);
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 显示节点绑定UI
    /// </summary>
    private void displayComponentBindNodes()
    {
        EditorGUILayout.BeginHorizontal();
        var preselectedtarget = mSelectedTarget;
        mSelectedTarget = EditorGUILayout.ObjectField(mSelectedTarget, typeof(Transform), true, GUILayout.MinWidth(150.0f)) as Transform;
        if (preselectedtarget != mSelectedTarget)
        {
            resetUINodeBindData();
        }
        var pretypeindex = mCurrentSelectedComponentIndex;
        mCurrentSelectedComponentIndex = EditorGUILayout.Popup(mCurrentSelectedComponentIndex, mAvalibleSelectedComponentArray, GUILayout.MinWidth(100.0f));
        if (pretypeindex != mCurrentSelectedComponentIndex && mCurrentSelectedComponentIndex > 0)
        {
            var newuinodedata = new ComponentBinderData(mAvalibleSelectedComponents[mCurrentSelectedComponentIndex]);
            mTargetComponentBinder.NodeDatas.Add(newuinodedata);
            mComponentBindNodeDatas.Add(new ComponentBinderNodeData(mTargetComponentBinder.NodeDatas.Count - 1, newuinodedata));
            resetUINodeBindData();
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 显示UI绑定节点标题
    /// </summary>
    private void displayComponentBindNodesTitle()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("已绑定UI节点", mCustomGUISkin.customStyles[0]);
        EditorGUILayout.Space();
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 显示所有绑定的Node Editor节点
    /// </summary>
    private void displayAllComponentBindNodes()
    {
        EditorGUILayout.BeginVertical();
        for (int i = 0; i < mComponentBindNodeDatas.Count; i++)
        {
            displayUINode(mComponentBindNodeDatas[i]);
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 显示可交互部分
    /// </summary>
    private void displayUIInteraction()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("当前绑定类型 : ", GUILayout.Width(80.0f));
        mTargetComponentBinder.BindCodeTypeIndex = EditorGUILayout.Popup(mTargetComponentBinder.BindCodeTypeIndex, mAvalaibleBindCodeTypes, GUILayout.Width(120.0f));
        if (GUILayout.Button("生成代码", GUILayout.Width(100.0f)))
        {
            var bindCodeType = (ComponentBinderCodeGenerator.BinderCodeType)Enum.Parse(typeof(ComponentBinderCodeGenerator.BinderCodeType), mAvalaibleBindCodeTypes[mTargetComponentBinder.BindCodeTypeIndex]);
            var targetGameObject = (target as ComponentBinder).gameObject;
            if (ComponentBinderCodeGenerator.GenerateCode(bindCodeType, targetGameObject, mTargetComponentBinder.NodeDatas))
            {
                OpenExplore();
            }
        }
        if (IsUnderPrefabMode)
        {
            if (GUILayout.Button("预制件标脏", GUILayout.Width(100.0f)))
            {
                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 显示一个UI Node Editor节点
    /// </summary>
    /// <param name="componentNodeData"></param>
    private void displayUINode(ComponentBinderNodeData componentNodeData)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(componentNodeData.NodeIndex.ToString(), GUILayout.MinWidth(20.0f));
        EditorGUILayout.ObjectField(componentNodeData.NodeData.NodeTarget, componentNodeData.NodeData.NodeTarget.GetType(), false, GUILayout.MinWidth(150.0f));
        EditorGUILayout.LabelField("变量别名: ", GUILayout.MinWidth(60.0f));
        componentNodeData.NodeData.VariableAlias = EditorGUILayout.TextField(componentNodeData.NodeData.VariableAlias, GUILayout.MinWidth(100.0f));
        EditorGUILayout.LabelField("节点描述: ", GUILayout.MinWidth(60.0f));
        componentNodeData.NodeData.NodeDes = EditorGUILayout.TextField(componentNodeData.NodeData.NodeDes, GUILayout.MinWidth(100.0f));
        if (GUILayout.Button("移除", GUILayout.Width(50.0f)))
        {
            mTargetComponentBinder.NodeDatas.RemoveAt(componentNodeData.NodeIndex);
            mComponentBindNodeDatas.RemoveAt(componentNodeData.NodeIndex);
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 重置UI绑定数据
    /// </summary>
    private void resetUINodeBindData()
    {
        Debug.Log("resetUINodeBindData()");
        mAvalibleSelectedComponentArray = new string[2] { "Invalide", "GameObject" };
        mAvalibleSelectedComponents = new UnityEngine.Object[2] { null, mSelectedTarget.gameObject };
        var allcomponents = mSelectedTarget.GetComponents<Component>();
        var prelength = mAvalibleSelectedComponentArray.Length;
        Array.Resize<string>(ref mAvalibleSelectedComponentArray, prelength + allcomponents.Length);
        Array.Resize<UnityEngine.Object>(ref mAvalibleSelectedComponents, prelength + allcomponents.Length);
        for (int i = prelength, length = mAvalibleSelectedComponentArray.Length; i < length; i++)
        {
            if (allcomponents[i - prelength] != null)
            {
                mAvalibleSelectedComponentArray[i] = allcomponents[i - prelength].GetType().Name;
                mAvalibleSelectedComponents[i] = allcomponents[i - prelength];
            }
        }
        mCurrentSelectedComponent = null;
        mCurrentSelectedComponentIndex = 0;
    }

    /// <summary>
    /// 打开文件夹浏览
    /// </summary>
    private void OpenExplore()
    {
        var fullpath = Path.GetFullPath(ComponentBinderCodeGenerator.BinderCodeGeneratorOutputFolderPath);
        System.Diagnostics.Process.Start("explorer.exe", fullpath);
    }
}