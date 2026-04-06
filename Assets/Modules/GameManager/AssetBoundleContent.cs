using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase que contiene todos los elementos cargados del asset boundle y todo lo correspondiente para su limpieza.
/// </summary>
public class AssetBundleContent
{
    /// <summary>
    /// El asset bundle para limpiar despues.
    /// </summary>
    public AssetBundle AssetBundle = null;

    /// <summary>
    /// Lista de texturas de haber.
    /// </summary>
    public List<Texture2D> Textures2D = new List<Texture2D>();

    /// <summary>
    /// Lista de AnimationClips de haber.
    /// </summary>
    public List<AnimationClip> AnimationClips = new List<AnimationClip>();

    /// <summary>
    /// Lista de materiales.
    /// </summary>
    public List<Material> Materials = new List<Material>();

    /// <summary>
    /// Lista de GameObjects del item ya cargado pero no instanciado que es la base de los clones.
    /// </summary>
    public List<GameObject> GameObjects = new List<GameObject>();

    /// <summary>
    /// Lista de meshes creadas
    /// </summary>
    public Dictionary<string, List<SkinnedMeshRenderer>> ClonedSkinnedMeshRenderers = new Dictionary<string, List<SkinnedMeshRenderer>>();

    /// <summary>
    /// Lista de gameobjects instanciados que usan éste asset bundle.
    /// </summary>
    public Dictionary<string, Dictionary<GameObject, List<GameObject>>> Clones = new Dictionary<string, Dictionary<GameObject, List<GameObject>>>();

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="loAssetBundle"></param>
    public AssetBundleContent(AssetBundle loAssetBundle)
    {
        AssetBundle = loAssetBundle;
    }

    /// <summary>
    /// Nos indica si alguna de las texturas contiene la llave.
    /// </summary>
    /// <param name="lcKey"></param>
    /// <returns></returns>
    public bool Textures2DContains(string lcKey)
    {
        foreach (Texture2D loItem in Textures2D)
        {
            if (loItem.name.Contains(lcKey)) return true;
        }
        return false;
    }
}