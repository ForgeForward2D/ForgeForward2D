using UnityEngine;

public class PlayerHandController : MonoBehaviour
{
    [SerializeField] private Transform socketRightHand;

    private GameObject currentHandItem;

    private void OnEnable()
    {
        HotBar.OnHotBarUpdate += UpdateHandItem;
    }

    private void OnDisable()
    {
        HotBar.OnHotBarUpdate -= UpdateHandItem;
    }

    private void UpdateHandItem(HotBar hotBar)
    {

        if (currentHandItem != null)
        {
            Destroy(currentHandItem);
            currentHandItem = null;
        }

        Tool selected = hotBar.GetSelectedTool();

        if (selected == null || selected.prefab == null)
            return;

        currentHandItem = Instantiate(selected.prefab, socketRightHand);
        currentHandItem.transform.localPosition = Vector3.zero;
        currentHandItem.transform.localRotation = Quaternion.identity;
        currentHandItem.transform.localScale = Vector3.one;
    }
}
