//using System;
//using System.Diagnostics;
//using NUnit.Framework;

//namespace UnitTests.RuleExperiments.DP
//{
//    class MailAdress
//    {
//        private string _email;

//        public MailAdress(ClientA client)
//        {
//            Client = client;
//        }

//        public ClientA Client { get; set; }

//        public string Email
//        {
//            get { return _email; }
//            set
//            {
//                if (_email != value)
//                {
//                    _email = value;
//                    Client.SendToNewMail();
//                }
//            }
//        }
//    }

//    public class ClientA
//    {
//        MailAdress MailAdress { get; set; }

//        public ClientA()
//        {
//            MailAdress = new MailAdress(this);
//        }

//        public void UpdateEmail(string newEmail)
//        {
//            MailAdress.Email = newEmail;
//        }

//        internal void SendToNewMail()
//        {
//            Debug.WriteLine(string.Format("new mail sent to {0}", MailAdress.Email));
//            TestClass.MailSent = true;
//        }
//    }

//    static class TestClass
//    {
//        public static bool MailSent = false;
//    }

//    [TestFixture]
//    public class ObserverFixture
//    {
//        [Test]
//        public void ShouldSendMailWhenMailUpdated()
//        {
//            var client = new ClientA();
//            client.UpdateEmail("baydek@amadeus.com.tr");

//            Assert.AreEqual(true, TestClass.MailSent);
//        }
//    }
//}