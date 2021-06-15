using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : GenericSingleton<GameData>
{
    public Dictionary<Node, string> buildingDictionary;

    protected override void Awake()
    {
        base.Awake();

        buildingDictionary = new Dictionary<Node, string>();
    }
}
