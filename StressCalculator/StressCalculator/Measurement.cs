﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StressCalculator
{
    class Measurement
    {
        public String UserID;
        public DateTime Date;
        public double GPSLat;
        public double GPSLng;

        private List<double> RRIntervals;
        private List<double> IntervalsDiff; //the difference between each two following intervals
        public double TRI; //the triangular index, 1<TRI<n (n=number of intervals). higher => stressed
        public int NN50; //the number of following intervals that differ in more than 50ms. lower => stressed
        public double PNN50; //the % of NN50 relative to all intervals. lower => stressed
        public double SDNN; //the standard deviation of RR intervals. lower => stressed
        public double SDSD; //the standard deviation of RR intervals differences. lower => stressed
        public int StressIndex;
        public int IsStressed;



        //public double SD1;
        //public double SD2;

        public Measurement(List<double> RRIntervals)
        {
            this.RRIntervals = RRIntervals;

            this.IntervalsDiff = new List<double>();
            for (int i = 0; i < RRIntervals.Count - 1; i++)
            {
                this.IntervalsDiff.Add(RRIntervals[i] - RRIntervals[i + 1]); //the difference between two RR intervals
            }
            SetTRI();
            SetPNN50();
            SetSDNN();
            SetSDSD();
            SetStressIndex();
            SetIsStressed();
        }

        private void SetTRI()
        {
            //default TRI values:
            double binSize = (double)1 / 128;
            int lowBound = 0;
            int highBound = 5;
            List<int> counters = histCount(RRIntervals, binSize, lowBound, highBound);
            this.TRI = counters.Max(); //(double)counters.Sum() / (double)counters.Max();
        }
        private void SetPNN50()
        {
            List<double> temp = IntervalsDiff.Where(item => Math.Abs(item) > 0.05).ToList();
            NN50 = temp.Count;
            PNN50 = ((double)NN50 / (double)(IntervalsDiff.Count)); 
        }
        private void SetSDNN()
        {
            this.SDNN = getSTD(RRIntervals);

        }
        private void SetSDSD()
        {
            this.SDSD = getSTD(IntervalsDiff);
        }
        private double getSTD(List<double> vector)
        {
            List<double> temp = new List<double>();
            int n = vector.Count;
            double avg = vector.Average();
            if (n == 1)
                return 0;

            //copy.AddRange(vector);
            vector.ForEach(x => temp.Add((double)(Math.Pow(x - avg, 2)) / (double)n));
            return Math.Sqrt(temp.Sum());
        }
        private List<int> histCount(List<double> vector, double binSize, int lowBound, int highBound)
        {
            int numOfBins = 0;
            double currBin = lowBound;
            while (currBin < highBound)
            {
                currBin += binSize;
                numOfBins++;
            }
            //a list of counters
            List<int> counters = new List<int>(new int[numOfBins]);

            //create a sorted vector
            List<double> sortedVector = new List<double>();
            sortedVector.AddRange(vector);
            sortedVector.Sort();

            //count the sample in each bin
            int counterIdx = 0;
            int i = 0;
            currBin = lowBound;
            while (i < sortedVector.Count)
            {
                if (currBin <= sortedVector[i] && sortedVector[i] < currBin + binSize)
                {
                    counters[counterIdx]++;
                    i++;
                }
                else
                {
                    counterIdx++;
                    currBin += binSize;
                };
            }
            return counters;
        }
        
        private void SetStressIndex()
        {
            StressIndex = (int)Math.Floor((0.35 * TRI + 0.35*(1-PNN50) + 0.15 * (1-SDNN) + 0.15 * (1-SDSD))*10);
        }
        private async void SetIsStressed()
        {
            List<int> relaxedStressIndexes = await DBSender.GetPrevRelaxStressIndex(UserID);
            int stressedStressIndex = await DBSender.GetPrevStressedStressIndex(UserID);

            if (relaxedStressIndexes.Count == 0)
            {
                IsStressed = 0; //the first measurement for this user.
                return;
            }
            else if (relaxedStressIndexes.Count == 1)
            {
                IsStressed = 1; //second measurement
                return;
            }
            else
            {
                int maxRelaxed = relaxedStressIndexes.Max();
                if (this.StressIndex <= maxRelaxed || this.StressIndex <= 1.1 * maxRelaxed)
                    this.IsStressed = 0;
            }
            if (stressedStressIndex != -1) //the user already had a stress moment
            {
                if (0.80 * stressedStressIndex <= this.StressIndex) //stress index is at least 80% of the last stressed moment
                    IsStressed = 1;
            }
        }

        override
        public String ToString()
        {
            String str = "UserID: " + UserID + "\nDate: " + Date + "\nTRI: " + TRI +
                        "\nPNN50: " + PNN50 + "\nSDNN: " + SDNN + "\nSDSD: " + SDSD;
            return str;
        }

    }
}
