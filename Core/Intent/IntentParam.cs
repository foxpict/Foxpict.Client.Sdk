using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foxpict.Client.Sdk.Core.Intent
{
  public class IntentParam
  {
    /// <summary>
    /// 
    /// </summary>
    public string IntentName { get; }

    /// <summary>
    /// 
    /// </summary>
    public object ExtraData { get; set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="intentName"></param>
    public IntentParam(string intentName)
    {
      this.IntentName = intentName;
    }
  }
}
