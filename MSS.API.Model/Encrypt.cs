using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace MSS.API.Model
{
    /// <summary>
    /// 密码加解密
    /// </summary>
    public class Encrypt
    {
        private byte[] IV = { 0x1, 0x3, 0x5, 0x7, 0x9, 0x8, 0x10, 0x12, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF };
        /// <summary>
        /// 此.net core版本必须16位，之前.netFramework只要8为就足够了
        /// </summary>
        private byte[] KEY = { 0x61, 0x62, 0x63, 0x64, 0x65, 0x66, 0x67, 0x68, 0x71, 0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78 };
        private int rmBlockSize = 128;

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="originalString">原始字符串</param>
        /// <param name="randomNum">随机数</param>
        /// <returns>加密后的字符串</returns>
        public string DoEncrypt(string originalString, int randomNum)
        {
            string str = originalString + randomNum.ToString();
            byte[] bytValue = Encoding.ASCII.GetBytes(str.ToCharArray());
            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            RijndaelManaged rm = new RijndaelManaged();
            rm.BlockSize = rmBlockSize;

            CryptoStream cs = new CryptoStream(ms, rm.CreateEncryptor(KEY, IV), CryptoStreamMode.Write);

            try
            {
                cs.Write(bytValue, 0, bytValue.Length);
                cs.FlushFinalBlock();
                byte[] bytReturn = ms.ToArray();
                cs.Close();
                ms.Close();
                return (Convert.ToBase64String(bytReturn));
            }
            catch (Exception)
            {
                return "";
            }

        }

        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="encryptedString">加密后的字符串</param>
        /// <param name="randomNum">随机数</param>
        /// <returns>原始字符串</returns>
        public string DoDecrypt(string encryptedString, int randomNum)
        {
            byte[] byteArray = Convert.FromBase64String(encryptedString);

            System.IO.MemoryStream ms = new System.IO.MemoryStream(byteArray);
            RijndaelManaged rm = new RijndaelManaged();
            rm.BlockSize = rmBlockSize;

            CryptoStream cs = new CryptoStream(ms, rm.CreateDecryptor(KEY, IV), CryptoStreamMode.Read);

            System.IO.StreamReader sr = new System.IO.StreamReader(cs);

            string rtn = sr.ReadLine();
            sr.Close();
            cs.Close();

            return rtn.Substring(0, rtn.Length - randomNum.ToString().Length);

        }
    }
}
