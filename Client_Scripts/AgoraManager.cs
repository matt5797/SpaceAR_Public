using Agora.Rtc;
using Agora_RTC_Plugin.API_Example.Examples.Advanced.AudioMixing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AgoraManager : MonoBehaviour
{
    public static AgoraManager Instance;

    [Header("화면 공유")]
    public Slider volumeSlider;
    public bool sharingScreen = false;
    public TMP_Text shareScreenBtnText;
    public Button shareScreenBtn;
    public Toggle muteToggle;

    [Header("스트리밍")]
    public GameObject localView;
    public GameObject remoteView;
    public Button leaveButton;
    public Button joinButton;
    public Toggle broadCasterToggle;
    public Toggle audienceToggle;

    // Fill in your app ID.
    private string _appID = "50dbe192b6304c5bbbb4a36126fb85e7";
    // Fill in your channel name.
    private string _channelName = "SpaceAR";
    // Fill in the temporary token you obtained from Agora Console.
    private string _token = "007eJxTYHgb0O9Ud69qUji76nvWf/OsLsze+Xl+Ufos02P5DQ71yqYKDKYGKUmphpZGSWbGBibJpklAYJJobGZoZJaWZGGaap7UXJDcEMjI4F2nwMrIAIEgPjtDcEFicqpjEAMDAMQ9H+U=";
    // A variable to hold the user role.
    private string clientRole = "";
    // A variable to save the remote user uid.
    private uint remoteUid;

    internal VideoSurface LocalView;
    internal VideoSurface RemoteView;
    internal IRtcEngine RtcEngine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        SetupVideoSDKEngine();
        InitEventHandler();
        SetupUI();
    }

    void SetupUI()
    {
        // 화면 공유
        shareScreenBtn.onClick.AddListener(ShareScreen);
        volumeSlider.GetComponent<Slider>().maxValue = 100;
        volumeSlider.GetComponent<Slider>().onValueChanged.AddListener(delegate { ChangeVolume((int)volumeSlider.value); });
        muteToggle.isOn = false;
        muteToggle.onValueChanged.AddListener((value) => MuteRemoteAudio(value));

        // 스트리밍
        LocalView = localView.AddComponent<VideoSurface>();
        localView.transform.Rotate(0, 0, -180);

        RemoteView = remoteView.AddComponent<VideoSurface>();
        remoteView.transform.Rotate(0, 0, -180);

        leaveButton.onClick.AddListener(Leave);
        joinButton.onClick.AddListener(Join);

        broadCasterToggle.isOn = false;
        broadCasterToggle.onValueChanged.AddListener((value) =>
        {
            Func1(value);
        });
        
        audienceToggle.onValueChanged.AddListener((value) =>
        {
            Func2(value);
        });
    }

    public void StartStreaming()
    {
        RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
        clientRole = "Host";

        RtcEngine.EnableVideo();
        // 로컬 비디오 세팅
        LocalView.SetForUser(0, "", VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA);
        // 채널 들어가기
        RtcEngine.JoinChannel(_token, _channelName);

        ShareScreen();
    }

    public void ReceiveStreaming()
    {
        RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_AUDIENCE);
        clientRole = "Audience";
    }

    // 녹음된 오디오 볼륨 조정
    void ChangeVolume(int value)
    {
        RtcEngine.AdjustRecordingSignalVolume(value);
    }
    
    // 원격 사용자 음소거 및 음소거 해제 컨트롤
    void MuteRemoteAudio(bool value)
    {
        RtcEngine.MuteRemoteAudioStream(System.Convert.ToUInt32(remoteUid), value);
    }

    // 화면공유 트랙을 게시 가능하게 
    void UpdateChannelPublishOptions(bool publishMediaPlayer)
    {
        ChannelMediaOptions channelOptions = new ChannelMediaOptions();
        channelOptions.publishScreenTrack.SetValue(publishMediaPlayer);
        //channelOptions.publishAudioTrack.SetValue(true);
        channelOptions.publishSecondaryScreenTrack.SetValue(publishMediaPlayer);
        channelOptions.publishCameraTrack.SetValue(!publishMediaPlayer);
        RtcEngine.UpdateChannelMediaOptions(channelOptions);
    }

    // 화면 스트림을 로컬로 표시
    void SetupLocalVideo(bool isScreenSharing)
    {
        if (isScreenSharing)
        {
            localView.transform.Rotate(-180, 0, 180);
            LocalView = localView.AddComponent<VideoSurface>();
            LocalView.SetForUser(0, "", VIDEO_SOURCE_TYPE.VIDEO_SOURCE_SCREEN_PRIMARY);
            Debug.Log("============화면공유==========");
        }
        else
        {
            localView.transform.Rotate(180, 0, 180);
            LocalView = localView.AddComponent<VideoSurface>();
            LocalView.SetForUser(0, "", VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA_PRIMARY);
        }
    }

    // 화면 공유 시작
    public void ShareScreen()
    {
        if (!sharingScreen)
        {
            // The target size of the screen or window thumbnail (the width and height are in pixels).
            SIZE t = new SIZE(800, 800);
            // The target size of the icon corresponding to the application program (the width and height are in pixels)
            SIZE s = new SIZE(800, 800);
            // Get a list of shareable screens and windows
            var info = RtcEngine.GetScreenCaptureSources(t, s, true);

            Debug.Log("------" + info);

            // Get the first source id to share the whole screen.
            ulong dispId = info[5].sourceId;
            // To share a part of the screen, specify the screen width and size using the Rectangle class.
            RtcEngine.StartScreenCaptureByWindowId(System.Convert.ToUInt32(dispId), new Rectangle(),
                    default(ScreenCaptureParameters));
            // Publish the screen track and unpublish the local video track.
            UpdateChannelPublishOptions(true);
            // Display the screen track in the local view.
            SetupLocalVideo(true);
            // Change the screen sharing button text.
            shareScreenBtnText.text = "Stop Sharing";
            // Update the screen sharing state.
            sharingScreen = true;
        }
        else 
        {
            RtcEngine.StopScreenCapture();
            UpdateChannelPublishOptions(false);
            SetupLocalVideo(false);
            sharingScreen = false;
            shareScreenBtnText.text = "Share Screen";
        }
    }

    private void Func2(bool value)
    {
        if (value == true)
        {
            broadCasterToggle.isOn = false;
            RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_AUDIENCE);
            clientRole = "Audience";
        }
    }

    private void Func1(bool value)
    {
        if (value == true)
        {
            audienceToggle.isOn = false;
            RtcEngine.SetClientRole(CLIENT_ROLE_TYPE.CLIENT_ROLE_BROADCASTER);
            clientRole = "Host";
        }
    }

    // 엔진 생성
    void SetupVideoSDKEngine()
    {
        RtcEngine = Agora.Rtc.RtcEngine.CreateAgoraRtcEngine();
        RtcEngineContext context = new RtcEngineContext(_appID, 0, CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_LIVE_BROADCASTING, AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT);

        RtcEngine.Initialize(context);
    }

    void InitEventHandler()
    {
        UserEventHandler handler = new UserEventHandler(this);
        RtcEngine.InitEventHandler(handler);
    }

    internal class UserEventHandler : IRtcEngineEventHandler
    {
        private readonly AgoraManager _videoSample;

        internal UserEventHandler(AgoraManager videoSample)
        {
            _videoSample = videoSample;
        }

        public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
        {
            Debug.Log($"{connection.channelId} 채널에 입장 했습니다.");
        }

        public override void OnUserJoined(RtcConnection connection, uint uid, int elapsed)
        {
            Debug.Log($"사용자가 입장 했습니다. {connection.channelId}, {connection.localUid},  {uid}");
            _videoSample.RemoteView.SetForUser(uid, connection.channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
            if(_videoSample.clientRole == "Audience")
            {
                _videoSample.remoteUid = uid;
            }
            _videoSample.remoteUid = uid;
        }

        // 원격 사용자가 채널을 떠날 떄 원격 비디오 중지
        public override void OnUserOffline(RtcConnection connection, uint uid, USER_OFFLINE_REASON_TYPE reason)
        {
            Debug.Log($"사용자가 퇴장 했습니다. {connection.channelId}, {connection.localUid},  {reason}");
            _videoSample.RemoteView.SetEnable(false);
        }
    }


    public void Join()
    {
        if(broadCasterToggle.isOn == false && audienceToggle.isOn == false)
        {
            Debug.Log("역할을 먼저 선택해 주세요");
        }
        else
        {
            RtcEngine.EnableVideo();
            // 로컬 비디오 세팅
            LocalView.SetForUser(0, "", VIDEO_SOURCE_TYPE.VIDEO_SOURCE_CAMERA);
            // 채널 들어가기
            RtcEngine.JoinChannel(_token, _channelName);
        }
    }

    // 사용자가 종료하면 채널에서 나가기
    public void Leave()
    {
        RtcEngine.LeaveChannel();
        // 모듈 끄기
        RtcEngine.DisableVideo();
        // 원격 비디오 랜더링 중지
        RemoteView.SetEnable(false);
        // 로컬 비디오 랜더링 중지
        RemoteView.SetEnable(false);
    }

    // 리소스 정리
    void OnApplicationQuit()
    {
        if (RtcEngine != null)
        {
            Leave();
            RtcEngine.Dispose();
            RtcEngine = null;
        }
    }

}
