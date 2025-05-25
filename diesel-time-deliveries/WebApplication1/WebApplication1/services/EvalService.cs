using StringMath;

namespace WebApplication1.services;

public class EvalService
{
    public static double GetEval(string evalCode)
    {
        return evalCode.Eval();
    }
}