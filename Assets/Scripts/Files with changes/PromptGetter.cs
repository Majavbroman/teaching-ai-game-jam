using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
        PromptData[] poolPrompts = new PromptData[0];
        foreach(PromptPool pool in _promptDataByPool.Keys)
        {
            if (poolFlags.HasFlag(pool))
            {
                poolPrompts = poolPrompts.Concat(_promptDataByPool[pool]).ToArray();
            }
        }

        poolPrompts = poolPrompts.Distinct().ToArray();

        if (poolPrompts.Length == 0)
        {
            Debug.LogWarning($"No prompt data available for pools with flags: {poolFlags}");
            return GetRandomPromptData();
        }

        int index = Random.Range(0, poolPrompts.Length);
        return poolPrompts[index];
    }

    public static PromptData GetRandomPromptData()
    {
        PromptData[] allPrompts = new PromptData[0];
        foreach (var poolPrompts in _promptDataByPool.Values)
        {
            allPrompts = allPrompts.Concat(poolPrompts).ToArray();
        }

        allPrompts = allPrompts.Distinct().ToArray();

        if (allPrompts.Length == 0)
        {
            Debug.LogWarning("No prompt data available.");
            return null;
        }

        int index = Random.Range(0, allPrompts.Length);
        return allPrompts[index];
    }
}
