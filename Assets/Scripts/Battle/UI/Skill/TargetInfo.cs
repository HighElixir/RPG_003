using RPG_003.Battle.Characters;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TargetInfo
{
    [SerializeField, ReadOnly] private CharacterBase _mainTarget;
    [SerializeField, ReadOnly] private readonly List<CharacterBase> _additionalTargets = new();

    public int MaxTargetCount { get; }
    public int TargetCount => 1 + _additionalTargets.Count;
    public bool IsValid => MainTarget != null && TargetCount <= MaxTargetCount;

    public TargetInfo(CharacterBase mainTarget, List<CharacterBase> additionalTargets = null, int maxTargetCount = 1)
    {
        MaxTargetCount = Mathf.Max(maxTargetCount, 1);
        _mainTarget = mainTarget ?? throw new ArgumentNullException(nameof(mainTarget));
        if (additionalTargets != null)
            SetTargets(additionalTargets);
    }

    public CharacterBase MainTarget
    {
        get => _mainTarget;
        set => _mainTarget = value ?? throw new ArgumentNullException(nameof(value));
    }

    public IReadOnlyList<CharacterBase> AdditionalTargets => _additionalTargets.AsReadOnly();

    public void AddTarget(CharacterBase target)
    {
        if (target != null && !_additionalTargets.Contains(target) && !Equals(_mainTarget, target))
            _additionalTargets.Add(target);
        RemoveOverTarget();
    }

    public void RemoveTarget(CharacterBase target)
    {
        _additionalTargets.Remove(target);
    }

    public void ClearAdditional() => _additionalTargets.Clear();

    public List<CharacterBase> ToList()
    {
        var list = new List<CharacterBase> { _mainTarget };
        list.AddRange(_additionalTargets);
        return list;
    }

    // === Private Methode ===
    private void SetTargets(List<CharacterBase> targets)
    {
        ClearAdditional();
        var t = new List<CharacterBase>(collection: targets);
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
}
