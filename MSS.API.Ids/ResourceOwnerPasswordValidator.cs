using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Threading.Tasks;

namespace MSS.API.Ids
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {


        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {

                context.Result = new GrantValidationResult(
                 subject: "userid",
                 authenticationMethod: "custom"
                 //claims: GetUserClaims(user)
                 );
                return;

                //验证失败
                //context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom credential");

            }
            catch (Exception ex)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username or password");
            }
        }

    }
}