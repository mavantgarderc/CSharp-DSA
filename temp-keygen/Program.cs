using System;
using System.Security.Cryptography;

Console.WriteLine("Generating clean RSA keys...");

using var rsa = RSA.Create(2048);
var publicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey());
var privateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey());

Console.WriteLine("=== COPY ONLY THE KEYS BELOW ===");
Console.WriteLine();
Console.WriteLine("PUBLIC KEY:");
Console.WriteLine(publicKey);
Console.WriteLine();
Console.WriteLine("PRIVATE KEY:");
Console.WriteLine(privateKey);
Console.WriteLine();
Console.WriteLine("=== END KEYS ===");
