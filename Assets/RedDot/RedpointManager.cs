using System;
using System.IO;
using UnityEngine;

/// <summary>
/// 红点系统管理器
/// </summary>
public class RedpointManager : MonoBehaviour
{
    private static RedpointManager _instance;
    public static RedpointManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // 查找场景中是否已有
                _instance = FindObjectOfType<RedpointManager>();

                if (_instance == null)
                {
                    // 自动创建
                    GameObject go = new GameObject("[RedpointManager]");
                    _instance = go.AddComponent<RedpointManager>();

                    // 默认设置
                    go.transform.SetParent(null);
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    [SerializeField] private bool dontDestroyOnLoad = true;
    [SerializeField] private bool initializeOnAwake = true;
    private bool _initialized = false;

    private RedpointTree _system;
    public RedpointTree System
    {
        get
        {
            if (_system == null)
            {
                Debug.LogError("红点系统未初始化，请先调用Initialize()");
            }
            return _system;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

            if (dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (_instance != this)
        {
            // 如果已有实例，销毁自己
            Debug.LogWarning("发现多个RedpointManager，销毁新实例");
            Destroy(gameObject);
            return;
        }

        Initialize();
    }

    /// <summary>
    /// 初始化红点系统
    /// </summary>
    public void Initialize()
    {
        if (_initialized)
        {
            Debug.LogWarning("红点系统已经初始化过了");
            return;
        }

        try
        {
            // 确保组件存在
            if (_system == null)
            {
                _system = GetComponent<RedpointTree>();
                if (_system == null)
                {
                    _system = gameObject.AddComponent<RedpointTree>();
                }
            }

            // 初始化
            _system.Initialize();
            _initialized = true;

            Debug.Log("红点系统初始化完成");
        }
        catch (Exception e)
        {
            Debug.LogError($"红点系统初始化失败: {e.Message}");
            _initialized = false;
        }
    }

    /// <summary>
    /// 重新初始化红点系统
    /// </summary>
    public void Reinitialize()
    {
        if (_system != null)
        {
            Destroy(_system);
        }

        Initialize();
    }

    /// <summary>
    /// 清理所有红点和回调
    /// </summary>
    public void ClearAll()
    {
        System.ClearAllCallbacks();
        // 可以添加清理红点数据的逻辑
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
            _initialized = false;
        }
    }

    // 便捷静态访问方法
    public static void SetRedpoint(string path, int count)
    {
        if (Instance != null)
        {
            Instance.System.SetRedpointCount(path, count);
        }
    }

    public static int GetRedpoint(string path, bool includeChildren = true)
    {
        return Instance?.System.GetRedpointCount(path, includeChildren) ?? 0;
    }

    public static void AddListenerCallback(string path,Action<int> callback, string key = null)
    {
        Instance?.System.AddCallback(path,key,callback);
    }

    public static void RemoveListenerCallback(string path, string key = null)
    {
        Instance?.System.RemoveCallback(path,key);
    }
}