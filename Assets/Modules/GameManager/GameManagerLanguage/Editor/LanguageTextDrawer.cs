using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LanguageText))]
public class LanguageTextDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        //<< Dividir rectángulos
        Rect loURect = new Rect(position.x, position.y, position.width * 0.3f, position.height);
        Rect loKRect = new Rect(position.x + position.width * 0.3f + 5, position.y, position.width * 0.3f, position.height);
        Rect loTRect = new Rect(position.x + position.width * 0.6f + 10, position.y, position.width * 0.4f - 10, position.height);

        //<< Propiedades serializadas
        SerializedProperty loUProp = property.FindPropertyRelative("U");
        SerializedProperty loKProp = property.FindPropertyRelative("K");
        SerializedProperty loTProp = property.FindPropertyRelative("T");

        GUI.enabled = false; //<< Desactiva edición

        GameTextUsage loGameTextUsage = (GameTextUsage)loUProp.intValue;

        //<< Dibujar U como GameTextUsage
        loUProp.intValue = (int)(GameTextUsage)EditorGUI.EnumPopup(loURect, loGameTextUsage);

        loGameTextUsage = (GameTextUsage)loUProp.intValue;

        switch (loGameTextUsage)
        {
            case GameTextUsage.Word:
                loKProp.intValue = (int)(GameTextWord)EditorGUI.EnumPopup(loKRect, (GameTextWord)loKProp.intValue);
                break;
            case GameTextUsage.Sentence:
                loKProp.intValue = (int)(GameTextSentence)EditorGUI.EnumPopup(loKRect, (GameTextSentence)loKProp.intValue);
                break;
            case GameTextUsage.UnitRole:
                loKProp.intValue = (int)(UnitRole)EditorGUI.EnumPopup(loKRect, (UnitRole)loKProp.intValue);
                break;
            case GameTextUsage.UnitDescription:
            case GameTextUsage.UnitAttackName:
            case GameTextUsage.UnitAttackDescription:
            case GameTextUsage.UnitMovementDescription:
            case GameTextUsage.GameplayItemName:
            case GameTextUsage.GameplayItemDescription:
                loKProp.intValue = (int)EditorGUI.IntField(loKRect, loKProp.intValue);
                break;
            default: break;
        }

        GUI.enabled = true; //<< Activa edición

        //<< Dibujar T (string normal)
        EditorGUI.PropertyField(loTRect, loTProp, GUIContent.none);

        EditorGUI.EndProperty();
    }
}