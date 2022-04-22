using TMPro;
using UnityEngine;

public class Whiteboard : MonoBehaviour
{
    private TextMeshPro _text;
    private Syllable[] syllables;
    private string[] strSyllables = { "PA", "LA", "VRA" };
    
    private int x = 0;
    
    private int highlightedSyllableIndex = 0;
    
    void Start()
    {
        _text = this.GetComponentInChildren<TextMeshPro>();
        buildSyllables(strSyllables);
        writeWordOnBoard(syllables);
    }

    void Update()
    {
        // TEST
        x++;
        if (x > 500)
        {
            x = 0;
            highlightedSyllableIndex++;
            if (highlightedSyllableIndex > 2)
                highlightedSyllableIndex = 0;
        }
        
        highlightSyllable(highlightedSyllableIndex);
        //highlightCharacters(0, 7);
    }

    private void highlightSyllable(int highlightedSyllableIndex)
    {
        var highlightedSyllable = syllables[highlightedSyllableIndex];
        highlightCharacters(highlightedSyllable.InitialIndex, highlightedSyllable.EndIndex);
    }

    private void buildSyllables(string[] strSyllables)
    {
        syllables = new Syllable[strSyllables.Length];

        int acumulatedIndex = 0;
            
        for (int i = 0; i < strSyllables.Length; i++)
        {
            syllables[i] = new Syllable()
            {
                Text = strSyllables[i],
                InitialIndex = acumulatedIndex,
                EndIndex = acumulatedIndex + strSyllables[i].Length -1
            };
            acumulatedIndex += strSyllables[i].Length + 1;
        }
    }

    private void writeWordOnBoard(Syllable[] syllables)
    {
        if (_text == null)
            return;
        
        string concatenatedWord = string.Empty;
        foreach (var syllable in syllables)
        {
            concatenatedWord += syllable.Text + " ";
        }

        _text.text = concatenatedWord;
    }

    private void highlightCharacters(int startCharIndex, int endCharIndex)
    {
        Color initialColor = Color.black;
        Color highlightColor = Color.green;

        if (_text == null)
            return;
        
        var info = _text.textInfo;
        
        for (int charIndex = 0; charIndex < info.characterCount; charIndex++)
        {
            if (string.IsNullOrWhiteSpace(info.characterInfo[charIndex].character.ToString()))
                continue;
            
            int meshIndex = info.characterInfo[charIndex].materialReferenceIndex;
            int vertexIndex = _text.textInfo.characterInfo[charIndex].vertexIndex;
        
            Color32[] vertexColors = info.meshInfo[meshIndex].colors32;
            if (vertexColors != null)
            {
                Color color = charIndex >= startCharIndex && charIndex <= endCharIndex ? highlightColor : initialColor;
                
                vertexColors[vertexIndex + 0] = color;
                vertexColors[vertexIndex + 1] = color;
                vertexColors[vertexIndex + 2] = color;
                vertexColors[vertexIndex + 3] = color;
            }
        }
        
        _text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }
    
    private void highlightCharacters2(int startCharIndex, int endCharIndex)
    {
        Color initialColor = Color.black;
        Color highlightColor = Color.green;

        if (_text == null)
            return;
        
        var info = _text.textInfo;
        for (int charIndex = 0; charIndex < info.characterCount; charIndex++)
        {
            info.characterInfo[charIndex].material.color = Color.green;
        }
        
        _text.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
    }
    
    private class Syllable
    {
        public string Text { get; set; }
        public int InitialIndex { get; set; }
        public int EndIndex { get; set; }
    }
}


