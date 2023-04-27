using BackEnd;
using GooglePlayGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiFederationTest : MonoBehaviour
{
    // Start is called before the first frame update
    public void FederationLoginGoogle()
    {
        LoginManager.Instance.ChangeFederationToGoogle();
    }

    public void FederationLoginApple()
    {
        LoginManager.Instance.ChangeFederationToApple();
    }

    private string GetToken()
    {
        

        return null;
    }
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