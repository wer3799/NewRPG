using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GachaLevelData", menuName = "Scriptable Object/GachaLevelData", order = int.MaxValue)]
public class GachaLevelData : ScriptableObject
{
    private List<List<int>> gachaLevelMinNum = null;

    public List<List<int>> GachaLevelMinNum
    {
        get
        {
            if (gachaLevelMinNum==null)
            {
                gachaLevelMinNum = new List<List<int>>(){gachaLevelMinNum_subWeapon, gachaLevelMinNum_charm, gachaLevelMinNum_norigae,gachaLevelMinNum_skill};
            }

            return gachaLevelMinNum;
        }
    }
    
    public List<int> gachaLevelMinNum_subWeapon = new List<int>() { 0, 500, 2000, 5000, 20000, 50000, 80000, 120000, 160000, 200000 };
    public List<int> gachaLevelMinNum_charm = new List<int>() { 0, 500, 2000, 5000, 20000, 50000, 80000, 120000, 160000, 200000 };
    public List<int> gachaLevelMinNum_norigae = new List<int>() { 0, 125, 500, 1275, 4000, 10000, 15000, 20000, 30000, 40000 };
    public List<int> gachaLevelMinNum_skill = new List<int>() { 0, 125, 500, 1275, 4000, 10000, 15000, 20000, 30000, 40000 };
}