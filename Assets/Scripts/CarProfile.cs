using System;
using UnityEngine;
[Serializable]
public class CarProfile
{
public string name;
public int price;

    [Range(1, 5)] public int speed;
    [Range(1, 5)] public int acceleration;
    [Range(1, 5)] public int handling;
    [Range(1, 5)] public int braking;
    [Range(1, 5)] public int traction;
    
}