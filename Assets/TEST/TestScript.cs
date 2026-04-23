using UnityEngine;

public class TestScript : MonoBehaviour
{
    public void OnClick()
    {
        GameManager.Run(GameManager.Post("https://localhost:7037/Login", OnResult, null, new DTO2<string, string>("Name", "Robot"), new DTO2<string, string>("Password", "robotrobot")), "Post");
    }

    private void OnResult(bool lbResult, string lcResult)
    {
        Debug.Log($"Result: {lbResult} String: {lcResult}");

        if (!lbResult) return;

        GameManager.Token = lcResult;

        GameManager.Run(GameManager.Post("https://localhost:7037/Profile", OnResultProfile), "Post");
    }

    private void OnResultProfile(bool lbResult, string lcResult)
    {
        Debug.Log($"ResultProfile: {lbResult} String: {lcResult}");
    }
}
