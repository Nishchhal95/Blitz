using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RegionMenuItem : MonoBehaviour, IPointerClickHandler
{
    public event Action<string> RegionSelect;
    [SerializeField] private TextMeshProUGUI regionNameTextField;
    [field: SerializeField] public string RegionCode { get; private set; }

    public void SetData(string regionCode, string regionName)
    {
        RegionCode = regionCode;
        regionNameTextField.SetText(regionName);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        RegionSelect?.Invoke(RegionCode);
    }
}
