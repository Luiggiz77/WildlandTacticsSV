public class PlayablePrefabGroupEffectValued : PlayablePrefabGroupEffect<EffectValued>
{
    /// <summary>
    /// Es el valor del elemento.
    /// </summary>
    private string gcValue = string.Empty;

    public override bool Ended()
    {
        return !gameObject.activeSelf;
    }

    /// <summary>
    /// Establece un valor. En este caso de tiempo de esparcimiento.
    /// </summary>
    /// <param name="lcValue"></param>
    public override void SetValue(object loValue)
    {
        gcValue = loValue as string;
    }

    /// <summary>
    /// Es cuando se crea el elemento.
    /// </summary>
    /// <param name="loElement"></param>
    protected override void OnSpawn(EffectValued loElement)
    {
        loElement.text.text = gcValue;
    }

    /// <summary>
    /// Preview
    /// </summary>
    public override void Preview()
    {
    }
}