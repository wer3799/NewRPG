using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UiLoginButtonBoard : MonoBehaviour
{
    [SerializeField]
    private Button googleLoginButton;

    [SerializeField]
    private Button guestLoginButton;

    [SerializeField]
    private Button iosLoginButton;

    private void Start()
    {
        SetDefatult();
    }

    private void SetDefatult()
    {
#if UNITY_ANDROID
        iosLoginButton.gameObject.SetActive(false);
#endif
    }

    public void OnClickGoogleLoginButton()
    {

    }


    public void OnClickGuestLoginButton()
    {

    }


    public void OnClickIOSLoginButton()
    {

    }
}
