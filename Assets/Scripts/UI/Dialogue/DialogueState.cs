using UnityEngine;

public record DialogueState(
    string NpcName,
    string CurrentLine,
    int LineIndex,
    int TotalLines,
    Sprite CharacterSprite
);
