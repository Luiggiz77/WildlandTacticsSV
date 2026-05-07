using UnityEngine;

/// <summary>
/// Este componente debe ponerse en el root del objeto ya que de este se hacen las copias.
/// </summary>
public class UnitElement : MonoBehaviour
{
    [Header("UNIT ELEMENT")]
    [Tooltip("Son todas las mallas usadas.")]
    public Renderer[] renderers;

    [Tooltip("Son todas las mallas a las cuales se les aplica el outline de selección.")]
    public Renderer[] selectionOutlined;

    /// <summary>
    /// Son los materiales en instancia de nuestro modelo.
    /// </summary>
    private Material[][] goMaterials;

    /// <summary>
    /// Son los materiales compartidos de nuestro modelo.
    /// </summary>
    private Material[][] goSharedMaterials;

    /// <summary>
    /// Son los materiales originales de nuestro modelo.
    /// </summary>
    private Material[][] goOutlineMaterials;

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        goMaterials = new Material[renderers.Length][];
        goSharedMaterials = new Material[renderers.Length][];
        goOutlineMaterials = new Material[selectionOutlined.Length][];

        //<< Creamos copia de los materiales originales y creamos una copia para los de selección.
        int lnCount = renderers.Length;
        for (int i = 0; i < lnCount; i++)
        {
            //<< Guardardamos shared ORIGINAL.
            goSharedMaterials[i] = renderers[i].sharedMaterials.Clone() as Material[];

            //<< Creamos INSTANCIAS reales para runtime.
            var shared = renderers[i].sharedMaterials;
            var instanced = new Material[shared.Length];

            for (int m = 0; m < shared.Length; m++) instanced[m] = new Material(shared[m]);

            renderers[i].materials = instanced;
            goMaterials[i] = instanced;
        }

        int lnIndexOf;
        lnCount = selectionOutlined.Length;
        for (int i = 0; i < lnCount; i++)
        {
            lnIndexOf = System.Array.IndexOf(renderers, selectionOutlined[i]);
            if (lnIndexOf > -1) goOutlineMaterials[i] = goMaterials[lnIndexOf];
            else goOutlineMaterials[i] = selectionOutlined[i].materials.Clone() as Material[];
        }
    }

    /// <summary>
    /// Se llama para establecer la selección en nuestro elemento.
    /// </summary>
    /// <param name="lbSelected"></param>
    public void Select(Color loColor)
    {
        foreach (Material[] loMaterials in goOutlineMaterials)
        {
            foreach (Material loMaterial in loMaterials) loMaterial.SetColor("_OutlineColor", loColor);
        }
    }

    /// <summary>
    /// Se llkama para remover la seleccion de nuestro elemento.
    /// </summary>
    public void Deselect()
    {
        foreach (Material[] loMaterials in goOutlineMaterials)
        {
            foreach (Material loMaterial in loMaterials) loMaterial.SetColor("_OutlineColor", Color.black);
        }
    }

    /// <summary>
    /// Se llama para reiniciar los materiales de base.
    /// </summary>
    public void ResetMaterials()
    {
        int lnCount = renderers.Length;
        for (int i = 0; i < lnCount; i++)
        {
            renderers[i].sharedMaterials = goSharedMaterials[i];
        }
    }

    /// <summary>
    /// NOs da los materiales instanciados ya que son copias y se pueden modificar.
    /// </summary>
    /// <returns></returns>
    public Material[][] GetMaterialInstances()
    {
        return goMaterials;
    }

    /// <summary>
    /// Se llama para establecer el layer de nuestra unidad.
    /// </summary>
    /// <param name="lnLayer"></param>
    public virtual void SetLayer(int lnLayer)
    {
        foreach (Renderer loItem in renderers) loItem.gameObject.layer = lnLayer;
    }

    /// <summary>
    /// Para activar o desactivar los renderers.
    /// </summary>
    /// <param name="lbEnabled"></param>
    public virtual void SetRendererEnable(bool lbEnabled)
    {
        foreach (Renderer loItem in renderers)
        {
            if (loItem == null) continue;
            loItem.enabled = lbEnabled;
        }
    }
}