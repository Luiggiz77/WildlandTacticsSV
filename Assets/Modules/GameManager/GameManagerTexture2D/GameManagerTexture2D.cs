using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public partial class GameManager : MonoBehaviour
{
    /// <summary>
    /// Lista de texturas del juego. Nota: Deben ser descargadas previamente del servidor.
    /// </summary>
    private List<GameTexture2D> goGameTextures2D = new List<GameTexture2D>();

    /// <summary>
    /// Se llama en el Awake del GameManager
    /// </summary>
    void AwakeTexture2D()
    {
        //<< Nos conectamos a los comandos.
        AddListener(OnGameCommandTexture2D);
    }

    /// <summary>
    /// Para recepcion de comandos del juego.
    /// </summary>
    /// <param name="loCommand"></param>
    /// <param name="loParams"></param>
    private void OnGameCommandTexture2D(GameCommand loCommand, params object[] loParams)
    {
        switch (loCommand)
        {
            case GameCommand.GetTexture2D: GetTexture2D((UnityAction<Texture2D>)loParams[0], (GameTexture2DUsage)loParams[1], (int)loParams[2]); break;
            case GameCommand.GetTexture2DColor: GetTextureColor((UnityAction<Color32>)loParams[0], (GameTexture2DUsage)loParams[1], (int)loParams[2]); break;
            default: break;
        }
    }

    /// <summary>
    /// Se llama para dar el icono requerido.
    /// </summary>
    /// <param name="lnUnitStoredPropertiesId"></param>
    private void GetTexture2D(UnityAction<Texture2D> loCallback, GameTexture2DUsage loGameTexture2DUsage, int lnKey, bool lbSecondSearch = false)
    {
        GameTexture2D loGameTexture2D = null;

        //<< Buscamos la textura que contenga las dos llaves.
        foreach (GameTexture2D loItem in goGameTextures2D)
        {
            if (loItem.Usage != loGameTexture2DUsage) continue;
            if (loItem.Key != lnKey) continue;
            loGameTexture2D = loItem;
            break;
        }

        //<< Revisamos si encontramos el elemento o no.
        if (loGameTexture2D != null) { loCallback.Invoke(loGameTexture2D.Texture); return; }

        //<< Si no es por rol o es la segunda busqueda mandamos null.
        if (lbSecondSearch || loGameTexture2DUsage != GameTexture2DUsage.UnitRoleIcon) { loCallback.Invoke(null); return; }

        //<< Ya que no se encontró la textura buscamos el homlogo si es por rol.
        UnitRole loUnitRole = (UnitRole)lnKey;
        switch (loUnitRole)
        {
            case UnitRole.Mage:
            case UnitRole.Magician:
            case UnitRole.Sorcerer:
            case UnitRole.Necromancer:
                lnKey = (int)UnitRole.Mage;
                break;
            case UnitRole.Archer:
            case UnitRole.Hunter:
            case UnitRole.Sniper:
            case UnitRole.Tracker:
                lnKey = (int)UnitRole.Archer;
                break;
            case UnitRole.Warrior:
            case UnitRole.Knight:
            case UnitRole.Barbarian:
            case UnitRole.Paladin:
                lnKey = (int)UnitRole.Warrior;
                break;
            case UnitRole.Healer:
            case UnitRole.Cleric:
            case UnitRole.Bard:
            case UnitRole.Alchemist:
                lnKey = (int)UnitRole.Healer;
                break;
            case UnitRole.Rogue:
            case UnitRole.Thief:
            case UnitRole.Assasin:
            case UnitRole.Jester:
                lnKey = (int)UnitRole.Rogue;
                break;
            case UnitRole.Druid:
            case UnitRole.Pyromaniac:
            case UnitRole.Hydrosophist:
            case UnitRole.Geomancer:
                lnKey = (int)UnitRole.Druid;
                break;
            default: loCallback.Invoke(null); return;
        }

        GetTexture2D(loCallback, loGameTexture2DUsage, lnKey, true);
    }

    private void GetTextureColor(UnityAction<Color32> loCallback, GameTexture2DUsage loGameTexture2DUsage, int lnKey, bool lbSecondSearch = false)
    {
        GameTexture2D loGameTexture2D = null;

        //<< Buscamos la textura que contenga las dos llaves.
        foreach (GameTexture2D loItem in goGameTextures2D)
        {
            if (loItem.Usage != loGameTexture2DUsage) continue;
            if (loItem.Key != lnKey) continue;
            loGameTexture2D = loItem;
            break;
        }

        //<< Revisamos si encontramos el elemento o no.
        if (loGameTexture2D != null) { loCallback.Invoke(loGameTexture2D.Color); return; }

        //<< Si no es por rol o es la segunda busqueda mandamos null.
        if (lbSecondSearch || loGameTexture2DUsage != GameTexture2DUsage.UnitRoleIcon) { loCallback.Invoke(Color.black); return; }

        //<< Ya que no se encontró la textura buscamos el homlogo si es por rol.
        UnitRole loUnitRole = (UnitRole)lnKey;
        switch (loUnitRole)
        {
            case UnitRole.Mage:
            case UnitRole.Magician:
            case UnitRole.Sorcerer:
            case UnitRole.Necromancer:
                lnKey = (int)UnitRole.Mage;
                break;
            case UnitRole.Archer:
            case UnitRole.Hunter:
            case UnitRole.Sniper:
            case UnitRole.Tracker:
                lnKey = (int)UnitRole.Archer;
                break;
            case UnitRole.Warrior:
            case UnitRole.Knight:
            case UnitRole.Barbarian:
            case UnitRole.Paladin:
                lnKey = (int)UnitRole.Warrior;
                break;
            case UnitRole.Healer:
            case UnitRole.Cleric:
            case UnitRole.Bard:
            case UnitRole.Alchemist:
                lnKey = (int)UnitRole.Healer;
                break;
            case UnitRole.Rogue:
            case UnitRole.Thief:
            case UnitRole.Assasin:
            case UnitRole.Jester:
                lnKey = (int)UnitRole.Rogue;
                break;
            case UnitRole.Druid:
            case UnitRole.Pyromaniac:
            case UnitRole.Hydrosophist:
            case UnitRole.Geomancer:
                lnKey = (int)UnitRole.Druid;
                break;
            default: loCallback.Invoke(Color.black); return;
        }

        GetTextureColor(loCallback, loGameTexture2DUsage, lnKey, true);
    }
}
