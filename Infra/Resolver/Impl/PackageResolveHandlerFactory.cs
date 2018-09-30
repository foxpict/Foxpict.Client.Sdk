using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foxpict.Client.Sdk.Infra.Resolver.Impl
{
  /// <summary>
  /// 
  /// </summary>
  /// <remarks>
  /// 登録できるハンドラはPackageResolveHandlerを継承したクラスです。
  /// </remarks>
  public abstract class PackageResolveHandlerFactory : Dictionary<string, Type>, IResolveHandlerFactory
  {
    private readonly Container mContainer;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="container">アプリケーションコンテナ</param>
    /// <param name="resolvePaclageName"></param>
    public PackageResolveHandlerFactory(Container container, string resolvePaclageName) :
      this(container, resolvePaclageName, Lifestyle.Scoped)
    {
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="container"></param>
    /// <param name="resolvePaclageName"></param>
    /// <param name="lifeSycle">生成するハンドラのライフサイクル</param>
    public PackageResolveHandlerFactory(Container container, string resolvePaclageName, Lifestyle lifeSycle)
    {
      this.mContainer = container;
      RegisterResolveHandlers(resolvePaclageName, lifeSycle);
    }

    /// <summary>
    /// ハンドラ名からハンドラを取得する
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public IResolveHandler CreateNew(string name) => (IResolveHandler)this.mContainer.GetInstance(this[name]);

    /// <summary>
    /// 指定したパッケージ名のメタ情報を取得し、型情報をコンテナに登録する。
    /// </summary>
    /// <param name="resolvePaclageName"></param>
    /// <param name="lifeSycle">生成するハンドラのライフサイクル</param>
    private void RegisterResolveHandlers (string resolvePaclageName, Lifestyle lifeSycle)
    {
      var localContainer = new Container();
      var repositoryAssembly = typeof(PackageResolveHandlerFactory).Assembly;

      var registrations =
          from type in repositoryAssembly.GetExportedTypes()
          where type.Namespace == resolvePaclageName
          where type.GetInterfaces().Contains(typeof(IResolveDeclare))
          select new { Service = type.GetInterfaces().Single(), Implementation = type };

      List<Type> implementationList = new List<Type>();
      foreach (var reg in registrations)
      {
        implementationList.Add(reg.Implementation);
      }

      localContainer.RegisterCollection<IResolveDeclare>(implementationList);
      localContainer.Verify();

      // ローカルコンテナから登録したハンドラのインスタンスを作成し、
      // ハンドラをファクトリに登録する。
      // この時点では、ハンドラのメソッドは呼び出さない。
      foreach (var ext in localContainer.GetAllInstances<IResolveDeclare>())
      {
        Add(ext.ResolveName, ext.ResolveType);
        if (lifeSycle == Lifestyle.Singleton)
          mContainer.Register(ext.ResolveType, () => Activator.CreateInstance(ext.ResolveType, mContainer), lifeSycle); // ハンドラの型情報をコンテナに登録する
        else
          mContainer.Register(ext.ResolveType);
      }
    }
  }
}
