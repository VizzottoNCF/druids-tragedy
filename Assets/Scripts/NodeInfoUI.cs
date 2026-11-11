using TMPro;
using UnityEngine;

public class NodeInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject nodeInfoPanel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;
    public void ShowNodeInfo(string nodeName, string nodeDescription)
    {
        nodeInfoPanel.SetActive(true);
        nameText.text = nodeName;
        descriptionText.text = nodeDescription;
    }
    public void HideNodeInfo()
    {
        nodeInfoPanel.SetActive(false);
    }
}
