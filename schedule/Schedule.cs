using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Globalization;

namespace ScheduleNS
{
    public struct Interval
    {
        public DateTime Od, Do;

        public Interval (DateTime od, TimeSpan trvani)
        {
            Od = od;
            Do = od + trvani;
        }
    }

    public struct Udalost
    {
        public int Id;
        public Interval interval;

        // int id|DateTime(Od)|DateTime(Do)
        public static Udalost? UdalostZeStringu(string s)
        {
            Udalost u = new Udalost();
            string[] rec = s.Split('|');
            if (int.TryParse(rec[0], out u.Id) &&
                DateTime.TryParse(rec[2]+ " " + rec[3], CultureInfo.CreateSpecificCulture("cs-CZ"), 
                    DateTimeStyles.None, out u.interval.Od) &&
                DateTime.TryParse(rec[4]+ " " + rec[5], CultureInfo.CreateSpecificCulture("cs-CZ"), 
                    DateTimeStyles.None, out u.interval.Do))
                return u;
            else
                return null;
        }

        public static int[] NactiUcastniky(string s)
        {
            string[] UcastniciStr = s.Split(',');
            //{"8998", "5456", "5456"}
            int[] Ucastnici = new int[UcastniciStr.Length];
            //{8998, 5456, 5456}
            int idx = 0;
            foreach (string str in UcastniciStr)
            {
                if (int.TryParse(str, out Ucastnici[idx++]) == false)
                    Console.WriteLine("Účastník {0} nerozeznán.", str);
            }
            return Ucastnici;
        }

        public static bool ZaznamJeUcastniku(string line, int[] ucastnikId)
        {
            int i = 0;
            foreach (int id in ucastnikId)
            {
                if (int.TryParse(line.Split('|')[0], out i))
                    if (i == id)
                        return true;
            }
            return false;
        }

        public static Udalost?[] NactiUdalosti(int[] ucastnici, string[] udalostiStrArr)
        {
            Udalost?[] Udalosti = new Udalost?[udalostiStrArr.Length];

            int idx = 0;
            foreach (string s in udalostiStrArr)
            {
                if (ZaznamJeUcastniku(s, ucastnici))
                {
                    Udalost? u = Udalost.UdalostZeStringu(s);
                    if (u != null)
                        Udalosti[idx++] = u;
                }
            }
            return Udalosti;
        }
    }

    public class Schedule
    {   
        public static DateTime KonecPosledniSchuze(Udalost?[] uArr)
        {
            DateTime Nejvzdalenejsi = DateTime.Now;
            foreach (Udalost u in uArr)
                if (u.interval.Do > Nejvzdalenejsi)
                    Nejvzdalenejsi = u.interval.Do;
            return Nejvzdalenejsi;
        }

        public static Interval VolnyBlokV1Dnu(Udalost?[] udalosti, TimeSpan delkaVolnehoBloku,
            DateTime start, DateTime konec)
        {
            bool[] blockedArray = new bool[(konec.Hour-start.Hour) * 60];
            int startIndex = 0, endIndex = 0;
            foreach (Udalost? u in udalosti)
            {
                if (u != null)
                {
                    if (u?.interval.Od.Hour <= start.Hour)
                        startIndex = 0;
                    else
                        startIndex = (int)(u?.interval.Od - start)?.TotalMinutes;
                    if (u?.interval.Do.Hour >= konec.Hour)
                        endIndex = (konec.Hour - start.Hour) * 60 - 1;
                    else
                        endIndex = (int)(u?.interval.Do - start)?.TotalMinutes;
                    for (int k = startIndex; k<endIndex && k < blockedArray.Length; k++)
                        blockedArray[k] = true;
                }
            }

            int volnyStart = 0, volnyKonec = 0;
            int i = 0;
            while (i < blockedArray.Length)
            {
                if(blockedArray[i] == false)
                {
                    volnyStart = i;
                    while (blockedArray[i] == false && i < blockedArray.Length)
                        i++;
                    volnyKonec = i-1;
                }
               
                if ((volnyKonec - volnyStart) >= delkaVolnehoBloku.TotalMinutes)
                    return new Interval(start.AddMinutes(volnyStart), delkaVolnehoBloku);

                volnyKonec = volnyStart;
                i++;
            }
            return new Interval(konec, new TimeSpan(0));
        }

        static void Main(string[] args)
        {
            int[] ucastnici = Udalost.NactiUcastniky(Console.ReadLine());

            string[] UdalostiStrArr = File.ReadAllLines("data.txt");
            
        }
    }
}
