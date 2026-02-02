using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 红点节点类
/// </summary>
public class RedpointNode
{
    //节点名 --- 节点的唯一标识，用于在树结构中定位节点
    public string name;

    //父节点引用 --- 指向父节点的引用，形成树状结构的基础
    public RedpointNode parent;

    //当前节点自身的红点数（不包括子节点）
    public int redpointCount = 0;

    //当前节点及其所有子节点的红点总数
    public int totalRedpointCount = 0;

    //存储子节点字典 ，实现了一对多的父子关系，形成树结构
    public Dictionary<string, RedpointNode> children = new Dictionary<string, RedpointNode>();

    //回调字典（支持多个回调）- 允许同一个红点被多个UI组件监听
    public Dictionary<string, Action<int>> callbackDict = new Dictionary<string, Action<int>>();

    // 使用节点名作为默认回调键（只读属性）
    public string DefaultCallbackKey => name;

    public RedpointNode(string name, RedpointNode parent = null)
    {
        this.name = name;
        this.parent = parent;
    }



    // 重新计算总红点数
    public int RecalculateTotal()
    {
        int total = redpointCount;  
        foreach (var child in children.Values)  
        {
            total += child.RecalculateTotal();  
        }
        totalRedpointCount = total;  
        return total;  
    }


    //触发所有回调
    public void TriggerCallbacks()
    {
        foreach (var callback in callbackDict.Values)
            callback?.Invoke(totalRedpointCount);
    }

 
    public void AddCallback(string key, Action<int> callback)
    {
        if (string.IsNullOrEmpty(key))
            key = name; 
        if (callbackDict.ContainsKey(key) && key == name)
            Debug.Log($"红点节点 {name} 的默认回调被更新");
        else if (callbackDict.ContainsKey(key))
            Debug.LogWarning($"红点节点 {name} 的自定义回调键 {key} 将被覆盖");
        callbackDict[key] = callback;
    }

    public void RemoveCallback(string key)
    {
        if (string.IsNullOrEmpty(key))
            key = name;
        callbackDict.Remove(key);
    }


    public void ClearCallbacks()
    {
        callbackDict.Clear();
    }
}