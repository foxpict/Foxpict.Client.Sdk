using Foxpict.Client.Sdk.Models;

namespace Foxpict.Client.Sdk.Infra {
  public interface IContentDao {
    Content LoadContent (long contentId);

    void Update (Content content);

    void UpdateRead (long contentId);
  }
}
