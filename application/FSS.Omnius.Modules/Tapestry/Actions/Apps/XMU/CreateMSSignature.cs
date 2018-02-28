using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FSS.Omnius.Modules.CORE;
using Org.BouncyCastle.Utilities.IO.Pem;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;

namespace FSS.Omnius.Modules.Tapestry.Actions.other
{
    [OtherRepository]
    class CreateMSSignature : Action
    {
        public override int Id => 1985;

        public override string[] InputVar => new string[] { "s$Data" };

        public override string Name => "Create MoneyServer signature";

        public override string[] OutputVar => new string[] { "Result" };

        public override int? ReverseActionId => null;

        private const string XMUPrivateKey = @"-----BEGIN PRIVATE KEY-----
MIHuAgEAMBAGByqGSM49AgEGBSuBBAAjBIHWMIHTAgEBBEIAxqQzDeo8g4qn07/l
SHBpaRCalQ2x/I9QhE4poHK5pTG5XZLQjmrD1qvJeJzd1SOq/PwmcTuvvhubMMmP
f1P09wShgYkDgYYABABGiLsZe68bflN/obCraQvIB53O5WyDt5z8Ji6QoRbB0UOg
cqGWGqL8Tg2JU8rums+wO3hdBmn9t7nqr8telsQUHwH6DLeXvel7PATynY3Kj3Og
sE0FP1G8FoMc/5V0lRLGpwyUfH8rT9H/VYC4l2gM58TCmEZ4mu1ygy7mg7vhDZ4d
2A==
-----END PRIVATE KEY-----";

        public override void InnerRun(Dictionary<string, object> vars, Dictionary<string, object> outputVars, Dictionary<string, object> InvertedInputVars, Message message)
        {
            string timestamp = GetCurrentTimestamp();
            string body = vars.ContainsKey("Data") ? (string)vars["Data"] : "";

            byte[] data = Encoding.UTF8.GetBytes(string.Format("{0}@{1}", body, timestamp));

            StringReader reader = new StringReader(XMUPrivateKey);
            PemObject pem = (new PemReader(reader)).ReadPemObject();
            AsymmetricKeyParameter key = PrivateKeyFactory.CreateKey(pem.Content);

            ISigner signer = SignerUtilities.GetSigner("SHA256withECDSA");
            signer.Init(true, key);
            signer.BlockUpdate(data, 0, data.Length);

            byte[] result = signer.GenerateSignature();
            byte[] encodedBytes;
            using (MemoryStream encStream = new MemoryStream())
            {
                Base64.Encode(result, 0, result.Length, encStream);
                encodedBytes = encStream.ToArray();
            }

            List<string> headers = new List<string>();
            headers.Add(string.Format("X-MS-Signature: {0}", Encoding.ASCII.GetString(encodedBytes)));
            headers.Add(string.Format("X-MS-Timestamp: {0}", timestamp));

            outputVars["Result"] = headers;
        }

        private string GetCurrentTimestamp()
        {
            TimeSpan span = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
            double unixTime = span.TotalSeconds;
            return string.Format("{0}", System.Math.Floor(unixTime));
        }
    }
}
