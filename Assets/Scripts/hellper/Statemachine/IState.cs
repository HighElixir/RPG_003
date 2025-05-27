using System;
using System.Collections.Generic;
using UnityEditor;

namespace HighElixir.StateMachine
{

    /// <summary>
    /// ��ԁi�X�e�[�g�j���Ƃ̃C���^�[�t�F�[�X�B
    /// �e�X�e�[�g�i�s���p�^�[���j�ɂ��̃C���^�[�t�F�[�X������������B
    /// �Ԃ�l��bool�͏����̐����^���s�������B
    /// </summary>
    public interface IState<TParent> where TParent : class
    {
        // �X�e�[�g�ɓ������Ƃ��̏����i���s������false��Ԃ��ăG���[�n���h�����O�ցj
        bool Enter(IState<TParent> previousState, TParent parent);

        // �X�e�[�g���ɖ��t���[���Ă΂�鏈���ifalse�Ȃ烍�O�o�����Ǒ��s�j
        bool Stay(TParent parent);

        // �X�e�[�g���甲����Ƃ��̏����ifalse�ł����O�o���đ�����j
        bool Exit(IState<TParent> nextState, TParent parent);
    }
}
