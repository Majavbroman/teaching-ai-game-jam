using System.Collections.Generic;
using UnityEngine;

public static class PromptGetter
{
    private static readonly Dictionary<PromptPool, List<PromptData>> _promptDataByPool = new Dictionary<PromptPool, List<PromptData>>();

    public static void LoadPromptData()
    {
        PromptData[] dataArray = Resources.LoadAll<PromptData>("Prompts");

        foreach (PromptData promptData in dataArray)
        {
            foreach (PromptPool pool in System.Enum.GetValues(typeof(PromptPool)))
            {
                if (promptData.PromptPools.HasFlag(pool))
                {
                    if (!_promptDataByPool.ContainsKey(pool))
                    {
                        _promptDataByPool[pool] = new List<PromptData>();
                    }
                    _promptDataByPool[pool].Add(promptData);
                }
            }
        }
    }

    public static PromptData GetRandomPromptData(this PromptPool poolFlags)
    {
        List<PromptData> poolPrompts = new List<PromptData>();
        foreach(PromptPool pool in _promptDataByPool.Keys)
        {
            if (poolFlags.HasFlag(pool))
            {
                poolPrompts.AddRange(_promptDataByPool[pool]);
            }
        }

        if (poolPrompts.Count == 0)
        {
            Debug.LogWarning($"No prompt data available for pools with flags: {poolFlags}");
            return GetRandomPromptData();
        }

        int index = Random.Range(0, poolPrompts.Count);
        return poolPrompts[index];
    }

    public static PromptData GetRandomPromptData()
    {
        List<PromptData> allPrompts = new List<PromptData>();
        foreach (var poolPrompts in _promptDataByPool.Values)
        {
            allPrompts.AddRange(poolPrompts);
        }

        if (allPrompts.Count == 0)
        {
            Debug.LogWarning("No prompt data available.");
            return null;
        }

        int index = Random.Range(0, allPrompts.Count);
        return allPrompts[index];
    }
}
