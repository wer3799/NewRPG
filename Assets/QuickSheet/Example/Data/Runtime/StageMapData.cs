using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
/// 
[System.Serializable]
public class StageMapData
{
  [SerializeField]
  int id;
  public int Id { get {return id; } set { this.id = value;} }
  
  [SerializeField]
  int mappreset;
  public int Mappreset { get {return mappreset; } set { this.mappreset = value;} }
  
  [SerializeField]
  int chapter;
  public int Chapter { get {return chapter; } set { this.chapter = value;} }
  
  [SerializeField]
  int stage;
  public int Stage { get {return stage; } set { this.stage = value;} }
  
  [SerializeField]
  int monsterlevel;
  public int Monsterlevel { get {return monsterlevel; } set { this.monsterlevel = value;} }
  
  [SerializeField]
  double multiplebosspower;
  public double Multiplebosspower { get {return multiplebosspower; } set { this.multiplebosspower = value;} }
  
  [SerializeField]
  int spawnamount;
  public int Spawnamount { get {return spawnamount; } set { this.spawnamount = value;} }
  
  [SerializeField]
  float spawndelay;
  public float Spawndelay { get {return spawndelay; } set { this.spawndelay = value;} }
  
  [SerializeField]
  float movespeed;
  public float Movespeed { get {return movespeed; } set { this.movespeed = value;} }
  
  [SerializeField]
  float exp;
  public float Exp { get {return exp; } set { this.exp = value;} }
  
  [SerializeField]
  float gold;
  public float Gold { get {return gold; } set { this.gold = value;} }
  
  [SerializeField]
  float growthstoneamount;
  public float Growthstoneamount { get {return growthstoneamount; } set { this.growthstoneamount = value;} }
  
  [SerializeField]
  string[] spawnenemies = new string[0];
  public string[] Spawnenemies { get {return spawnenemies; } set { this.spawnenemies = value;} }
  
}