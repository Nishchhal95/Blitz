using System.Collections;
using TMPro;
using UnityEngine;

public class InGameMessageController : MonoBehaviour
{
    [SerializeField] private GameObject messageItem;
    [SerializeField] private TextMeshProUGUI messageTextField;

    [SerializeField] private Coroutine hideMessageItemRoutine;

    public static InGameMessageController Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        messageItem.SetActive(false);
        messageTextField.SetText("");
    }

    public void ShowMessage(string message, float durationInSeconds = 3f)
    {
        messageTextField.SetText(message);
        messageItem.SetActive(true);

        if(hideMessageItemRoutine != null)
        {
            StopCoroutine(hideMessageItemRoutine);
        }
        hideMessageItemRoutine = StartCoroutine(HideMessageItemAfterDelay(durationInSeconds));
    }

    private IEnumerator HideMessageItemAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        messageItem.SetActive(false);
        messageTextField.SetText("");
    }
}
