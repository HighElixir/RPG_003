using System;
using System.Collections.Generic;

namespace HighElixir.StateMachine
{
    // �X�e�[�g�}�V���{�̂̃C���^�[�t�F�[�X
    public interface IStateMachine<T> where T : class
    {
        Dictionary<string, IState<T>> StateMap { get; }  // �X�e�[�g�̈ꗗ�i���O�ƑΉ�����C���X�^���X�j
        (string Key, IState<T> State) CurrentState { get; }                  // ���݂̃X�e�[�g

        string DefaultStateKey { get; }

        void SetCondition(Func<string> condition);
        void ChangeState(string newStateKey);         // �����X�e�[�g�ύX
        void ChangeRequest(string requestKey);        // �X�e�[�g�ύX�̗\�� or �����t���ύX
        void Update();                                // �X�e�[�g�}�V���̍X�V�����iStay�̌Ăяo���Ȃǁj
        void AddState(string newStateKey, IState<T> state);  // �X�e�[�g�̒ǉ�
    }
}