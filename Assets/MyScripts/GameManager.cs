
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class GameManager : MonoBehaviour
{

}
[Serializable]
public class ChatData
{
#nullable enable
    public string? ip;
    public string? username;
    public string? usertext;
    public override string ToString()
    {
        return $"IP: {ip}, Username: {username}, Usertext: {usertext}";
    }
}
[Serializable]
public class ChatDataDTO
{
#nullable enable
    public List<ChatData>? chatDatas;
}
[Serializable]
public class IpResponse
{
#nullable enable
    public string? ip;
}
