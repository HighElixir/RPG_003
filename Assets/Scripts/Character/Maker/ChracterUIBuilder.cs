using RPG_003.DataManagements.Datas;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPG_003.Character
{
    public class CharacterUIBuilder : MonoBehaviour
    {
        [SerializeField] private TMP_Text _remaining;
        [SerializeField] private TMP_Text _page1;
        [SerializeField] private TMP_Text _page2;
        private CharacterStatsBuilder _stats = new();
        public void SetDelegate(List<CharacterBuilder.Container> containers)
        {
            foreach (var container in containers)
            {
                if (container.countable == null) continue;
                var button = container.countable;
                if (container.attribute != StatusAttribute.None)
                {
                    if (container.attribute == StatusAttribute.CriticalRate)
                    {
                        button.OnChanged.AddListener(b =>
                        {
                            var point = b.CurrentAmount;
                            var button = container.countable;
                            var cr = CoreDatas.CRTPerPoint;
                            var text = $"CR:{cr.rate * 100 * point}%/CD:{cr.bonus * 100 * point}%";
                            container.text.SetText(text);
                        });
                    }
                    else
                    {
                        button.OnChanged.AddListener(b =>
                        {
                            var point = container.countable.CurrentAmount;
                            var button = container.countable;
                            var bonus = container.attribute.GetPoint();
                            var text = "";
                            if (container.attribute == StatusAttribute.TakeDamageScale) 
                                text += $"{bonus * container.countable.CurrentAmount * 100}%";
                            else
                                text = (bonus * container.countable.CurrentAmount).ToString();
                            container.text.SetText(text);
                        });
                    }
                }
                if (container.elements != Elements.None)
                {

                    button.OnChanged.AddListener(b =>
                    {
                        var point = container.countable.CurrentAmount;
                        var text = $"Take: {point * CoreDatas.ElementPerPoint.take * 100}%\nGive: {point * CoreDatas.ElementPerPoint.give * 100}%";
                        container.text.SetText(text);
                    });
                }
            }
        }
        public void UpdateUI(List<CharacterBuilder.Container> containers, CharacterDataHolder character)
        {
            _remaining.SetText($"残りポイント（{CoreDatas.UsablePoint - character.UsedPoints()}）");
            UpdateStats(character);
        }

        public void UpdateStats(CharacterDataHolder character)
        {
            _page1.SetText(_stats.Enter(character).AddName().AddStatus().Build());
            _page2.SetText(_stats.Enter(character).AddElementData().Build());
        }
    }
}