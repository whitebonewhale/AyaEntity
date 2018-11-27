using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace WebConsole.KiraEntity
{

	public class RSAWrapper
	{
		private RSAWrapper() { }
		private static string xmlString = AppSettings.Configuration["RSA:XmlString"];
		public static string Encrypt(string content)
		{
			var rsa = RSA.Create();
			rsa.FromPrivateXml(xmlString);
			byte[] bs = Encoding.Default.GetBytes(content);
			var cst = rsa.Encrypt(bs, RSAEncryptionPadding.Pkcs1);
			return Convert.ToBase64String(cst);
		}

		public static string Decrypt(string content)
		{
			var rsa = RSA.Create();
			rsa.FromPrivateXml(xmlString);
			var cst = rsa.Decrypt(Convert.FromBase64String(content), RSAEncryptionPadding.Pkcs1);
			return Encoding.UTF8.GetString(cst);
		}
	}

	public static class RsaExtention
	{

		public static void FromPrivateXml(this RSA rsa, string xmlString)
		{
			RSAParameters parameters = new RSAParameters();
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(xmlString);
			if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
			{
				foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
				{
					switch (node.Name)
					{
						case "Modulus": parameters.Modulus = Convert.FromBase64String(node.InnerText); break;
						case "Exponent": parameters.Exponent = Convert.FromBase64String(node.InnerText); break;
						case "P": parameters.P = Convert.FromBase64String(node.InnerText); break;
						case "Q": parameters.Q = Convert.FromBase64String(node.InnerText); break;
						case "DP": parameters.DP = Convert.FromBase64String(node.InnerText); break;
						case "DQ": parameters.DQ = Convert.FromBase64String(node.InnerText); break;
						case "InverseQ": parameters.InverseQ = Convert.FromBase64String(node.InnerText); break;
						case "D": parameters.D = Convert.FromBase64String(node.InnerText); break;
					}
				}
			}
			else
			{
				throw new Exception("Invalid XML RSA key.");
			}

			rsa.ImportParameters(parameters);
		}

		public static string ToFromPrivateXml(this RSA rsa, bool includePrivateParameters)
		{
			RSAParameters parameters = rsa.ExportParameters(includePrivateParameters);

			if (includePrivateParameters)
			{
				return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
						Convert.ToBase64String(parameters.Modulus),
						Convert.ToBase64String(parameters.Exponent),
						Convert.ToBase64String(parameters.P),
						Convert.ToBase64String(parameters.Q),
						Convert.ToBase64String(parameters.DP),
						Convert.ToBase64String(parameters.DQ),
						Convert.ToBase64String(parameters.InverseQ),
						Convert.ToBase64String(parameters.D));
			}
			return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>",
							Convert.ToBase64String(parameters.Modulus),
							Convert.ToBase64String(parameters.Exponent));
		}

	}

	//public class RSAWrapper
	//{
	//	private RSA privateKeyRsaProvider;

	//	private readonly RSA publicKeyRsaProvider;

	//	/// <summary>
	//	/// 实例化RSAHelper
	//	/// </summary>
	//	/// <param name="rsaType">加密算法类型 RSA SHA1;RSA2 SHA256 密钥长度至少为2048</param>
	//	/// <param name="encoding">编码类型</param>
	//	/// <param name="privateKey">私钥</param>
	//	/// <param name="publicKey">公钥</param>
	//	public RSAWrapper(string privateKey,string publickkey)
	//	{
	//		if (!string.IsNullOrEmpty(privateKey))
	//		{
	//			privateKeyRsaProvider = CreateFromPrivate(privateKey);
	//		}
	//		if (!string.IsNullOrEmpty(publicKey))
	//		{
	//			publicKeyRsaProvider = CreateRsaProviderFromPublicKey(publicKey);
	//		}
	//	}

	//	/// <summary>
	//	///  解密
	//	/// </summary>
	//	/// <param name="cipherText"></param>
	//	/// <returns></returns>

	//	public string Decrypt(string cipherText)
	//	{
	//		if (privateKeyRsaProvider == null)
	//		{
	//			throw new Exception("_privateKeyRsaProvider is null");
	//		}
	//		return Encoding.UTF8.GetString(privateKeyRsaProvider.Decrypt(Convert.FromBase64String(cipherText), RSAEncryptionPadding.Pkcs1));
	//	}
	//	/// <summary>
	//	/// 加密
	//	/// </summary>
	//	/// <param name="text"></param>
	//	/// <returns></returns>
	//	public string Encrypt(string text)
	//	{
	//		if (_publicKeyRsaProvider == null)
	//		{
	//			throw new Exception("_publicKeyRsaProvider is null");
	//		}
	//		return Convert.ToBase64String(_publicKeyRsaProvider.Encrypt(Encoding.UTF8.GetBytes(text), RSAEncryptionPadding.Pkcs1));
	//	}


	//	public string en

	//	public RSA CreateFromPrivate(string privateKey)
	//	{
	//		var privateKeyBits = Convert.FromBase64String(privateKey);

	//		var rsa = RSA.Create();
	//		var rsaParameters = new RSAParameters();

	//		using (BinaryReader binr = new BinaryReader(new MemoryStream(privateKeyBits)))
	//		{
	//			byte bt = 0;
	//			ushort twobytes = 0;
	//			twobytes = binr.ReadUInt16();
	//			if (twobytes == 0x8130)
	//				binr.ReadByte();
	//			else if (twobytes == 0x8230)
	//				binr.ReadInt16();
	//			else
	//				throw new Exception("Unexpected value read binr.ReadUInt16()");

	//			twobytes = binr.ReadUInt16();
	//			if (twobytes != 0x0102)
	//				throw new Exception("Unexpected version");

	//			bt = binr.ReadByte();
	//			if (bt != 0x00)
	//				throw new Exception("Unexpected value read binr.ReadByte()");

	//			rsaParameters.Modulus = binr.ReadBytes(GetIntegerSize(binr));
	//			rsaParameters.Exponent = binr.ReadBytes(GetIntegerSize(binr));
	//			rsaParameters.D = binr.ReadBytes(GetIntegerSize(binr));
	//			rsaParameters.P = binr.ReadBytes(GetIntegerSize(binr));
	//			rsaParameters.Q = binr.ReadBytes(GetIntegerSize(binr));
	//			rsaParameters.DP = binr.ReadBytes(GetIntegerSize(binr));
	//			rsaParameters.DQ = binr.ReadBytes(GetIntegerSize(binr));
	//			rsaParameters.InverseQ = binr.ReadBytes(GetIntegerSize(binr));
	//		}

	//		rsa.ImportParameters(rsaParameters);
	//		return rsa;
	//	}
	//	public RSA CreateRsaProviderFromPublicKey(string publicKeyString)
	//	{
	//		// encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
	//		byte[] seqOid = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
	//		byte[] seq = new byte[15];

	//		var x509Key = Convert.FromBase64String(publicKeyString);

	//		// ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
	//		using (MemoryStream mem = new MemoryStream(x509Key))
	//		{
	//			using (BinaryReader binr = new BinaryReader(mem))  //wrap Memory Stream with BinaryReader for easy reading
	//			{
	//				byte bt = 0;
	//				ushort twobytes = 0;

	//				twobytes = binr.ReadUInt16();
	//				if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
	//					binr.ReadByte();    //advance 1 byte
	//				else if (twobytes == 0x8230)
	//					binr.ReadInt16();   //advance 2 bytes
	//				else
	//					return null;

	//				seq = binr.ReadBytes(15);       //read the Sequence OID
	//				if (!CompareBytearrays(seq, seqOid))    //make sure Sequence for OID is correct
	//					return null;

	//				twobytes = binr.ReadUInt16();
	//				if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
	//					binr.ReadByte();    //advance 1 byte
	//				else if (twobytes == 0x8203)
	//					binr.ReadInt16();   //advance 2 bytes
	//				else
	//					return null;

	//				bt = binr.ReadByte();
	//				if (bt != 0x00)     //expect null byte next
	//					return null;

	//				twobytes = binr.ReadUInt16();
	//				if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
	//					binr.ReadByte();    //advance 1 byte
	//				else if (twobytes == 0x8230)
	//					binr.ReadInt16();   //advance 2 bytes
	//				else
	//					return null;

	//				twobytes = binr.ReadUInt16();
	//				byte lowbyte = 0x00;
	//				byte highbyte = 0x00;

	//				if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
	//					lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
	//				else if (twobytes == 0x8202)
	//				{
	//					highbyte = binr.ReadByte(); //advance 2 bytes
	//					lowbyte = binr.ReadByte();
	//				}
	//				else
	//					return null;
	//				byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
	//				int modsize = BitConverter.ToInt32(modint, 0);

	//				int firstbyte = binr.PeekChar();
	//				if (firstbyte == 0x00)
	//				{   //if first byte (highest order) of modulus is zero, don't include it
	//					binr.ReadByte();    //skip this null byte
	//					modsize -= 1;   //reduce modulus buffer size by 1
	//				}

	//				byte[] modulus = binr.ReadBytes(modsize);   //read the modulus bytes

	//				if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
	//					return null;
	//				int expbytes = (int)binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
	//				byte[] exponent = binr.ReadBytes(expbytes);

	//				// ------- create RSACryptoServiceProvider instance and initialize with public key -----
	//				var rsa = RSA.Create();
	//				RSAParameters rsaKeyInfo = new RSAParameters
	//				{
	//					Modulus = modulus,
	//					Exponent = exponent
	//				};
	//				rsa.ImportParameters(rsaKeyInfo);

	//				return rsa;
	//			}

	//		}
	//	}
	//	private int GetIntegerSize(BinaryReader binr)
	//	{
	//		byte bt = 0;
	//		int count = 0;
	//		bt = binr.ReadByte();
	//		if (bt != 0x02)
	//			return 0;
	//		bt = binr.ReadByte();

	//		if (bt == 0x81)
	//			count = binr.ReadByte();
	//		else
	//		if (bt == 0x82)
	//		{
	//			var highbyte = binr.ReadByte();
	//			var lowbyte = binr.ReadByte();
	//			byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
	//			count = BitConverter.ToInt32(modint, 0);
	//		}
	//		else
	//		{
	//			count = bt;
	//		}

	//		while (binr.ReadByte() == 0x00)
	//		{
	//			count -= 1;
	//		}
	//		binr.BaseStream.Seek(-1, SeekOrigin.Current);
	//		return count;
	//	}

	//}
}
