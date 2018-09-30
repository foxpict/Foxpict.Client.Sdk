using Foxpict.Client.Sdk.Models;

namespace Foxpict.Client.Sdk.Infra {
  public interface ICategoryDao {

    Category LoadCategory (long categoryId, int offsetSubCategory = 0, int limitSubCategory = Constant.MAXLIMIT, int offsetContent = 0);

    Category LoadParentCategory (long categoryId);
  }
}
