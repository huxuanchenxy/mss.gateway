using MSS.API.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSS.Web.Auth.Provider
{
    public interface IAPITokenDataProvider
    {
        Task<TokenResponse> GetApiTokenAsync(TokenRequest req);
        Task<TokenResponse> GetApiNewTokenAsync(TokenRequest req);
    }
}
