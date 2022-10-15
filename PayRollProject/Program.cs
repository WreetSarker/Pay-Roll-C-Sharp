using System;
using System.Globalization;
using System.IO;
using System.Numerics;
using System.Xml.Linq;

namespace PayRoll
{

    class Staff
    {
        private float hourlyRate;
        private int hWorked;

        public float TotalPay { get; protected set; }
        public float BasicPay { get; private set; }
        public string NameOfStaff { get; private set; }

        public int HoursWorked
        {
            get { return hWorked; }
            set { 
                if(value > 0)
                {
                    hWorked = value;
                }
                else
                {
                    hWorked = 0;
                }
            }
        }


        public Staff(string name, float rate)
        {
            NameOfStaff = name;
            hourlyRate = rate;
        }

        public virtual void CalculatePay()
        {
            Console.WriteLine("Calculating pay........");

            BasicPay = hourlyRate * hWorked;
            TotalPay = BasicPay;
        }

        public override string ToString()
        {
            return "Name: " + NameOfStaff + "\nTotal Pay: " + TotalPay;
        }
    }

    class Manager: Staff
    {
        private const float managerHourlyRate = 50;
        public int Allowance { get; private set; }

        public Manager(string name):base(name, managerHourlyRate) { } 

        public override void CalculatePay() { 
            base.CalculatePay();
            Allowance = 1000;
            if(HoursWorked > 160)
            {
                TotalPay = TotalPay + Allowance;
            }
        }

        public override string ToString()
        {
            return "Name: " + NameOfStaff + "\nTotal Pay: " + TotalPay;
        }

    }

    class Admin: Staff
    {
        private const float overTimeRate = 15.5f;
        private const float adminHourlyRate = 30;

        public float OverTime { get; private set; }
        public Admin(string name) : base(name, adminHourlyRate) { }

        public override void CalculatePay()
        {
            base.CalculatePay();
            OverTime = overTimeRate * (HoursWorked - 160);
            if(HoursWorked> 160)
            {
                TotalPay += OverTime;
            }
        }

        public override string ToString()
        {
            return "Name: " + NameOfStaff + "\nTotal Pay: " + TotalPay;
        }


    }

    class FileReader
    {

        public List<Staff> ReadFile()
        { 

            List<Staff> myStaff = new List<Staff>();
            string[] result = new string[2];
            string path = "staff.txt";
            string[] separator = {", "};

            if (File.Exists(path))
            {
                using(StreamReader sr = new StreamReader(path))
                {
                    while(sr.EndOfStream != true)
                    {
                        result = sr.ReadLine().Split(separator, StringSplitOptions.None);
                        if (result[1] == "Admin")
                        {
                            Admin admin = new Admin(result[0]);
                            myStaff.Add(admin);
                        }
                        else
                        {
                            Manager manager = new Manager(result[0]);
                            myStaff.Add(manager);
                        }
                    }
                    sr.Close();
                }
            }
            return myStaff;
        }
    }

    class PaySlip
    {
        private int month;
        private int year;

        enum MonthsOfYear { JAN = 1, FEB, MAR, APR, MAY, JUN, JUL, AUG, SEP, OCT, NOV, DEC}
        
        public PaySlip(int payMonth, int payYear)
        {
            month = payMonth;
            year = payYear;
        }

        public void GeneratePaySlip(List<Staff> myStaff)
        {
            string path = "";

            foreach(Staff f in myStaff)
            {
                path = f.NameOfStaff + ".txt";
                using(StreamWriter sw = new StreamWriter(path))
                {
                                        
                    sw.WriteLine("PAYSLIP FOR " + (MonthsOfYear)month + " " + year);
                    sw.WriteLine("==========================");
                    sw.WriteLine("Name of Staff: " + f.NameOfStaff);
                    sw.WriteLine("Hours Worked: " + f.HoursWorked);
                    sw.WriteLine();
                    sw.WriteLine("Basic Pay: " + f.BasicPay.ToString("C3", CultureInfo.CurrentCulture));
                    if (f.GetType() == typeof(PayRoll.Manager))
                    {
                        sw.WriteLine("Allowance: " + ((Manager)f).Allowance);
                    }
                    else
                    {
                        sw.WriteLine("Over Time Pay: " + ((Admin)f).OverTime);
                    }
                    sw.WriteLine();
                    sw.WriteLine("==========================");
                    sw.WriteLine("Total Pay: " + f.TotalPay);

                    sw.Close();

                }
            }
        }

        public override string ToString()
        {
            return "Pay Slip Generated";
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            List<Staff> myStaff = new List<Staff>();
            FileReader fr = new FileReader();
            int month = 0, year = 0;
            while (year == 0)
            {
                Console.Write("\nPlease enter the year: ");
                try
                {
                    year = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + " Please try again.");
                }
            } while (month == 0)
            {
                Console.Write("\nPlease enter the month: ");
                try
                {
                    month = Convert.ToInt32(Console.ReadLine());
                    if (month < 1 || month > 12)
                    {
                        Console.WriteLine("Month must be from 1 to 12.Please try again.");
                        month = 0;
                        }
}
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + " Please try again.");
}
            }
            myStaff = fr.ReadFile();
            for (int i = 0; i < myStaff.Count; i++)
            {
                try
                {
                    Console.Write("\nEnter hours worked for {0}: ",
                    myStaff[i].NameOfStaff);
                    myStaff[i].HoursWorked =
                    Convert.ToInt32(Console.ReadLine());
                    myStaff[i].CalculatePay();
                    Console.WriteLine(myStaff[i].ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    i--;
                }
            }
            PaySlip ps = new PaySlip(month, year);
            ps.GeneratePaySlip(myStaff);
            Console.Read();
        }
    }
}