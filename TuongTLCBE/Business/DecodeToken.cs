using System.IdentityModel.Tokens.Jwt;

namespace TuongTLCBE.Business;
public class DecodeToken
{
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public DecodeToken()
    {
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public string Decode(string token, string nameClaim)
    {
        System.Security.Claims.Claim? claim = _tokenHandler.ReadJwtToken(token).Claims.FirstOrDefault(selector => selector.Type.ToString().Equals(nameClaim));
        return claim != null ? claim.Value : "Error!!!";
    }

}



