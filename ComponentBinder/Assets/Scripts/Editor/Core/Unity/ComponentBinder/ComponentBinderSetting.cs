/*
 * Description:             ComponentBinderSetting.cs
 * Author:                  TONYTANG
 * Create Date:             2022/05/29
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using static ComponentBinderCodeGenerator;

/// <summary>
/// ComponentBindSetting.cs
/// 组件绑定设置数据
/// </summary>
[CreateAssetMenu(fileName = "ComponentBinderSetting", menuName = "ScriptableObjects/ComponentBinderSetting", order = 1)]
public class ComponentBinderSetting : ScriptableObject
{
    /// <summary>
    /// 绑定模板数据
    /// </summary>
    [Serializable]
    public class BinderTemplateData
    {
        /// <summary>
        /// 绑定模板类型
        /// </summary>
        [Header("绑定模板类型")]
        public BinderTemplateType TemplateType = BinderTemplateType.UIWindowTemplate;

        /// <summary>
        /// 代码输出目录
        /// </summary>
        [Header("代码输出目录")]
        public string CodeOutputPath;
    }

    /// <summary>
    /// 组件绑定设置数据目录
    /// </summary>
    private const string BinderSettingFolderPath = "Assets/ComponentBinder";

    /// <summary>
    /// 组件绑定设置数据文件名
    /// </summary>
    private const string BinderSettingFileName = "ComponentBinderSetting";

    /// <summary>
    /// 绑定模板数据列表
    /// </summary>
    [Header("绑定模板数据列表")]
    public List<BinderTemplateData> BinderTemplateDataList;

    /// <summary>
    /// 组件绑定设置数据
    /// </summary>
    public static ComponentBinderSetting BinderSetting
    {
        get
        {
            if(mBinderSetting == null)
            {
                mBinderSetting = LoadComponentBinderSetting();
            }
            return mBinderSetting;
        }
    }
    private static ComponentBinderSetting mBinderSetting;

    /// <summary>
    /// 获取组件绑定设置数据路径
    /// </summary>
    /// <returns></returns>
    private static string GetComponentBinderSettingPath()
    {
        return $"{BinderSettingFolderPath}/{BinderSettingFileName}.asset";
    }

    /// <summary>
    /// 加载组件绑定设置数据
    /// </summary>
    /// <returns></returns>
    private static ComponentBinderSetting LoadComponentBinderSetting()
    {
        if(mBinderSetting == null)
        {
            var componentBinderFolderPath = GetComponentBinderSettingPath();
            mBinderSetting = AssetDatabase.LoadAssetAtPath<ComponentBinderSetting>(componentBinderFolderPath);
            if (mBinderSetting == null)
            {
                CreateComponentBinderSetting();
            }
        }
        return mBinderSetting;
    }

    /// <summary>
    /// 创建组件绑定设置数据
    /// </summary>
    /// <returns></returns>
    private static void CreateComponentBinderSetting()
    {
        if(!AssetDatabase.IsValidFolder(BinderSettingFolderPath))
        {
            AssetDatabase.CreateFolder("Assets", BinderSettingFolderPath);
        }
        mBinderSetting = new ComponentBinderSetting();
        var componentBinderFolderPath = GetComponentBinderSettingPath();
        AssetDatabase.CreateAsset(mBinderSetting, componentBinderFolderPath);
    }

    /// <summary>
    /// 获取指定绑定类型的代码输出目录
    /// </summary>
    /// <param name="templateType"></param>
    /// <returns></returns>
    public string GetCodeOutputPathByTemplateType(BinderTemplateType templateType)
    {
        if(mBinderSetting == null)
        {
            LoadComponentBinderSetting();
        }
        var findTemplateData = mBinderSetting.BinderTemplateDataList.Find((templateData) => templateData.TemplateType == templateType);
        if(findTemplateData == null)
        {
            Debug.LogError($"找不到绑定模板类型:{templateType}的代码输出设置!");
            return null;
        }
        if(string.IsNullOrEmpty(findTemplateData.CodeOutputPath))
        {
            Debug.LogError($"未设置:{templateType}的代码输出目录!");
            return null;
        }
        return findTemplateData.CodeOutputPath;
    }
}