using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
/// 
[System.Serializable]
public class SkillTableData
{
  [SerializeField]
  int id;
  public int Id { get {return id; } set { this.id = value;} }
  
  [SerializeField]
  string skillclassname;
  public string Skillclassname { get {return skillclassname; } set { this.skillclassname = value;} }
  
  [SerializeField]
  string skillname;
  public string Skillname { get {return skillname; } set { this.skillname = value;} }
  
  [SerializeField]
  string skilldesc;
  public string Skilldesc { get {return skilldesc; } set { this.skilldesc = value;} }
  
  [SerializeField]
  int awakemaxnum;
  public int Awakemaxnum { get {return awakemaxnum; } set { this.awakemaxnum = value;} }
  
  [SerializeField]
  float cooltime;
  public float Cooltime { get {return cooltime; } set { this.cooltime = value;} }
  
  [SerializeField]
  float damageper;
  public float Damageper { get {return damageper; } set { this.damageper = value;} }
  
  [SerializeField]
  float damageaddvalue;
  public float Damageaddvalue { get {return damageaddvalue; } set { this.damageaddvalue = value;} }
  
  [SerializeField]
  int targetcount;
  public int Targetcount { get {return targetcount; } set { this.targetcount = value;} }
  
  [SerializeField]
  int hitcount;
  public int Hitcount { get {return hitcount; } set { this.hitcount = value;} }
  
  [SerializeField]
  float targetrange;
  public float Targetrange { get {return targetrange; } set { this.targetrange = value;} }
  
  [SerializeField]
  int skillgrade;
  public int Skillgrade { get {return skillgrade; } set { this.skillgrade = value;} }
  
  [SerializeField]
  string activeeffectname1;
  public string Activeeffectname1 { get {return activeeffectname1; } set { this.activeeffectname1 = value;} }
  
  [SerializeField]
  string hiteffectname;
  public string Hiteffectname { get {return hiteffectname; } set { this.hiteffectname = value;} }
  
  [SerializeField]
  int skilltype;
  public int Skilltype { get {return skilltype; } set { this.skilltype = value;} }
  
  [SerializeField]
  string soundname;
  public string Soundname { get {return soundname; } set { this.soundname = value;} }
  
  [SerializeField]
  int haseffecttype;
  public int Haseffecttype { get {return haseffecttype; } set { this.haseffecttype = value;} }
  
  [SerializeField]
  float haseffectvalue;
  public float Haseffectvalue { get {return haseffectvalue; } set { this.haseffectvalue = value;} }
  
  [SerializeField]
  float activeoffset;
  public float Activeoffset { get {return activeoffset; } set { this.activeoffset = value;} }
  
  [SerializeField]
  SkillCastType skillcasttype;
  public SkillCastType SKILLCASTTYPE { get {return skillcasttype; } set { this.skillcasttype = value;} }
  
  [SerializeField]
  bool iseffectrootplayer;
  public bool Iseffectrootplayer { get {return iseffectrootplayer; } set { this.iseffectrootplayer = value;} }
  
}