using RCI.ACC.SharedMemory.Core;
using RCI.ACC.SharedMemory.Core.Enums;
using RCI.ACC.SharedMemory.Core.Interfaces;
using RCI.ACC.SharedMemory.Core.Structs;

namespace RCI.ACC.SharedMemory.Static;

public class AccSharedMemoryStaticReader : EventArgs
{
  private AccSharedMemoryStatus _accSharedMemoryStatus = AccSharedMemoryStatus.Disconnected;
  private GameStatus _gameStatus = GameStatus.Off;
  private readonly AccSharedMemoryConfig _config;

  private static TimeSpan RetryPeriod = TimeSpan.FromSeconds(2);
  private readonly Timer _retryTimer;

  private readonly Timer _physicsTimer;
  private readonly Timer _graphicsTimer;
  private readonly Timer _staticInfoTimer;

  private ISharedMemoryFile<Physics> _physics = new LocalSharedMemoryFile<Physics>(SharedMemoryFileNames.Physics);
  private ISharedMemoryFile<Graphics> _graphics = new LocalSharedMemoryFile<Graphics>(SharedMemoryFileNames.Graphics);
  private ISharedMemoryFile<StaticInfo> _staticInfo = new LocalSharedMemoryFile<StaticInfo>(SharedMemoryFileNames.StaticInfo);

  public delegate void PhysicsUpdatedHandler(Physics physics);
  public delegate void GraphicsUpdatedHandler(Graphics graphics);
  public delegate void StaticInfoUpdatedHandler(StaticInfo staticInfo);
  public delegate void GameStatusChangedHandler(GameStatus gameStatus);

  public event PhysicsUpdatedHandler? PhysicsUpdated;
  public event GraphicsUpdatedHandler? GraphicsUpdated;
  public event StaticInfoUpdatedHandler? StaticInfoUpdated;
  public event GameStatusChangedHandler? GameStatusUpdated;

  public bool IsRunning => _accSharedMemoryStatus == AccSharedMemoryStatus.Connected;

  public string GameStatusString => _gameStatus switch
  {
    GameStatus.Off => "Off",
    GameStatus.Live => "Live",
    GameStatus.Pause => "Pause",
    GameStatus.Replay => "Replay",
    _ => throw new NotImplementedException(),
  };

  public AccSharedMemoryStaticReader(AccSharedMemoryConfig? config = null)
  {
    _config = config ?? new AccSharedMemoryConfig();
    _retryTimer = new Timer(OnRetryTimer, null, Timeout.Infinite, Timeout.Infinite);

    _physicsTimer = new Timer(OnPhysicsTimer, null, Timeout.Infinite, Timeout.Infinite);
    _graphicsTimer = new Timer(OnGraphicsTimer, null, Timeout.Infinite, Timeout.Infinite);
    _staticInfoTimer = new Timer(OnPhysicsTimer, null, Timeout.Infinite, Timeout.Infinite);
  }

  public void Connect()
  {
    _retryTimer.Start(RetryPeriod);
  }

  public void Disconnect()
  {
    _accSharedMemoryStatus = AccSharedMemoryStatus.Disconnected;

    _retryTimer.Stop();

    _physicsTimer.Stop();
    _graphicsTimer.Stop();
    _staticInfoTimer.Stop();
  }

  private bool ConnectInternal()
  {
    try
    {
      _accSharedMemoryStatus = AccSharedMemoryStatus.Connecting;

      if (_config.Physics.Enabled)
      {
        _physics.Connect();
        _physicsTimer.Start(TimeSpan.FromMilliseconds(_config.Physics.UpdateInterval));
      }

      if (_config.Graphics.Enabled)
      {
        _graphics.Connect();
        _graphicsTimer.Start(TimeSpan.FromMilliseconds(_config.Graphics.UpdateInterval));
      }

      if (_config.StaticInfo.Enabled)
      {
        _staticInfo.Connect();
        _staticInfoTimer.Start(TimeSpan.FromMilliseconds(_config.StaticInfo.UpdateInterval));
      }

      // Stop retry timer
      _retryTimer.Stop();
      _accSharedMemoryStatus = AccSharedMemoryStatus.Connected;
      return true;
    }
    catch (FileNotFoundException)
    {
      _physicsTimer.Stop();
      _graphicsTimer.Stop();
      _staticInfoTimer.Stop();
      return false;
    }
  }

  private void OnRetryTimer(object? _)
  {
    ConnectInternal();
  }

  private void OnPhysicsTimer(object? _)
    => PhysicsUpdated?.Invoke(_physics.Read());

  private void OnGraphicsTimer(object? _)
  {
    if (GraphicsUpdated is null)
      return;

    var graphics = _graphics.Read();
    GraphicsUpdated.Invoke(graphics);
    if (_gameStatus != graphics.GameStatus)
    {
      _gameStatus = graphics.GameStatus;
      if(GameStatusUpdated is not null)
        GameStatusUpdated.Invoke(_gameStatus);
    }

  }

  private void OnStaticInfoTimer(object? _)
    => StaticInfoUpdated?.Invoke(_staticInfo.Read());
}

