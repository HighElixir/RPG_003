namespace HighElixir.UI
{
    public static class CountableSwitchBuilder
    {
        public static CountableSwitch SetMin(this CountableSwitch countable, int min)
        {
            countable.min = min;
            return countable;
        }
        public static CountableSwitch SetMax(this CountableSwitch countable, int max)
        {
            countable.max = max;
            return countable;
        }
        public static CountableSwitch SetRange(this CountableSwitch countable, int min, int max)
        {
            countable.min = min;
            countable.max = max;
            return countable;
        }
    }
}