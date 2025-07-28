using RPG_003.Character;
using RPG_003.Skills;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG_003.Equipments
{
    public class PlayerIndicator : MonoBehaviour
    {
        [SerializeField] private Button[] _skillButtons = new Button[3];

        public Button[] SetPlayer(CharacterDataHolder chara)
        {
            var i = 0;
            foreach (var button in _skillButtons)
            {
                if (i < chara.Skills.Count)
                {
                    UpdateSkill(button, chara.Skills[i]);
                    i++;
                }
                else
                    UpdateSkill(button);
            }
            return _skillButtons;
        }

        public void UpdateSkill(Button button, SkillHolder skill = null)
        {
            var item = GetChind(button.gameObject);
            if (skill != null)
            {
                item.image.sprite = skill.Icon;
                item.image.color = new(1, 1, 1, 1);
                item.text.SetText(skill.Name);
            }
            else
            {
                item.image.color = new(1, 1, 1, 0);
            }
        }
        // Buttonの子要素を取得。どちらかが欠けている場合、エラーを返す
        private (Image image, TMP_Text text) GetChind(GameObject obj)
        {
            (Image image, TMP_Text text) res = (null, null);
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                var g = obj.transform.GetChild(i);
                if (res.image == null && g.TryGetComponent<Image>(out var image))
                    res.image = image;
                if (res.text == null && g.TryGetComponent<TMP_Text>(out var text))
                    res.text = text;
            }
            if (res.image == null || res.text == null)
                throw new MissingComponentException();
            return res;
        }
    }
}