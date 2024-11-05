using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;

public class FileDownloader : SingletonMono<FileDownloader>
{
    public bool isDownloading { get {return _isDownloading; } }
    public float Progress { get {return GetProgress(); } }
    public List<GalleryInfo> downloadQueue = new List<GalleryInfo>();

    [Header("★ [Parameter] Privates")]
    private bool _isDownloading = false;
    private bool isBusy = false;
    private int downloadedCount = 0;
    private int queueCount = 0;
    private int skipCount = 0;
    private string serverRoot;
    private string localRoot;
    private const float queueDelay = 0.5f;
    private const float requestDelay = 0.1f;
    private WaitForSecondsRealtime qDelay = new WaitForSecondsRealtime(queueDelay);
    private WaitForSecondsRealtime rDelay = new WaitForSecondsRealtime(requestDelay);
    private Coroutine downloadCoroutine;    

    public void Initialize(string serverURL, string rootPath)
    {
        serverRoot = serverURL;
        localRoot = rootPath;
        
        downloadQueue.Clear();
        queueCount = 0;
        downloadedCount = 0;
        skipCount = 0;
        _isDownloading = false;
    }

    public void DownloadContents(List<GalleryInfo> downloadList, bool downloadOverride = false)
    {
        if (downloadList.Count > 0)
        {
            if (isBusy && downloadCoroutine != null)
            {
                //Debug.Log(CodeManager.GetMethodName() + "Stop Previous Download Coroutine");
                StopCoroutine(downloadCoroutine);
            }

            downloadCoroutine = StartCoroutine(Co_DownloadFiles(downloadList, downloadOverride));
        }    
    }

    private IEnumerator Co_DownloadFiles(List<GalleryInfo> addList, bool downloadOverride = false)
    {
        isBusy = true;

        for (int i=0; i < addList.Count; i++)
        {
            if(!downloadQueue.Contains(addList[i]))
            {
                downloadQueue.Add(addList[i]);
                queueCount++;
            }
        }

        yield return qDelay;
        
        for(int i=downloadQueue.Count-1; i >= 0; i--)
        {
            yield return rDelay;

            if (!ExistFile(downloadQueue[i], downloadOverride))
            {
                DownloadFile(downloadQueue[i]);
            }
            else if (downloadQueue[i].video && !ExistFile(downloadQueue[i], downloadOverride))
            {
                DownloadFile(downloadQueue[i]);
            }
            else
            {
                skipCount++;
                DownloadComplete(downloadQueue[i], downloadQueue[i].video ? ServerDefine.VideoFileFormat : ServerDefine.ImageFileFormat, downloadedCount, queueCount, true);
            }
            downloadQueue.RemoveAt(i);
        }

        isBusy = false;
    }

    private void DownloadFile(GalleryInfo gallery)
    {
        _isDownloading = true;

        string downloadFolder = Path.Combine(serverRoot, gallery.theme);
        string downloadUrl = Path.Combine(downloadFolder, string.Format(ServerDefine.ImageFileFormat, gallery.name));
        string saveFolder = Path.Combine(localRoot, ServerDefine.LocalRootFolder, gallery.theme);
        string savePath = Path.Combine(saveFolder, string.Format(ServerDefine.ImageFileFormat, gallery.name));

        Debug.Log(CodeManager.GetMethodName() + string.Format("<color=#EC46EB>[Download] server/{0}/{1} ({2}/{3} : {4:0.0}%)</color>", gallery.theme, string.Format(ServerDefine.ImageFileFormat, gallery.name), downloadedCount, queueCount, Progress));

        StartCoroutine(SendRequestTexture(downloadUrl, (UnityWebRequest req) =>
        {
            if (req.result == UnityWebRequest.Result.Success)
            {
                if (!Directory.Exists(saveFolder))
                    Directory.CreateDirectory(saveFolder);
                
                File.WriteAllBytes(savePath, req.downloadHandler.data);
            }
            else
            {
                Debug.LogWarning(CodeManager.GetMethodName() + $"{req.error} : {req.downloadHandler.text}");
            }

            if (!gallery.video)
                downloadedCount++;
            
            DownloadComplete(gallery, ServerDefine.ImageFileFormat, downloadedCount, queueCount);
        }));

        if (gallery.video)
        {
            string videoDownloadUrl = Path.Combine(downloadFolder, string.Format(ServerDefine.VideoFileFormat, gallery.name));
            string videoSavePath = Path.Combine(saveFolder, string.Format(ServerDefine.VideoFileFormat, gallery.name));

            Debug.Log(CodeManager.GetMethodName() + string.Format("<color=#EC46EB>[Download] server/{0}/{1} ({2}/{3} : {4:0.0}%)</color>", gallery.theme, string.Format(ServerDefine.VideoFileFormat, gallery.name), downloadedCount, queueCount, Progress));

            StartCoroutine(SendRequestVideo(videoDownloadUrl, (UnityWebRequest req) =>
            {
                if (req.result == UnityWebRequest.Result.Success)
                {
                    if (!Directory.Exists(saveFolder))
                        Directory.CreateDirectory(saveFolder);
                    
                    File.WriteAllBytes(videoSavePath, req.downloadHandler.data);
                }
                else
                {
                    Debug.LogWarning(CodeManager.GetMethodName() + $"{req.error} : {req.downloadHandler.text}");
                }

                downloadedCount++;
                DownloadComplete(gallery, ServerDefine.VideoFileFormat, downloadedCount, queueCount);
            }));
        }
    }

    private void DownloadComplete(GalleryInfo gallery, string fileNameFormat, int currentCount, int allCount, bool skip = false)
    {
        if (!skip)
            Debug.Log(CodeManager.GetMethodName() + string.Format("<color=#FFFF00>[Complete] {0}/{1} ({2}/{3} : {4:0.0}%)</color>", gallery.theme, string.Format(fileNameFormat, gallery.name), currentCount, allCount, Progress));

        if (gallery.Exist() && currentCount == allCount)
        {
            DownloadAllComplete();
        }
    }

    private void DownloadAllComplete()
    {
        Debug.Log(CodeManager.GetMethodName() + string.Format("<color=#FFFF00>[Complete] {0}/{1} : {2:0.0}%</color>", downloadedCount, queueCount, Progress));

        downloadQueue.Clear();
        queueCount = 0;
        downloadedCount = 0;
        skipCount = 0;
        _isDownloading = false;
    }

    private float GetProgress()
    {
        if (queueCount == 0)
            return 100f;
        
        return ((float)downloadedCount/queueCount) * 100f;
    }

    private bool ExistFile(GalleryInfo gallery, bool downloadOverride = false)
    {
        if (downloadOverride)
            return false;
        
        return gallery.Exist();
    }

    private IEnumerator SendRequestTexture(string url, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest req = UnityWebRequestTexture.GetTexture(url))
        {
            yield return req.SendWebRequest();
            callback(req);
        }
    }

    private IEnumerator SendRequestVideo(string url, Action<UnityWebRequest> callback)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();
            callback(req);
        }
    }
}
