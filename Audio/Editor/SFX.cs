using System.Collections.Generic;
using Bartox.Audio;
using FMODUnity;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class SFX
{
    [LabelText("SFX Type")] [LabelWidth(100)] [OnValueChanged("SFXChange")] [InlineButton("PlaySFX")]
    public SFXManager.SFXType SfxType = SFXManager.SFXType.UI;

    [LabelText("$_sfxLabel")]
    [LabelWidth(100)]
    [ValueDropdown("SFXType")]
    [OnValueChanged("SFXChange")]
    [InlineButton("SelectSFX")]
    public FMODEvent EventToPlay;

    string _sfxLabel = "SFX";

    [SerializeField] bool _showSettings = false;
    [SerializeField] bool _editSettings = false;

    [InlineEditor(InlineEditorObjectFieldModes.Hidden)]
    [ShowIf("_showSettings")]
    [EnableIf("_editSettings")]
    [SerializeField]
    FMODEvent _eventBase;

    [Title("Event Emitter")] [ShowIf("_showSettings")] [EnableIf("_editSettings")] [SerializeField]
    bool _waitToPlay = true;

    [ShowIf("_showSettings")] [EnableIf("_editSettings")] [SerializeField]
    bool _useDefault = true;

    [DisableIf("_useDefault")] [ShowIf("_showSettings")] [EnableIf("_editSettings")] [SerializeField]
    StudioEventEmitter _eventEmitter;

    void SFXChange()
    {
        // keep the label up to date
        _sfxLabel = SfxType.ToString() + " SFX";

        // keep the displayed "FMODEvent" up to date
        _eventBase = EventToPlay;
    }


    void SelectSFX()
    {
        UnityEditor.Selection.activeObject = EventToPlay;
    }

    // Gets list of SFX from manager, used in the inspector
    List<FMODEvent> SFXType()
    {
        List<FMODEvent> sfxList;

        switch (SfxType)
        {
            case SFXManager.SFXType.UI:
                sfxList = SFXManager.Singleton._uiSFX;
                break;
            case SFXManager.SFXType.Ambient:
                sfxList = SFXManager.Singleton._ambientSFX;

                break;
            case SFXManager.SFXType.Violin:
                sfxList = SFXManager.Singleton._violinSFX;

                break;
            case SFXManager.SFXType.Bart:
                sfxList = SFXManager.Singleton._bartSFX;

                break;
            case SFXManager.SFXType.Interaction:
                sfxList = SFXManager.Singleton._interactionSFX;

                break;
            default:
                sfxList = SFXManager.Singleton._uiSFX;
                break;
        }

        return sfxList;
    }

    public void PlaySFX()
    {
        if (_useDefault || _eventEmitter == null)
        {
            SFXManager.PlayEvent(EventToPlay, _waitToPlay, null);
        }
        else
        {
            SFXManager.PlayEvent(EventToPlay, _waitToPlay, _eventEmitter);
        }
    }

    public void PlayOneShot()
    {
    }
}