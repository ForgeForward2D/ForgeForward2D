using System.Collections;
using UnityEngine;

public class TutorialStarter : MonoBehaviour
{
    [SerializeField] private NpcController npcController;

    private bool isTutorialDialogueOpen;

    private void Awake()
    {
        InputManager.OnTutorialInput += HandleTutorialInput;
        NpcController.OnNpcControllerUpdate += npc => { if (npc == npcController) isTutorialDialogueOpen = true; };
        UIManager.OnUpdatePage += page => { if (page != UIPage.Dialogue) isTutorialDialogueOpen = false; };
    }

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => FindAnyObjectByType<UIManager>() != null);
        yield return null;
        npcController.HandleInteraction(UIPage.None);
    }

    private void HandleTutorialInput(UIPage uiPage)
    {
        if (uiPage == UIPage.Dialogue && isTutorialDialogueOpen)
        {
            npcController.CloseDialogue();
            return;
        }
        if (uiPage != UIPage.None) return;
        npcController.HandleInteraction(UIPage.None);
    }
}
