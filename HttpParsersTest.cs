using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

werwer

using System.Linq;
using Logger.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proxy.Parsers;

namespace Proxy_UnitTest
{
    [TestClass]
    public class HttpParsersTest
    {
        private readonly string _postContent =
            @"----------------------------7d71e911240248
Content-Disposition: form-data; name=""james""

Hi
----------------------------7d71e911240248
Content-Disposition: form-data; name=""sb""

CLICK
----------------------------7d71e911240248
Content-Disposition: form-data; name=""sdf""

bill
----------------------------7d71e911240248
Content-Disposition: form-data; name=""somename""


----------------------------7d71e911240248--
";

        [TestMethod]
        public void TestQueryParams()
        {
            var streamRead = new MemoryStream(EncodingExt.ANSI.GetBytes(_postContent));
            var streamWrite = new MemoryStream(EncodingExt.ANSI.GetBytes(_postContent));
            var reader = new BinaryReader(streamRead);
            var writer = new BinaryWriter(streamWrite);
            var queryParams = new QueryParams { BinaryMode = true };
            Assert.AreEqual(queryParams.BoundaryString.Length, 40);
            queryParams.BoundaryString = "--------------------------7d71e911240248";
            Assert.AreEqual(queryParams.BoundaryString.Length, 40);
            if (!queryParams.Parse(reader, writer, _postContent.Length))
            {
                Assert.Fail(queryParams.ParseError);  
            }
            string _parsedContent = queryParams.ToString();
            Assert.AreEqual(_postContent, _parsedContent);
        }

        [TestMethod]
        public void TestByteArrayStorageInANSI()
        {
            var array = new byte[256];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (byte)i;
            }
            Assert.IsTrue(checkStringStorage(array, EncodingExt.ANSI));
            var rnd = new Random();
            array = new byte[100000];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = (byte)rnd.Next();
            }
            Assert.IsTrue(checkStringStorage(array, EncodingExt.ANSI));
        }

        private bool checkStringStorage(byte[] byteArray, Encoding encoding)
        {
            var s = encoding.GetString(byteArray);
            var storageArray = encoding.GetBytes(s);
            if (byteArray.Length != storageArray.Length) return false;
            for (int i = 0; i < byteArray.Length; i++)
            {
                if (byteArray[i] != storageArray[i]) return false;
            }
            return true;
        }
    }
}
