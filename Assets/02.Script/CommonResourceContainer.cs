using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class CommonResourceContainer 
{
    public static List<Sprite> subWeaponSprites;
    
    public static Sprite GetSubWeaponSprite(int idx)
    {
        if (subWeaponSprites == null)
        {
            var weaponIcons = Resources.LoadAll<Sprite>("SubWeapon/");
            subWeaponSprites = weaponIcons.ToList();


            subWeaponSprites.Sort((a, b) =>
            {
                if (int.Parse(a.name) < int.Parse(b.name)) return -1;

                return 1;

            });
        }

        if (idx < subWeaponSprites.Count)
        {
            return subWeaponSprites[idx];
        }
        else
        {
            Debug.LogError($"Weapon icon {idx} is not exist");
            return null;
        }
    }
    
    public static List<Sprite> charmSprites;
    
    public static Sprite GetCharmSprite(int idx)
    {
        if (charmSprites == null)
        {
            var icons = Resources.LoadAll<Sprite>("Charm/");
            charmSprites = icons.ToList();


            charmSprites.Sort((a, b) =>
            {
                if (int.Parse(a.name) < int.Parse(b.name)) return -1;

                return 1;

            });
        }

        if (idx < charmSprites.Count)
        {
            return charmSprites[idx];
        }
        else
        {
            Debug.LogError($"charm icon {idx} is not exist");
            return null;
        }
    }

    public static List<Sprite> norigaeSprites;
    
    public static Sprite GetNorigaeSprite(int idx)
    {
        if (norigaeSprites == null)
        {
            var icons = Resources.LoadAll<Sprite>("Norigae/");
            norigaeSprites = icons.ToList();


            norigaeSprites.Sort((a, b) =>
            {
                if (int.Parse(a.name) < int.Parse(b.name)) return -1;

                return 1;

            });
        }

        if (idx < norigaeSprites.Count)
        {
            return norigaeSprites[idx];
        }
        else
        {
            Debug.LogError($"norigae icon {idx} is not exist");
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
