using UnityEngine;

namespace Part20250609
{
    public class Part20250609_2 : MonoBehaviour
    {
        private void Start()
        {
            new Question1().CheckAnswer();
            new Question2().CheckAnswer();
        }

        /**
         * 問1：静的クラスと静的メソッドの基本
         * 期待される出力: "Result: 8, MathUtilは静的クラス: true"
         */

        // TODO MathUtil クラスを静的クラスに再定義する
        private static class MathUtil
        {
            // 最大公約数取得
            public static int GetGCD(int a, int b)
            {
                while (b != 0)
                {
                    var temp = b;
                    b = a % b;
                    a = temp;
                }
                return a;
            }
        }

        private class Question1
        {
            private bool IsValid(out string message)
            {
                // TODO MathUtil クラスを静的クラスにしてインスタンス生成を廃止する
                const int a = 24;
                const int b = 32;

                // 最大公約数を求める
                // TODO GetGCD メソッドの呼び出し方を修正する
                var result = MathUtil.GetGCD(a, b);

                // MathUtilが静的クラスか
                var isStatic = typeof(MathUtil).IsAbstract && typeof(MathUtil).IsSealed;

                message = $"Result: {result}, MathUtilは静的クラス: {isStatic}";
                return result == 8 && isStatic;
            }

            public void CheckAnswer()
            {
                Debug.Log($"Question1: <color={(IsValid(out var msg) ? "green" : "red")}>{msg}</color>");
            }
        }

        /**
         * 問2：拡張メソッドの定義と使用
         * 期待される出力: "所持金: 100, 値段: 50, 購入できるか: true"
         */
        public class Question2
        {
            // アイテムクラス
            public class Item
            {
                // 名前
                public readonly string Name;
                // 数量
                public readonly int Value;

                public Item(string name, int value)
                {
                    Name = name;
                    Value = value;
                }
            }

            private bool IsValid(out string message)
            {
                var money = new Item("所持金", 100);
                var requireMoney = new Item("値段", 50);

                // TODO 下段にある IntExtensions クラスの IsEnough メソッドを拡張メソッドに書き換える
                // TODO 拡張メソッドを使った所持金チェックに書き換える => money.IsEnough(requireMoney)
                var enough = money.IsEnough(requireMoney);

                // IntExtensions.IsEnoughが拡張メソッドか判定
                var isExtension = typeof(ItemExtensions).GetMethod("IsEnough")?.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false) ?? false;

                message = $"購入できるか: {enough}, 拡張メソッドになっているか: {isExtension}";
                return enough;
            }

            public void CheckAnswer()
            {
                Debug.Log($"Question2: <color={(IsValid(out var msg) ? "green" : "red")}>{msg}</color>");
            }
        }
    }

    // アイテム関連の関数を拡張するクラス
    public static class ItemExtensions
    {
        // TODO メソッド IsEnough を拡張メソッドに再定義する
        public static bool IsEnough(this Part20250609_2.Question2.Item target, Part20250609_2.Question2.Item require)
        {
            // TODO ロジック実装
            return target.Value >= require.Value;
        }
    }

}

