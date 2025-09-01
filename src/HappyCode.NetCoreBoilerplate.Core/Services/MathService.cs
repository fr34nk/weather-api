namespace HappyCode.NetCoreBoilerplate.Core.Services
{
    public class MathMethods () {
        public static double Truncate(double value, int decimals)
        {
            double factor = Math.Pow(10, decimals);
            return Math.Truncate(value * factor) / factor;
        }
    }
}


