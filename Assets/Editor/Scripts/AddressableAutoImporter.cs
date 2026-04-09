using UnityEditor;
using UnityEngine;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using System.IO;
using System;

public class AddressableAutoImporter : EditorWindow
{
    /// <summary>
    /// Es la ruta donde se establecen nuestros adressables.
    /// </summary>
    private const string gcRootPath = "Assets/Editor/Addressables";

    [MenuItem("GameManager/Update Addressables")]
    public static void UpdateAdressables()
    {
        if (!Directory.Exists(gcRootPath))
        {
            Debug.LogError($"La ruta {gcRootPath} no existe.");
            return;
        }

        AddressableAssetSettings loSettings = AddressableAssetSettingsDefaultObject.Settings;
        if (loSettings == null)
        {
            Debug.LogError("No se encontró la configuración de Addressables. Ve a Window > Asset Management > Addressables > Settings.");
            return;
        }

        //<< 1. Obtenemos las carpetas que serán Grupos (Heads, Bodies, etc...)
        string[] loGroupFolders = Directory.GetDirectories(gcRootPath);
        foreach (string lcGroupPath in loGroupFolders)
        {
            string lcGroupName = Path.GetFileName(lcGroupPath);
            AddressableAssetGroup loGroup = loSettings.FindGroup(lcGroupName);

            //<< Creamos el grupo si no existe
            if (loGroup == null)
            {
                loGroup = loSettings.CreateGroup(lcGroupName, false, false, true, null);
                Debug.Log($"<color=green>Grupo creado: {lcGroupName}</color>");
            }

            //<< 2. Configuramos el grupo para "Pack Separately"
            BundledAssetGroupSchema loSchema = loGroup.GetSchema<BundledAssetGroupSchema>();
            if (loSchema != null)
            {
                loSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackSeparately;
                //<< Configurar para que cargue de servidor (Remote)
                loSchema.BuildPath.SetVariableByName(loSettings, "RemoteBuildPath");
                loSchema.LoadPath.SetVariableByName(loSettings, "RemoteLoadPath");
            }

            //<< 3. Obtenemos los directorios "Id".
            string[] loIdsPath = Directory.GetDirectories(lcGroupPath);
            foreach (string lcIdPath in loIdsPath)
            {
                //<< 4. Buscar Prefabs dentro de las subcarpetas de IDs
                //<< Buscamos todos los .prefab en cualquier nivel dentro de este grupo
                string[] loPrefabFiles = Directory.GetFiles(lcGroupPath, "*.prefab", SearchOption.TopDirectoryOnly);

                if (loPrefabFiles.Length == 0) continue;
                if (loPrefabFiles.Length > 1) Debug.LogError($"Existe una carpeta con mas de un prefab dentro! {lcIdPath}");
                string lcPrefabPath = loPrefabFiles[0];

                string lcAssetPath = lcPrefabPath.Replace("\\", "/");
                string lcGuid = AssetDatabase.AssetPathToGUID(lcAssetPath);

                if (string.IsNullOrEmpty(lcGuid)) continue;

                AddressableAssetEntry loEntry = loSettings.CreateOrMoveEntry(lcGuid, loGroup);

                //<< 4. Asignamos el nombre del archivo como "Grupo + Id"
                int lnId = Convert.ToInt32(Path.GetFileName(lcIdPath));
                loEntry.address = lcGroupName + lnId.ToString();
            }
        }

        //<< Guardamos los cambios.
        loSettings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, true, true);
        AssetDatabase.SaveAssets();
        Debug.Log("<b>[Addressables]</b> Actualización completada con éxito.");
    }
}