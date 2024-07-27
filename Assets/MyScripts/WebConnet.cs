
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.Networking;

public class WebConnet : MonoBehaviour
{
    [Header("서버주소")]
    public static string url = "https://xn--4k0b998acvh.xn--yq5b.xn--3e0b707e";

    /*private string getUrl =  $"{url}/unity/data?param=test";*/
    private string postUrl = $"{url}/unity/chat";
    private string postUrl2 = $"{url}/unity/unity_python3";

    //public TextMeshProUGUI getText;
    public TextMeshProUGUI getPost;
    public TMP_InputField inputField;
    private string myip;

    //서버로 받은 리스트 데이터
    public List<ChatData> chatDatas;
    void Start()
    {


        
        }
    
    public void text_btn()
    {
        print("보네기");

        ChatData chatData = new ChatData();
        chatData.ip = myip;
        chatData.username = "test";

        PlayerPrefs.SetString("입려하세요", inputField.text);//유니티 사용자 입력 
        chatData.username="test";
        chatData.usertext = inputField.text;
        if (inputField.text == string.Empty) { return; }//사용자 입력갑이 없으면 그냥 끝네기
        string json = JsonUtility.ToJson(chatData);
        print(json);
        //{"ip":"192.168.52.13","username":"test","usertext":"32452345"



        StartCoroutine(PostAnimaiRequest(postUrl2, inputField.text));//anima start
        StartCoroutine(PostRequest(postUrl, json));
    }

    IEnumerator PostAnimaiRequest(string url, string inputStr)
    {
        // 데이터를 x-www-form-urlencoded 형식으로 인코딩
        string postData = "inputStr=" + UnityWebRequest.EscapeURL(inputStr);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(postData);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            // Content-Type을 application/x-www-form-urlencoded로 설정
            webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                //print(webRequest.downloadHandler.text);
                string result = webRequest.downloadHandler.text;


                // 앞뒤 따옴표 제거
                string trimmedResult = result.Trim('\"');


                int index = 0;
                if (int.TryParse(trimmedResult, out index))
                { 
                    print(index);
                    if (index > 0 && index < 4) // 1~3 사이면 애니메이션 실행
                    {
                        OpenAiAnima.instance.animaIndex(index);
                    }
                }
                Debug.Log(webRequest.downloadHandler.text);
                print(trimmedResult);
            }
        }
    }



    //user chat
    IEnumerator PostRequest(string url, string jsonData)
    {

        byte[] bodyRaw = new System.Text.UTF8Encoding().GetBytes(jsonData);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                print(webRequest.downloadHandler.text);
                ChatDataDTO chatDataDTO = JsonUtility.FromJson<ChatDataDTO>(webRequest.downloadHandler.text);
                List<ChatData> chatData = chatDataDTO.chatDatas;
                if (chatData != null && chatDataDTO.chatDatas != null)//받은 값이 있으면
                {
                    getPost.text = "";//출력할 채팅창 초기화
                    foreach (var data in chatData)
                    {
                        getPost.text += $"{data.ip} --> {data.usertext}\n";
                        print(data.ToString());
                    }
                }
                Debug.Log(webRequest.downloadHandler.text);
                //chatDataDTO.chatDatas.Last<ChatData>().usertext


            }
        }
    }





    /*IEnumerator GetRequest(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {

                getText.text = myip;
                Debug.Log(webRequest.downloadHandler.text);
            }
        }
    }*/
}
