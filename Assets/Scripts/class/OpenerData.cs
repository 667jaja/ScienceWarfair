using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OpenerData", menuName = "Opener Data")]

public class OpenerData : ScriptableObject
{
    public string openerName;
    public List<CardData> opener;
}
