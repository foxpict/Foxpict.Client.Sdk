using System.Collections.Generic;
using Foxpict.Client.Sdk.Models;

namespace Foxpict.Client.Sdk.Infra {
  public interface ILabelDao {
    ICollection<Label> LoadLabel ();

    Label LoadLabel (long labelId);

    ICollection<Label> LoadRoot ();

    ICollection<Category> LoadLabelLinkCategory (string query, int offset, int limit);
  }
}
