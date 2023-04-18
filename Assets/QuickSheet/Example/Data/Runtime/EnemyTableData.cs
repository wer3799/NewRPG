using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
/// 
[System.Serializable]
public class EnemyTableData
{
  [SerializeField]
  int id;
  public int Id { get {return id; } set { this.id = value;} }
  
  [SerializeField]
  string prefabname;
  public string Prefabname { get {return prefabname; } set { this.prefabname = value;} }
  
  [SerializeField]
  int minlevel;
  public int Minlevel { get {return minlevel; } set { this.minlevel = value;} }
  
  [SerializeField]
  int maxlevel;
  public int Maxlevel { get {return maxlevel; } set { this.maxlevel = value;} }
  
  [SerializeField]
  double starthp;
  public double Starthp { get {return starthp; } set { this.starthp = value;} }
  
  [SerializeField]
  double perhp;
  public double Perhp { get {return perhp; } set { this.perhp = value;} }
  
  [SerializeField]
  double startatt;
  public double Startatt { get {return startatt; } set { this.startatt = value;} }
  
  [SerializeField]
  double peratt;
  public double Peratt { get {return peratt; } set { this.peratt = value;} }
  
  [SerializeField]
  float startdef;
  public float Startdef { get {return startdef; } set { this.startdef = value;} }
  
  [SerializeField]
  float intervaldef;
  public float Intervaldef { get {return intervaldef; } set { this.intervaldef = value;} }
  
  [SerializeField]
  double startexocism;
  public double Startexocism { get {return startexocism; } set { this.startexocism = value;} }
  
  [SerializeField]
  double intervalexocisim;
  public double Intervalexocisim { get {return intervalexocisim; } set { this.intervalexocisim = value;} }
  
}