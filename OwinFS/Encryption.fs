module Encryption

open System.IO
open System.Security.Cryptography
open System.Text
open Newtonsoft.Json

let ConvertKeyIVToJSON key IV =
    ""

let ConvertJSONKeyToXML json =
    ""

let EncryptWithPK pk (content:string) =
    let rsa = new RSACryptoServiceProvider()
    rsa.FromXmlString(pk)
    let bytes = Encoding.UTF8.GetBytes(content)
    rsa.Encrypt(bytes ,false)

let GenerateEncryptor stream =
    let rij = Rijndael.Create()
    rij.GenerateIV()
    rij.GenerateKey()
    let key, IV = rij.Key, rij.IV
    let encryptor = rij.CreateEncryptor()
    use memStream = new MemoryStream()
    (key, IV, memStream :> Stream)
    
type KeyIV = { Key : byte array; IV : byte array }

let StringifyKeyIV key iv =
    let keyIV = { Key = key; IV = iv }
    JsonConvert.SerializeObject(keyIV)