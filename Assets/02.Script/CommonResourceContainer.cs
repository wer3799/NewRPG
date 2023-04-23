using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class CommonResourceContainer 
{
    public static List<Sprite> weaponSprites;
    
    public static Sprite GetWeaponSprite(int idx)
    {
        if (weaponSprites == null)
        {
            var weaponIcons = Resources.LoadAll<Sprite>("Weapon/");
            weaponSprites = weaponIcons.ToList();


            weaponSprites.Sort((a, b) =>
            {
                if (int.Parse(a.name) < int.Parse(b.name)) return -1;

                return 1;

            });
        }

        if (idx < weaponSprites.Count)
        {
            return weaponSprites[idx];
        }
        else
        {
            Debug.LogError($"Weapon icon {idx} is not exist");
            return null;
        }
    }
    
    public static List<Sprite> itemGradeFrame;
    
    public static Sprite GetGradeFrame(int idx)
    {
        if (itemGradeFrame == null)
        {
            var weaponIcons = Resources.LoadAll<Sprite>("itemGradeFrame/");
            itemGradeFrame = weaponIcons.ToList();


            itemGradeFrame.Sort((a, b) =>
            {
                if (int.Parse(a.name) < int.Parse(b.name)) return -1;

                return 1;

            });
        }

        if (idx < itemGradeFrame.Count)
        {
            return itemGradeFrame[idx];
        }
        else
        {
            Debug.LogError($"itemGradeFrame {idx} is not exist");
            return null;
        }
    }
}
