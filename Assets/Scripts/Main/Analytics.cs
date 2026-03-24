using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Analytics;


public class Analytics : MonoBehaviour
{
    async void Start()
    {
        await UnityServices.InitializeAsync();

        // AnalyticsService.Instance.StartDataCollection();
        // EndUserConsent.SetConsentState(true);
        AnalyticsService.Instance.RecordEvent("analytics_started");

        BlockBreakingManager.OnBlockBroken += HandleBlockBroken;
    }

    private void HandleBlockBroken((BlockType, Vector2Int, Tool) brokenBlockInfo)
    {
        AnalyticsService.Instance.RecordEvent("block_broken");
    }
}