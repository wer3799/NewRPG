using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class Utils
{
    public static DateTime ConvertFromUnixTimestamp(double timestamp)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return origin.AddSeconds(timestamp + 1620000000f);
    }

    public static double ConvertToUnixTimestamp(DateTime date)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = date.ToUniversalTime() - origin;
        return diff.TotalSeconds - 1620000000f;
    }

    public static bool HasBadWord(string word)
    {
        return false;
    }

    public static int GetWeekNumber(DateTime currentDate)
    {
        DateTime startDate = new DateTime(2021, 1, 1); //기준일

        Calendar calenderCalc = CultureInfo.CurrentCulture.Calendar;

        return calenderCalc.GetWeekOfYear(currentDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday) -
               calenderCalc.GetWeekOfYear(startDate, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
    }

    public static void RestartApplication()
    {
#if UNITY_IOS
        PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "네트워크 연결이 끊겼습니다.\n앱을 종료합니다.",confirmCallBack:()=>
        {
            Application.Quit();
        });

        return;
#endif
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaObject pm = currentActivity.Call<AndroidJavaObject>("getPackageManager");
            AndroidJavaObject intent = pm.Call<AndroidJavaObject>("getLaunchIntentForPackage", Application.identifier);
            intent.Call<AndroidJavaObject>("setFlags", 0x20000000); //Intent.FLAG_ACTIVITY_SINGLE_TOP

            AndroidJavaClass pendingIntent = new AndroidJavaClass("android.app.PendingIntent");
            AndroidJavaObject contentIntent =
                pendingIntent.CallStatic<AndroidJavaObject>("getActivity", currentActivity, 0, intent,
                    0x8000000); //PendingIntent.FLAG_UPDATE_CURRENT = 134217728 [0x8000000]
            AndroidJavaObject alarmManager = currentActivity.Call<AndroidJavaObject>("getSystemService", "alarm");
            AndroidJavaClass system = new AndroidJavaClass("java.lang.System");
            long currentTime = system.CallStatic<long>("currentTimeMillis");
            alarmManager.Call("set", 1, currentTime + 1000, contentIntent); // android.app.AlarmManager.RTC = 1 [0x1]

            Debug.LogError("alarm_manager set time " + currentTime + 1000);
            currentActivity.Call("finish");

            AndroidJavaClass process = new AndroidJavaClass("android.os.Process");
            int pid = process.CallStatic<int>("myPid");
            process.CallStatic("killProcess", pid);
        }
    }

    private static string[] goldUnitArr = new string[]
    {
        "", "만", "억", "조", "경", "해", "자", "양", "구", "간", "정", "재", "극", "항", "아", "나", "불", "무", "대", "겁", "업", "긍",
        "갈", "라", "가", "언"
    };

    private static double p = (double)Mathf.Pow(10, 4);
    private static List<double> numList = new List<double>();
    private static List<string> numStringList = new List<string>();
    private static string zeroString = "0";

    public static string ConvertBigNum(double data)
    {
#if UNITY_EDITOR
        bool isUnderZero = data < 0;
        if (data < 0)
        {
            data *= -1f;
        }
#endif
        //
        if (data == 0f)
        {
            return zeroString;
        }

        double value = data;

        numList.Clear();
        numStringList.Clear();

        do
        {
            numList.Add((value % p));
            value /= p;
        } while (value >= 1);

        string retStr = "";

        if (numList.Count >= 3)
        {
            for (int i = numList.Count - 1; i >= numList.Count - 2; i--)
            {
                if (numList[i] == 0) continue;

                numStringList.Add(Math.Truncate(numList[i]) + goldUnitArr[i]);
            }

            for (int i = 0; i < numStringList.Count; i++)
            {
                retStr += numStringList[i];
            }
#if UNITY_EDITOR
            if (isUnderZero)
            {
                return "-" + retStr;
            }
#endif
            return retStr;
        }
        else
        {
            for (int i = 0; i < numList.Count; i++)
            {
                if (numList[i] == 0) continue;
                retStr = Math.Truncate(numList[i]) + goldUnitArr[i] + retStr;
            }
#if UNITY_EDITOR
            if (isUnderZero)
            {
                return "-" + retStr;
            }
#endif
            return retStr;
        }
    }


    public static int GetRandomIdx(List<float> inputDatas)
    {
        float total = 0;

        for (int i = 0; i < inputDatas.Count; i++)
        {
            total += inputDatas[i];
        }

        float pivot = UnityEngine.Random.Range(0f, 1f) * total;

        for (int i = 0; i < inputDatas.Count; i++)
        {
            if (pivot < inputDatas[i])
            {
                return i;
            }
            else
            {
                pivot -= inputDatas[i];
            }
        }

        return 0;
    }

    public static bool IsStageBoss(this EnemyType type)
    {
        return type == EnemyType.StageBoss;
    }

    public static bool EnemyCanDead(this EnemyType type)
    {
        return type == EnemyType.Normal ||
               type == EnemyType.StageBoss;
    }
}