using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foxpict.Client.Sdk.Workflow
{
  public class ReqInvalidatePreviewParameter
  {
    /** オペレーション名 */
    public string Operation;

    /** コンテントID(任意) */
    public long ContentId;

    /** コンテント一覧での位置(任意) */
    public int Position;
  }
}
