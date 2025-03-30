using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Entry : MonoBehaviour
{
    private void Start()
    {
        IEnumerable<int> randomList = new List<int>();

        for (int i = 0; i < 100; i++)
        {
            randomList = randomList.Append(Random.Range(0, 100));
        }
        
        // Greater Than 10
        var greaterThanTen = randomList.Where(x => x > 10);
        var hasDivisibleByFive = randomList.Any(x => x % 5 == 0);
        var hasGreaterThan0 = randomList.Any(x => x > 0);
        var allIsPositive = randomList.All(x => x > 0);
        var arr = randomList.ToArray();
        var list = randomList.ToList();

        Debug.Log($"greater than ten: {String.Join(" ", greaterThanTen)}");
        Debug.Log($"has divisible by 5: {hasDivisibleByFive}");
        Debug.Log($"has greater than 0: {hasGreaterThan0}");
        Debug.Log($"all is positive: {allIsPositive}");
        Debug.Log($"array: {String.Join(" ", arr)}");
        Debug.Log($"list: {String.Join(" ", list)}");
    }
}

