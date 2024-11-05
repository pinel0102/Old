using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GalleryModule : MonoBehaviour
{
    [Header("★ [Parameter] Gallery Info")]
    public CollectionType collectionType = CollectionType.JigsawPuzzle;
    public bool imageLoaded = false;
    public GalleryInfo galleryInfo = new GalleryInfo();
    public SnapshotInfo snapshotInfo = new SnapshotInfo();    

    [Header("★ [Parameter] For Test")]
    public CollectionType startType = CollectionType.JigsawPuzzle;
    [Tooltip("Load contents on Start() when bigger than 0")]
    public int startIndex;

    [Header("★ [Parameter] Reference : Image")]
    public GameObject imageModuleObject;
    public Image m_Image;
    
    [Header("★ [Parameter] Privates")]
    private bool _initialized = false;
    private WaitForSecondsRealtime waitDelay = new WaitForSecondsRealtime(0.5f);
    private GalleryManager galleryManager { get { return GalleryManager.Instance; } }
    private ServerClient serverClient { get { return ServerClient.Instance; } }
    private GameManager gameManager { get { return GameManager.Instance; } }
    
    private IEnumerator Start()
    {
        if (startIndex > 0)
        {
            while(!serverClient.isInitialized)
            {
                yield return waitDelay;
            }
            
            Initialize(startType, startIndex);
        }
    }

    private void OnEnable()
    {
        if (_initialized && !imageLoaded)
        {
            // Reload Image
            switch(collectionType)
            { 
                case CollectionType.JigsawPuzzle:
                    LoadImage(collectionType, galleryInfo.index, true);
                    break;

                case CollectionType.PixelArt:
                case CollectionType.Bookmark:
                    LoadImage(collectionType, snapshotInfo.level, true);
                    break;
            }
        }
    }

    public void Initialize(CollectionType type, int index = 0)
    {
        LoadImage(type, index);
    }

    private void LoadImage(CollectionType type, int index, bool reload = false)
    {
        if (index > 0)
        {
            collectionType = type;

            switch(collectionType)
            { 
                case CollectionType.JigsawPuzzle:

                    if (galleryInfo.index != index || reload)
                    {
                        if (reload)
                        {
                            Debug.Log(CodeManager.GetMethodName() + string.Format("Reload Image : {0}_{1}", collectionType, galleryInfo.index));
                        }
                        
                        serverClient.LoadContents(index, this, (OnComplete) =>
                        {
                            if (OnComplete)
                            {
                                ShowImage();
                            }
                            else
                            {
                                HideImage();
                            }
                        });
                    }
                    break;
                
                case CollectionType.PixelArt:
                case CollectionType.Bookmark:

                    if (snapshotInfo.level != index || reload)
                    {
                        if (reload)
                        {
                            Debug.Log(CodeManager.GetMethodName() + string.Format("Reload Image : {0}_{1}", collectionType, snapshotInfo.level));
                        }
                        
                        serverClient.LoadSnapshot(index, this, (OnComplete) =>
                        {
                            if (OnComplete)
                            {
                                ShowImage();
                            }
                            else
                            {
                                HideImage();
                            }
                        });
                    }

                    break;
            }            
        }        
    }

    public void ShowImage()
    {
        imageModuleObject.SetActive(true);
        imageLoaded = true;
        _initialized = true;
    }

    public void HideImage()
    {
        imageModuleObject.SetActive(false);
        imageLoaded = false;
        _initialized = true;
    }

    public void OnClick_SelectModule()
    {
        //Debug.Log(CodeManager.GetMethodName() + galleryInfo.name);
        switch(collectionType)
        { 
            case CollectionType.JigsawPuzzle:
        
                galleryManager.ChangeGallery(this);
                break;

            case CollectionType.PixelArt:
            case CollectionType.Bookmark:

                galleryManager.ChangeSnapshot(this);
                break;
        }

        gameManager.uiManager.currentWindow = string.Empty;
        gameManager.uiManager.OpenWindow(GlobalDefine.window_collection_gallery);
        gameManager.audioManager.PopupSoundPlay();
    }
}
