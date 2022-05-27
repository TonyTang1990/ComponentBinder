# ComponentBinder
## 工具
1. Unity版本(2019 4.31f1c1)
2. Visual Studio 2019

## 需求

游戏开发过程中我们经常会需要访问一些指定节点，组件和脚本。

常规方式会通过编写GetChild(?)和GetComponent<?>()代码的方式去获取节点和组件，但这样对于开发来说并不高效同时运行时的GetChild(?)和GetComponent<?>()也是有运行开销的。

为了寻求更高效，高性能和便捷的方式，这里引出组件绑定的工具方案。

## 最终目标

实现一套通用的组件绑定方案，为未来游戏快速开发打下基础。

## 组件绑定原理

1. 通过Object数组实现通用类型的组件绑定存储
2. 通过自定义Inspector实现绑定对象，绑定对象组件绑定选择以及模板类型选择，代码生成等UI操作界面
3. 通过模板+代码生成实现组件绑定的自定义快速代码生成(通过生成partial代码不入侵原始逻辑代码)

## 实战实现

### 组件绑定代

```CS
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
```

```CS
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
```

从上面的可以看出节点组件和脚本的数据绑定**核心是通过脚本序列化Object[]数组以及相关数据的方式实现绑定节点数据存储的。**

### 组件绑定自定义面板

```CS
/// <summary>
/// 组件绑定自定义Editor
/// </summary>
[CustomEditor(typeof(ComponentBinder))]
[CanEditMultipleObjects]
public class ComponentBinderEditor : Editor
{
    ******
}
```

这部分代码主要是实现ComponentBinder的Inspector自定义GUI显示，从而实现我们想要的自定义UI操作。

### 组件绑定模板代码生成

```CS
/// <summary>
/// CompoenntBinder代码生成工具
/// </summary>
public static class ComponentBinderCodeGenerator
{
	******
}
```

```CS
/// <summary>
/// 模板数据处理类
/// </summary>
public class TTemplate
{
    ******
}
```

此文件主要是通过自定义选择的模板文件***.txt结合TTemplate工具类实现模板文件txt里的内容自定义生成输出我们想要的代码。

窗口模板文件示例:

```CS
namespace Game.Modules.UI
{
    public class #ClassName# : BaseWindow
    {
		 #MEMBER_DEFINITION_LOOP#
        /// <summary> #NodeDes# /// </summary>
        private #NodeType# #NodeName#;#MEMBER_DEFINITION_LOOP#

		public #ClassName#()
		{

		}
		   
		/// <summary>
		/// 缓存组件
		/// </summary>
		protected override void cacheComponents()
		{
			base.cacheComponents();
            #MEMBER_INIT_LOOP#
            #NodeName# = #NodeMemberName#.NodeDatas[#NodeIndex#].NodeTarget as #NodeType#;#MEMBER_INIT_LOOP#
        }

		/// <summary>
		/// 添加监听
		/// </summary>
		protected override void addListeners()
		{
			base.addListeners();
		}

		/// <summary>
		/// 窗口显示
		/// </summary>
		protected override void onShow()
		{
			base.onShow();
		}
		
		/// <summary>
		/// 移除监听
		/// </summary>
		protected override void removeListeners()
		{
			base.removeListeners();
		}
		
		/// <summary>
		/// 释放组件
		/// </summary>
		protected override void disposeComponents()
		{
			base.disposeComponents();
			#MEMBER_DISPOSE_LOOP#
            #NodeName# = null;#MEMBER_DISPOSE_LOOP#
		}

		/// <summary>
		/// 窗口销毁
		/// </summary>
		protected override void onDestroy()
		{
			base.onDestroy();
		}
    }
}
```

窗口组件绑定模板示例:

```CS

```





### 实战使用

- 组件绑定选择

[ComponentBindChosenUI](/img/Unity/ComponentBinder/ComponentBindChosenUI.PNG)

- 自定义绑定节点代码注释

[CustomScriptNotation](/img/Unity/ComponentBinder/CustomScriptNotation.PNG)

- 

## 重点知识

1. 

## 博客

[碰撞检测](