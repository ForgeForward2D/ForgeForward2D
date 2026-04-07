using System.Collections;
using UnityEngine;

public class TutorialStarter : MonoBehaviour
{
    [SerializeField] private NpcController npcController;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => FindAnyObjectByType<UIManager>() != null);
        yield return null;
        npcController.HandleInteraction(UIPage.None);
    }
}
