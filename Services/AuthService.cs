using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using bugtracker.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace bugtracker.Services
{

    public interface IUserService
    {
        UserModel Authenticate(string username, string password);
    }

    public class AuthService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<UserModel> _users = new List<UserModel>
        {
            new UserModel { email = "test", password = "test" }
        };

        private IConfiguration configuration;
        public AuthService(IConfiguration config){
            configuration = config;
        }



        // private readonly AppSettings _appSettings;


        // public UserService(IOptions<AppSettings> appSettings)
        // {
        //     _appSettings = appSettings.Value;
        // }

        public UserModel Authenticate(string username, string password)
        {

            //iterates through users list and returns
            //one where it matches email and password
            var user = _users.SingleOrDefault(x => x.email == username && x.password == password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            // var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var key = Encoding.ASCII.GetBytes(configuration.GetSection("Secret").Value);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.id.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.token = tokenHandler.WriteToken(token);

            user.password = null;
            return user;
        }

    }

}