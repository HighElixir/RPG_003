using RPG_003.Battle;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TargetInfo : IEnumerable<Unit>
{
    [SerializeField, ReadOnly] private Unit _mainTarget;
    [SerializeField, ReadOnly] private readonly List<Unit> _additionalTargets = new();

    public int MaxTargetCount { get; }
    public int TargetCount => 1 + _additionalTargets.Count;
    public bool IsValid => MainTarget != null && TargetCount <= MaxTargetCount;

    public TargetInfo(Unit mainTarget, List<Unit> additionalTargets = null, int maxTargetCount = 1)
    {
        MaxTargetCount = Mathf.Max(maxTargetCount, 1);
        _mainTarget = mainTarget ?? throw new ArgumentNullException(nameof(mainTarget));
        if (additionalTargets != null)
            SetTargets(additionalTargets);
    }

    public Unit MainTarget
    {
        get => _mainTarget;
        set => _mainTarget = value ?? throw new ArgumentNullException(nameof(value));
    }

    public IReadOnlyList<Unit> AdditionalTargets => _additionalTargets.AsReadOnly();

    public void AddTarget(Unit target)
    {
        if (target != null && !_additionalTargets.Contains(target) && !Equals(_mainTarget, target))
            _additionalTargets.Add(target);
        RemoveOverTarget();
    }

    public void RemoveTarget(Unit target)
    {
        _additionalTargets.Remove(target);
    }

    public void ClearAdditional() => _additionalTargets.Clear();

    public List<Unit> ToList()
    {
        var list = new List<Unit> { _mainTarget };
        list.AddRange(_additionalTargets);
        return list;
    }

    // === Private Methode ===
    private void SetTargets(List<Unit> targets)
    {
        ClearAdditional();
        var t = new List<Unit>(collection: targets);
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
    public override string ToString()
    {
        // Main and additional target names
        var mainName = MainTarget.Data.Name;
        var additionalNames = AdditionalTargets.Count > 0
            ? string.Join(", ", AdditionalTargets.Select(t => t.Data.Name))
            : "None";

        return $"[TargetInfo] Main: {mainName}, Additional: [{additionalNames}], MaxCount: {MaxTargetCount}";
    }
    public IEnumerator<Unit> GetEnumerator()
    {
        var res = new List<Unit>();
        res.AddRange(_additionalTargets);
        res.Add(_mainTarget);
        return res.GetEnumerator();
    }

    // privateじゃないといけないらしい (そもそもアクセス修飾子がつけられない)
    // また、↑のほうをIEnumerator GetEnumrator()とすると、foreachステートメントで返される型がobjectになる
    // 明示的インターフェース実装で、キャスト時にだけ使われるらしい
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable<Unit>)this).GetEnumerator();
    }
}
