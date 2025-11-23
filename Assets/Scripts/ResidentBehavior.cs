using UnityEngine;

public class ResidentBehavior : MonoBehaviour
{
    [Header("Resident Info")]
    public string residentName;

    [Header("Responses (custom per resident)")]
    [TextArea] public string talkResponse = "Hello.";
    [TextArea] public string testResponse = "I pass the test.";
    [TextArea] public string giveResponse = "Thank you.";
    [TextArea] public string leaveResponse = "Goodbye.";

    public string GetResponseForOption(int optionIndex)
    {
        switch (optionIndex)
        {
            case 0: return talkResponse;   // Talk
            case 1: return testResponse;   // Test
            case 2: return giveResponse;   // Give
            case 3: return leaveResponse;  // Leave
            default: return "...";
        }
    }
}
