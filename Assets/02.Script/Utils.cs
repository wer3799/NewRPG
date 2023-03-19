using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class Utils 
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
}
