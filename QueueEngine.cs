namespace QueueCalc;

public class QueueResults
{
    public string ModelName { get; set; } = "";
    public double Lambda { get; set; }
    public double Mu { get; set; }
    public double Rho { get; set; }
    public double Lq { get; set; }
    public double Wq { get; set; }
    public double W { get; set; }
    public double L { get; set; }
    public double IdleProp { get; set; }
    public int Servers { get; set; }
}

public static class QueueEngine
{
    static double Factorial(int n)
    {
        double r = 1;
        for (int i = 2; i <= n; i++) r *= i;
        return r;
    }

    public static QueueResults Compute(
        double arrivalMean, double arrivalVar,
        double serviceMean, double serviceVar,
        bool arrivalIsM,
        bool serviceIsM)
    {
        double lambda = 1.0 / arrivalMean;
        double mu = 1.0 / serviceMean;
        double rho = lambda / mu;

        if (rho >= 1)
            throw new InvalidOperationException(
                "System is unstable: rho = " + rho.ToString("F4") + " >= 1.\n" +
                "Increase the number of servers or reduce the arrival rate.");

        double Lq, Wq, W, L, idleProp;
        string modelName;

        double Ca2 = arrivalVar / (arrivalMean * arrivalMean);
        double Cs2 = serviceVar / (serviceMean * serviceMean);

        if (arrivalIsM && serviceIsM)
        {
            modelName = "M/M/1";
            Lq = rho * rho / (1 - rho);
            Wq = Lq / lambda;
            W = Wq + 1.0 / mu;
            L = lambda * W;
            idleProp = 1 - rho;
        }
        else if (arrivalIsM && !serviceIsM)
        {
            modelName = "M/G/1";
            Lq = (lambda * lambda * serviceVar + rho * rho) / (2.0 * (1 - rho));
            Wq = Lq / lambda;
            W = Wq + 1.0 / mu;
            L = lambda * W;
            idleProp = 1 - rho;
        }
        else
        {
            modelName = "G/G/1";
            Lq = (rho * rho * (1 + Cs2) * (Ca2 + rho * rho * Cs2))
               / (2.0 * (1 - rho) * (1 + rho * rho * Cs2));
            Wq = Lq / lambda;
            W = Wq + 1.0 / mu;
            L = lambda * W;
            idleProp = 1 - rho;
        }

        return new QueueResults
        {
            ModelName = modelName,
            Lambda = lambda,
            Mu = mu,
            Rho = rho,
            Lq = Lq,
            Wq = Wq,
            W = W,
            L = L,
            IdleProp = idleProp,
            Servers = 1
        };
    }

    public static double GetUniformMean(double min, double max)
    {
        return (min + max) / 2.0;
    }

    public static double GetUniformVariance(double min, double max)
    {
        return Math.Pow(max - min, 2) / 12.0;
    }

    public static double GetExponentialVariance(double mean)
    {
        return mean * mean;
    }

    public static double GetGammaMean(double shape, double scale)
    {
        return shape * scale;
    }

    public static double GetGammaVariance(double shape, double scale)
    {
        return shape * scale * scale;
    }

    public static double GetNormalMean(double mean)
    {
        return mean;
    }

    public static double GetNormalVariance(double stddev)
    {
        return stddev * stddev;
    }
}
