using UnityEngine;

[System.Serializable]
public class VisitorData2
{
    public string visitorName;
    public bool isMonster;

    [Header("When they first arrive")]
    [TextArea] public string[] introHumanComments;
    [TextArea] public string[] introMonsterComments;

    [Header("If the player lets them in")]
    [TextArea] public string[] letInHumanComments;
    [TextArea] public string[] letInMonsterComments;

    [Header("If the player denies them")]
    [TextArea] public string[] denyHumanComments;
    [TextArea] public string[] denyMonsterComments;

    [Header("Special Behavior")]
    public bool canReturn; // if this visitor can come back later
    public bool isSpecial; // for story/important visitors
}
