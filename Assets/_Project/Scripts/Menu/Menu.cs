using System;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Menu : MonoBehaviour
{
    public static event Action<Menu> OnMenuOpen;
    public static event Action<Menu> OnMenuClosed;

    [SerializeField] private GameObject menuRoot;
    [SerializeField] private Button closeButton;

    protected virtual void OnEnable()
    {
        closeButton.onClick.AddListener(Close);
    }

    protected virtual void OnDisable()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(Close);
        }
    }

    public virtual void OpenWithoutNotify()
    {
        if (menuRoot != null)
        {
            menuRoot.SetActive(true);
        }
        gameObject.SetActive(true);
    }

    public virtual void CloseWithoutNotify()
    {
        if (menuRoot != null)
        {
            menuRoot.SetActive(false);
        }
        gameObject.SetActive(false);
    }

    public virtual void Open()
    {
        OpenWithoutNotify();
        OnMenuOpen?.Invoke(this);
    }

    public virtual void Close()
    {
        CloseWithoutNotify();
        OnMenuClosed?.Invoke(this);
    }
}
