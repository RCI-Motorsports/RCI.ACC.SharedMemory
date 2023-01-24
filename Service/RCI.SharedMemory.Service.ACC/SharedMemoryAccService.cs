using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RCI.SharedMemory.ACC;
using RCI.SharedMemory.ACC.Structs;
using RCI.SharedMemory.Service.Interfaces;

namespace RCI.SharedMemory.Service.ACC;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class SharedMemoryAccService : ISharedMemoryService
{
  private readonly ISharedMemoryProvider _provider;
  private readonly Subject<Physics> _physicsSubject = new();
  private readonly Subject<Graphics> _graphicsSubject = new();
  private readonly Subject<StaticInfo> _staticInfoSubject = new();
  private readonly IDisposable _configMonitor;

  private ISharedMemoryConnection<Physics>? _physics;
  private ISharedMemoryConnection<Graphics>? _graphics;
  private ISharedMemoryConnection<StaticInfo>? _staticInfo;

  public IConnectableObservable<Physics>? Physics { get; }
  public IConnectableObservable<Graphics>? Graphics { get; }
  public IConnectableObservable<StaticInfo>? StaticInfo { get; }

  public SharedMemoryAccService(ISharedMemoryProvider provider, IServiceProvider serviceProvider)
  {
    _provider = provider;

    using var scope = serviceProvider.CreateScope();
    var configMonitor = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<SharedMemoryAccConfig>>();
    _configMonitor = configMonitor.OnChange(OnChange)!;
    OnChange(configMonitor.CurrentValue);

    Physics = _physicsSubject.Publish();
    _ = Physics.Connect();

    Graphics = _graphicsSubject.Publish();
    _ = Graphics.Connect();

    StaticInfo = _staticInfoSubject.Publish();
    _ = StaticInfo.Connect();
  }

  private void OnChange(SharedMemoryAccConfig config)
  {
    DisposeCore();

    if (config.Physics.Enabled)
    {
      _physics = _provider.GetConnection<Physics>(config.Physics.Interval);
      _physics.Observable.Subscribe(p => _physicsSubject.OnNext(p));
    }

    if (config.Graphics.Enabled)
    {
      _graphics = _provider.GetConnection<Graphics>(config.Graphics.Interval);
      _graphics.Observable.Subscribe(g => _graphicsSubject.OnNext(g));
    }

    if (config.StaticInfo.Enabled)
    {
      _staticInfo = _provider.GetConnection<StaticInfo>(config.StaticInfo.Interval);
      _staticInfo.Observable.Subscribe(s => _staticInfoSubject.OnNext(s));
    }
  }


  public void Dispose()
  {
    _configMonitor.Dispose();
    _physicsSubject.Dispose();
    _graphicsSubject.Dispose();
    _staticInfoSubject.Dispose();
    _configMonitor.Dispose();
    DisposeCore();
  }

  private void DisposeCore()
  {
    if (_physics is not null)
    {
      _physics.Dispose();
      _physics = null;
    }

    if (_graphics is not null)
    {
      _graphics.Dispose();
      _graphics = null;
    }
    
    if (_staticInfo is not null)
    {
      _staticInfo.Dispose();
      _staticInfo = null;
    }
  }
}