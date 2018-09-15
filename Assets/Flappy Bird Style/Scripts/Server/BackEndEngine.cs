using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BackEnd;
using Facebook.Unity;

using UnityEngine.SceneManagement;

using System;

// EAAGuB9pDOLsBAOZCkb4ZBZBg5MLwDXF1s8qmgOcDNRrqKqdqHdA6HWDWsRAhaR5rRoxs6HmlZBDPqQwxh0enq6NwyY5cAuVs9pj2ayQ9zBJQF9KDi3jdxlT494rcL9YtbX19wwC4OoyDuAG8p2HZAFrS5NUURvINM3jn3LiAfAbbUSg7U5pBKYM9uFgNmr7cZD

public class BackEndEngine : MonoBehaviour
{
    public string userIndate = "";

    private void Awake()
    {
// ------------------------------------------------------------------------------------ Facebook ------------------------------------------------------------------------------------
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitFBCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }
// ------------------------------------------------------------------------------------ -------- ------------------------------------------------------------------------------------
    }

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        Debug.Log("뒤끝 SDK 초기화");

        // 서버가 초기화되지 않은 경우
        if (!Backend.IsInitialized)
        {
            Backend.Initialize(InitializeBackend);
        }
        else
        {
            // 초기화
            InitializeBackend();
        }
    }

    public void InitializeBackend()
    {
        Debug.Log(Backend.Utils.GetServerTime().GetReturnValue()); // 뒤끝 서버의 현재 시간을 받아오는 메서드

        if (!Backend.Utils.GetGoogleHash().Equals("")) // 빌드된 안드로이드 apk에서 구글 해 시키가 있으면
            Debug.Log(Backend.Utils.GetGoogleHash()); // 이를 출력
    }

    // ------------------------------------------------------------------------------------ Facebook ------------------------------------------------------------------------------------
    private void InitFBCallback()
    {
        if (FB.IsInitialized)
        {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        }
        else
        {
            Debug.Log("Failed to Initialize the Facebook SDK");
        }
    }

    private void OnHideUnity(bool isGameShown)
    {
        if (!isGameShown)
        {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        }
        else
        {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }
    
    // 회원가입 후 결과 Callback
    private void AuthCallback(ILoginResult result)
    {
        // 로그인 성공
        if (FB.IsLoggedIn)
        {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            // Print current access token's User IDDebug.Log(aToken);
            string facebookToken = aToken.TokenString;

            // 가입 여부 확인
            //BackendReturnObject userInBackendResult = Backend.BMember.CheckUserInBackend(facebookToken, FederationType.Facebook);
            //string userInBackendCode = userInBackendResult.GetStatusCode();
            //if (userInBackendCode == "204")
            //{
            //    Debug.Log("가입되어 있지 않습니다.");
            //}
            //else if (userInBackendCode == "200")
            //{
            //    Debug.Log("가입되어 있습니다.");
            //}

            // 뒤끝 서버에 획득한 페이스북 토큰으로 가입요청
            // 동기 방법으로 가입 요청
            BackendReturnObject signUpResult = Backend.BMember.AuthorizeFederation(facebookToken, FederationType.Facebook); // 비동기로 돌리고, 그 동안 Status 바 등을 돌리는 것으로 생각해야겠다.
            if (signUpResult.GetStatusCode() == "200")
            {
                BackendReturnObject userInfo = Backend.BMember.GetUserInfo();
                LitJson.JsonData userInfoJson = userInfo.GetReturnValuetoJSON();

                userIndate = userInfoJson["row"]["inDate"].ToString();

                SceneManager.LoadScene("Main");
                //if (userInfoJson["row"]["nickname"] == null)
                //{
                //    Debug.Log("닉네임을 설정하세요.");
                //}
            }
        }
        else
        {
            Debug.Log("User cancelled login");
        }
    }
    // ------------------------------------------------------------------------------------ -------- ------------------------------------------------------------------------------------

    public void SignUpWithFacebook()
    {
        // 읽어올 권한을 설정
        var perms = new List<string>() { "public_profile", "email" }; // 뒤끝 콘솔 > 유저관리에서 회원의 이메일 정보를 알 수 있습니다.
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }

    public void SignInWithFacebook()
    {
        
    }
}
