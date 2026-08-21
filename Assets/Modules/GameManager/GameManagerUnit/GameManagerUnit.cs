using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    /// <summary>
    /// Construye la unidad con base en los datos de configuración visual de la misma y la da al callback.
    /// </summary>
    /// <param name="lnUnitStoredPropertiesId"></param>
    /// <returns></returns>
    private UnitInstance CreateUnitInstance(UnitProperties loUnitProperties, IEnumerable<UnitVisualElement> loUnitVisualElements = null)
    {
        //<< Obtenemos solo las partes visuales de nuestra unidad.
        if (loUnitVisualElements == null) loUnitVisualElements = loUnitProperties.UnitVisualElements;

        UnitAssetBundle loUnitAssetBundle = null;

        //<< Obtenemos primero el cuerpo de la unidad.
        UnitAssetBundle loUnitAssetBundleBody = null;
        foreach (UnitVisualElement loItem in loUnitVisualElements)
        {
            if(!goUnitAssetBundles.TryGetValue(loItem.UnitAssetBundleId, out loUnitAssetBundle)) continue;
            if (loUnitAssetBundleBody.Type != UnitAssetBundleType.Body) continue;
            loUnitAssetBundleBody = loUnitAssetBundle;
            break;
        }

        //<< Ya con el cuerpo creamos una instancia usando el prefab.
        UnitInstance loUnitInstance = Spawn(loUnitAssetBundleBody.unitInstance.GetHashCode(), loUnitAssetBundleBody.unitInstance);

        //<< Para cada elemento que no sea cuerpo...
        UnitElement loUnitElement;
        foreach (UnitVisualElement loItem in loUnitVisualElements)
        {
            if (!goUnitAssetBundles.TryGetValue(loItem.UnitAssetBundleId, out loUnitAssetBundle)) continue;
            if (loUnitAssetBundle.Type == UnitAssetBundleType.Body) continue;

            //<< Creamos instancia del elemento y lo agregamos a nuestra lista de elementos visuales.
            loUnitElement = Spawn(loUnitAssetBundle.unitElement.GetHashCode(), loUnitAssetBundle.unitElement);
            loUnitInstance.UnitElements.Add(loUnitElement);

            //<< Asignamos su padre con base en el tipo de elemento y lado de la unidad.
            Transform loParent = loUnitInstance.transform;
            if (!string.IsNullOrWhiteSpace(loUnitAssetBundle.AttachTransformName))
            {
                Transform loFound = loUnitInstance.transform.FindNearest(loUnitAssetBundle.AttachTransformName);
                if (loFound != null) loParent = loFound;
            }
            loUnitElement.transform.SetParent(loParent, true);

            loUnitElement.transform.localPosition = Vector3.zero;
            loUnitElement.transform.localRotation = Quaternion.identity;
        }

        //<< Establecemos el nombre como el id de la unidad.
        loUnitInstance.name = loUnitProperties.Id.ToString();

        //<< Almacenamos referencias para reciclar al limpiar el mapa.
        loUnitInstance.UnitProperties = loUnitProperties;

        //<< Establecemos el layer default a nuestra unidad.
        loUnitInstance.SetLayer(Layer.Unit);

        //<< Le indicamos a la instancia que debe tener un idle.
        if (loUnitInstance.animator != null)
        {
            loUnitInstance.animator.SetFloats(0.0f);
            string lcRole = loUnitProperties.UnitBattleProperties.Role.ToString();
            if (loUnitInstance.animator.HasParameter(lcRole)) loUnitInstance.animator.SetFloat(lcRole, 1.0f);
            else
            {
                switch (loUnitProperties.UnitBattleProperties.Role)
                {
                    case UnitRole.Mage:
                    case UnitRole.Magician:
                    case UnitRole.Sorcerer:
                    case UnitRole.Necromancer:
                        lcRole = UnitRole.Mage.ToString();
                        break;

                    case UnitRole.Archer:
                    case UnitRole.Hunter:
                    case UnitRole.Sniper:
                    case UnitRole.Tracker:
                        lcRole = UnitRole.Archer.ToString();
                        break;

                    case UnitRole.Warrior:
                    case UnitRole.Knight:
                    case UnitRole.Barbarian:
                    case UnitRole.Paladin:
                        lcRole = UnitRole.Warrior.ToString();
                        break;

                    case UnitRole.Healer:
                    case UnitRole.Cleric:
                    case UnitRole.Bard:
                    case UnitRole.Alchemist:
                        lcRole = UnitRole.Healer.ToString();
                        break;

                    case UnitRole.Rogue:
                    case UnitRole.Thief:
                    case UnitRole.Assasin:
                    case UnitRole.Jester:
                        lcRole = UnitRole.Rogue.ToString();
                        break;

                    case UnitRole.Druid:
                    case UnitRole.Pyromaniac:
                    case UnitRole.Hydrosophist:
                    case UnitRole.Geomancer:
                        lcRole = UnitRole.Druid.ToString();
                        break;

                    default: break;
                }

                if (loUnitInstance.animator.HasParameter(lcRole)) loUnitInstance.animator.SetFloat(lcRole, 1.0f);
                else
                {
                    lcRole = UnitRole.Any.ToString();
                    loUnitInstance.animator.SetFloat(lcRole, 1.0f);
                }
            }
        }

        //<< Regresamos la instancia para tener control sobre la misma.
        return loUnitInstance;
    }
}
