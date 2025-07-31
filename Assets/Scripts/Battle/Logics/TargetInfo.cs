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
    public Unit MainTarget
    {
        get => _mainTarget;
        set => _mainTarget = value ?? throw new ArgumentNullException(nameof(value));
    }

    public IReadOnlyList<Unit> AdditionalTargets => _additionalTargets.AsReadOnly();
    public TargetInfo(Unit mainTarget, List<Unit> additionalTargets = null, int maxTargetCount = 1)
    {
        MaxTargetCount = Mathf.Max(maxTargetCount, (additionalTargets != null ? additionalTargets.Count : 1));
        _mainTarget = mainTarget ?? throw new ArgumentNullException(nameof(mainTarget));
        if (additionalTargets != null)
            SetTargets(additionalTargets);
    }


    public void AddTarget(Unit target)
    {
        if (target != null && !_additionalTargets.Contains(target) && !Equals(_mainTarget, target))
            _additionalTargets.Add(target);
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
        _additionalTargets.AddRange(targets);
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
        // 追加ターゲットを先に返す
        foreach (var t in _additionalTargets)
            yield return t;
        // 最後にメインターゲット
        yield return _mainTarget;
    }

    // 非ジェネリック版はおまけ
    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

}
