using System;
using Unity.VisualScripting;
using UnityEngine;

//ADDED PROMPT POOL ENUM TO EASILY ORGANIZE PROMPTS INTO DIFFERENT CATEGORIES
[Flags]
public enum PromptPool
{
    Level1 = 1 << 0,
    Level2 = 1 << 1,
    Level3 = 1 << 2,
    Absurd = 1 << 3,
    Copyright = 1 << 4,
    GoodBoy = 1 << 5,
    Dumb = 1 << 6
}

[CreateAssetMenu(fileName = "PromptData", menuName = "Prompt")]
public class PromptData : ScriptableObject
{
    public string PromptHeader;
    public IndicatorTypes IndicatorType;
    public bool Expandable { get; private set; }
    [Space(5)]
    [Header("Severity Factors")]
    [SerializeField] private bool NotPublicInfo;
    [SerializeField] private bool CopyrightedData;
    [SerializeField] private bool SubjectNotInformed;
    public int Severity { get; private set; }

    [Space(15)]
    [SerializeField] private PromptCheck[] Checks;
    private PromptCheck _currentCheck;

    [field: Space(15)]
    [field: Header("Prompt Pool")]
    [field: SerializeField] public PromptPool PromptPools { get; private set; }

    public void SetRandomCheck()
    {
        if (Checks == null || Checks.Length == 0)
        {
            _currentCheck = new PromptCheck
            {
                CheckText = "Remember to set a check for a prompt you doofus!",
                Disapproved = false,
                NotPublicInfo = false,
                CopyrightedData = false,
                SubjectNotInformed = false
            };
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, Checks.Length);
        _currentCheck = Checks[randomIndex];

        Expandable = true;
    }

    public PromptCheck GetCurrentCheck()
    {
        return _currentCheck;
    }

    [Serializable]
    public struct PromptCheck
    {
        [TextArea(2, 5)] public string CheckText;
        public bool Disapproved;
        public bool NotPublicInfo;
        public bool CopyrightedData;
        public bool SubjectNotInformed;

        public readonly int GetSeverity()
        {
            int severity = 0;
            severity += Disapproved ? 1 : 0;
            severity += NotPublicInfo ? 1 : 0;
            severity += CopyrightedData ? 1 : 0;
            severity += SubjectNotInformed ? 1 : 0;
            return severity;
        }
    }

    public int GetSeverity()
    {
        Severity = 0;
        if (IndicatorType == IndicatorTypes.Approved)
        {
            return 0;
        }

        if (IndicatorType == IndicatorTypes.Disapproved)
        {
            Severity += 1;
            Severity += NotPublicInfo ? 1 : 0;
            Severity += CopyrightedData ? 1 : 0;
            Severity += SubjectNotInformed ? 1 : 0;
            return Severity;
        }

        if (IndicatorType == IndicatorTypes.Check)
        {
            Severity = _currentCheck.GetSeverity();
            return Severity;
        }

        return 0;
    }
}

