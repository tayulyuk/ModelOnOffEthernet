using System;
using UnityEngine;

/// <summary>
/// 클라이언트에서 서버로 보내는 버튼 정보
/// </summary>
[Serializable]
public class PacketEche
{
    public string buttonNum;
    public string buttonState;
}
