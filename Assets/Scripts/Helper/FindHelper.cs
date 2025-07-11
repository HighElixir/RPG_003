using Sirenix.Utilities;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG_003.Helper
{
    public static class FindHelper
    {
        public static Canvas Canvas { get 
            {
                return SceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault(g => g.TryGetComponent<Canvas>(out _)).GetComponent<Canvas>();
            }
        }
    }
}