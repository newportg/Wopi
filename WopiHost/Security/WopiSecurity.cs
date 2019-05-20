using com.microsoft.dx.officewopi.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace com.microsoft.dx.officewopi.Security
{
    /// <summary>
    /// Class handles token generation and validation for the WOPI host
    /// </summary>
    public class WopiSecurity
    {
        ///// <summary>
        ///// Generates an access token specific to a user and file
        ///// </summary>
        //public static bool ValidateToken(string tokenString, string container, string docId)
        //{
        //    // Initialize the token handler and validation parameters
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var tokenValidation = new TokenValidationParameters()
        //    {
        //        ValidAudience = SettingsHelper.Audience,
        //        ValidIssuer = SettingsHelper.Audience,
        //        IssuerSigningToken = new X509SecurityToken(getCert())
        //    };

        //    try
        //    {
        //        // Try to validate the token
        //        SecurityToken token = null;
        //        var principal = tokenHandler.ValidateToken(tokenString, tokenValidation, out token);
        //        return (principal.HasClaim("container", container) &&
        //            principal.HasClaim("docid", docId));
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        return false;
        //    }
        //}

        ///// <summary>
        ///// Extracts the user information from a provided access token
        ///// </summary>
        //public static string GetUserFromToken(string tokenString)
        //{
        //    // Initialize the token handler and validation parameters
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var tokenValidation = new TokenValidationParameters()
        //    {
        //        ValidAudience = SettingsHelper.Audience,
        //        ValidIssuer = SettingsHelper.Audience,
        //        IssuerSigningToken = new X509SecurityToken(getCert())
        //    };

        //    try
        //    {
        //        // Try to extract the user principal from the token
        //        SecurityToken token = null;
        //        var principal = tokenHandler.ValidateToken(tokenString, tokenValidation, out token);
        //        return principal.Identity.Name;
        //    }
        //    catch (Exception)
        //    {
        //        return String.Empty;
        //    }
        //}

        ///// <summary>
        ///// Private token handler used in instance classes
        ///// </summary>
        //private JwtSecurityTokenHandler tokenHandler;

        /// <summary>
        /// Generates an access token for the user and file
        /// </summary>
        //public JwtSecurityToken GenerateToken(string user, string container, string docId)
        //{
        //    var now = DateTime.UtcNow;
        //    tokenHandler = new JwtSecurityTokenHandler();
        //    var signingCert = getCert();
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[]
        //            {
        //                new Claim(ClaimTypes.Name, user),
        //                new Claim("container", container),
        //                new Claim("docid", docId)
        //        }),
        //        TokenIssuerName = SettingsHelper.Audience,
        //        AppliesToAddress = SettingsHelper.Audience,
        //        Lifetime = new Lifetime(now, now.AddHours(1)),
        //        EncryptingCredentials = new X509EncryptingCredentials(signingCert),
        //        SigningCredentials = new X509SigningCredentials(signingCert)
        //    };
        //    var token = (JwtSecurityToken)tokenHandler.CreateToken(tokenDescriptor);
        //    return token;
        //}

        ///// <summary>
        ///// Converts the JwtSecurityToken to a Base64 string that can be used by the Host
        ///// </summary>
        //public string WriteToken(JwtSecurityToken token)
        //{
        //    return tokenHandler.WriteToken(token);
        //}

        ///// <summary>
        ///// Gets the self-signed certificate used to sign the access tokens
        ///// </summary>
        //private static X509Certificate2 getCert()
        //{
        //    // Load Certificate from file
        //    //var certPath = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~"), "hub_wildcard_cert_12_2018.pfx");
        //    //X509Certificate2 cert = new X509Certificate2(certPath, ConfigurationManager.AppSettings["CertPfxPassword"]);

        //    //var certfile = System.IO.File.OpenRead(certPath);
        //    //var certificateBytes = new byte[certfile.Length];
        //    //certfile.Read(certificateBytes, 0, (int)certfile.Length);

        //    //var cert = new X509Certificate2( certPath,
        //    //    //certificateBytes,
        //    //    //ConfigurationManager.AppSettings["CertPassword"],
        //    //    ConfigurationManager.AppSettings["CertPfxPassword"],
        //    //    X509KeyStorageFlags.Exportable |
        //    //    X509KeyStorageFlags.MachineKeySet |
        //    //    X509KeyStorageFlags.PersistKeySet);


        //    // Load Certificate from environment variables
        //    var certificateBytes = System.Environment.GetEnvironmentVariable("CertPfxBase64");
        //    var certPwd = System.Environment.GetEnvironmentVariable("CertPfxPassword");

        //    byte[] p8bytes = Convert.FromBase64String(certificateBytes);
        //    var cert = new X509Certificate2(p8bytes, certPwd);
        //    Console.WriteLine(cert.FriendlyName);

        //    // Load Certificate from store
        //    //X509Certificate2 cert = new X509Certificate2();
        //    //X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
        //    //certStore.Open(OpenFlags.ReadOnly);
        //    //X509Certificate2Collection certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, "FCA004BF226B569223ACCC3ED10345CE5700DAFF", false);
        //    //// Get the first cert with the thumbprint
        //    //if (certCollection.Count > 0)
        //    //{
        //    //    cert = certCollection[0];
        //    //    // Use certificate
        //    //    Console.WriteLine(cert.FriendlyName);
        //    //}
        //    //certStore.Close();

        //    return cert;
        //}


        //----------------------------------------------

        /// <summary>
        /// Private token handler used in instance classes
        /// </summary>
        private JwtSecurityTokenHandler tokenHandler;

        public JwtSecurityToken GenerateToken(string user, string container, string docId)
        {
            var now = DateTime.UtcNow;
            tokenHandler = new JwtSecurityTokenHandler();
            var signingKey = getCert();
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>()
            {
                new Claim( ClaimTypes.Name, "Gary.Newport@knightfrank.com"),
                new Claim( "container", "container"),
                new Claim( "docid", "DocumentId")
            }, "Custom");

            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                TokenIssuerName = "https://wopi.hub.knightfrank.com",
                AppliesToAddress = "https://wopi.hub.knightfrank.com",
                Lifetime = new Lifetime(now, now.AddHours(1)),
                Subject = claimsIdentity,
                SigningCredentials = signingCredentials,
            };

            var token = (JwtSecurityToken)tokenHandler.CreateToken(securityTokenDescriptor);
            return token;
        }

        /// <summary>
        /// Generates an access token specific to a user and file
        /// </summary>
        public bool ValidateToken(string tokenString, string container, string docId)
        {
            return true;

            //var tokenValidationParameters = new TokenValidationParameters()
            //{
            //    ValidAudiences = new string[]
            //    {
            //        "https://wopi.hub.knightfrank.com"
            //    },
            //    ValidIssuers = new string[]
            //    {
            //        "https://wopi.hub.knightfrank.com"
            //    },
            //    IssuerSigningKey = getCert()
            //};

            //try
            //{
            //    SecurityToken validatedToken;
            //    var principal = tokenHandler.ValidateToken(tokenString, tokenValidationParameters, out validatedToken);

            //    Console.WriteLine(validatedToken.ToString());
            //    Console.WriteLine("HasClaim Container :{0}", principal.HasClaim("container", container).ToString());
            //    Console.WriteLine("HasClaim Container :{0}", principal.HasClaim("docid", docId).ToString());

            //    return (principal.HasClaim("container", container) && principal.HasClaim("docid", docId));
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Invalid Token : {0}", ex.Message);
            //    return false;
            //}
        }

        public static string GetUserFromToken(string tokenString)
        {
            return "gary.newport@knightfrank.com";

            //// Initialize the token handler and validation parameters
            //var tokenHandler = new JwtSecurityTokenHandler();
            //var tokenValidationParameters = new TokenValidationParameters()
            //{
            //    ValidAudiences = new string[]
            //     {
            //        "https://wopi.hub.knightfrank.com"
            //     },
            //    ValidIssuers = new string[]
            //     {
            //        "https://wopi.hub.knightfrank.com"
            //     },
            //    IssuerSigningKey = getCert()
            //};

            //try
            //{
            //    // Try to extract the user principal from the token
            //    SecurityToken validatedToken;
            //    var principal = tokenHandler.ValidateToken(tokenString, tokenValidationParameters, out validatedToken);
            //    return principal.Identity.Name;
            //}
            //catch (Exception)
            //{
            //    return String.Empty;
            //}
        }

        /// <summary>
        /// Converts the JwtSecurityToken to a Base64 string that can be used by the Host
        /// </summary>
        public string WriteToken(JwtSecurityToken token)
        {
            return tokenHandler.WriteToken(token);
        }

        public static SymmetricSecurityKey getCert()
        {
            var plainTextSecurityKey = "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";
            var signingKey = new InMemorySymmetricSecurityKey(Encoding.UTF8.GetBytes(plainTextSecurityKey));
            return signingKey;
        }
    }
}
