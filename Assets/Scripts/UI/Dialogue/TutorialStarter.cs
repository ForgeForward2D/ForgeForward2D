using System.Collections;
using UnityEngine;

public class TutorialStarter : MonoBehaviour
{
    [SerializeField] private NpcController npcController;

    private IEnumerator Start()
    {
        // Wait until UIManager is ready (UI scene may still be loading)
        yield return new WaitUntil(() => FindAnyObjectByType<UIManager>() != null);
        // One extra frame so all Start() methods across scenes have run
        yield return null;
        npcController.HandleInteraction(UIPage.None);
    }
}
