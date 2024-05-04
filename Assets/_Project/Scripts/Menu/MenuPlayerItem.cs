using TMPro;
using UnityEngine;

public class MenuPlayerItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameTextField;

    public void Init(string playerName)
    {
        playerNameTextField.SetText(playerName);
    }
}
