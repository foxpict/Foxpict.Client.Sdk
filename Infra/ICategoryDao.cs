using System.Collections.Generic;
using Foxpict.Client.Sdk.Models;

namespace Foxpict.Client.Sdk.Infra {
  public interface ICategoryDao {

    Category LoadCategory (long categoryId, int offsetSubCategory = 0, int limitSubCategory = Constant.MAXLIMIT, int offsetContent = 0);

    Category LoadParentCategory (long categoryId);

    /// <summary>
    /// 条件に一致するカテゴリ一覧を取得します
    /// </summary>
    /// <param name="albumCategory">条件にアルバムカテゴリフラグの状態を含めます。nullの場合は、条件に含めません。</param>
    /// <param name="labelId">条件にラベル(AND)を含めます。nullの場合は、条件に含めません。</param>
    /// <returns></returns>
    List<Category> FindCategory (bool? albumCategory, long[] labelId);
  }
}
