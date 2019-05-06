using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScheduleNS;

namespace UnitTestSchedule1
{
    [TestClass]
    public class UnitTest1
    {
        string[] sArr = {
            "4227|Jan Novák|10.4.2019|8:00|10.4.2019|12:15\n",
            "5899|Tomáš Jandásek|10.4.2019|8:30|10.4.2019|13:30\n",
            "5899|Tomáš Jandásek|10.4.2019|14:45|10.4.2019|16:30\n",
            "7889|Eva Jelínková|10.4.2019|7:00|10.4.2019|9:30\n",
            "4227|Jan Novák|10.4.2019|15:30|10.4.2019|17:15\n",
            "7889|Eva Jelínková|10.4.2019|14:35|10.4.2019|15:20\n"
        };

        string sUcastnici = "4227, 7889, 5899"; // volný termín od 13:30 do 14:30

        [TestMethod]
        public void TestVytvorUdalostZeStringu()
        {
            Udalost? u = Udalost.UdalostZeStringu("7889|Eva Jelínková|28.4.2019|14:35|10.4.2019|15:20");
            Assert.IsNotNull(u);
        }

        [TestMethod]
        public void TestVytvorPoleUdalosti()
        {
            int pocetNotNullPrvku = 0;

            int[] ucastnici = Udalost.NactiUcastniky(sUcastnici);
            Assert.IsTrue(ucastnici.Length == 3);

            Udalost?[] u = Udalost.NactiUdalosti(ucastnici, sArr);
            foreach (var v in u)
                if (v != null) pocetNotNullPrvku++;
            Assert.IsTrue(pocetNotNullPrvku == 6);
        }

        [TestMethod]
        public void TestNajdiMozneTerminy()
        {
            int[] ucastnici = Udalost.NactiUcastniky(sUcastnici);

            Udalost?[] u = Udalost.NactiUdalosti(ucastnici, sArr);

            Interval volnyTermin = Schedule.VolnyBlokV1Dnu(u, new TimeSpan(1, 0, 0),
            new DateTime(2019, 4, 10, 7, 0, 0), new DateTime(2019, 4, 10, 18, 0, 0));

            Assert.IsTrue(volnyTermin.Od.Hour == 13 && volnyTermin.Od.Minute == 30
                && volnyTermin.Do.Hour == 14 && volnyTermin.Do.Minute == 30);
        }
    }

    [TestClass]
    public class UnitTest2
    {
        string[] sArr = {
            "4227|Jan Novák|10.4.2019|8:00|10.4.2019|12:15\n",
            "5899|Tomáš Jandásek|10.4.2019|8:30|10.4.2019|13:30\n",
            "5899|Tomáš Jandásek|10.4.2019|14:45|10.4.2019|16:30\n",
            "7889|Eva Jelínková|10.4.2019|7:00|10.4.2019|9:30\n",
            "4227|Jan Novák|10.4.2019|15:30|10.4.2019|17:15\n",
            "7889|Eva Jelínková|10.4.2019|14:35|10.4.2019|15:20\n"
        };

        string sUcastnici = "4227, 7889"; // volný termín od 12:15 do 13:15

        [TestMethod]
        public void TestVytvorPoleUdalosti()
        {
            int pocetNotNullPrvku = 0;

            int[] ucastnici = Udalost.NactiUcastniky(sUcastnici);
            Assert.IsTrue(ucastnici.Length == 2);

            Udalost?[] u = Udalost.NactiUdalosti(ucastnici, sArr);
            foreach (var v in u)
                if (v != null) pocetNotNullPrvku++;
            Assert.IsTrue(pocetNotNullPrvku == 4);
        }

        [TestMethod]
        public void TestNajdiMozneTerminy()
        {
            int[] ucastnici = Udalost.NactiUcastniky(sUcastnici);

            Udalost?[] u = Udalost.NactiUdalosti(ucastnici, sArr);

            Interval volnyTermin = Schedule.VolnyBlokV1Dnu(u, new TimeSpan(1, 0, 0),
                new DateTime(2019, 4, 10, 7, 0, 0), 
                new DateTime(2019, 4, 10, 18, 0, 0));

            Assert.IsTrue(volnyTermin.Od.Hour == 12 && volnyTermin.Od.Minute == 15
                && volnyTermin.Do.Hour == 13 && volnyTermin.Do.Minute == 15);
        }
    }
}
