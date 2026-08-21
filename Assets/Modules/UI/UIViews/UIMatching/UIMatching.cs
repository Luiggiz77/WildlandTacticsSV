using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMatching : UIView
{
    /// <summary>
    /// OnGameCommand
    /// </summary>
    /// <param name="lcCommand"></param>
    /// <param name="loParams"></param>
    protected override void OnGameCommand(GameCommand loCommand, params object[] loParams)
    {
        switch (loCommand)
        {
            case GameCommand.ShowUIMatching: panel.SetActive(true); break;
            default: break;
        }
    }
}
