using System;
using System.Collections.Generic;
using Model;
using TMPro;
using UnityEngine;

public class Whiteboard : MonoBehaviour
{
    
    private TextMeshPro _log;
    private bool _started;
    private TextMeshPro _text;
    private Syllable[] _syllables;
    private bool _spoke = false;
    private int x = 0;
    private int _highlightedSyllableIndex = 0;
    
    void Start()
    {
        TTSManager.SetLocale("en");
        
        var goText = GameObject.FindGameObjectsWithTag("Text")[0];
        _text = goText.GetComponent<TextMeshPro>();
        
        var goLog = GameObject.FindGameObjectsWithTag("Log")[0];
        _log = goLog.GetComponent<TextMeshPro>();
        _log.text = String.Empty;
        
        buildSyllables(CaptureButton.IdentifiedWord.Syllables);
        writeWordOnBoard(_syllables);
    }

    void Update()
    {
        if (!_started)
            return;
        
        x++;
        if (x > 60)
        {
            x = 0;
            _spoke = false;
            _highlightedSyllableIndex++;

            if (_highlightedSyllableIndex > _syllables.Length)
                _started = false;
        }
        
        highlightSyllable(_highlightedSyllableIndex);
    }

    private void highlightSyllable(int highlightedSyllableIndex)
    {
        var highlightedSyllable = _syllables[highlightedSyllableIndex];
        highlightCharacters(highlightedSyllable.InitialIndex, highlightedSyllable.EndIndex);

        if (!_spoke)
        {
            //TTSManager.SetSpeechRate(1);
            //TTSManager.SetPitch(1);
            if (TTSManager.IsBootedUp())
            {
                TTSManager.Speak(highlightedSyllable.Text);
                _spoke = true;                
            }    
        }
    }

    private void buildSyllables(List<string> strSyllables)
    {
        _syllables = new Syllable[strSyllables.Count];

        int acumulatedIndex = 0;
            
        for (int i = 0; i < strSyllables.Count; i++)
        {
            _syllables[i] = new Syllable()
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

    public void StartSpelling()
    {
        _started = true;
        _highlightedSyllableIndex = 0;

    }
    
    private class Syllable
    {
        public string Text { get; set; }
        public int InitialIndex { get; set; }
        public int EndIndex { get; set; }
    }
}


