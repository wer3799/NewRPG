using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GoodsEnumMaker))]
public class GoodsEnumGenerateButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GoodsEnumMaker generator = (GoodsEnumMaker)target;
        
        if (GUILayout.Button("Generate GoodsEnum"))
        {
            generator.MakeGoodsEnum();
        }
        
        if (GUILayout.Button("Generate MainContentsEnum"))
        {
            generator.MakeMainContentsEnum();
        }
    }
}

