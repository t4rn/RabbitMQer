namespace DataModel
{
    public class Fibonacci
    {
        public decimal Calculate(int n)
        {
            if (n == 0 || n == 1)
            {
                return n;
            }

            return Calculate(n - 1) + Calculate(n - 2);
        }
    }
}
