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
  string contentsname;
  public string Contentsname { get {return contentsname; } set { this.contentsname = value;} }
  
  [SerializeField]
  ContentsType contentstype;
  public ContentsType CONTENTSTYPE { get {return contentstype; } set { this.contentstype = value;} }
  
  [SerializeField]
  ContentsWhere contentswhere;
  public ContentsWhere CONTENTSWHERE { get {return contentswhere; } set { this.contentswhere = value;} }
  
  [SerializeField]
  int timer;
  public int Timer { get {return timer; } set { this.timer = value;} }
  
}