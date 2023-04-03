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
  float spawndelay;
  public float Spawndelay { get {return spawndelay; } set { this.spawndelay = value;} }
  
  [SerializeField]
  int spawnnum;
  public int Spawnnum { get {return spawnnum; } set { this.spawnnum = value;} }
  
  [SerializeField]
  string[] spawnenemies = new string[0];
  public string[] Spawnenemies { get {return spawnenemies; } set { this.spawnenemies = value;} }
  
  [SerializeField]
  float nextspawndelay;
  public float Nextspawndelay { get {return nextspawndelay; } set { this.nextspawndelay = value;} }
  
}