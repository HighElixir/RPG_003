namespace HighElixir.UI
{
    public partial class CountableSwitch
    {
        public CountableSwitch SetMin(int min)
        {
            this.min = min;
            return this;
        }
        public CountableSwitch SetMax(int max)
        {
            this.max = max;
            return this;
        }
        public CountableSwitch SetRange(int min, int max)
        {
            this.min = min;
            this.max = max;
            if (min > max)
            {
                this.min = max;
            }
            return this;
        }
    }
}