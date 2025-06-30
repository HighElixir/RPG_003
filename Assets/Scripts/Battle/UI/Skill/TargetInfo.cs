using RPG_003.Battle;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TargetInfo : IEnumerable<CharacterObject>
{
    [SerializeField, ReadOnly] private CharacterObject _mainTarget;
    [SerializeField, ReadOnly] private readonly List<CharacterObject> _additionalTargets = new();

    public int MaxTargetCount { get; }
    public int TargetCount => 1 + _additionalTargets.Count;
    public bool IsValid => MainTarget != null && TargetCount <= MaxTargetCount;

    public TargetInfo(CharacterObject mainTarget, List<CharacterObject> additionalTargets = null, int maxTargetCount = 1)
    {
        MaxTargetCount = Mathf.Max(maxTargetCount, 1);
        _mainTarget = mainTarget ?? throw new ArgumentNullException(nameof(mainTarget));
        if (additionalTargets != null)
            SetTargets(additionalTargets);
    }

    public CharacterObject MainTarget
    {
        get => _mainTarget;
        set => _mainTarget = value ?? throw new ArgumentNullException(nameof(value));
    }

    public IReadOnlyList<CharacterObject> AdditionalTargets => _additionalTargets.AsReadOnly();

    public void AddTarget(CharacterObject target)
    {
        if (target != null && !_additionalTargets.Contains(target) && !Equals(_mainTarget, target))
            _additionalTargets.Add(target);
        RemoveOverTarget();
    }

    public void RemoveTarget(CharacterObject target)
    {
        _additionalTargets.Remove(target);
    }

    public void ClearAdditional() => _additionalTargets.Clear();

    public List<CharacterObject> ToList()
    {
        var list = new List<CharacterObject> { _mainTarget };
        list.AddRange(_additionalTargets);
        return list;
    }

    // === Private Methode ===
    private void SetTargets(List<CharacterObject> targets)
    {
        ClearAdditional();
        var t = new List<CharacterObject>(collection: targets);
        _additionalTargets.AddRange(targets);
        RemoveOverTarget();
    }

    private void RemoveOverTarget()
    {
        var c = TargetCount - MaxTargetCount;
        if (c > 0)
        {
            for (int i = 0; i < c; i++)
                _additionalTargets.RemoveAt(0);
        }
    }

    public IEnumerator<CharacterObject> GetEnumerator()
    {
        var res = new List<CharacterObject>();
        res.AddRange(_additionalTargets);
        res.Add(_mainTarget);
        return res.GetEnumerator();
    }

    // privateじゃないといけないらしい (そもそもアクセス修飾子がつけられない)
    // また、↑のほうをIEnumerator GetEnumrator()とすると、foreachステートメントで返される型がobjectになる
    // 明示的インターフェース実装で、キャスト時にだけ使われるらしい
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<CharacterObject>)this).GetEnumerator();
    }
}
