using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VocabularyEntry
{
    public int Packet;
    public int Vocabulary_ID;
    public string English_Word;
    public string ASL_Sign_and_Spelled;
    public string ASL_Definition;
    public string ASL_Sign;
    public string ASL_Spelled;
    public string English_Definition;
}

[System.Serializable]
public class VocabularyPacket
{
    public string PacketName;
    public List<VocabularyEntry> Entries;
}

[System.Serializable]
public class VocabularyData
{
    public List<VocabularyPacket> Packets;
}


public class VocabularyLoader : MonoBehaviour
{
    public TextAsset vocabularyJson; // Assign the .txt file here via the Inspector
    private VocabularyData vocabularyData;

    void Start()
    {
        LoadVocabularyData();
    }

    void LoadVocabularyData()
    {
        if (vocabularyJson != null)
        {
            string jsonContent = vocabularyJson.text;
            vocabularyData = JsonUtility.FromJson<VocabularyData>(jsonContent);

            if (vocabularyData != null && vocabularyData.Packets != null)
            {
                Debug.Log("Vocabulary data loaded successfully!");

                // Example: Accessing data
                foreach (var packet in vocabularyData.Packets)
                {
                    Debug.Log($"Packet: {packet.PacketName}");
                    foreach (var entry in packet.Entries)
                    {
                        Debug.Log($"Word: {entry.English_Word}, Definition: {entry.English_Definition}");
                    }
                }
            }
            else
            {
                Debug.LogError("Failed to parse JSON. Check the JSON structure.");
            }
        }
        else
        {
            Debug.LogError("Vocabulary JSON not assigned!");
        }
    }
}
