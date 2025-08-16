public interface IRegenTarget
{
    // totalAmount：總共要回多少；duration：幾秒內回完
    void ApplyRegen(int totalAmount, float duration, RegenStackingMode stacking = RegenStackingMode.Refresh);
}

public enum RegenStackingMode { Refresh, StackAmount, Parallel }