using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CustomToggle : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent<bool> onValueChanged;

    [SerializeField] private GameObject fill;
    [SerializeField] private bool isOn;

    private void Start()
    {
        fill.SetActive(isOn);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        isOn = !isOn;
        fill.SetActive(isOn);
        onValueChanged?.Invoke(isOn);
    }
}
