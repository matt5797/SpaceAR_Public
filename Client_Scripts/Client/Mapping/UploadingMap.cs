using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.Android;
using System;
using SpaceAR.Core.AWS;
using Newtonsoft.Json.Linq;
using SpaceAR.Client.Vuforia;
using TMPro;
using UnityEngine.SceneManagement;
public class UploadingMap : MonoBehaviour
{
    public GameObject panel;
    public InputField WorldNameInput;
    public Toggle yesToggle;
    public Button acceptBtn;
    public Button ExitBtn;
    string id;

    #region 텍스트 UI 변수 - 위도, 경도 
    public float latitude = 0;
    public float longitude = 0;
    #endregion

    public string worldnameText;
    public bool isPublic;
    #region GPS 수신 대기시간
    public float maxWaitTime = 10.0f;
    private float waitTime = 0;
    public float resendTime = 1.0f;
    private bool receiveGPS = false;
    #endregion
    public Button uploadPanelBtn;

    public string AreaTargetsDataDirectory = "AreaTargetData";
    public string realpath;

    JArray jar;

    public GameObject messageBox;
    public GameObject GObackHomeBtn;
    private void Awake()
    {
        StartCoroutine("GPS_On");
    }
    // Start is called before the first frame update
    void Start()
    {
        
        realpath = Path.Combine(Application.persistentDataPath, AreaTargetsDataDirectory);
        //////////////////////////////
        //
        uploadPanelBtn.onClick.AddListener(() =>
        {
            panel.SetActive(true);
        });
        ExitBtn.onClick.AddListener(() =>
        {
            panel.SetActive(false);
        });

        WorldNameInput.onValueChanged.AddListener(ValueChanged);

        acceptBtn.onClick.AddListener(() =>
        {
            Upload(jar);
        });

        yesToggle.onValueChanged.AddListener(OnToggleChanged);

        sendMapinfo();  //post 한번 함, jar에 저장됨

        GObackHomeBtn.GetComponent<Button>().onClick.AddListener(() => {
            GoBackToHomeScene();
        });

    }

    private void OnToggleChanged(bool isOn)
    {
        if (isOn == true)
        {
            isPublic = true;
        }
        else
        {
            isPublic = false;
        }
    }

    void ValueChanged(string text) //이때는 원하는 매개변수를 사용할 수 있다
    {
        worldnameText = text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public async void sendMapinfo()
    {

        jar =  await AWSManager.Instance.PostMap(AWSManager.Instance.UserId);
        //jar =  await AWSManager.Instance.PostMap(name, AWSManager.Instance.UserId,is_public,longitude,latitude);
        //jar에 return 값 저장됨 -> return JArray.Parse(result["body"].ToString());
        //Upload(jar);
        foreach (var data in jar)
        {
            id = data["id"].ToString();
        }
        ChangeTargetName.instance.ChangeName(id);

    }


    public async void Upload(JArray jar)
    {
        /*
        //����ũ���̵�-��¥-�ð�
        //���� �Ǿ������״�..realpath���� ��¥�ð� ��ģ���¸� ����Ʈ ���ҷ� add
        //�������� �̸��� ����Ʈ���ҷ� add (�׷��� �����Ҷ� ������ü�� ���ÿ� �Ȱ��� �Ҽ�����)
        //
        DirectoryInfo di = new DirectoryInfo(realpath);
        DirectoryInfo[] dirinfos = di.GetDirectories();
        string recentFolderName;
        //List<string> dirNameList = new List<string>();
        List<long> dirNumList = new List<long>();

        SortedDictionary<long, string> dic = new SortedDictionary<long, string>();

        foreach (DirectoryInfo Dir in dirinfos)
        {
            string[] partName = Dir.Name.Split('-');  //[0]=����ũ, [1] = ��¥, [2] = �ð�
            string newName = partName[1] + partName[2]; //��¥�ð� ������ string
            dirNumList.Add((long)Convert.ToInt64(newName));
            dic.Add((long)Convert.ToInt64(newName), Dir.Name); //������ �̸�, �����̸�Ű�� ������ ��ųʸ�
        }
        dirNumList.Sort();
        recentFolderName = dic[dirNumList[dirNumList.Count - 1]]; //���ĵ� ��������, �̴� �� �ֽ� �����̸�*/
        string recentFolderName = ChangeTargetName.instance.acb.FileName;
        print("recentfoldername : " + recentFolderName);

        DirectoryInfo di = new DirectoryInfo(realpath);
        DirectoryInfo[] dirinfos = di.GetDirectories();
        foreach (DirectoryInfo Dir in dirinfos)
        {
            print("existing my realpath dir:" + Dir.Name);
        }

        DirectoryInfo zo = new DirectoryInfo(realpath + "/" + recentFolderName);
        DirectoryInfo[] zos = zo.GetDirectories();
        foreach (DirectoryInfo Dir in zos)
        {
            print("last files pleasessss:" + Dir.Name);
        }
        messageBox.SetActive(true);
        await AWSManager.Instance.PutMapInfo(worldnameText, id, isPublic, longitude, latitude);
        await AWSManager.Instance.PostMapAreaTarget(id, recentFolderName);
        await AWSManager.Instance.UploadDirectory("spacear-maps/" + recentFolderName, realpath + "/" + recentFolderName, OpenMessageBox);
        print("dudududududdadad::::::::::::::::::::" + "spacear-maps/" + recentFolderName);
        print("want to send directory : " + realpath + "/" + recentFolderName);



        //await AWSManager.Instance.UploadDirectory("spacear-maps/" + id, realpath + "/" + id);
    }

    void OpenMessageBox()
    {
        messageBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "업로드 완료!";
        GObackHomeBtn.SetActive(true);

    }

    void GoBackToHomeScene()
    {
        SceneManager.LoadScene(0);
    }


    public IEnumerator GPS_On()
    {
        /* GPS 처리 함수 */
        // 1-1 만약 GPS 사용 허가를 받지 못하면, 
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            // 1-2 권한 허가 팝업 요청한다
            Permission.RequestUserPermission(Permission.FineLocation);

            while (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
            {
                yield return null;
            }
        }

        // 2-1 만약 GPS 장치가 켜져있지 않으면, 
        if (!Input.location.isEnabledByUser)
        {
            // 2-2 위치정보 수신 불가 팝업 띄운다
            print("GPS Off");
            print("GPS OFF");

            yield break;
        }

        /* 수신 대기 설정 */
        // 1 위치 데이터를 요청한다 -> 수신대기
        Input.location.Start();

        // 2 GPS 수신 상태가 초기 상태에서 일정 시간 동안 대기
        while (Input.location.status == LocationServiceStatus.Initializing && waitTime < maxWaitTime)
        {
            yield return new WaitForSeconds(1.0f);
            waitTime++;
        }
        // 3 수신 실패 시 수신이 실패 팝업 표시
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("위치 정보 수신 실패");
            print("위치 정보 수신 실패");
        }

        // 4 응답 대기시간 초과 시 시간초과 팝업 표시
        if (waitTime >= maxWaitTime)
        {
            print("응답 대기 시간 초과");
            print("응답 대기 시간 초과");
        }

        /* GPS 데이터 성공 */
        // 1 수신된 데이터를 화면에 출력한다
        LocationInfo li = Input.location.lastData;
        latitude = li.latitude;
        longitude = li.longitude;
        print("위도: " + latitude.ToString());
        print("경도: " + longitude.ToString());


        /*FileInfo file = new FileInfo(realpath + "/" + "GPS.txt");
        if (!file.Exists)
        {
            FileStream fs1 = file.Create();
            fs1.Close();
        }
        FileStream fs = file.OpenWrite();
        TextWriter tw = new StreamWriter(fs);
        tw.Write(longitude.ToString() + "\n" + latitude.ToString());
        tw.Close();
        fs.Close();*/
    }
}
