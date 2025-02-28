﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;  

[Serializable]
public struct GameData
{
    public Playlist[] playlists;
}
[Serializable]
public struct Playlist
{
    public string id;
    public Question[] questions;
    public string playlist;
}
[Serializable]
public struct Question
{
    public string id;
    public int answerIndex;
    public Choice[] choices;
    public Song song;
}

[Serializable]
public struct Choice
{
    public string artist;
    public string title;
}

[Serializable]
public struct Song
{
    public string id;
    public string title;
    public string artist;
    public string picture;
    public string sample;

    public Sprite pictureSprite;
    public AudioClip sampleClip;
}

[Serializable]
public struct GameResults
{
    public string playlistName;
    public RoundResult[] rounds;
}

[Serializable]
public struct RoundResult
{
    public Song song;
    public bool won;
    public float time;
}

[Serializable]
public struct Screen
{
    public string id;
    public GameObject ScreenObject;
}

