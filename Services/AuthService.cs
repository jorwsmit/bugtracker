using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Data;
using bugtracker.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace bugtracker.Services
{

    public interface IAuthService
    {
        UserModel Authenticate(string email, string password);
    }

    public class AuthService : IAuthService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<UserModel> _users = new List<UserModel>
        {
            new UserModel { email = "test", password = "test" }
        };

        private readonly IConfiguration _configuration;
        public AuthService(IConfiguration configuration)
        {
            Debug.WriteLine("\n\n\n-------AuthService Contructor--------");
            _configuration = configuration;
        }



        // private readonly AppSettings _appSettings;


        // public UserService(IOptions<AppSettings> appSettings)
        // {
        //     _appSettings = appSettings.Value;
        // }

        public UserModel Authenticate(string email, string password)
        {

            Debug.WriteLine("\n\n\n-------Calling Authenticate--------");
            //iterates through users list and returns
            //one where it matches email and password
            var user = _users.SingleOrDefault(x => x.email == email && x.password == password);

            try
            {
                string connStr = _configuration["ConnectionString"];
            MySqlConnection conn = new MySqlConnection(connStr);
                Debug.WriteLine("\n\n\n-------Connecting to DB--------");
                conn.Open();

                string sql = "select email, first_name, last_name, photo_url from bugtracker.users where email='"+email+"';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                MySqlDataReader dr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(dr);
                int numberOfResults = dt.Rows.Count;

                Debug.WriteLine("\n\n------Number of Rows------");
                Debug.WriteLine(numberOfResults);
                conn.Close();
            Debug.WriteLine("Done.");

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            



            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            // var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            var key = Encoding.ASCII.GetBytes(_configuration.GetSection("Secret").Value);

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