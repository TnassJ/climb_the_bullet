using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Button�ɓo�^����Ă���TargetGraphic�ȊO�̉摜�̃J���[���ꏏ�ɕς������Ƃ��Ɏg��
/// ����N���X��DoStateTransition�ɏ]���ē����^�C�~���O�œ��삷��
/// </summary>
[RequireComponent(typeof(TransitionFollowingColorsButtonTargets))]

public class Button_ColorLink : Button
{
    private TransitionFollowingColorsButtonTargets data;

    protected override void Awake()
    {
        base.Awake();
        data = GetComponent<TransitionFollowingColorsButtonTargets>();
    }

    // ����N���X�ɂ�����������̂܂܃J���[�p�ɔ����o���āA�����摜�ɓ����^�C�~���O�œK�p�o����悤�ɂ���
    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        base.DoStateTransition(state, instant);
        Color tintColor;
        switch (state)
        {
            case SelectionState.Normal:
                tintColor = colors.normalColor;
                break;
            case SelectionState.Highlighted:
                tintColor = colors.highlightedColor;
                break;
            case SelectionState.Pressed:
                tintColor = colors.pressedColor;
                break;
            case SelectionState.Selected:
                tintColor = colors.selectedColor;
                break;
            case SelectionState.Disabled:
                tintColor = colors.disabledColor;
                break;
            default:
                tintColor = Color.black;
                break;
        }

        foreach (var target in data.Targets)
        {
            target.CrossFadeColor(tintColor * colors.colorMultiplier, instant ? 0f : colors.fadeDuration, true, true);
        }
    }
}
