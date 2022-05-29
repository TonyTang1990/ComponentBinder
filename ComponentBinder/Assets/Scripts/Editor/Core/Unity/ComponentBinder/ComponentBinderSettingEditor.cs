/*
 * Description:             ComponentBinderSettingEditor.cs
 * Author:                  TONYTANG
 * Create Date:             2022/05/29
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static ComponentBinderCodeGenerator;

/// <summary>
/// ComponentBinderSettingEditor.cs
/// 组件绑定自定义Editor
/// </summary>
[CustomEditor(typeof(ComponentBinderSetting))]
[DisallowMultipleComponent]
public class ComponentBinderSettingEditor : Editor
{
    /// <summary>
    /// TextArea Style
    /// </summary>
    private GUIStyle mTextAreaStyle;

    /// <summary>
    /// 绑定模板数据列表属性
    /// </summary>
    private SerializedProperty mBinderTemplateDataListProperty;

    protected void OnEnable()
    {
        mBinderTemplateDataListProperty = serializedObject.FindProperty("BinderTemplateDataList");
    }

    public override void OnInspectorGUI()
    {
        InitGUIStyles();
        serializedObject.Update();

        DrawBinderTemplateDataUI();

        serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// 初始化GUI Styles
    /// </summary>
    private void InitGUIStyles()
    {
        if (mTextAreaStyle == null)
        {
            mTextAreaStyle = new GUIStyle("textarea");
        }   
    }

    /// <summary>
    /// 绘制组件绑定设置数据面板
    /// </summary>
    private void DrawBinderTemplateDataUI()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("绑定模板数据列表:", GUILayout.Width(150.0f), GUILayout.Height(20.0f));
        for (int i = 0; i < mBinderTemplateDataListProperty.arraySize; i++)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            var binderTemplateDataProperty = mBinderTemplateDataListProperty.GetArrayElementAtIndex(i);
            var templateTypeProperty = binderTemplateDataProperty.FindPropertyRelative("TemplateType");
            var codeOutputPathProperty = binderTemplateDataProperty.FindPropertyRelative("CodeOutputPath");
            EditorGUILayout.LabelField("绑定模板类型:", GUILayout.Width(120.0f), GUILayout.Height(20.0f));
            EditorGUI.BeginChangeCheck();
            var preTemplateTypeValue = (BinderTemplateType)templateTypeProperty.intValue;
            var newTemplateTypeValue = (BinderTemplateType)EditorGUILayout.EnumPopup(preTemplateTypeValue, GUILayout.ExpandWidth(true), GUILayout.Height(20f));
            if (EditorGUI.EndChangeCheck())
            {
                if (CheckTemplateTypeDumplicated(newTemplateTypeValue))
                {
                    Debug.LogError($"不支持重复设置相同模板类型:{newTemplateTypeValue}");
                    templateTypeProperty.intValue = (int)preTemplateTypeValue;
                }
                else
                {
                    templateTypeProperty.intValue = (int)newTemplateTypeValue;
                }
            }
            if (GUILayout.Button("-", GUILayout.Width(100f), GUILayout.Height(20f)))
            {
                mBinderTemplateDataListProperty.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("代码输出目录:", GUILayout.Width(120.0f), GUILayout.Height(20.0f));
            EditorGUILayout.LabelField(codeOutputPathProperty.stringValue, mTextAreaStyle, GUILayout.ExpandWidth(true), GUILayout.Height(20.0f));
            if(GUILayout.Button("选择目录", GUILayout.Width(100f), GUILayout.Height(20f)))
            {
                var preFolderPathChosen = string.IsNullOrEmpty(codeOutputPathProperty.stringValue) ? Application.dataPath : codeOutputPathProperty.stringValue;
                var newFolderPathChosen = EditorUtility.OpenFolderPanel("代码输出目录选择", preFolderPathChosen, string.Empty);
                if(!IsValideCodeOutputPath(newFolderPathChosen))
                {
                    var valideCodeOutputPath = GetValideCoudeOutputPath();
                    Debug.LogError($"代码路径必须在:{valideCodeOutputPath}目录下,设置代码输出目录:{newFolderPathChosen}无效!");
                    newFolderPathChosen = preFolderPathChosen;
                }
                codeOutputPathProperty.stringValue = newFolderPathChosen;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }
        if (GUILayout.Button("+", GUILayout.ExpandWidth(true), GUILayout.Height(20f)))
        {
            mBinderTemplateDataListProperty.InsertArrayElementAtIndex(mBinderTemplateDataListProperty.arraySize);
        }
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 检查绑定模板类型是否重复
    /// </summary>
    /// <param name="templateType"></param>
    /// <returns></returns>
    private bool CheckTemplateTypeDumplicated(BinderTemplateType templateType)
    {
        for (int i = 0; i < mBinderTemplateDataListProperty.arraySize; i++)
        {
            var binderTemplateDataProperty = mBinderTemplateDataListProperty.GetArrayElementAtIndex(i);
            var templateTypeProperty = binderTemplateDataProperty.FindPropertyRelative("TemplateType");
            if(templateTypeProperty.intValue == (int)templateType)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 指定路径是否是有效代码输出路径
    /// </summary>
    /// <param name="codeOutputPath"></param>
    /// <returns></returns>
    private bool IsValideCodeOutputPath(string codeOutputPath)
    {
        var valideCodeOutputPath = GetValideCoudeOutputPath();
        return codeOutputPath.StartsWith(valideCodeOutputPath);
    }

    /// <summary>
    /// 获取有效的代码输出路径
    /// </summary>
    /// <returns></returns>
    private string GetValideCoudeOutputPath()
    {
        var assetsLength = "Assets".Length;
        var valideCodeOutputPath = Application.dataPath;
        return Application.dataPath.Substring(0, valideCodeOutputPath.Length - assetsLength);
    }
}