using TuongTLCBE.Business;

namespace TuongTLCBE.Tests;

public class DecodeTokenTests
{
    [Fact]
    public void Decode_ReturnsError_WhenClaimNotFound()
    {
        var decodeToken = new DecodeToken();
        var token = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIn0.dozjgNryP4J3jVmNHl0w5N_XgL0n3I9PlFUP0THsR8U";
        var result = decodeToken.Decode(token, "username");
        Assert.Equal("Error!!!", result);
    }
}
