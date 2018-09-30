using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foxpict.Client.Sdk.Infra.Resolver
{
  public interface IResolveDeclare
  {
    /// <summary>
    ///
    /// </summary>
    string ResolveName { get; }

    /// <summary>
    ///
    /// </summary>
    Type ResolveType { get; }
  }
}
