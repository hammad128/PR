using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace PublicRelationWeb.Common
{
    public class SecureQueryString : NameValueCollection
    {

        #region Variables



        public static string QueryStringVar = "data";

        private readonly byte[] IV = new byte[8] { 240, 3, 45, 29, 0, 76, 173, 59 };

        private const string cryptoKey = "5A4n3n2u1a0l9T8e7a6m5M4e3e2t1";

        private const string timeStampKey = "__TimeStamp__";



        #endregion



        #region Constructor



        public SecureQueryString()

            : base()
        {

        }



        public SecureQueryString(string encryptedString)
        {

            deserialize(decrypt(encryptedString));

            // Compare the Expiration Time with the current Time to ensure

            // that the queryString has not expired.

            if (DateTime.Compare(ExpireTime, DateTime.Now) < 0)
            {

                throw new ExpiredQueryStringException();

            }

        }



        public SecureQueryString(string encryptedString, string ecryptionKey)
        {

            deserialize(decrypt(encryptedString, ecryptionKey));

            // Compare the Expiration Time with the current Time to ensure

            // that the queryString has not expired.

            if (DateTime.Compare(ExpireTime, DateTime.Now) < 0)
            {

                throw new ExpiredQueryStringException();

            }

        }



        #endregion



        #region Methods



        public string decrypt(string encryptedQueryString)
        {

            try
            {

                byte[] buffer = Convert.FromBase64String(encryptedQueryString.Replace(" ", "+"));

                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

                MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();

                des.Key = MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(cryptoKey));

                des.IV = IV;

                return Encoding.ASCII.GetString(des.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length));



            }

            catch (CryptographicException)
            {

                throw new InvalidQueryStringException();

            }

            catch (FormatException)
            {

                throw new InvalidQueryStringException();

            }

        }

        public string decrypt(string encryptedQueryString, string ecryptionKey)
        {

            try
            {

                byte[] buffer = Convert.FromBase64String(encryptedQueryString.Replace(" ", "+"));

                TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

                MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();

                des.Key = MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(ecryptionKey));

                des.IV = IV;

                return Encoding.ASCII.GetString(des.CreateDecryptor().TransformFinalBlock(buffer, 0, buffer.Length));



            }

            catch (CryptographicException)
            {

                throw new InvalidQueryStringException();

            }

            catch (FormatException)
            {

                throw new InvalidQueryStringException();

            }

        }



        public string encrypt(string serializedQueryString)
        {

            byte[] buffer = Encoding.ASCII.GetBytes(serializedQueryString);

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();

            des.Key = MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(cryptoKey));

            des.IV = IV;

            return Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length));

        }

        public string encrypt(string serializedQueryString, string EcriptionKey)
        {

            byte[] buffer = Encoding.ASCII.GetBytes(serializedQueryString);

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();

            MD5CryptoServiceProvider MD5 = new MD5CryptoServiceProvider();

            des.Key = MD5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(EcriptionKey));

            des.IV = IV;

            return Convert.ToBase64String(des.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length));

        }



        private void deserialize(string decryptedQueryString)
        {

            string[] nameValuePairs = decryptedQueryString.Split('&');

            for (int i = 0; i < nameValuePairs.Length; i++)
            {

                string[] nameValue = nameValuePairs[i].Split('=');

                if (nameValue.Length == 2)
                {

                    base.Add(nameValue[0], nameValue[1]);

                }

            }



            if (base[timeStampKey] != null)

                _expireTime = DateTime.Parse(base[timeStampKey]);

        }



        private string serialize()
        {

            StringBuilder sb = new StringBuilder();

            foreach (string key in base.AllKeys)
            {

                sb.Append(key);

                sb.Append('=');

                sb.Append(base[key]);

                sb.Append('&');

            }





            sb.Append(timeStampKey);

            sb.Append('=');

            sb.Append(_expireTime);



            return sb.ToString();

        }



        #endregion



        #region Property



        public string EncryptedString
        {

            get
            {

                return HttpUtility.UrlEncode(encrypt(serialize()));

            }

        }



        private DateTime _expireTime = DateTime.MaxValue;

        // The timestamp in which the EncryptedString should expire



        public DateTime ExpireTime
        {

            get
            {

                return _expireTime;

            }

            set
            {

                _expireTime = value;

            }

        }

        // Returns the EncryptedString property.



        public override string ToString()
        {

            return EncryptedString;

        }



        #endregion

    }



    public class ExpiredQueryStringException : System.Exception
    {

        public ExpiredQueryStringException() : base() { }

    }



    public class InvalidQueryStringException : System.Exception
    {

        public InvalidQueryStringException() : base() { }

    }






}
