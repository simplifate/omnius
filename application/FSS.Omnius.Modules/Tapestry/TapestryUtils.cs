using Newtonsoft.Json.Linq;
using System;
using System.Web;
using System.Collections.Generic;

namespace FSS.Omnius.Modules.Tapestry
{
    class TapestryUtils
    {
        static string decSeparator = System.Windows.Forms.Application.CurrentCulture.NumberFormat.NumberDecimalSeparator;

        public static string GetServerHostName()
        {
            string port = HttpContext.Current.Request.ServerVariables["SERVER_PORT"];
            if (port == null || port == "80" || port == "443")
            {
                port = "";
            }
            else
            {
                port = ":" + port;
            }

            string protocol = HttpContext.Current.Request.ServerVariables["SERVER_PORT_SECURE"];
            if (protocol == null || protocol == "0")
            {
                protocol = "http://";
            }
            else
            {
                protocol = "https://";
            }

            return protocol + HttpContext.Current.Request.ServerVariables["SERVER_NAME"] + port;
        }

        public static double ParseDouble(object input)
        {
            if (input is string)
            {
                string inputWithDecimalDot = ((string)input).Replace(decSeparator == "," ? '.' : ',', decSeparator == "," ? ',' : '.');
                return Convert.ToDouble(inputWithDecimalDot);
            }
            else if (input is JValue)
            {
                return ((JValue)input).ToObject<double>();
            }
            else
            {
                return Convert.ToDouble(input);
            }
        }

        /// <summary>  
        ///  Metoda pro prevod double hodnoty na string odpovidajici hexadecimalni hodnoty.
        /// </summary>  
        /// <param name="value">hodnota v double</param>
        /// <param name="maxDecimals">maximalni pocet iterovanych cislic hodnoty</param>
        /// <returns>string hodnota v hexadecimalni podobe</returns>
        public static string DoubleToHex(double value, int maxDecimals)
        {
            string result = string.Empty;
            if (value < 0)
            {
                result += "-";
                value = -value;
            }
            if (value > ulong.MaxValue)
            {
                result += double.PositiveInfinity.ToString();
                return result;
            }
            ulong trunc = (ulong)value;
            result += trunc.ToString("X");
            value -= trunc;
            if (value == 0)
            {
                return result;
            }
            result += ".";
            byte hexdigit;
            while ((value != 0) && (maxDecimals != 0))
            {
                value *= 16;
                hexdigit = (byte)value;
                result += hexdigit.ToString("X");
                value -= hexdigit;
                maxDecimals--;
            }
            return result;
        }

        /// <summary>
        /// Převede několikaúrovňový JObject na slovník v podstatě na na formát JProperty.Name:JProperty.Value.
        /// Příklad: "money":{"actual":{"btc":{"amount":... na money_actual_btc_amount
        /// </summary>
        /// <param name="token">Parsovaný objekt</param>
        /// <param name="parsedColumns">Kolekce výsledných řádků</param>
        /// <param name="colName">Nositel jména sloupce, měl by začít null</param>
        public static void ParseJObjectRecursively(JObject token, Dictionary<string, object> parsedColumns, string colName = null)
        {
            foreach (JProperty item in token.Properties())
            {
                if (item.Value.Type == JTokenType.Object)
                {
                    ParseJObjectRecursively((JObject)item.Value, parsedColumns, (string.IsNullOrEmpty(colName)) ? item.Name : colName + "_" + item.Name);
                }
                else
                {
                    parsedColumns.Add((string.IsNullOrEmpty(colName)) ? item.Name : colName + "_" + item.Name, item.Value.ToObject<object>());
                }
            }
        }

    }
}
