using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class GoogleDriveImageImporter : MonoBehaviour
{
    public string folderId = "YOUR_FOLDER_ID";
    public string apiKey = "YOUR_API_KEY";
    public List<Texture2D> importedTextures = new List<Texture2D>();

    public SlideshowController slideshowController; // Reference to the SlideshowController

    [System.Obsolete]
    void Start()
    {
        StartCoroutine(ImportImagesFromDrive());
    }

    public void UploadImages()
    {
        StartCoroutine(ImportImagesFromDrive());
    }

    [System.Obsolete]
    IEnumerator ImportImagesFromDrive()
    {
        string url = $"https://www.googleapis.com/drive/v3/files?q='{folderId}'+in+parents&fields=files(id,name,mimeType)&key={apiKey}";

        Debug.Log($"Google Drive API URL: {url}");

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = www.downloadHandler.text;
                Debug.Log($"Google Drive API Response: {jsonResponse}");

                GoogleDriveFileList fileList = JsonUtility.FromJson<GoogleDriveFileList>(jsonResponse);
                Debug.Log($"Number of files in fileList: {fileList.files.Count}");

                foreach (var file in fileList.files)
                {
                    Debug.Log($"File Name: {file.name}, MIME Type: {file.mimeType}");

                    if (file.mimeType.StartsWith("image/"))
                    {
                        string imageUrl = $"https://drive.google.com/uc?id={file.id}&export=download";
                        Debug.Log($"Downloading image: {imageUrl}");

                        using (UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(imageUrl))
                        {
                            yield return imageRequest.SendWebRequest();

                            if (!imageRequest.isNetworkError && !imageRequest.isHttpError)
                            {
                                Texture2D texture = DownloadHandlerTexture.GetContent(imageRequest);
                                importedTextures.Add(texture);
                                Debug.Log($"Image downloaded successfully: {file.name}");
                            }
                            else
                            {
                                Debug.LogError($"Failed to download image: {imageUrl}. Error: {imageRequest.error}");
                            }
                        }
                    }
                }

                if (slideshowController != null)
                {
                    slideshowController.SetSlideshowImages(importedTextures.ToArray());
                }
            }
            else
            {
                Debug.LogError($"Failed to fetch files from Google Drive. Error: {www.error}");
            }
        }
    }
}


[System.Serializable]
public class GoogleDriveFileList
{
    public List<GoogleDriveFile> files;
}

[System.Serializable]
public class GoogleDriveFile
{
    public string id;
    public string name;
    public string mimeType;
}
