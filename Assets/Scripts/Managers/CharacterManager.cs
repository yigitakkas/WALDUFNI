using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance;
    private Dictionary<string, GameObject> _characterPrefabs = new Dictionary<string, GameObject>();

    public List<string> characterNames; // Karakter isimlerini ekle
    public List<GameObject> characterPrefabs; // Prefablarý ekle

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        for (int i = 0; i < characterNames.Count; i++)
        {
            if (i < characterPrefabs.Count)
            {
                _characterPrefabs[characterNames[i]] = characterPrefabs[i];
            }
        }
    }

    public GameObject GetCharacterPrefab(string characterName)
    {
        if (_characterPrefabs.TryGetValue(characterName, out GameObject prefab))
        {
            return prefab;
        }

        Debug.LogWarning($"Prefab for character '{characterName}' not found.");
        return null;
    }
}
