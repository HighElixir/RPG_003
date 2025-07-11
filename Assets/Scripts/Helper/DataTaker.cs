using Cysharp.Threading.Tasks;
using RPG_003.Skills;
using UnityEngine.AddressableAssets;
using System.Linq;

namespace RPG_003.Helper
{
    public static class DataTaker
    {
        public static async UniTask<SkillData> SkillReaderAsync(string label, string targetID)
        {
            var task = Addressables.LoadAssetsAsync<SkillData>(label).ToUniTask();
            var loaded = await task;
            if (task.Status == UniTaskStatus.Faulted)
                return null;
            return loaded.FirstOrDefault(data => data.ID == targetID);
        }
    }
}