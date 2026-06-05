using UnityEngine;

public class TestScript : MonoBehaviour
{
    public void OnClick()
    {
        GameManager.Run(GameManager.Post(new System.Uri("https://localhost:7037/Login"), OnResult, null, new DTO2<string, string>("Name", "Robot"), new DTO2<string, string>("Password", "robotrobot")), "Post");
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

/*

Sigo trabajando para mostrar el tablero con las unidades.


1. Crear una cuenta. (User)
2. Crear unidades base para la cuenta.
3. Configurar tablero de distribución.
 


La cuenta está enlazada a una facción? El la elije.

Las configuraciones basicas se guardan en servidor? Se guarda en local.

A la hora de la batalla quien decide que terreno se ve? Cada quien por separado? Arriba jugador el local quien tiene el terreno los dos ven el mismo terreno.

El terreno se guarda en servidor como configuración.

Todo se queda en c#

*/