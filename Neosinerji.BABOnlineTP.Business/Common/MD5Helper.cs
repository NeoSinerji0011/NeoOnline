﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Neosinerji.BABOnlineTP.Business.Common
{
    public static class MD5Helper
    {
        public static string GetMd5Hash(string input)
        {
            string result = String.Empty;

            try
            {
                #region Get MD5

                using (MD5 md5Hash = MD5.Create())
                {
                    // Convert the input string to a byte array and compute the hash. 
                    byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                    // Create a new Stringbuilder to collect the bytes 
                    // and create a string.
                    StringBuilder sBuilder = new StringBuilder();

                    // Loop through each byte of the hashed data  
                    // and format each one as a hexadecimal string. 
                    for (int i = 0; i < data.Length; i++)
                    {
                        sBuilder.Append(data[i].ToString("x2"));
                    }

                    // Return the hexadecimal string. 
                    result = sBuilder.ToString();
                }

                #endregion
            }
            catch (Exception)
            { }

            return result;
        }

        public static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            bool result = false;

            try
            {
                #region COMPRARER
                // Hash the input. 
                string hashOfInput = GetMd5Hash(input);

                // Create a StringComparer an compare the hashes.
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;

                if (0 == comparer.Compare(hashOfInput, hash))
                {
                    return true;
                }
                else
                {
                    return false;
                }
                #endregion
            }
            catch (Exception)
            { }

            return result;
        }
    }
}
