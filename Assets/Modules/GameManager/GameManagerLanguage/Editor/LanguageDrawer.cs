using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(Language))]
public class LanguageDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        // Create property container element.
        VisualElement loContainer = new VisualElement();

        // Create property fields.
        var loVersion = new PropertyField(property.FindPropertyRelative("V"), "Version");
        var loLanguageTexts = new PropertyField(property.FindPropertyRelative("LT"), "Language Texts");

        // Add fields to the container.
        loContainer.Add(loVersion);
        loContainer.Add(loLanguageTexts);

        return loContainer;
    }
}