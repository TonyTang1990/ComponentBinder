/*
 * Description:             ComponentBinderCodeGenerator.cs
 * Author:                  TONYTANG
 * Create Date:             2019//04/23
 */

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Text;
using UnityEditor;
using UnityEngine;

/// <summary>
/// CompoenntBinder代码生成工具
/// </summary>
public static class ComponentBinderCodeGenerator
{

    /// <summary> 绑定代码生成类型 /// </summary>
    public enum BinderCodeType
    {
        UIWindow = 1,                           // UI窗口绑定类型
        UICell,                                 // UI Cell绑定类型
        GameObject,                             // GameObject绑定类型
    }

    /// <summary> 绑定模板类型 /// </summary>
    public enum BinderTemplateType
    {
        UIWindowTemplate = 1,                           // UI窗口模板类型
        UIWindowBinder,                                 // UI窗口组件绑定类型
        UICellTemplate,                                 // 单元格模板类型
        UICellBinder,                                   // 单元格组件绑定类型
        GameObjectBinder,                               // GameObject组件绑定类型
    }

    /// <summary>
    /// 绑定模板类型与模板文件名映射Map
    /// Key为代码绑定模板类型，Value为对应模板文件名
    /// </summary>
    private static Dictionary<int, string> BinderTemplateTypeMap = new Dictionary<int, string>()
    {
        { (int)BinderTemplateType.UIWindowTemplate, "WindowUITemplate.txt" },
        { (int)BinderTemplateType.UIWindowBinder, "WindowUIBinder.txt" },
        { (int)BinderTemplateType.UICellTemplate, "CellUITemplate.txt" },
        { (int)BinderTemplateType.UICellBinder, "CellUIBinder.txt" },
        { (int)BinderTemplateType.GameObjectBinder, "GameObjectUIBinder.txt" },
    };

    /// <summary> UIBinder模本文件目录路径 /// </summary>
    private const string UIBinderTemplateFolderPath = "ComponentBind/UIBinderTemplate/";

    /// <summary> UIBinder自动化代码生成输出目录 /// </summary>
    public static string UIBinderCodeGeneratorOutputFolderPath = Path.Combine(Application.dataPath, "../UICodeAutogenerate/");

    /// <summary> 默认的UI节点绑定成员名 /// </summary>
    private const string DefaultUIBinderMemberName = "mComponentBinder";

    #region 替换标签常量定义
    /// <summary> 成员循环替换标签 /// </summary>
    private const string MemberDefinitionLoopTag = "#MEMBER_DEFINITION_LOOP#";

    /// <summary> 成员循环初始化标签 /// </summary>
    private const string MemberInitLoopTag = "#MEMBER_INIT_LOOP#";

    /// <summary> 文件作者名替换标签 /// </summary>
    private const string AuthorSingleTag = "#Author#";

    /// <summary> 文件创建时间替换标签 /// </summary>
    private const string CreatedDateSingleTag = "#CreatedDate#";

    /// <summary> 单个类名替换标签 /// </summary>
    private const string ClassNameSingleTag = "#ClassName#";

    /// <summary> 单个绑定节点成员变量名替换标签 /// </summary>
    private const string BinderMemberNameSingleTag = "#BinderMemberName#";

    /// <summary> 单个节点描述替换标签 /// </summary>
    private const string NodeDesSingleTag = "#NodeDes#";

    /// <summary> 单个节点类型替换标签 /// </summary>
    private const string NodeTypeSingleTag = "#NodeType#";

    /// <summary> 单个节点名字替换标签 /// </summary>
    private const string NodeNameSingleTag = "#NodeName#";

    /// <summary> 单个节点成员名替换标签 /// </summary>
    private const string NodeMemberNameSingleTag = "#NodeMemberName#";

    /// <summary> 单个索引号替换标签 /// </summary>
    private const string NodeIndexSingleTag = "#NodeIndex#";

    /// <summary> 单个绑定节点绝对路径替换标签 /// </summary>
    private const string NodeAbsPathSingleTag = "#NodeAbsPath#";

    /// <summary> 成员循环释放标签 /// </summary>
    private const string MemberDisposeLoopTag = "#MEMBER_DISPOSE_LOOP#";
    #endregion
    
    /// <summary>
    /// 生成指定类型的模板代码
    /// </summary>
    /// <param name="bct">代码生成类型</param>
    /// <param name="go">UIBinder绑定挂载对象</param>
    /// <param name="uinodedatalist">UI绑定节点数据</param>
    public static bool GenerateCode(BinderCodeType bct, GameObject go, List<ComponentBindData> uinodedatalist)
    {
        string templatefile;
        var scriptownername = go.name;
        if(!Directory.Exists(UIBinderCodeGeneratorOutputFolderPath))
        {
            Directory.CreateDirectory(UIBinderCodeGeneratorOutputFolderPath);
        }
        var uinodenamelist = new List<string>();
        if (checkNodeNameDumplicated(uinodedatalist, ref uinodenamelist))
        {
            if (checkNodeBindTypeValidity(bct, uinodedatalist))
            {
                switch (bct)
                {
                    case BinderCodeType.UIWindow:
                        GenerateUIWindowCode(scriptownername, uinodedatalist);
                        GenerateUIWindowBinderCode(scriptownername, uinodedatalist);
                        return true;
                    case BinderCodeType.UICell:
                        GenerateUICellCode(scriptownername, uinodedatalist);
                        GenerateUICellBinderCode(scriptownername, uinodedatalist);
                        return true;
                    case BinderCodeType.GameObject:
                        GenerateGameObjectBinderCode(scriptownername, uinodedatalist);
                        return true;
                    default:
                        Debug.LogError(string.Format("不支持的绑定代码生成类型 : {0}！", bct));
                        return false;
                }
            }
            else
            {
                Debug.LogError("绑定UI绑定类型不存在，请先解决绑定节点类型问题再生成代码！");
                return false;
            }
        }
        else
        {
            return false;
        }
    }


    /// <summary>
    /// 获取指定绑定模板类型的模板文件名
    /// </summary>
    /// <param name="binderTemplateType"></param>
    /// <returns></returns>
    private static string GetBinderTemplateFileName(BinderTemplateType binderTemplateType)
    {
        string templatefile;
        if (!BinderTemplateTypeMap.TryGetValue((int)binderTemplateType, out templatefile))
        {
            Debug.LogError(string.Format("不支持的模板生成类型 : {0}", binderTemplateType));
            return null;
        }
        return templatefile;
    }


    /// <summary>
    /// 获取指定绑定模板类型的模板文件内容
    /// </summary>
    /// <param name="binderTemplateType"></param>
    /// <returns></returns>
    private static string GetBinderTemplateFileContent(BinderTemplateType binderTemplateType)
    {
        string templateFile = GetBinderTemplateFileName(binderTemplateType);
        if(templateFile == null)
        {
            return null;
        }
        var templateasset = GetTemplateAsset(templateFile);
        if (templateasset == null)
        {
            return null;
        }
        return templateasset.text;
    }

    /// <summary>
    /// 获取指定模板TextAsset
    /// </summary>
    /// <param name="templatefilename">模板文件名</param>
    /// <returns></returns>
    private static TextAsset GetTemplateAsset(string templatefilename)
    {
        var relativepath = Path.Combine(UIBinderTemplateFolderPath, templatefilename);
        var textasset = EditorGUIUtility.Load(relativepath);
        if (textasset != null)
        {
            return textasset as TextAsset;
        }
        else
        {
            Debug.LogError(string.Format("模板文件不存在 : Assets/Editor Default Resources/{0}", relativepath));
            return null;
        }
    }

    /// <summary>
    /// 检查节点是否有重名
    /// </summary>
    /// <param name="componentBindNodeDatalist"></param>
    /// <param name="componentNodeNameList"></param>
    /// <returns></returns>
    private static bool checkNodeNameDumplicated(List<ComponentBindData> componentBindNodeDatalist, ref List<string> componentNodeNameList)
    {
        foreach (var componentBindNodeData in componentBindNodeDatalist)
        {
            if (componentBindNodeData.NodeTarget != null)
            {
                if (componentBindNodeData.NodeTarget is ComponentBinder)
                {
                    //递归判定
                    var newComponentBinder = componentBindNodeData.NodeTarget as ComponentBinder;
                    if (newComponentBinder != null)
                    {
                        if (!checkNodeNameDumplicated(newComponentBinder.NodeDatas, ref componentNodeNameList))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        Debug.Log(string.Format("节点 : {0}找不到绑定组件类型 : UIBinder", componentBindNodeData.NodeTarget.name));
                        return false;
                    }
                }
                else
                {
                    // 变量别名为空的话，默认使用节点自身的名字作为变量名
                    var variablename = componentBindNodeData.VariableAlias.IsNullOrEmpty() ? componentBindNodeData.NodeTarget.name : componentBindNodeData.VariableAlias;
                    if (componentNodeNameList.Contains(variablename))
                    {
                        Debug.LogError(string.Format("节点别名重名 : {0}", variablename));
                        return false;
                    }
                    else
                    {
                        componentNodeNameList.Add(variablename);
                    }
                }
            }
            else
            {
                Debug.LogError("有空节点!");
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 检查节点绑定类型是否不存在
    /// </summary>
    /// <param name="bct">当前代码生成类型</param>
    /// <param name="componentNodeDataList">节点信息列表</param>
    /// <returns></returns>
    private static bool checkNodeBindTypeValidity(BinderCodeType bct, List<ComponentBindData> componentNodeDataList)
    {
        foreach (var componentNodeData in componentNodeDataList)
        {
            if (componentNodeData.NodeTarget != null)
            {
                if (componentNodeData.NodeTarget is ComponentBinder)
                {
                    //递归判定
                    var newComponentBinder = componentNodeData.NodeTarget as ComponentBinder;
                    if (checkNodeBindTypeValidity(bct, newComponentBinder.NodeDatas) == false)
                    {
                        return false;
                    }
                }
            }
            else
            {
                Debug.LogError("有节点绑定空组件!");
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 生成窗口UI模板代码
    /// </summary>
    /// <param name="scriptownername">脚本拥有者名字</param>
    /// <param name="uinodedatalist">UI绑定节点数据</param>
    private static void GenerateUIWindowCode(string scriptownername, List<ComponentBindData> uinodedatalist)
    {
        var templatecontent = GetBinderTemplateFileContent(BinderTemplateType.UIWindowTemplate);
        if (templatecontent == null)
        {
            return;
        }
        var ttemplate = new TTemplate(templatecontent);
        //通过自定义的替换规则和内容进行替换
        var classname = scriptownername;
        //替换类名
        ttemplate.setValue(ClassNameSingleTag, classname);
        //作者名替换
        ttemplate.setValue(AuthorSingleTag, GetAuthorName());
        //创建日期替换
        ttemplate.setValue(CreatedDateSingleTag, DateTime.Now.ToString("yyyy//MM/dd"));
        //递归判定，生成UI节点成员变量声明
        ttemplate.beginLoop(MemberDefinitionLoopTag);
        GenerateMemberDefinitionCode(ttemplate, uinodedatalist);
        ttemplate.endLoop();
        //替换UI节点成员变量初始化
        ttemplate.beginLoop(MemberInitLoopTag);
        GenerateMemberInitCode(ttemplate, uinodedatalist, DefaultUIBinderMemberName);
        ttemplate.endLoop();
        //替换UI节点成员释放
        ttemplate.beginLoop(MemberDisposeLoopTag);
        GenerateMemberDisposeCode(ttemplate, uinodedatalist, DefaultUIBinderMemberName);
        ttemplate.endLoop();

        var finalcontent = ttemplate.getContent();
        outputTemplateCodeGeneration(classname + ".cs", finalcontent);
    }

    /// <summary>
    /// 生成窗口UI组件绑定代码
    /// </summary>
    /// <param name="scriptownername">脚本拥有者名字</param>
    /// <param name="uinodedatalist">UI绑定节点数据</param>
    private static void GenerateUIWindowBinderCode(string scriptownername, List<ComponentBindData> uinodedatalist)
    {
        var templatecontent = GetBinderTemplateFileContent(BinderTemplateType.UIWindowBinder);
        if(templatecontent == null)
        {
            return;
        }
        var ttemplate = new TTemplate(templatecontent);
        //通过自定义的替换规则和内容进行替换
        var classname = scriptownername;
        //替换类名
        ttemplate.setValue(ClassNameSingleTag, classname);
        //作者名替换
        ttemplate.setValue(AuthorSingleTag, GetAuthorName());
        //创建日期替换
        ttemplate.setValue(CreatedDateSingleTag, DateTime.Now.ToString("yyyy//MM/dd"));
        //递归判定，生成UI节点成员变量声明
        ttemplate.beginLoop(MemberDefinitionLoopTag);
        GenerateMemberDefinitionCode(ttemplate, uinodedatalist);
        ttemplate.endLoop();
        //替换UI节点成员变量初始化
        ttemplate.beginLoop(MemberInitLoopTag);
        GenerateMemberInitCode(ttemplate, uinodedatalist, DefaultUIBinderMemberName);
        ttemplate.endLoop();
        //替换UI节点成员释放
        ttemplate.beginLoop(MemberDisposeLoopTag);
        GenerateMemberDisposeCode(ttemplate, uinodedatalist, DefaultUIBinderMemberName);
        ttemplate.endLoop();

        var finalcontent = ttemplate.getContent();
        outputTemplateCodeGeneration(classname + ".cs", finalcontent);
    }

    /// <summary>
    /// 生成UI Cell模板代码
    /// </summary>
    /// <param name="scriptownername">脚本拥有者名字</param>
    /// <param name="uinodedatalist">UI绑定节点数据</param>
    private static void GenerateUICellCode(string scriptownername, List<ComponentBindData> uinodedatalist)
    {
        var templatecontent = GetBinderTemplateFileContent(BinderTemplateType.UICellTemplate);
        if (templatecontent == null)
        {
            return;
        }
        var ttemplate = new TTemplate(templatecontent);
        //通过自定义的替换规则和内容进行替换
        var classname = scriptownername;
        //替换类名
        ttemplate.setValue(ClassNameSingleTag, classname);
        //作者名替换
        ttemplate.setValue(AuthorSingleTag, GetAuthorName());
        //创建日期替换
        ttemplate.setValue(CreatedDateSingleTag, DateTime.Now.ToString("yyyy//MM/dd"));
        //递归判定，生成UI节点成员变量声明
        ttemplate.beginLoop(MemberDefinitionLoopTag);
        GenerateMemberDefinitionCode(ttemplate, uinodedatalist);
        ttemplate.endLoop();
        //替换UI节点成员变量初始化
        ttemplate.beginLoop(MemberInitLoopTag);
        GenerateMemberInitCode(ttemplate, uinodedatalist, DefaultUIBinderMemberName);
        ttemplate.endLoop();
        //替换UI节点成员释放
        ttemplate.beginLoop(MemberDisposeLoopTag);
        GenerateMemberDisposeCode(ttemplate, uinodedatalist, DefaultUIBinderMemberName);
        ttemplate.endLoop();

        var finalcontent = ttemplate.getContent();
        outputTemplateCodeGeneration(classname + ".cs", finalcontent);
    }

    /// <summary>
    /// 生成UI Cell组件绑定代码
    /// </summary>
    /// <param name="scriptownername">脚本拥有者名字</param>
    /// <param name="uinodedatalist">UI绑定节点数据</param>
    private static void GenerateUICellBinderCode(string scriptownername, List<ComponentBindData> uinodedatalist)
    {
        var templatecontent = GetBinderTemplateFileContent(BinderTemplateType.UICellBinder);
        if (templatecontent == null)
        {
            return;
        }
        var ttemplate = new TTemplate(templatecontent);
        //通过自定义的替换规则和内容进行替换
        var classname = scriptownername;
        //替换类名
        ttemplate.setValue(ClassNameSingleTag, classname);
        //作者名替换
        ttemplate.setValue(AuthorSingleTag, GetAuthorName());
        //创建日期替换
        ttemplate.setValue(CreatedDateSingleTag, DateTime.Now.ToString("yyyy//MM/dd"));
        //递归判定，生成UI节点成员变量声明
        ttemplate.beginLoop(MemberDefinitionLoopTag);
        GenerateMemberDefinitionCode(ttemplate, uinodedatalist);
        ttemplate.endLoop();
        //替换UI节点成员变量初始化
        ttemplate.beginLoop(MemberInitLoopTag);
        GenerateMemberInitCode(ttemplate, uinodedatalist, DefaultUIBinderMemberName);
        ttemplate.endLoop();
        //替换UI节点成员释放
        ttemplate.beginLoop(MemberDisposeLoopTag);
        GenerateMemberDisposeCode(ttemplate, uinodedatalist, DefaultUIBinderMemberName);
        ttemplate.endLoop();

        var finalcontent = ttemplate.getContent();
        outputTemplateCodeGeneration(classname + ".cs", finalcontent);
    }

    /// <summary>
    /// 生成GameObject Cell组件绑定代码
    /// </summary>
    /// <param name="scriptownername">脚本拥有者名字</param>
    /// <param name="uinodedatalist">UI绑定节点数据</param>
    private static void GenerateGameObjectBinderCode(string scriptownername, List<ComponentBindData> uinodedatalist)
    {
        var templatecontent = GetBinderTemplateFileContent(BinderTemplateType.GameObjectBinder);
        if (templatecontent == null)
        {
            return;
        }
        var ttemplate = new TTemplate(templatecontent);
        //通过自定义的替换规则和内容进行替换
        var classname = scriptownername;
        //替换类名
        ttemplate.setValue(ClassNameSingleTag, classname);
        //作者名替换
        ttemplate.setValue(AuthorSingleTag, GetAuthorName());
        //创建日期替换
        ttemplate.setValue(CreatedDateSingleTag, DateTime.Now.ToString("yyyy/MM/dd"));
        //递归判定，生成UI节点成员变量声明
        ttemplate.beginLoop(MemberDefinitionLoopTag);
        GenerateMemberDefinitionCode(ttemplate, uinodedatalist);
        ttemplate.endLoop();
        //替换UI节点成员变量初始化
        ttemplate.beginLoop(MemberInitLoopTag);
        GenerateMemberInitCode(ttemplate, uinodedatalist, DefaultUIBinderMemberName);
        ttemplate.endLoop();
        //替换UI节点成员释放
        ttemplate.beginLoop(MemberDisposeLoopTag);
        GenerateMemberDisposeCode(ttemplate, uinodedatalist, DefaultUIBinderMemberName);
        ttemplate.endLoop();

        var finalcontent = ttemplate.getContent();
        outputTemplateCodeGeneration(classname + ".cs", finalcontent);
    }

    /// <summary>
    /// 生成节点成员变量声明
    /// </summary>
    /// <param name="ttemplate">模板处理对象</param>
    /// <param name="uinodedatalist">需要处理的ui节点数据列表</param>
    private static void GenerateMemberDefinitionCode(TTemplate ttemplate, List<ComponentBindData> uinodedatalist)
    {
        for (int i = 0, length = uinodedatalist.Count; i < length; i++)
        {
            var uinodedata = uinodedatalist[i];
            if (uinodedata.NodeTarget != null)
            {
                var nodevariablename = uinodedata.VariableAlias.IsNullOrEmpty() ? uinodedata.NodeTarget.name : uinodedata.VariableAlias;
                ttemplate.setValue(NodeDesSingleTag, uinodedata.NodeDes);
                ttemplate.setValue(NodeTypeSingleTag, uinodedata.NodeTarget.GetType().Name);
                ttemplate.setValue(NodeNameSingleTag, nodevariablename);
                ttemplate.nextLoop();
                //如果绑定的节点是UIBinder，递归生成成员变量定义
                if (uinodedata.NodeTarget is ComponentBinder)
                {
                    var newuibinder = uinodedata.NodeTarget as ComponentBinder;
                    GenerateMemberDefinitionCode(ttemplate, newuibinder.NodeDatas);
                }
            }
        }
    }

    /// <summary>
    /// 生成节点成员初始化声明
    /// </summary>
    /// <param name="ttemplate">模板处理对象</param>
    /// <param name="uinodedatalist">需要处理的ui节点数据列表</param>
    /// <param name="uinodemembername">当前处理的ui节点成员变量名</param>
    private static void GenerateMemberInitCode(TTemplate ttemplate, List<ComponentBindData> uinodedatalist, string uinodemembername)
    {
        for (int i = 0, length = uinodedatalist.Count; i < length; i++)
        {
            var uinodedata = uinodedatalist[i];
            if (uinodedata.NodeTarget != null)
            {
                var nodevariablename = uinodedata.VariableAlias.IsNullOrEmpty() ? uinodedata.NodeTarget.name : uinodedata.VariableAlias;
                ttemplate.setValue(NodeMemberNameSingleTag, uinodemembername);
                ttemplate.setValue(NodeNameSingleTag, nodevariablename);
                ttemplate.setValue(NodeIndexSingleTag, i.ToString());
                ttemplate.setValue(NodeTypeSingleTag, uinodedata.NodeTarget.GetType().Name);
                ttemplate.nextLoop();
                //如果绑定的节点是UIBinder，递归生成成员变量定义
                if (uinodedata.NodeTarget is ComponentBinder)
                {
                    var newuibinder = uinodedata.NodeTarget as ComponentBinder;
                    GenerateMemberInitCode(ttemplate, newuibinder.NodeDatas, nodevariablename);
                }
            }
        }
    }

    /// <summary>
    /// 生成节点成员释放声明
    /// </summary>
    /// <param name="ttemplate">模板处理对象</param>
    /// <param name="uinodedatalist">需要处理的ui节点数据列表</param>
    /// <param name="uinodemembername">当前处理的ui节点成员变量名</param>
    private static void GenerateMemberDisposeCode(TTemplate ttemplate, List<ComponentBindData> uinodedatalist, string uinodemembername)
    {
        for (int i = 0, length = uinodedatalist.Count; i < length; i++)
        {
            var uinodedata = uinodedatalist[i];
            if (uinodedata.NodeTarget != null)
            {
                var nodevariablename = uinodedata.VariableAlias.IsNullOrEmpty() ? uinodedata.NodeTarget.name : uinodedata.VariableAlias;
                ttemplate.setValue(NodeNameSingleTag, nodevariablename);
                ttemplate.nextLoop();
                //如果绑定的节点是UIBinder，递归释放成员变量定义
                if (uinodedata.NodeTarget is ComponentBinder)
                {
                    var newuibinder = uinodedata.NodeTarget as ComponentBinder;
                    GenerateMemberDisposeCode(ttemplate, newuibinder.NodeDatas, nodevariablename);
                }
            }
        }
    }

    /// <summary>
    /// 获取当前电脑用户名
    /// </summary>
    /// <returns></returns>
    private static string GetAuthorName()
    {
        string author = WindowsIdentity.GetCurrent().Name;
        // 只取最终用户名
        var splashindex = author.IndexOf("\\");
        if (splashindex > 0)
        {
            author = author.Substring(0, splashindex);
        }
        return author;
    }

    /// <summary>
    /// 输出自动化代码生成文件
    /// </summary>
    /// <param name="filename">文件名</param>
    /// <param name="filecontent">文件内容</param>
    private static void outputTemplateCodeGeneration(string filename, string filecontent)
    {
        if(!Directory.Exists(UIBinderCodeGeneratorOutputFolderPath))
        {
            Directory.CreateDirectory(UIBinderCodeGeneratorOutputFolderPath);
        }
        var filefullpath = Path.Combine(UIBinderCodeGeneratorOutputFolderPath, filename);
        using (var fs = File.Open(filefullpath, FileMode.Create, FileAccess.Write))
        {
            Byte[] info = new UTF8Encoding(true).GetBytes(filecontent);
            fs.Write(info, 0, info.Length);
            fs.Flush();
            fs.Close();
            Debug.Log(string.Format("生成完毕 : {0}", filefullpath));
        }
    }
}
#endif