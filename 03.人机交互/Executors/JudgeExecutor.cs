using Microsoft.Agents.AI.Workflows;

namespace HitlDemo.Executors;

[YieldsOutput(typeof(string))]
internal sealed class JudgeExecutor(int targetNumber) : Executor<int>("Judge")
{
    private readonly int _targetNumber = targetNumber;
    private int _tries;

    public override async ValueTask HandleAsync(int message, IWorkflowContext context, CancellationToken cancellationToken = default)
    {
        _tries++;

        if (message == _targetNumber)
        {
            await context.YieldOutputAsync($"Correct. The target number is {_targetNumber}. Total tries: {_tries}.", cancellationToken);
            return;
        }

        if (message < _targetNumber)
        {
            await context.YieldOutputAsync(Enum.GetName(NumberSignal.Below) ?? "", cancellationToken);
            return;
        }

        await context.YieldOutputAsync(NumberSignal.Above, cancellationToken);
    }
}
