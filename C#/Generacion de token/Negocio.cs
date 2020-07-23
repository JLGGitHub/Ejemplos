using EPM.DigiTools.Autorizacion.Entities.DTO;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class JwtTokenCreator
{
	private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

	public JwtTokenCreator()
	{
		this._jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
	}

	public string GenerateJwtToken(GenerarToken modelo)
	{
		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("d9db4fc5-4ddd-4309-97e3-4d0e74fe01f9"));

		// Creamos los claims (pertenencias, características) del usuario
		var claims = new[]
		{
			new Claim(ClaimTypes.Name, modelo.Cuenta),
			new Claim(ClaimTypes.Email, modelo.Correo)
		};

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Audience = "Autorizacion:debe75a4-9780-42de-8438-8c4c8cdd61a1", 
			Issuer = "https://api-epm-dev-autorizacion.azurewebsites.net/", 
			Expires = DateTime.UtcNow.AddHours(1),
			SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature)
		};
		var tokenHandler = new JwtSecurityTokenHandler();
		var createdToken = tokenHandler.CreateToken(tokenDescriptor);

		return this._jwtSecurityTokenHandler.WriteToken(createdToken);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	public string GenerateRefreshToken()
	{
		var randomNumber = new byte[32];
		using (var rng = RandomNumberGenerator.Create())
		{
			rng.GetBytes(randomNumber);
			return Convert.ToBase64String(randomNumber);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="token"></param>
	/// <returns></returns>
	public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
	{
		var tokenValidationParameters = new TokenValidationParameters
		{
			ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
			ValidateIssuer = false,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("d9db4fc5-4ddd-4309-97e3-4d0e74fe01f9")),
			ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
		};

		var tokenHandler = new JwtSecurityTokenHandler();
		SecurityToken securityToken;
		var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
		var jwtSecurityToken = securityToken as JwtSecurityToken;
		if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			throw new SecurityTokenException("Invalid token");

		return principal;
	}

}