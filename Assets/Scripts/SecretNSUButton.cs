using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SecretNSUButton : MonoBehaviour, IPointerClickHandler
{
    [Header("Nazwa sceny do wczytania")]
    public string sceneName = "NSU_Presentation";   // zmieñ na swoj¹ prawdziw¹ nazwê

    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene(sceneName);
    }
}
