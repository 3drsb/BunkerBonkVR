using UnityEngine;
using System.Collections.Generic;

public class ResidentBehavior : MonoBehaviour
{
    [Header("Basic Info")]
    public string residentName = "Resident";

    [Header("Talk Topics (custom per resident)")]
    [Tooltip("These will appear as the TALK submenu options")]
    public List<string> talkTopics = new List<string>();

    [Header("Talk Responses (match talkTopics list index)")]
    [Tooltip("Each entry must correspond to talkTopics")]
    [TextArea] public List<string> talkResponses = new List<string>();

    [Header("Give Options (custom per resident)")]
    [Tooltip("These will appear as the GIVE submenu options")]
    public List<string> giveItems = new List<string>();

    [Header("Give Responses (match giveItems list index)")]
    [TextArea] public List<string> giveResponses = new List<string>();

    [Header("Shared Test Options (same for all residents)")]
    public static readonly string[] testOptions =
    {
        "Check Teeth",
        "Check Eyes",
        "Sonar Scan",
        "Breathing Test"
    };

    [Header("Shared Test Responses (must match above testOptions)")]
    public static readonly string[] testResponses =
    {
        "My teeth are okay, I think.",
        "Can you see my eyes clearly?",
        "Sonar scan completed.",
        "Breathing steady and normal."
    };

    [Header("Default Leave Response")]
    public string leaveResponse = "Goodbye.";

    //-----------------------------------------------------------
    // Retrieve submenu options
    //-----------------------------------------------------------

    public List<string> GetTalkOptions()
    {
        return talkTopics;
    }

    public string[] GetTestOptions()
    {
        return testOptions;
    }

    public List<string> GetGiveOptions()
    {
        return giveItems;
    }

    //-----------------------------------------------------------
    // Retrieve response for selected TALK option
    //-----------------------------------------------------------
    public string GetTalkResponse(int index)
    {
        if (index < 0 || index >= talkResponses.Count)
            return "...";
        return talkResponses[index];
    }

    //-----------------------------------------------------------
    // Retrieve response for selected TEST option
    //-----------------------------------------------------------
    public string GetTestResponse(int index)
    {
        if (index < 0 || index >= testResponses.Length)
            return "...";
        return testResponses[index];
    }

    //-----------------------------------------------------------
    // Retrieve response for selected GIVE option
    //-----------------------------------------------------------
    public string GetGiveResponse(int index)
    {
        if (index < 0 || index >= giveResponses.Count)
            return "...";
        return giveResponses[index];
    }

    //-----------------------------------------------------------
    // Leave response
    //-----------------------------------------------------------
    public string GetLeaveResponse()
    {
        return leaveResponse;
    }
}
