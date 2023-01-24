using System.Runtime.Versioning;
using RCI.SharedMemory.ACC;
using RCI.SharedMemory.ACC.Enums;
using RCI.SharedMemory.ACC.Structs;
using RCI.SharedMemory.Core;
using RCI.SharedMemory.Core.Enums;

namespace RCI.SharedMemory.Static.ACC;

[SupportedOSPlatform("windows")]
public class AccSharedMemoryStaticReader : IDisposable
{
  private SharedMemoryStatus _sharedMemoryStatus = SharedMemoryStatus.Disconnected;
  private GameStatus _gameStatus = GameStatus.Off;

  private readonly SharedMemoryAccConfig _config;
  private readonly Timer _retryTimer;

  private readonly SharedMemoryStaticConnection<Physics>? _physics;
  private readonly SharedMemoryStaticConnection<Graphics>? _graphics;
  private readonly SharedMemoryStaticConnection<StaticInfo>? _staticInfo;

  public delegate void PhysicsUpdatedHandler(Physics physics);

  public delegate void GraphicsUpdatedHandler(Graphics graphics);

  public delegate void StaticInfoUpdatedHandler(StaticInfo staticInfo);

  public delegate void GameStatusChangedHandler(GameStatus gameStatus);

  public event PhysicsUpdatedHandler? PhysicsUpdated;
  public event GraphicsUpdatedHandler? GraphicsUpdated;
  public event StaticInfoUpdatedHandler? StaticInfoUpdated;
  public event GameStatusChangedHandler? GameStatusUpdated;

  public bool IsRunning => _sharedMemoryStatus == SharedMemoryStatus.Connected;

  public string GameStatusString => _gameStatus switch
  {
    GameStatus.Off => "Off",
    GameStatus.Live => "Live",
    GameStatus.Pause => "Pause",
    GameStatus.Replay => "Replay",
    _ => throw new NotImplementedException(),
  };

  public AccSharedMemoryStaticReader(SharedMemoryAccConfig? config = null)
  {
    _config = config ?? new SharedMemoryAccConfig();
    _retryTimer = new Timer(ConnectInternal, null, Timeout.Infinite, Timeout.Infinite);

    if (_config.Physics.Enabled)
    {
      _physics = SharedMemoryStatic.Connect<Physics>(_config.Physics.Interval);
      _physics.Updated += physics => PhysicsUpdated?.Invoke(physics);
    }

    if (_config.Graphics.Enabled)
    {
      _graphics = SharedMemoryStatic.Connect<Graphics>(_config.Graphics.Interval);
      _graphics.Updated += graphics =>
      {
        GraphicsUpdated?.Invoke(graphics);
        if (_gameStatus != graphics.GameStatus)
        {
          _gameStatus = graphics.GameStatus;
          GameStatusUpdated?.Invoke(_gameStatus);
        }
      };
    }

    if (_config.StaticInfo.Enabled)
    {
      _staticInfo = SharedMemoryStatic.Connect<StaticInfo>(_config.StaticInfo.Interval);
      _staticInfo.Updated += staticInfo => StaticInfoUpdated?.Invoke(staticInfo);
    }
  }

  public void Connect()
  {
    _retryTimer.Start(TimeSpan.FromMilliseconds(_config.RetryTimerInterval));
  }

  public void Disconnect()
  {
    _sharedMemoryStatus = SharedMemoryStatus.Disconnected;

    _retryTimer.Stop();

    _physics?.Stop();
    _graphics?.Stop();
    _staticInfo?.Stop();
  }

  private void ConnectInternal(object? _)
  {
    try
    {
      _sharedMemoryStatus = SharedMemoryStatus.Connecting;

      _physics?.Restart();
      _graphics?.Restart();
      _staticInfo?.Restart();

      _retryTimer.Stop();
      _sharedMemoryStatus = SharedMemoryStatus.Connected;
    }
    catch (FileNotFoundException)
    {
      _physics?.Stop();
      _graphics?.Stop();
      _staticInfo?.Stop();
    }
  }


  public void Dispose()
  {
    _retryTimer.Dispose();
    _physics?.Dispose();
    _graphics?.Dispose();
    _staticInfo?.Dispose();
  }
}