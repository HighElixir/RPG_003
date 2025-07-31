using HighElixir.UI;
using RPG_003.SceneManage;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG_003.Character
{
    public class ConfirmCharacterSave : MonoBehaviour
    {
        [SerializeField] private CharacterBuilder _characterBuilder;
        [SerializeField] private TextThrower _thrower;
        [SerializeField] private Button _confirm;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private SceneLoader _sceneLoader;

        public void OnOn()
        {
            var t = _characterBuilder.Temp;
            _text.SetText(MakeText(t));
        }

        private string MakeText(CharacterDataHolder character)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"セーブしますか？");
            sb.AppendLine($"名前: {character.Name}");
            sb.AppendLine($"残りポイント: {character.usablePoints - character.UsedPoints()}");
            return sb.ToString();
        }

        private void Awake()
        {
            _confirm.onClick.AddListener(() =>
            {
                var code = _characterBuilder.Temp.IsValid;
                if (code == 0)
                {
                    _characterBuilder.Save();
                    _sceneLoader.SceneToBefore();
                }
                else
                {
                    var text = code switch
                    {
                        -1 => "名前が入力されていないよ！",
                        -2 => "ポイントを使いすぎているよ！",
                        _ => "不明なエラー"
                    };
                    _thrower.Create(_confirm.GetComponent<RectTransform>().anchoredPosition3D, Quaternion.identity, text, Color.red);
                }
            });
        }
    }
}