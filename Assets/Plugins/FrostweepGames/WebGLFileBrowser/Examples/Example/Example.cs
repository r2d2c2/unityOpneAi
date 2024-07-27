using DG.DemiLib.Utils;

using System;
using System.Collections;
using System.IO;
using System.Security.Policy;

using TMPro;

using Unity.VisualScripting;

using UnityEditor;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace FrostweepGames.Plugins.WebGLFileBrowser.Examples
{
    public class Example : MonoBehaviour
    {
        public Image myimage;//글자 백그라운드
        public TextMeshProUGUI getPost;//출력할 글자
        public TMP_InputField inputField;//사용자 입력 필드
        public AudioSource audioSource;//이미지 해석 오디오

        public Image contentImage;//출력할 파일 이미지

        public Button openFileDialogButton;

        [HideInInspector]//사용 하진 안음
        public Button saveOpenedFileButton;
        [HideInInspector]//사용 하진 안음
        public Button cleanupButton;
        [HideInInspector]//사용 하진 안음
        public Button openFolderDialogButton;
        [HideInInspector]//사용 하진 안음
        public Toggle isMultipleSelectionToggle;
        [HideInInspector]//사용 하진 안음
        public InputField filterOfTypesField;//타입 설정
        [HideInInspector]//사용 하진 안음
        public Text fileNameText,
                    fileInfoText;//이름,용량

        private string _enteredFileExtensions="png,jpg,pdf";

        private File[] _loadedFiles;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();

            openFileDialogButton.onClick.AddListener(OpenFileDialogButtonOnClickHandler);
            /*openFolderDialogButton.onClick.AddListener(OpenFolderDialogButtonOnClickHandler);
            saveOpenedFileButton.onClick.AddListener(SaveOpenedFileButtonOnClickHandler);
            cleanupButton.onClick.AddListener(CleanupButtonOnClickHandler);
            filterOfTypesField.onValueChanged.AddListener(FilterOfTypesFieldOnValueChangedHandler);*/

            WebGLFileBrowser.FilesWereOpenedEvent += FilesWereOpenedEventHandler;
            WebGLFileBrowser.FilePopupWasClosedEvent += FilePopupWasClosedEventHandler;
            WebGLFileBrowser.FileOpenFailedEvent += FileOpenFailedEventHandler;
            WebGLFileBrowser.FolderOpenFailedEvent += FolderOpenFailedEventHandler;
            WebGLFileBrowser.FileWasSavedEvent += FileWasSavedEventHandler;
            WebGLFileBrowser.FileSaveFailedEvent += FileSaveFailedEventHandler;

            // 파일 브라우저 팝업에 대해 사용자 지정 위치를 설정하려면 -> 해당 기능을 사용합니다
            // WebGLFileBrowser.SetLocalization(LocalizationKey.DESCRIPTION_TEXT, "Select file for loading:");
        }

        private void OnDestroy()
        {
            WebGLFileBrowser.FilesWereOpenedEvent -= FilesWereOpenedEventHandler;
            WebGLFileBrowser.FilePopupWasClosedEvent -= FilePopupWasClosedEventHandler;
            WebGLFileBrowser.FileOpenFailedEvent -= FileOpenFailedEventHandler;
            WebGLFileBrowser.FolderOpenFailedEvent -= FolderOpenFailedEventHandler;
            WebGLFileBrowser.FileWasSavedEvent -= FileWasSavedEventHandler;
            WebGLFileBrowser.FileSaveFailedEvent -= FileSaveFailedEventHandler;
        }

        private void SaveOpenedFileButtonOnClickHandler()
        {
            if (_loadedFiles != null && _loadedFiles.Length > 0)
                WebGLFileBrowser.SaveFile(_loadedFiles[0]);

            // 사용자 정의 파일을 저장하려면 다음 흐름을 사용합니다:
            //File file = new File()
            //{
            //    fileInfo = new FileInfo()
            //    {
            //        fullName = "Myfile.txt"
            //    },
            //    data = System.Text.Encoding.UTF8.GetBytes("my text content!")
            //};
            //WebGLFileBrowser.SaveFile(file);
        }

        private void OpenFileDialogButtonOnClickHandler()
        {
            WebGLFileBrowser.SetLocalization(LocalizationKey.DESCRIPTION_TEXT, "Select file to load or use drag & drop");

            // you could paste types like: ".png,.jpg,.pdf,.txt,.json"
            //WebGLFileBrowser.OpenFilePanelWithFilters(".png,.jpg,.pdf");
            WebGLFileBrowser.OpenFilePanelWithFilters(WebGLFileBrowser.GetFilteredFileExtensions(_enteredFileExtensions), false);
            //WebGLFileBrowser.OpenFilePanelWithFilters(WebGLFileBrowser.GetFilteredFileExtensions(_enteredFileExtensions), isMultipleSelectionToggle.isOn);
            //WebGLFileBrowser.OpenFilePanelWithFilters(WebGLFileBrowser.GetFilteredFileExtensions(".png,.jpg,.pdf"));
        }

        private void OpenFolderDialogButtonOnClickHandler()
        {
            WebGLFileBrowser.SetLocalization(LocalizationKey.DESCRIPTION_TEXT, "Select folder to load files in or use drag & drop");

            // you could paste types like: ".png,.jpg,.pdf,.txt,.json"
            // WebGLFileBrowser.OpenFolderPanelWithFilters(".png,.jpg,.pdf,.txt,.json");
            WebGLFileBrowser.OpenFolderPanelWithFilters(WebGLFileBrowser.GetFilteredFileExtensions(_enteredFileExtensions));
        }

        private void CleanupButtonOnClickHandler()
        {
            _loadedFiles = null; // 파일에 대한 링크를 제거해야 합니다. 그러면 GarbageCollector는 해당 개체에 대한 링크가 없다고 생각합니다
            saveOpenedFileButton.gameObject.SetActive(false);
            cleanupButton.gameObject.SetActive(false);

            fileInfoText.text = string.Empty;
            fileNameText.text = string.Empty;

            contentImage.color = new Color(1, 1, 1, 0);
            contentImage.sprite = null;

            WebGLFileBrowser.FreeMemory(); // free used memory and destroy created content
        }

        private void FilesWereOpenedEventHandler(File[] files)
        {
            _loadedFiles = files;

            if (_loadedFiles != null && _loadedFiles.Length > 0)
            {
                var file = _loadedFiles[0];

                if (_loadedFiles.Length > 1)
                {
                    fileInfoText.text = $"Loaded files amount: {files.Length}\n";
                }

                foreach (var loadedFile in _loadedFiles)
                {
                    //fileInfoText.text += $"Name: {loadedFile.fileInfo.name}.{loadedFile.fileInfo.extension}, Size: {loadedFile.fileInfo.SizeToString()}\n";
                    //파일이름 . 확자, 용량
                    print($"{loadedFile.fileInfo.path}");
                    //경로와 파일이름 
                    //{loadedFile.fileInfo.fullName}
                    //파일이름과 확장 자만
                    //webgl 에서는 않됨
                }

                //saveOpenedFileButton.gameObject.SetActive(true);
                //cleanupButton.gameObject.SetActive(true);

                if (_loadedFiles.Length == 1)
                {
                    contentImage.enabled= true;
                    if (file.IsImage())
                    {
                        contentImage.color = new Color(1, 1, 1, 1);
                        contentImage.sprite = file.ToSprite();
                        // 사용하지 않는 개체를 삭제하여 메모리를 사용할 수 있도록 하는 것을 잊지 마십시오!

                        WebGLFileBrowser.RegisterFileObject(contentImage.sprite);
                        //텍스처가 있는 스프라이트를 캐시 목록에 추가합니다. 더 이상 필요 없을 때 WebGL FileBrowser.FreeMemory()와 함께 사용해야 합니다

                        // 업로드 시작
                        StartCoroutine(UploadSpriteCoroutine(contentImage.sprite));
                    }
                    else
                    {
                        contentImage.color = new Color(1, 1, 1, 0);
                    }

                    if (file.IsText())
                    {
                        string content = file.ToStringContent();
                        fileInfoText.text += $"\nFile content: {content.Substring(0, Mathf.Min(30, content.Length))}...";
                    }

                    if (file.IsAudio(AudioType.WAV))
                    {
                        Debug.Log("Its audio. try play it");

                        AudioClip clip = file.ToWavAudioClip();

                        WebGLFileBrowser.RegisterFileObject(clip); // 오디오 클립을 캐시 목록에 추가해서 사용해야 합니다 WebGLFileBrowser.FreeMemory() when its no need anymore

                        // 주의하세요, PlayClipAtPoint는 오디오 클립이 파괴된 경우에도 여전히 재생됩니다. 대신 사용자 지정 오디오 소스를 사용하여 메모리를 더 잘 제어할 수 있습니다
                        AudioSource.PlayClipAtPoint(clip, transform.position);
                    }
                }
            }
        }

        private void FilePopupWasClosedEventHandler()
        {
            if (_loadedFiles == null)
                saveOpenedFileButton.gameObject.SetActive(false);
        }

        private void FileWasSavedEventHandler(File file)
        {
            Debug.Log($"file {file.fileInfo.fullName} was saved");
        }

        private void FileSaveFailedEventHandler(string error)
        {
            Debug.Log(error);
        }

        private void FileOpenFailedEventHandler(string error)
        {
            Debug.Log(error);
        }

        private void FolderOpenFailedEventHandler(string error)
        {
            Debug.Log(error);
        }

        private void FilterOfTypesFieldOnValueChangedHandler(string value)
        {
            _enteredFileExtensions = value;
        }


        // 이미지 업로드
        public IEnumerator UploadSpriteCoroutine(Sprite sprite)
        {
            // 스프라이트의 텍스처를 Texture2D로 캐스팅
            Texture2D texture = sprite.texture;

            // 이미지 크기 조정을 위한 비율 설정 (예: 0.5는 원본 크기의 50%)
            float resizeRatio = 0.5f;
            Texture2D resizedTexture = ResizeTexture(texture, resizeRatio);

            // 텍스처를 PNG 바이트 배열로 변환
            //byte[] imageData = texture.EncodeToPNG();
            byte[] imageData = resizedTexture.EncodeToPNG();

            // WWWForm 객체 생성
            WWWForm form = new WWWForm();
            // "image"라는 이름으로 이미지 데이터 추가
            form.AddBinaryData("image", imageData, "image.png", "image/png");

            // UnityWebRequest 객체 생성 및 설정
            using (UnityWebRequest www = UnityWebRequest.Post("https://xn--4k0b998acvh.xn--yq5b.xn--3e0b707e/unity/uploadImage", form))
            {
                // 요청 전송 및 응답 대기
                yield return www.SendWebRequest();

                // 에러 체크
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Image upload failed: " + www.error);
                }
                else
                {
                    myimage.enabled = true;
                    Debug.Log("Image upload successful: " + www.downloadHandler.text);
                    getPost.text = www.downloadHandler.text;
                    // 이미지 해석 성공시 오디오 받기

                    yield return new WaitForSeconds(2.0f);
                    StartCoroutine(DownloadAndPlay());
                    //StartCoroutine(DownloadAndSaveFile());
                }
            }
        }
        //이미지 크기 조정
        Texture2D ResizeTexture(Texture2D sourceTexture, float ratio)
        {
            int newWidth = Mathf.RoundToInt(sourceTexture.width * ratio);
            int newHeight = Mathf.RoundToInt(sourceTexture.height * ratio);
            Texture2D result = new Texture2D(newWidth, newHeight, sourceTexture.format, false);
            float incX = (1.0f / (float)newWidth);
            float incY = (1.0f / (float)newHeight);
            for (int i = 0; i < result.height; ++i)
            {
                for (int j = 0; j < result.width; ++j)
                {
                    float newX = (float)j / (float)result.width;
                    float newY = (float)i / (float)result.height;
                    result.SetPixel(j, i, sourceTexture.GetPixelBilinear(newX, newY));
                }
            }
            result.Apply();
            return result;
        }


        // 오디오 받기

        IEnumerator DownloadAndPlay()
        {
            string myurl = "https://xn--4k0b998acvh.xn--yq5b.xn--3e0b707e/unity/audio";
            // 서버에서 .wav 파일 다운로드

            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(myurl, UnityEngine.AudioType.WAV))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(www.error);
                    Debug.LogError("파일 다운로드 실패: " + www.error);
                }
                else
                {

                    AudioClip clip = null;
                    clip = DownloadHandlerAudioClip.GetContent(www);
                   
                    
                    if (clip == null)
                    {
                        Debug.LogError("Failed to load AudioClip from downloaded data.");
                        
                    }
                    else
                    {

                        audioSource.clip = clip;
                        audioSource.Play();
                        Debug.Log("AudioClip successfully loaded and playing.");
                    }
                }
            }
        }
        IEnumerator DownloadAndSaveFile()
        {
            string myurl = "https://xn--4k0b998acvh.xn--yq5b.xn--3e0b707e/unity/audio";
            using (UnityWebRequest www = UnityWebRequest.Get(myurl))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("파일 다운로드 실패: " + www.error);
                }
                else
                {
                    // 다운로드한 데이터를 바이트 배열로 가져옴
                    byte[] results = www.downloadHandler.data;


                    // 바이트 배열을 파일로 저장
                    File myfile = new File()
                    {
                        fileInfo = new FileInfo()
                        {
                            fullName = "test.wav"
                        },
                        data = results
                    };
                    AudioClip clip = myfile.ToWavAudioClip();

                    WebGLFileBrowser.RegisterFileObject(clip); // 오디오 클립을 캐시 목록에 추가해서 사용해야 합니다 WebGLFileBrowser.FreeMemory() when its no need anymore

                    // 주의하세요, PlayClipAtPoint는 오디오 클립이 파괴된 경우에도 여전히 재생됩니다. 대신 사용자 지정 오디오 소스를 사용하여 메모리를 더 잘 제어할 수 있습니다
                    AudioSource.PlayClipAtPoint(clip, transform.position);
                    Debug.Log("파일 다운로드 및 저장 성공: ");
                }
            }
        }
    }
}