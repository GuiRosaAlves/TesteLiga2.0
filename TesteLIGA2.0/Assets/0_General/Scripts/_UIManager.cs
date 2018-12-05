using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

public class _UIManager : MonoBehaviour
{
    [SerializeField] private Canvas _pauseCanvas;
    [SerializeField] private StandaloneInputModule _inputModule;
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private GameObject _pauseMenuFirstSelection;
    [SerializeField] public Image[] _dynamicUIColor;
    
    public EventSystem EvtSys
    {
        get { return _eventSystem; }
    }
    
    public event Action OnUpdateUI;
    
    private void Awake()
    {
        GameObject.DontDestroyOnLoad(_pauseCanvas.gameObject);
        _pauseCanvas.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (OnUpdateUI != null) 
            OnUpdateUI();
    }
}