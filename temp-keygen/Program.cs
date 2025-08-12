using System;
using System.Security.Cryptography;

Console.WriteLine("=== RSA Key Generator ===");
Console.WriteLine();

// Generate RSA key pair
using var rsa = RSA.Create(2048);

var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());

Console.WriteLine("PUBLIC KEY (add to appsettings.json):");
Console.WriteLine(publicKey);
Console.WriteLine();

Console.WriteLine("PRIVATE KEY (store securely for JWT signing):");
Console.WriteLine(privateKey);
Console.WriteLine();

Console.WriteLine("=== Instructions ===");
Console.WriteLine("1. Copy the PUBLIC KEY above");
Console.WriteLine("2. Add it to your appsettings.json in the JWT:PublicKey field");
Console.WriteLine("3. Store the PRIVATE KEY securely (you'll need it for signing JWTs)");
Console.WriteLine();
Console.WriteLine("Press any key to exit...");
Console.ReadKey();
