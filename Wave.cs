﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purpose
{
    //Using the StreamReader gets infor needed to run the waves
    public class Wave
    {
        //Fields for the number of enemies to spawn
        private int numberOfMelee;
        private int numberOfRanged;

        //Holds level of difficulty for enemies
        private int difficulty;

        //Properties for enemies
        public int NumberOfMelee
        {
            get { return numberOfMelee; }
            set { numberOfMelee = value; }
        }

        public int NumberOfRanged
        {
            get { return numberOfRanged; }
            set { numberOfRanged = value; }
        }

        //Property for Difficulty
        public int Difficulty
        {
            get { return difficulty; }
            set { difficulty = value; }
        }

        //Parameterized Constructor
        public Wave(int numberOfMelee, int numberOfRanged, int difficulty)
        {
            //Numbers for enemies and difficulty saved into gamemanager
            this.numberOfMelee = numberOfMelee;
            this.numberOfRanged = numberOfRanged;
            this.difficulty = difficulty;
        }
    }
}
