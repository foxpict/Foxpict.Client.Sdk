using Foxpict.Client.Sdk.Models;
using RestSharp;

namespace Foxpict.Client.Sdk.Dao {
  public abstract class DaoBase {
    protected readonly RestClient mClient;

    protected readonly string mServiceServerUrl;

    public DaoBase (AppSettings appSettings) {
      mServiceServerUrl = appSettings.ServiceServerUrl;
      mClient = new RestClient (mServiceServerUrl);
    }
  }
}
