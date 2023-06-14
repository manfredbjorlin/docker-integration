public class NGTimerService
{
    private int _pollingDelayMs;
    private Action _inputHandler;
    public NGTimerService(int pollingDelayMs, Action inputHandler, CancellationToken cancellationToken)
    {
        _pollingDelayMs = pollingDelayMs;
        _inputHandler = inputHandler;

        var timer = new Timer(
            o => 
            {
                _inputHandler();
            },
            null,
            _pollingDelayMs,
            _pollingDelayMs
        );

        _ = new Timer(
            o =>
            {
                if(cancellationToken.IsCancellationRequested)
                    timer.Dispose();
            },
            null,
            100,
            100
        );
    }
}