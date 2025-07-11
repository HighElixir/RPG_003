using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG_003.Tutor
{
    public class TutorHandler : MonoBehaviour
    {
        [SerializeReference] private List<ITutor> _tutors;
        public async UniTask WakeUp()
        {
            foreach (var tutor in _tutors)
            {
                await tutor.Script();
            }
        }
    }
}