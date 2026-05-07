using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GameTexture2D))]
public class GameTexture2DDrawer : PropertyDrawer
{
    /// <summary>
    /// OnGUI
    /// </summary>
    /// <param name="loPosition"></param>
    /// <param name="loSerizalizedProperty"></param>
    /// <param name="loLabel"></param>
    public override void OnGUI(Rect loPosition, SerializedProperty loSerizalizedProperty, GUIContent loLabel)
    {
        EditorGUI.BeginProperty(loPosition, loLabel, loSerizalizedProperty);

        //<< Dibujamos foldout
        loPosition.height = EditorGUIUtility.singleLineHeight;
        loSerizalizedProperty.isExpanded = EditorGUI.Foldout(loPosition, loSerizalizedProperty.isExpanded, loLabel, true);
        if (!loSerizalizedProperty.isExpanded)
        {
            EditorGUI.EndProperty();
            return;
        }

        EditorGUI.indentLevel++;

        float lnY = loPosition.y + EditorGUIUtility.singleLineHeight + 2;
        float lnLineHeight = EditorGUIUtility.singleLineHeight;
        float lnSpacing = 2f;

        // Obtener propiedades
        SerializedProperty loNameProp = loSerizalizedProperty.FindPropertyRelative("Name");
        SerializedProperty loIdProp = loSerizalizedProperty.FindPropertyRelative("GameTexture2DId");
        SerializedProperty loUsageProp = loSerizalizedProperty.FindPropertyRelative("Usage");
        SerializedProperty loKeyProp = loSerizalizedProperty.FindPropertyRelative("Key");
        SerializedProperty loTextureProp = loSerizalizedProperty.FindPropertyRelative("Texture");
        SerializedProperty loColorProp = loSerizalizedProperty.FindPropertyRelative("Color");

        Rect loFieldRect = new Rect(loPosition.x, lnY, loPosition.width, lnLineHeight);

        EditorGUI.PropertyField(loFieldRect, loNameProp);
        lnY += lnLineHeight + lnSpacing;

        loFieldRect.y = lnY;
        EditorGUI.PropertyField(loFieldRect, loIdProp);
        lnY += lnLineHeight + lnSpacing;

        loFieldRect.y = lnY;
        EditorGUI.PropertyField(loFieldRect, loUsageProp);
        lnY += lnLineHeight + lnSpacing;

        //<< Lógica especial del Key
        loFieldRect.y = lnY;
        DrawKeyProperty(loFieldRect, loUsageProp, loKeyProp);
        lnY += lnLineHeight + lnSpacing;

        loFieldRect.y = lnY;
        EditorGUI.PropertyField(loFieldRect, loTextureProp);
        lnY += lnLineHeight + lnSpacing;

        loFieldRect.y = lnY;
        EditorGUI.PropertyField(loFieldRect, loColorProp);

        EditorGUI.indentLevel--;
        EditorGUI.EndProperty();
    }

    /// <summary>
    /// Para dibujar con base en el tipo de uso.
    /// </summary>
    /// <param name="loPosition"></param>
    /// <param name="loUsageProperty"></param>
    /// <param name="loKeyProperty"></param>
    void DrawKeyProperty(Rect loPosition, SerializedProperty loUsageProperty, SerializedProperty loKeyProperty)
    {
        GameTexture2DUsage usage = (GameTexture2DUsage)loUsageProperty.enumValueIndex;

        EditorGUI.BeginChangeCheck();

        switch (usage)
        {
            case GameTexture2DUsage.UnitRoleIcon:
                UnitRole unitRole = (UnitRole)loKeyProperty.intValue;
                unitRole = (UnitRole)EditorGUI.EnumPopup(loPosition, "Key", unitRole);
                loKeyProperty.intValue = (int)unitRole;
                break;

            case GameTexture2DUsage.PurchasableCategoryIcon:
                PurchasableCategory category = (PurchasableCategory)loKeyProperty.intValue;
                category = (PurchasableCategory)EditorGUI.EnumPopup(loPosition, "Key", category);
                loKeyProperty.intValue = (int)category;
                break;

            case GameTexture2DUsage.UnitFactionIcon:
                UnitFaction loUnitFaction = (UnitFaction)loKeyProperty.intValue;
                loUnitFaction = (UnitFaction)EditorGUI.EnumPopup(loPosition, "Key", loUnitFaction);
                loKeyProperty.intValue = (int)loUnitFaction;
                break;

            default: EditorGUI.PropertyField(loPosition, loKeyProperty); break;
        }

        if (EditorGUI.EndChangeCheck()) loKeyProperty.serializedObject.ApplyModifiedProperties();
    }

    /// <summary>
    /// Para sacar las alturas.
    /// </summary>
    /// <param name="loSerializedProperty"></param>
    /// <param name="loLabel"></param>
    /// <returns></returns>
    public override float GetPropertyHeight(SerializedProperty loSerializedProperty, GUIContent loLabel)
    {
        if (!loSerializedProperty.isExpanded) return EditorGUIUtility.singleLineHeight;
        int lnLines = 6; //<< Name, Id, Usage, Key, Texture, Color
        float lnLineHeight = EditorGUIUtility.singleLineHeight;
        float lnSpacing = 2f;
        return (lnLineHeight + lnSpacing) * lnLines + lnLineHeight;
    }
}