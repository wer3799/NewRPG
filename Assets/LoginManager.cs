using BackEnd;

#if UNITY_ANDROID
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System;
#elif UNITY_IOS
#endif
using AppleAuth;
using AppleAuth.Interfaces;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UniRx;

public class LoginManager : SingletonMono<LoginManager>
{
    public enum LoginState
    {
        Offline = 0,
        IsGuestLogin = 1,
        IsFederationGoogleLogin = 2,
        IsFederationIOSLogin = 3,
    }

    private readonly string SAVE_KEY_LOGIN_STATE = "IS_LOGIN";

    [SerializeField]
    private string editorLoginId;

    [SerializeField]
    private string testId = "a_8846847867697156085";

    [SerializeField]
    private UiNickNameInputBoard nickNameInputBoard;

    [SerializeField]
    private UiLoginButtonBoard loginButtonBoard;

    private bool isSignIn = false;

    public string loginId;

    public static string email { get; private set; } = "Editor";
   
    public enum IOS_LoginType
    {
        GameCenter, Custom
    }

    private string iOS_LoginType = string.Empty;

#if UNITY_IOS
#endif
    private AppleAuthManager appleAuthManager;


    bool IsFederationLogin(LoginState loginState) =>
        loginState == LoginState.IsFederationGoogleLogin ||
        loginState == LoginState.IsFederationIOSLogin;

    private new void Awake()
    {

        base.Awake();
        Backend.InitializeAsync(true, callback => {
            if (callback.IsSuccess())
            {
                if (Backend.IsInitialized)
                {
#if UNITY_EDITOR
                    WhenVersionCheckSuccess();
                    return;
#endif
                    CheckCurrentVersion();

                }
            }
            else
            {
                Debug.LogError("Failed to initialize the backend");
            }
        });

        GleyNotifications.Initialize();
    }

    private void CheckCurrentVersion()
    {
        var servercheckBro = Backend.Utils.GetServerStatus();

        if (servercheckBro.IsSuccess())
        {
            var json = servercheckBro.GetReturnValuetoJSON();
            var state = json["serverStatus"].ToString();

            //온라인
            if (state == "0")
            {
                float clientVersion = float.Parse(Application.version);

                var bro = Backend.Utils.GetLatestVersion();

                if (bro.IsSuccess())
                {
                    var jsonData = bro.GetReturnValuetoJSON();
                    string serverVersion = jsonData["version"].ToString();

                    //버전이 높거나 같음
                    if (clientVersion >= float.Parse(serverVersion))
                    {
                        Debug.LogError($"클라이언트 버전 {clientVersion} 서버 버전 {serverVersion} 같음");

                        WhenVersionCheckSuccess();
                    }
                    else
                    {
                        Debug.LogError($"클라이언트 버전 {clientVersion} 서버 버전 {serverVersion} 다름");

                        PopupManager.Instance.ShowVersionUpPopup(CommonString.Notice, "업데이트 버전이 있습니다. 스토어로 이동합니다.", () =>
                        {
#if UNITY_ANDROID
                            Application.OpenURL("https://play.google.com/store/apps/details?id=com.DragonGames.yoyo");
#endif
#if UNITY_IOS
                    Application.OpenURL("itms-apps://itunes.apple.com/app/id1587651736");
#endif
                        }, false);
                    }

                    Debug.LogError($"clientVersion = {clientVersion} serverVersion {serverVersion}");
                }
                else
                {
                    PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "클라이언트 버전 정보 로드에 실패했습니다.\n버전 정보를 다시 요청합니다.", CheckCurrentVersion);
                }
            }
            else
            {
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "서버 점검중입니다. 자세한 사항은\n네이버 카페에서 확인 부탁드립니다!", () =>
                {
                    Application.OpenURL("https://cafe.naver.com/yokiki");
                });
            }

        }
    }

    private void WhenVersionCheckSuccess()
    {
        //작업- 로그인 상태인지 체크 후 로그인상태 아니면 로그인 버튼 팝업띄움
        Debug.Log(Backend.BMember.GetGuestID());
        Debug.Log("버전체크 완료");
        Debug.Log(Backend.Utils.GetGoogleHash());

        /*
                              * 
        var bro = Backend.BMember.GetUserInfo();
        if(bro.IsSuccess())
        {
            Debug.Log("게스트 로그인됨");
        }
        else
        {
            Debug.Log("게스트 로그인 안됨");
        }
        Debug.Log("로그인시도끝");*/

        /*#if UNITY_ANDROID
                

                iOS_LoginType = PlayerPrefs.GetString(CommonString.IOS_loginType, string.Empty);

                //GPGS 시작.
                PlayGamesPlatform.Activate();
        #endif*/

        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
                    .Builder()
                    .RequestServerAuthCode(false)
                    .RequestEmail()                 // 이메일 요청
                    .RequestIdToken()               // 토큰 요청
                    .Build();

        //커스텀된 정보로 GPGS 초기화
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;

        // LoginByCustumId();

        //게스트 로그인 여부


        BackendFederation.Android.OnGoogleLogin += (bool isSuccess, string errorMessage, string token) =>
        {
            if (isSuccess == false)
            {
                Debug.LogError(errorMessage);

                PopupManager.Instance.ShowYesNoPopup("알림", "구글 로그인 실패 재시도 합니다", LoginGoogle, () =>
                {
                    Application.Quit();
                });

                return;
            }

            // 로그인이 성공되었습니다.

            /*
            loginId = Social.localUser.id;
            email = ((PlayGamesLocalUser)Social.localUser).Email;

            Debug.Log($"Hash {Backend.Utils.GetGoogleHash()}");
            Debug.Log("Email - " + ((PlayGamesLocalUser)Social.localUser).Email);
            Debug.Log("GoogleId - " + Social.localUser.id);
            Debug.Log("UserName - " + Social.localUser.userName);*/

            var loginBro = Backend.BMember.AuthorizeFederation(token, FederationType.Google);

            if (loginBro.IsSuccess() == true)
            {
                PlayerPrefs.SetInt(SAVE_KEY_LOGIN_STATE, (int)LoginState.IsFederationGoogleLogin);

                //LoginByCustumId();
                OnLoginSuccess();
            }
            else
            {
                OnLoginFail();
            }
        };

        //최초실행하여 키가 없을때
        if(PlayerPrefs.HasKey(SAVE_KEY_LOGIN_STATE) == false)
        {
            PlayerPrefs.SetInt(SAVE_KEY_LOGIN_STATE, (int)LoginState.Offline);
        }

        switch ((LoginState)PlayerPrefs.GetInt(SAVE_KEY_LOGIN_STATE))
        {
            case LoginState.Offline:
                {
                    loginButtonBoard.gameObject.SetActive(true);
                }
                break;


            case LoginState.IsGuestLogin:
                {
                    LoginGuest();
                }
                break;


            case LoginState.IsFederationGoogleLogin:
                {
                    LoginGoogle();
                }
                break;

#if UNITY_IOS
            case LoginState.IsFederationIOSLogin:
                {
                    LoginApple();
                }
                break;
#endif
        }
    }

#if UNITY_ANDROID || UNITY_IOS
    public void LoginGoogle()
    {
        string message;

        var result = BackendFederation.Android.GoogleLogin("814858628636-gsml4865jqd0i08bsgf30o28rmkh9h38.apps.googleusercontent.com", out message);

        if (result == false)
        {
            Debug.LogError(message);
        }
    }
#endif

    public void LoginGuest()
    {
        BackendReturnObject BRO = Backend.BMember.GuestLogin();

        if (BRO.IsSuccess())
        {
            Debug.Log("게스트 로그인 + 회원가입 성공");

            PlayerPrefs.SetInt(SAVE_KEY_LOGIN_STATE, (int)LoginState.IsGuestLogin);

            OnLoginSuccess();
        }
        else
        {
            Debug.Log("게스트 로그인 실패" + BRO.GetMessage());

            OnLoginFail();
            //작업- 에러분기에 따른 처리 필요
        }
    }


#if UNITY_IOS
    public void LoginApple()
    {
        var loginArgs = new AppleAuthLoginArgs(AppleAuth.Enums.LoginOptions.IncludeEmail | AppleAuth.Enums.LoginOptions.IncludeFullName);

        appleAuthManager.LoginWithAppleId(
            loginArgs,
            credential =>
            {
                if (credential is IAppleIDCredential appleIdCredential)
                {
                    var userId = appleIdCredential.User;
                    var email = appleIdCredential.Email;
                    var fullName = appleIdCredential.FullName;
                    var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
                    var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);

                    // 로그인처리
                    BackendReturnObject bro = Backend.BMember.AuthorizeFederation(identityToken, FederationType.Apple);

                    Debug.Log("APPLE 로그인 성공 : " + identityToken);

                    if (bro.IsSuccess())
                    {
                        PlayerPrefs.SetString(CommonString.SavedLoginTypeKey, loginId);

                        PlayerPrefs.SetString(CommonString.IOS_loginType, IOS_LoginType.GameCenter.ToString());

                        
                        PlayerPrefs.SetInt(SAVE_KEY_LOGIN_STATE, (int)LoginState.IsFederationLogin);
                        
                        OnLoginSuccess();

                        //LoginByCustumId();
                        //성공 처리
                    }
                    else
                    {
                        Debug.LogError("Bro Apple 로그인 실패");

                        UiIosLoginBoard.Instance.loginProcess = false;
                        
                        OnLoginFail();
                        //실패 처리
                    }
                }
            },
            error =>
            {
                Debug.Log("Apple Signin Error");
            });
    }
#endif


#if UNITY_ANDROID || UNITY_IOS
    public void ChangeFederationToGoogle()
    {
        Social.localUser.Authenticate((bool success) =>
        {
            if (success)
            {
                BackendReturnObject bro = Backend.BMember.ChangeCustomToFederation(GetGoogleTokens(), FederationType.Google);

                if (bro.IsSuccess())
                {
                    Debug.Log("로그인 타입 전환에 성공했습니다");

                    PlayerPrefs.SetInt(SAVE_KEY_LOGIN_STATE, (int)LoginState.IsFederationGoogleLogin);
                }
                else
                {
                    Debug.Log(bro.GetMessage());

                    switch (bro.GetStatusCode())
                    {
                        case "400":
                            {

                            }
                            break;

                        case "409":
                            {

                            }
                            break;
                    }


                    /*
      errorCode : BadParameterException
      message : bad type, 잘못된 type 입니다

      이미 Federation 계정으로 가입된 계정에 커스텀 아이디 변경을 시도한 경우
      statusCode : 409
      errorCode : DuplicatedParameterException
      message : Duplicated federationId, 중복된 federationId 입니다

      statusCode : 400
      errorCode : UndefinedParameterException
      message : undefined access_token, access_token을(를) 확인할 수 없습니다

      statusCode : 400
      errorCode : UndefinedParameterException
      message : undefined federation_access_token, federation_access_token을(를) 확인할 수 없습니다
      */
                }
            }
        else
        {
            // 로그인 실패
            Debug.Log("Login failed for some reason");
        }
    });
}

#endif
    public void ChangeFederationToApple()
    {
#if UNITY_IOS || true
        var loginArgs = new AppleAuthLoginArgs(AppleAuth.Enums.LoginOptions.IncludeEmail | AppleAuth.Enums.LoginOptions.IncludeFullName);

        appleAuthManager.LoginWithAppleId(
            loginArgs,
            credential =>
            {
                if (credential is IAppleIDCredential appleIdCredential)
                {
                    var userId = appleIdCredential.User;
                    var email = appleIdCredential.Email;
                    var fullName = appleIdCredential.FullName;
                    var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
                    var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);

                    //페더레이션 전환
                    Debug.Log("APPLE 로그인 성공 : " + identityToken);

                    BackendReturnObject bro = Backend.BMember.ChangeCustomToFederation(identityToken, FederationType.Apple);

                    if (bro.IsSuccess())
                    {
                        Debug.Log("로그인 타입 전환에 성공했습니다");

                        PlayerPrefs.SetInt(SAVE_KEY_LOGIN_STATE, (int)LoginState.IsFederationIOSLogin);
                    }
                    else
                    {
                        Debug.Log(bro.GetMessage());

                        switch (bro.GetStatusCode())
                        {
                            case "400":
                                {

                                }
                                break;

                            case "409":
                                {

                                }
                                break;
                        }

                    }
                }
            },
            error =>
            {
                Debug.Log("Apple Signin Error");
            });
#endif
    }


    private IEnumerator SceneChangeRoutine()
    {

#if UNITY_IOS
    SendQueue.ResumeSendQueue();
#endif
        ServerData.LoadTables();
        PlayerData.Instance.LoadUserNickName();

        while (SendQueue.UnprocessedFuncCount != 0)
        {
            yield return null;
        }

        //닉네임 입력이 된 상태라면 (테스트 필요)
        if (PlayerData.Instance.NickName.HasValue == true)
        {
            loginButtonBoard.gameObject.SetActive(false);
        }
        else
        {
            while (true)
            {
                yield return null;

                if (SendQueue.UnprocessedFuncCount <= 0 && isSignIn)
                {
                    loginButtonBoard.gameObject.SetActive(false);
                    nickNameInputBoard.gameObject.SetActive(true);

                    break;
                }
            }
        }
    }

    private void OnLoginSuccess()
    {
        isSignIn = true;

        StartCoroutine(SceneChangeRoutine());
    }

    private void OnLoginFail()
    {
    }

    private string GetGoogleTokens()
{
    if (PlayGamesPlatform.Instance.localUser.authenticated)
    {
        string _IDtoken = PlayGamesPlatform.Instance.GetIdToken();

        // string _IDtoken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
        return _IDtoken;
    }
    else
    {
        Debug.Log("접속되어 있지 않습니다. PlayGamesPlatform.Instance.localUser.authenticated :  fail");
        return null;
    }
}

    private string GetSocialLoginKey()
    {
        //테스트용 a_8846847867697156085
        //로꼬 a_3961873472804492579
#if UNITY_EDITOR
        return editorLoginId;
#endif

        Debug.LogError($"GetGoogleLoginKey {loginId}");
        return loginId;
    }

private void LoginByCustumId(string id = null, string password = null)
{
    BackendReturnObject bro = null;

    if (id == null)
    {
        bro = Backend.BMember.CustomLogin(GetSocialLoginKey(), GetSocialLoginKey());
    }
    else
    {
        bro = Backend.BMember.CustomLogin(id, password);
        email = password;
    }

    //회원가입 안됨
    if (bro.IsSuccess())
    {
        Debug.LogError("Login success");

        //UiIosLoginBoard.Instance.CloseCustomGuestCreateBoard();
        StartCoroutine(SceneChangeRoutine());
    }
    else
    {
        Debug.Log($"LoginFail bro.GetStatusCode() {bro.GetStatusCode()}");

        if (bro.GetStatusCode() == "403")
        {
            PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "서버에 문제가 있습니다. 앱을 종료합니다. \n 잠시후 다시 시도해주세요", () =>
            {
                Application.Quit();
            });
        }

        //구글인경우 id=null
        if (bro.GetStatusCode() == "401" && id == null)
        {
            SignIn();
        }
        else if (bro.GetStatusCode() == "401")
        {
            //암호 틀림
            if (bro.GetMessage().Contains("Password"))
            {
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "잘못된 패스워드 입니다.", null);
            }
            else
            {
                SignIn(id, password);
            }
        }
    }
}
public void SignIn(string id = null, string password = null)
{
    BackendReturnObject BRO = null;

    if (id == null)
    {
        BRO = Backend.BMember.CustomSignUp(GetSocialLoginKey(), GetSocialLoginKey());
    }
    else
    {
        BRO = Backend.BMember.CustomSignUp(id, password);
    }

    if (BRO.IsSuccess())
    {
        isSignIn = true;
        Debug.Log("Sign in success");
        LoginByCustumId(id, password);
    }
    else
    {
        Debug.Log($"SignIn error {BRO.GetStatusCode()}");
        switch (BRO.GetStatusCode())
        {
            case "401":
                Debug.Log("이미 회원가입된 회원");
                PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "아이디나 패스워드가 잘못됐습니다.", null);
                break;
            case "409":
                Debug.Log("이미 회원가입된 회원");
                if (id == null)
                {
                    PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "이미 등록된 계정 입니다.", null);
                }
                else
                {
                    LoginByCustumId(id, password);
                }
                break;

            case "403":
                Debug.Log("차단된 사용자 입니다. 차단 사유 : " + BRO.GetErrorCode());
                break;
        }
    }
}

#if UNITY_EDITOR
[ContextMenu(nameof(DeleteGuestInfo))]
public void DeleteGuestInfo()
{
    Backend.BMember.DeleteGuestInfo();
}
#endif

/*
 public void LoginBySavedData()
 {
     string savedData = PlayerPrefs.GetString(CommonString.IOS_loginType, string.Empty);

     Debug.Log($"savedData {savedData}");
     if (savedData.Equals(string.Empty))
     {
         //가입 진행
         UiIosLoginBoard.Instance.ShowCustomGuestCreateBoard();
     }
     //커스텀 계정
     else if (savedData.Equals(IOS_LoginType.Custom.ToString()))
     {
         string id = PlayerPrefs.GetString(CommonString.SavedLoginTypeKey, string.Empty);
         string passWord = PlayerPrefs.GetString(CommonString.SavedLoginPassWordKey, string.Empty);

         LoginByIdPassWord(id, passWord);
     }
     else if (savedData.Equals(IOS_LoginType.GameCenter.ToString()))
     {
         LoginApple();
     }
 }


 public void LoginByIdPassWord(string id = null, string password = null)
 {
     loginId = PlayerPrefs.GetString(CommonString.SavedLoginTypeKey, loginId);
     LoginByCustumId(id, password);
 }





#if UNITY_ANDROID
        if (id == null)
        {
            PopupManager.Instance.ShowConfirmPopup(CommonString.Notice, "로그인 실패 재시도 합니까?", () =>
            {
                SignIn();
            });
        }
#endif
    }
}
*/
            }
