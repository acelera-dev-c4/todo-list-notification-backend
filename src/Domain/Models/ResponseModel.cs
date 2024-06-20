public class TokenData
{
    public string Token { get; set; } = "";
    public DateTime Expiration { get; set; }
}

public class UserData
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}

public class ResponseModel
{
    public TokenData Token { get; set; } = new TokenData();
    public UserData UserData { get; set; } = new UserData();
}