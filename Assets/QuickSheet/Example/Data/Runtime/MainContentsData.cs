using UnityEngine;
using System.Collections;

///
/// !!! Machine generated code !!!
/// !!! DO NOT CHANGE Tabs to Spaces !!!
/// 
[System.Serializable]
public class MainContentsData
{
  [SerializeField]
  int id;
  public int Id { get {return id; } set { this.id = value;} }
  
  [SerializeField]
  string contentstype;
  public string Contentstype { get {return contentstype; } set { this.contentstype = value;} }
  
  [SerializeField]
  int type;
  public int Type { get {return type; } set { this.type = value;} }
  
  [SerializeField]
  ContentsWhere contentswhere;
  public ContentsWhere CONTENTSWHERE { get {return contentswhere; } set { this.contentswhere = value;} }
  
  [SerializeField]
  int timer;
  public int Timer { get {return timer; } set { this.timer = value;} }
  
}