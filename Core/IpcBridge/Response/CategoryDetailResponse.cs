using Foxpict.Client.Sdk.Models;

namespace Foxpict.Client.Sdk.IpcApi.Response {
  public class CategoryDetailResponse {
    public Category Category { get; set; }

    public Category[] SubCategory { get; set; }

    public Content[] Content { get; set; }
  }
}
