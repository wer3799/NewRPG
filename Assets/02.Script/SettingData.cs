using BackEnd;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public static class SettingData
{
    //볼룸
    public static ReactiveProperty<float> bgmVolume = new ReactiveProperty<float>();
    public static ReactiveProperty<float> efxVolume = new ReactiveProperty<float>();
    public static ReactiveProperty<float> view = new ReactiveProperty<float>();
    public static ReactiveProperty<float> uiView = new ReactiveProperty<float>();
    public static ReactiveProperty<float> joyStick = new ReactiveProperty<float>();
    public static ReactiveProperty<int> GraphicOption = new ReactiveProperty<int>(); //하중상최상
    public static ReactiveProperty<int> FrameRateOption = new ReactiveProperty<int>(); //30 45 60
    public static ReactiveProperty<int> ShowDamageFont = new ReactiveProperty<int>();
    public static ReactiveProperty<int> ShowEffect = new ReactiveProperty<int>();
    public static ReactiveProperty<int> GlowEffect = new ReactiveProperty<int>();
    public static ReactiveProperty<int> PotionUseHpOption = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)
    public static ReactiveProperty<int> GachaWhiteEffect = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)
    public static ReactiveProperty<int> ShowSleepPush = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)
    public static ReactiveProperty<int> YachaEffect = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)
    public static ReactiveProperty<int> HpBar = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)

    public static ReactiveProperty<int> ViewEnemy = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)

    //
    public static ReactiveProperty<int> sonView = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)
    public static ReactiveProperty<int> dogView = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)
    public static ReactiveProperty<int> marbleCircleView = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)
    public static ReactiveProperty<int> asuarView = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)
    public static ReactiveProperty<int> akGuiView = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)
    public static ReactiveProperty<int> tailView = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)


    public static ReactiveProperty<int> hyonMu = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)
    public static ReactiveProperty<int> baekHo = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)
    public static ReactiveProperty<int> pet = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)
    public static ReactiveProperty<int> orb = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)
    public static ReactiveProperty<int> indra = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)
    public static ReactiveProperty<int> dragon = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)
    public static ReactiveProperty<int> showOneSkillEffect = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)

    public static ReactiveProperty<int> fourView = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)

    public static ReactiveProperty<int> showOtherPlayer = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)

    public static ReactiveProperty<int> showFoxCup = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)
    public static ReactiveProperty<int> showRingEffect = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)
    public static ReactiveProperty<int> showVisionSkill = new ReactiveProperty<int>(); //x이하일떄 (3개옵션)

    public static int screenWidth = Screen.width;
    public static int screenHeight = Screen.height;

    public static void InitFirst()
    {
        FirstInit();
        Initialize();
    }

    static void FirstInit()
    {
        if (PlayerPrefs.HasKey(SettingKey.bgmVolume) == false)
            PlayerPrefs.SetFloat(SettingKey.bgmVolume, 0.5f);

        if (PlayerPrefs.HasKey(SettingKey.efxVolume) == false)
            PlayerPrefs.SetFloat(SettingKey.efxVolume, 0.5f);

        if (PlayerPrefs.HasKey(SettingKey.view) == false)
            PlayerPrefs.SetFloat(SettingKey.view, 1f);

        if (PlayerPrefs.HasKey(SettingKey.GraphicOption) == false)
            PlayerPrefs.SetInt(SettingKey.GraphicOption, 2);

        if (PlayerPrefs.HasKey(SettingKey.FrameRateOption) == false)
            PlayerPrefs.SetInt(SettingKey.FrameRateOption, 2);

        if (PlayerPrefs.HasKey(SettingKey.ShowDamageFont) == false)
            PlayerPrefs.SetInt(SettingKey.ShowDamageFont, 1);

        if (PlayerPrefs.HasKey(SettingKey.ShowEffect) == false)
            PlayerPrefs.SetInt(SettingKey.ShowEffect, 1);

        if (PlayerPrefs.HasKey(SettingKey.GlowEffect) == false)
            PlayerPrefs.SetInt(SettingKey.GlowEffect, 0);

        if (PlayerPrefs.HasKey(SettingKey.PotionUseHpOption) == false)
            PlayerPrefs.SetInt(SettingKey.PotionUseHpOption, 1);

        if (PlayerPrefs.HasKey(SettingKey.uiView) == false)
            PlayerPrefs.SetFloat(SettingKey.uiView, 0f);

        if (PlayerPrefs.HasKey(SettingKey.joyStick) == false)
            PlayerPrefs.SetFloat(SettingKey.joyStick, 0f);

        if (PlayerPrefs.HasKey(SettingKey.GachaWhiteEffect) == false)
            PlayerPrefs.SetInt(SettingKey.GachaWhiteEffect, 1);

        if (PlayerPrefs.HasKey(SettingKey.ShowSleepPush) == false)
            PlayerPrefs.SetInt(SettingKey.ShowSleepPush, 1);

        if (PlayerPrefs.HasKey(SettingKey.YachaEffect) == false)
            PlayerPrefs.SetInt(SettingKey.YachaEffect, 1);

        if (PlayerPrefs.HasKey(SettingKey.HpBar) == false)
            PlayerPrefs.SetInt(SettingKey.HpBar, 1);

        //
        if (PlayerPrefs.HasKey(SettingKey.sonView) == false)
            PlayerPrefs.SetInt(SettingKey.sonView, 1);


        if (PlayerPrefs.HasKey(SettingKey.dogView) == false)
            PlayerPrefs.SetInt(SettingKey.dogView, 1);

        if (PlayerPrefs.HasKey(SettingKey.marbleCircleView) == false)
            PlayerPrefs.SetInt(SettingKey.marbleCircleView, 1);

        if (PlayerPrefs.HasKey(SettingKey.asuarView) == false)
            PlayerPrefs.SetInt(SettingKey.asuarView, 1);

        if (PlayerPrefs.HasKey(SettingKey.akGuiView) == false)
            PlayerPrefs.SetInt(SettingKey.akGuiView, 1);

        if (PlayerPrefs.HasKey(SettingKey.tailView) == false)
            PlayerPrefs.SetInt(SettingKey.tailView, 1);

        //
        if (PlayerPrefs.HasKey(SettingKey.hyonMu) == false)
            PlayerPrefs.SetInt(SettingKey.hyonMu, 1);

        if (PlayerPrefs.HasKey(SettingKey.baekHo) == false)
            PlayerPrefs.SetInt(SettingKey.baekHo, 1);

        if (PlayerPrefs.HasKey(SettingKey.pet) == false)
            PlayerPrefs.SetInt(SettingKey.pet, 1);

        if (PlayerPrefs.HasKey(SettingKey.orb) == false)
            PlayerPrefs.SetInt(SettingKey.orb, 1);

        if (PlayerPrefs.HasKey(SettingKey.indra) == false)
            PlayerPrefs.SetInt(SettingKey.indra, 1);

        if (PlayerPrefs.HasKey(SettingKey.dragon) == false)
            PlayerPrefs.SetInt(SettingKey.dragon, 1);

        if (PlayerPrefs.HasKey(SettingKey.oneSkill) == false)
            PlayerPrefs.SetInt(SettingKey.oneSkill, 0);
        //


        if (PlayerPrefs.HasKey(SettingKey.fourView) == false)
            PlayerPrefs.SetInt(SettingKey.fourView, 1);

        if (PlayerPrefs.HasKey(SettingKey.ViewEnemy) == false)
            PlayerPrefs.SetInt(SettingKey.ViewEnemy, 1);

        if (PlayerPrefs.HasKey(SettingKey.showOtherPlayer) == false)
            PlayerPrefs.SetInt(SettingKey.showOtherPlayer, 1);

        if (PlayerPrefs.HasKey(SettingKey.showFoxCup) == false)
            PlayerPrefs.SetInt(SettingKey.showFoxCup, 1);

        if (PlayerPrefs.HasKey(SettingKey.showRingEffect) == false)
            PlayerPrefs.SetInt(SettingKey.showRingEffect, 1);
    }

    static void Initialize()
    {
        bgmVolume.Value = PlayerPrefs.GetFloat(SettingKey.bgmVolume, 1f);
        efxVolume.Value = PlayerPrefs.GetFloat(SettingKey.efxVolume, 1f);
        view.Value = PlayerPrefs.GetFloat(SettingKey.view, 0.5f);
        GraphicOption.Value = PlayerPrefs.GetInt(SettingKey.GraphicOption, 2);
        FrameRateOption.Value = PlayerPrefs.GetInt(SettingKey.FrameRateOption, 2);
        ShowDamageFont.Value = PlayerPrefs.GetInt(SettingKey.ShowDamageFont, 1);
        ShowEffect.Value = PlayerPrefs.GetInt(SettingKey.ShowEffect, 1);
        GlowEffect.Value = PlayerPrefs.GetInt(SettingKey.GlowEffect, 1);
        PotionUseHpOption.Value = PlayerPrefs.GetInt(SettingKey.PotionUseHpOption, 1);
        uiView.Value = PlayerPrefs.GetFloat(SettingKey.uiView, 0f);
        joyStick.Value = PlayerPrefs.GetFloat(SettingKey.joyStick, 0f);
        GachaWhiteEffect.Value = PlayerPrefs.GetInt(SettingKey.GachaWhiteEffect, 1);
        ShowSleepPush.Value = PlayerPrefs.GetInt(SettingKey.ShowSleepPush, 1);
        YachaEffect.Value = PlayerPrefs.GetInt(SettingKey.YachaEffect, 1);
        HpBar.Value = PlayerPrefs.GetInt(SettingKey.HpBar, 1);
        ViewEnemy.Value = PlayerPrefs.GetInt(SettingKey.ViewEnemy, 1);
        //
        sonView.Value = PlayerPrefs.GetInt(SettingKey.sonView, 1);
        fourView.Value = PlayerPrefs.GetInt(SettingKey.fourView, 1);
        dogView.Value = PlayerPrefs.GetInt(SettingKey.dogView, 1);
        marbleCircleView.Value = PlayerPrefs.GetInt(SettingKey.marbleCircleView, 1);
        asuarView.Value = PlayerPrefs.GetInt(SettingKey.asuarView, 1);
        akGuiView.Value = PlayerPrefs.GetInt(SettingKey.akGuiView, 1);
        tailView.Value = PlayerPrefs.GetInt(SettingKey.tailView, 1);

        hyonMu.Value = PlayerPrefs.GetInt(SettingKey.hyonMu, 1);
        baekHo.Value = PlayerPrefs.GetInt(SettingKey.baekHo, 1);
        pet.Value = PlayerPrefs.GetInt(SettingKey.pet, 1);
        orb.Value = PlayerPrefs.GetInt(SettingKey.orb, 1);
        indra.Value = PlayerPrefs.GetInt(SettingKey.indra, 1);
        dragon.Value = PlayerPrefs.GetInt(SettingKey.dragon, 1);


        showOneSkillEffect.Value = PlayerPrefs.GetInt(SettingKey.oneSkill, 0);

        showOtherPlayer.Value = PlayerPrefs.GetInt(SettingKey.showOtherPlayer, 1);

        showFoxCup.Value = PlayerPrefs.GetInt(SettingKey.showFoxCup, 1);
        showRingEffect.Value = PlayerPrefs.GetInt(SettingKey.showRingEffect, 1);

        Subscribe();
    }

    static void Subscribe()
    {
        bgmVolume.AsObservable().Subscribe(e => { PlayerPrefs.SetFloat(SettingKey.bgmVolume, e); });

        efxVolume.AsObservable().Subscribe(e => { PlayerPrefs.SetFloat(SettingKey.efxVolume, e); });

        view.AsObservable().Subscribe(e => { PlayerPrefs.SetFloat(SettingKey.view, e); });
        ;

        joyStick.AsObservable().Subscribe(e => { PlayerPrefs.SetFloat(SettingKey.joyStick, e); });

        uiView.AsObservable().Subscribe(e => { PlayerPrefs.SetFloat(SettingKey.uiView, e); });

        GraphicOption.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.GraphicOption, e); });
        FrameRateOption.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.FrameRateOption, e); });

        ShowDamageFont.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.ShowDamageFont, e); });
        ShowEffect.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.ShowEffect, e); });
        GlowEffect.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.GlowEffect, e); });
        PotionUseHpOption.AsObservable().Subscribe(e =>
        {
            Debug.LogError($"Potion optionChanged {e}");
            PlayerPrefs.SetInt(SettingKey.PotionUseHpOption, e);
        });

        GraphicOption.AsObservable().Subscribe(e =>
        {
            PlayerPrefs.SetInt(SettingKey.GraphicOption, e);
            SetGraphicOption(e);
        });

        GachaWhiteEffect.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.GachaWhiteEffect, e); });

        ShowSleepPush.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.ShowSleepPush, e); });

        YachaEffect.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.YachaEffect, e); });

        HpBar.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.HpBar, e); });

        ViewEnemy.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.ViewEnemy, e); });
        //
        sonView.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.sonView, e); });
        dogView.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.dogView, e); });
        marbleCircleView.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.marbleCircleView, e); });
        asuarView.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.asuarView, e); });
        akGuiView.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.akGuiView, e); });
        tailView.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.tailView, e); });
        //
        hyonMu.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.hyonMu, e); });
        baekHo.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.baekHo, e); });
        pet.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.pet, e); });
        orb.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.orb, e); });
        indra.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.indra, e); });
        dragon.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.dragon, e); });
        showOneSkillEffect.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.oneSkill, e); });
        //
        fourView.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.fourView, e); });

        showOtherPlayer.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.showOtherPlayer, e); });

        showFoxCup.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.showFoxCup, e); });

        showRingEffect.AsObservable().Subscribe(e => { PlayerPrefs.SetInt(SettingKey.showRingEffect, e); });
    }

    public static void SetGraphicOption(int option)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (option == 0)
            {
                Screen.SetResolution(640, 640 * screenHeight / screenWidth, true);
            }
            else if (option == 1)
            {
                Screen.SetResolution(1280, 1280 * screenHeight / screenWidth, true);
            }
            else if (option == 2)
            {
                Screen.SetResolution(1500, 1500 * screenHeight / screenWidth, true);
            }
            else
            {
                Screen.SetResolution(screenWidth, screenHeight, true);
            }
        }
    }
}

public static class SettingKey
{
    public static string bgmVolume = "bgmVolume";
    public static string efxVolume = "efxVolume";
    public static string view = "view";
    public static string uiView = "uiView";
    public static string joyStick = "joyStick";
    public static string GraphicOption = "GraphicOption";
    public static string FrameRateOption = "FrameRateOption";
    public static string ShowDamageFont = "ShowDamageFont";
    public static string ShowEffect = "ShowEffect";
    public static string GlowEffect = "GlowEffect";
    public static string PotionUseHpOption = "PotionUseHpOption";
    public static string GachaWhiteEffect = "GachaWhiteEffect";
    public static string ShowSleepPush = "ShowSleepPush";
    public static string YachaEffect = "YachaEffect";
    public static string HpBar = "HpBar";

    public static string ViewEnemy = "ViewEnemy";

    //
    public static string sonView = "sonView";
    public static string dogView = "dogView";
    public static string marbleCircleView = "marbleCircleView";
    public static string asuarView = "asuarView";
    public static string akGuiView = "akGuiView";

    public static string tailView = "tailView";

    //
    public static string hyonMu = "hyonMu";
    public static string baekHo = "baekHo";
    public static string pet = "pet";
    public static string orb = "orb";
    public static string indra = "indra";
    public static string dragon = "dragon";
    public static string oneSkill = "oneSkill";

    public static string fourView = "fourView";
    public static string showOtherPlayer = "showOtherPlayer";

    public static string showFoxCup = "showFoxCup";
    public static string showRingEffect = "showRingEffect";
}