using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "StateIconSet", menuName = "Scriptable Objects/StateIconSet")]
public class StateIconSet : ScriptableObject
{
    [System.Serializable] 
    public struct Icon
    {
        public string id;
        public Sprite sprite;
    }

    [SerializeField] List<Icon> icons = new();

    Dictionary<string, Sprite> map;

    void OnEnable()
    {
        map = icons?.ToDictionary(e => e.id, e => e.sprite) ?? new();
    }

    public Sprite GetSprite(string id)
    {
        return map.TryGetValue(id, out var sprite) ? sprite : null;
    }
}
