using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 红点树管理系统
/// </summary>
public class RedpointTree : MonoBehaviour
{
    #region 节点路径定义
 
    #endregion

    #region 私有字段
    private RedpointNode root;
    private Dictionary<string, RedpointNode> nodeCache = new Dictionary<string, RedpointNode>();
    #endregion


    /// <summary>初始化红点系统 </summary>

    public void Initialize()
    {
        // 创建根节点
        root = new RedpointNode("AllRoot");
        nodeCache["AllRoot"] = root;

        // 构建所有节点
        foreach (var path in NodePaths.AllPaths)
        {
            if (path != "AllRoot") // 根节点已创建
            {
                InsertNode(path);
            }
        }

        Debug.Log("红点系统初始化完成，共创建节点：" + nodeCache.Count);
    }


    #region 节点操作
    /// <summary>
    /// 插入节点
    /// </summary>
    public void InsertNode(string path)
    {
        if (string.IsNullOrEmpty(path) || nodeCache.ContainsKey(path))
        {
            return;
        }

        string[] parts = path.Split('/');
        RedpointNode currentNode = root;

        // 逐级创建或查找节点
        for (int i = 0; i < parts.Length; i++)
        {
            string nodeName = parts[i];

            // 跳过根节点
            if (nodeName == "AllRoot" && i == 0)
                continue;

            // 如果子节点不存在，创建它
            if (!currentNode.children.ContainsKey(nodeName))
            {
                var newNode = new RedpointNode(nodeName, currentNode);
                currentNode.children[nodeName] = newNode;

                // 缓存完整路径
                string fullPath = GetFullPath(newNode);
                nodeCache[fullPath] = newNode;
            }

            currentNode = currentNode.children[nodeName];
        }
    }

    /// <summary>
    /// 查找节点
    /// </summary>
    public RedpointNode GetNode(string path)
    {
        if (nodeCache.TryGetValue(path, out RedpointNode node))
        {
            return node;
        }
        return null;
    }

    /// <summary>
    /// 检查节点是否存在
    /// </summary>
    public bool HasNode(string path)
    {
        return nodeCache.ContainsKey(path);
    }
    #endregion

    #region 红点操作


    //设置红点数量
    public void SetRedpointCount(string path, int count, bool autoUpdateParents = true)
    {
        RedpointNode node = GetNode(path);
        if (node == null)
        {
            Debug.LogError($"红点节点不存在：{path}");
            return;
        }

        count = Mathf.Max(0, count); // 确保不小于0
        if (node.redpointCount == count)
            return;

        node.redpointCount = count;

        if (autoUpdateParents)
        {
            UpdateNodeAndParents(node);
        }
        else
        {
            UpdateSingleNode(node);
        }
    }


 

    /// <summary>
    /// 获取红点数量
    /// </summary>
    public int GetRedpointCount(string path, bool includeChildren = true)
    {
        RedpointNode node = GetNode(path);
        if (node == null)
        {
            return 0;
        }

        return includeChildren ? node.totalRedpointCount : node.redpointCount;
    }

    /// <summary>
    /// 检查是否有红点
    /// </summary>
    public bool HasRedpoint(string path, bool includeChildren = true)
    {
        return GetRedpointCount(path, includeChildren) > 0;
    }
    #endregion

    #region 回调管理

    /// <summary>
    /// 添加回调（支持多个）
    /// </summary>
    public void AddCallback(string path, string key, Action<int> callback)
    {
        RedpointNode node = GetNode(path);
        if (node != null)
        {
            node.AddCallback(key, callback);
            // 立即触发一次
            callback?.Invoke(node.totalRedpointCount);
        }
    }

    /// <summary>
    /// 移除回调
    /// </summary>
    public void RemoveCallback(string path, string key)
    {
        RedpointNode node = GetNode(path);
        if (node != null)
        {
            node.RemoveCallback(key);
        }
    }

    /// <summary>
    /// 移除所有回调
    /// </summary>
    public void ClearAllCallbacks()
    {
        foreach (var node in nodeCache.Values)
        {
            node.ClearCallbacks();
        }
    }
    #endregion

    #region 内部方法
    /// <summary>
    /// 更新单个节点（不更新父节点）
    /// </summary>
    private void UpdateSingleNode(RedpointNode node)
    {
        if (node == null) return;

        node.RecalculateTotal();
        node.TriggerCallbacks();
    }

    /// <summary>
    /// 更新节点及其所有父节点
    /// </summary>
    private void UpdateNodeAndParents(RedpointNode node)
    {
        if (node == null) return;

        // 先更新当前节点
        UpdateSingleNode(node);

        // 递归更新所有父节点
        RedpointNode parent = node.parent;
        while (parent != null)
        {
            UpdateSingleNode(parent);
            parent = parent.parent;
        }
    }

    /// <summary>
    /// 获取节点的完整路径
    /// </summary>
    private string GetFullPath(RedpointNode node)
    {
        List<string> names = new List<string>();
        RedpointNode current = node;

        while (current != null)
        {
            names.Insert(0, current.name);
            current = current.parent;
        }

        return string.Join("/", names);
    }

    /// <summary>
    /// 强制刷新所有节点
    /// </summary>
    public void RefreshAll()
    {
        root.RecalculateTotal();

        // 触发所有节点的回调
        foreach (var node in nodeCache.Values)
        {
            node.TriggerCallbacks();
        }
    }
    #endregion

    #region 调试方法
    /// <summary>
    /// 打印树结构
    /// </summary>
    public void PrintTree()
    {
        Debug.Log("=== 红点树结构 ===");
        PrintNode(root, 0);
    }

    private void PrintNode(RedpointNode node, int depth)
    {
        string indent = new string(' ', depth * 2);
        string status = node.totalRedpointCount > 0 ? $" [红点:{node.totalRedpointCount}]" : "";
        Debug.Log($"{indent}{node.name}{status}");

        foreach (var child in node.children.Values)
        {
            PrintNode(child, depth + 1);
        }
    }

    /// <summary>
    /// 获取所有节点信息
    /// </summary>
    public Dictionary<string, int> GetAllNodeInfo()
    {
        var info = new Dictionary<string, int>();
        foreach (var kvp in nodeCache)
        {
            info[kvp.Key] = kvp.Value.totalRedpointCount;
        }
        return info;
    }
    #endregion
}