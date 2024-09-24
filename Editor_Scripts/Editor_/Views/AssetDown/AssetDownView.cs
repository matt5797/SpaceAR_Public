using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using Battlehub.RTEditor.Views;
using SpaceAR.Core.AWS;
using Battlehub.RTSL;
using System.IO;
using Amazon.S3.Transfer;
using Amazon.S3;
using System;
using Battlehub.RTCommon;
using Battlehub.RTEditor;
using UnityEditor;
using Battlehub.RTSL.Interface;
using static DG.Tweening.DOTweenAnimation;
using Application = UnityEngine.Application;

namespace SpaceAR.Editor.Views.AssetDown.Views
{
    public class AssetDownView : View
    {
        public Button refreshButton;
        public Button purchaseButton;
        
        public Button assetContentPrefab;
        public Transform assetParent;
        public GameObject detailContent;
        List<Button> buttonList = new List<Button>();

        Button assetContent;
        string userId;
        string assetId;
        string assetName;

        Image assetThumbnail;
        string assetTitle;
        string assetOverview;

        string assetBtnId;

        protected override void Start()
        {
            base.Start();

            // Userid �޾ƿ���
            userId = AWSManager.Instance.GetUsersId();

            //assetThumbnail = detailContent.GetComponentInChildren<Image>();
            //assetTitle = detailContent.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text;
            //assetOverview = detailContent.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text;

            Debug.Log($"UserId : {userId.GetType()}");

            // ���� ����Ʈ ���ΰ�ħ
            OnRefreshButton();
            refreshButton.onClick.AddListener(OnRefreshButton);

            //���� ���� ��ư
            purchaseButton.onClick.AddListener(OnPurchase);
            
        }

        // ���� ����Ʈ ���ΰ�ħ
        public async void OnRefreshButton()
        {
            var res = await AWSManager.Instance.GetAssetPurchaseList();

            // ���� ����Ʈ �ʱ�ȭ
            
            foreach (var item in buttonList)
            {
                Destroy(item.gameObject);
               
            }
            buttonList.Clear();

            print($"res : {res}");

            foreach(var data in res)
            {
                if(data["owner"].ToString() == userId)
                {
                    // ���� ���̵� ����Ʈ�� ����
                    assetId = data["asset"].ToString();

                    // ���� �̸� ã�ƿ���
                    assetName = data["name"].ToString();

                    assetContent = GameObject.Instantiate(assetContentPrefab, assetParent) as Button;

                    buttonList.Add(assetContent);

                    assetContent.name = assetId;

                    // ���� ��ư�� ���̵� ����
                    assetContent.GetComponent<AssetContent>().assetId = assetId;

                    assetContent.GetComponentInChildren<TextMeshProUGUI>().text = assetName;
                    assetTitle = data["name"].ToString();
                    assetOverview = data["overview"].ToString();
                }
            }

            foreach (Button btn in buttonList)
            {
                // ���� ����Ʈ ����Ʈ ��������
                btn.onClick.AddListener(() => OnContentUpdate(btn));
            }
        }

        // ���� �� ���� ������Ʈ
        public async void OnContentUpdate(Button btn)
        {
            var res = await AWSManager.Instance.GetAssetPurchaseList();

            // ��ư ���̵� ��������
            assetBtnId = btn.gameObject.GetComponent<AssetContent>().assetId;
            Debug.Log($"���� ���̵� : {assetBtnId}");

            // ���̵� ���ؼ� ���� ������Ʈ
            foreach (var data in res)
            { 
                if(assetBtnId == data["asset"].ToString())
                {
                    // ==============������==============
                    detailContent.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = data["name"].ToString();
                    detailContent.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = data["overview"].ToString();            
                    // ====================================
                }
            }

            string burketName = "spacear-asset-thumbnail";

            string fileName = assetBtnId + "/1.png";

            var tex = await AWSManager.Instance.GetTextureAsync(burketName, fileName);

            // �̹��� �ֱ�
            // texture to sprite
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            detailContent.GetComponentInChildren<Image>().sprite = sprite;
            
        }

        // ���� �ٿ�ε�
        public async void OnPurchase()
        {
            string addPath = "DefaultProject/Assets";

            if (!Directory.Exists(Application.persistentDataPath + "/" + addPath + "/DownloadAsset"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/" + addPath + "/DownloadAsset");
            }

            string path = Path.Combine(Application.persistentDataPath, addPath, "DownloadAsset", assetBtnId);
            string burketName = "spacear-asset-data";

            await AWSManager.Instance.DownloadDirectoryAsync(burketName, path, assetBtnId, () => { OnComplete(); }, (e) => { Debug.Log(e); });

        }

        public void OnComplete()
        {
            IWindowManager wm = IOC.Resolve<IWindowManager>();

            wm.MessageBox("Success", "Download complete!!!", (sender, args) =>
            {
                Debug.Log("OK Click");
                // ���ΰ�ħ 
            });
        }
    }
}


