using System;
using System.Collections;

public class PlayablePrefabSound : PlayablePrefab
{
    private SoundType goSoundType = SoundType.None;

    public override void Play(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
        SetValue(loValue);
        GameManager.Send(GameCommand.PlaySound, goSoundType);
    }

    public override IEnumerator PlayCoroutine(object loValue, int lnAreaSections, params BoardCell<BoardUnit>[] loCells)
    {
        yield break;
    }

    public override IEnumerator PlayCoroutineBySection(object loValue, int lnDistanceMax, int lnDistanceCurrent, BoardCell<BoardUnit>[] loCells, int lnCells)
    {
        yield return PlayCoroutine(loValue, lnDistanceMax, loCells);
    }

    public override bool Ended()
    {
        return !gameObject.activeSelf;
    }

    public override void SetValue(object loValue)
    {
        string lcValue = loValue as string;
        if (string.IsNullOrWhiteSpace(lcValue)) return;
        if (!Enum.TryParse<SoundType>(lcValue, out goSoundType)) return;
    }

    public override void Stop()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Preview
    /// </summary>
    public override void Preview()
    {
    }
}
