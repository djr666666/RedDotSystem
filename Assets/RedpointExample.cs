using UnityEngine;
using UnityEngine.UI;

public class RedpointExample : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button buttonA1;
    [SerializeField] private Button buttonA2;
    [SerializeField] private Text textModelA;
    [SerializeField] private Text textModelA_Sub1;
    [SerializeField] private Text textModelA_Sub2;

    [Header("Redpoint Icons")]
    [SerializeField] private GameObject redpointIconModelA;
    [SerializeField] private GameObject redpointIconSub1;
    [SerializeField] private GameObject redpointIconSub2;

    private void Start()
    {
        // 1. 先设置初始红点（应该在叶子节点上）设置红点根据你的UI逻辑判断
        RedpointManager.SetRedpoint(NodePaths.ModelA_Sub_1, 2);
        RedpointManager.SetRedpoint(NodePaths.ModelA_Sub_2, 3);

        // 2. 注册各个节点的回调
        // ModelA（父节点）的回调
        RedpointManager.AddListenerCallback(NodePaths.ModelA, OnModelAChanged);

        // ModelA_Sub_1 的回调
        RedpointManager.AddListenerCallback(NodePaths.ModelA_Sub_1, OnSub1Changed);

        // ModelA_Sub_2 的回调
        RedpointManager.AddListenerCallback(NodePaths.ModelA_Sub_2, OnSub2Changed);

        // 3. 按钮事件
        // 按钮A1点击：减少 ModelA_Sub_1 的红点
        if (buttonA1 != null)
        {
            buttonA1.onClick.AddListener(() => {
                int current = RedpointManager.GetRedpoint(NodePaths.ModelA_Sub_1, false);
                RedpointManager.SetRedpoint(NodePaths.ModelA_Sub_1, current - 1);
            });
        }

        // 按钮A2点击：减少 ModelA_Sub_2 的红点
        if (buttonA2 != null)
        {
            buttonA2.onClick.AddListener(() => {
                int current = RedpointManager.GetRedpoint(NodePaths.ModelA_Sub_2, false);
                RedpointManager.SetRedpoint(NodePaths.ModelA_Sub_2, current - 1);
            });
        }
    }

    // ModelA 的回调（包含子节点的总红点）
    private void OnModelAChanged(int totalCount)
    {
        Debug.Log($"ModelA 总红点变化: {totalCount}");

        // 更新UI
        if (textModelA != null)
            textModelA.text = $"ModelA 总红点: {totalCount}";

        if (redpointIconModelA != null)
            redpointIconModelA.SetActive(totalCount > 0);
    }

    // ModelA_Sub_1 的回调
    private void OnSub1Changed(int count)
    {
        Debug.Log($"ModelA_Sub_1 红点变化: {count}");

        if (textModelA_Sub1 != null)
            textModelA_Sub1.text = $"Sub1: {count}";

        if (redpointIconSub1 != null)
            redpointIconSub1.SetActive(count > 0);
    }

    // ModelA_Sub_2 的回调
    private void OnSub2Changed(int count)
    {
        Debug.Log($"ModelA_Sub_2 红点变化: {count}");

        if (textModelA_Sub2 != null)
            textModelA_Sub2.text = $"Sub2: {count}";

        if (redpointIconSub2 != null)
            redpointIconSub2.SetActive(count > 0);
    }

    private void OnDestroy()
    {
        // 清理按钮事件
        if (buttonA1 != null)
            buttonA1.onClick.RemoveAllListeners();

        if (buttonA2 != null)
            buttonA2.onClick.RemoveAllListeners();

        RedpointManager.RemoveListenerCallback(NodePaths.ModelA_Sub_2);
    }
}