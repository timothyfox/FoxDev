using System.Drawing;
using CFLS;

namespace FuzzySim.Simulators
{
    using System.Collections.Generic;
    using Core;

    class HarrierController : AIController
    {
        private SimVars.HarrierVars harrier;

        //RULESETS
        private FuzzyCollection RuleSetThrottle;
        private FuzzyCollection RuleSetThrustVec;

        //ACCUMULATORS
        private FuzzyCollection ThrottleAccum;
        private FuzzyCollection ThrustVecAccum;


        //FUZZY SETS
        private FuzzyCollection YVel;
        private FuzzyCollection XVel;
        private FuzzyCollection Alt;
        private FuzzyCollection XDist;

        private FuzzyCollection Throttle;
        private FuzzyCollection ThrustVec;


        double throttle;

        private double tv;

        public override void ButtonAPress()
        {
            throttle--;
        }
        public override void ButtonBPress()
        {
            throttle++;
        }
        public override void ButtonCPress() { }
        public override void ButtonDPress() { }
        public override void ButtonEPress()
        {
            tv--;
        }
        public override void ButtonFPress()
        {
            tv++;
        }
        public override void ButtonRandomPress()
        {
            if (harrier != null)
            {
                harrier.X = (double)new System.Random().Next(5, 300);
                harrier.Y = (double)new System.Random().Next(50, 200);
            }
        }

        public override List<FuzzyCollection> GetFuzzyLogic()
        {
            List<FuzzyCollection> ret = new List<FuzzyCollection>();

            ret.Add(XVel);
            ret.Add(YVel);

            ret.Add(Alt);
            ret.Add(XDist);

            ret.Add(Throttle);
            ret.Add(ThrustVec);

            ret.Add(ThrottleAccum);
            ret.Add(ThrustVecAccum);

            ret.Add(RuleSetThrottle);
            ret.Add(RuleSetThrustVec);

            return ret;
        }

        public override void CalculateFuzzyLogic()
        {
            //Get the state of the Harrier 
            harrier = ((HarrierSim) Globals.Simulator).Harrier;

            //Declare some local variables for easy reference
            double height = harrier.Y;
            double speedY = harrier.YVel;
            double speedX = harrier.XVel;
            double safeX = harrier.X - harrier.MidSafeX - 20;



            //Save Accumulator  
            FuzzySet throttleOutput     = ThrottleAccum["ThrottleOutput"];
            FuzzySet thrustVectorOutput = ThrustVecAccum["ThrustVector"];

            double thrustVec = Operations.DeFuzzifyCOG(thrustVectorOutput);

            throttleOutput.Clear();
            thrustVectorOutput.Clear();
            throttleOutput.SetRangeWithPoints(0, 100);
            thrustVectorOutput.SetRangeWithPoints(-5,5);

            //if height is high and speed is med, throttle soft		:R1
            RuleSetThrottle["Rule0"] = Rule.AND(height, Alt["h_high"], speedY, YVel["y_med"], ref throttleOutput, Throttle["t_soft"], RuleSetThrottle["Rule0"]);	

	        //if height is low and speed is slow, throttle soft		:R2
            RuleSetThrottle["Rule1"] = Rule.AND(height, Alt["h_low"], speedY, YVel["y_slow"], ref throttleOutput,
                                                Throttle["t_soft"], RuleSetThrottle["Rule1"]);
	
	        //if height is landing and speed is slow, throttle soft
            RuleSetThrottle["Rule2"] = Rule.AND(height, Alt["h_landing"], speedY, YVel["y_slow"], ref throttleOutput,
                                                Throttle["t_soft"], RuleSetThrottle["Rule2"]);

	        //if height is landing and speed is med, throttle hard
            RuleSetThrottle["Rule3"] = Rule.AND(height, Alt["h_landing"], speedY, YVel["y_med"], ref throttleOutput,
                                                Throttle["t_hard"], RuleSetThrottle["Rule3"]);

	        //IF speed IS up THEN Throttle <- off
            RuleSetThrottle["Rule4"] = Rule.IS(speedY, YVel["y_up"], ref throttleOutput, Throttle["t_off"],
                                                RuleSetThrottle["Rule4"]);

	        //if HEIGHT is HIGH and SPEED is slow, then throttle -> soft

	        //if height is high and speed is slow, throttle soft		:R1
            RuleSetThrottle["Rule5"] = Rule.AND(height, Alt["h_high"], speedY, YVel["y_slow"], ref throttleOutput,
                                                Throttle["t_soft"], RuleSetThrottle["Rule5"]);
	        
                       

	        //IF XSpeed is fwdMed, pitch //> back
            RuleSetThrustVec["Rule0"] = Rule.IS(speedX, XVel["x_f_med"], ref thrustVectorOutput,
                                                ThrustVec["p_backHard"], RuleSetThrustVec["Rule0"]);


	        //if position is right pitch back
            RuleSetThrustVec["Rule1"] = Rule.IS(safeX, XDist["x_right"], ref thrustVectorOutput, ThrustVec["p_back"],
                                                 RuleSetThrustVec["Rule1"]);

	        //if position is right pitch back
            RuleSetThrustVec["Rule2"] = Rule.IS(safeX, XDist["x_veryRight"], ref thrustVectorOutput,
                                                 ThrustVec["p_backHard"], RuleSetThrustVec["Rule2"]);	

	        //if position is right and travelling fwd, hardback
            RuleSetThrustVec["Rule3"] = Rule.IS(safeX, XDist["x_right"], ref thrustVectorOutput, ThrustVec["p_backHard"],
                                                RuleSetThrustVec["Rule3"]);

            RuleSetThrustVec["Rule4"] = Rule.IS(safeX, XDist["x_veryLeft"], ref thrustVectorOutput, ThrustVec["p_fwdMax"],
                                                RuleSetThrustVec["Rule4"]);
            
            RuleSetThrustVec["Rule5"] = Rule.AND(speedX, XVel["x_b_slow"], thrustVec, ThrustVec["p_fwd"] , ref thrustVectorOutput, ThrustVec["p_fwdMax"],
                                                RuleSetThrustVec["Rule5"]);


            ThrottleAccum["ThrottleOutput"] = throttleOutput;
            ThrustVecAccum["ThrustVector"] = thrustVectorOutput;


            if (!Manual)
            {
                harrier.Throttle = throttle;
                harrier.ThrustVector = tv;
            }
            else
            {
                harrier.Throttle = Operations.DeFuzzifyCOG(ThrottleAccum["ThrottleOutput"]);
                harrier.ThrustVector = Operations.DeFuzzifyCOG(ThrustVecAccum["ThrustVector"]); 
            }
            
            harrier.Throttle += 25;
            harrier.ThrustVector += 88;

            ((HarrierSim) Globals.Simulator).Harrier = harrier;
        }

        public override void InitializeController()
        {
            throttle = 60;
            tv = 0;

            SetupVelocitySets();

            SetupHeightAndDistanceSets();

            SetupThrottleSets();

            SetupRuleSets();

            SetupAccumulators();
        }

        private void SetupAccumulators()
        {
            ThrottleAccum = new FuzzyCollection("Throttle Output", null);

            ThrottleAccum.Add(new FuzzySet("ThrottleOutput", 0, 100));
            ThrottleAccum["ThrottleOutput"].SetRangeWithPoints(0, 100);

            ThrustVecAccum = new FuzzyCollection("Thrust Vector Output", null);

            ThrustVecAccum.Add(new FuzzySet("ThrustVector", -5, 5));
            ThrustVecAccum["ThrustVector"].SetRangeWithPoints(-5, 5);
        }

        private void SetupHeightAndDistanceSets()
        {
            Alt = new FuzzyCollection("Height Sets", null);
            XDist = new FuzzyCollection("Distance from SafeZone", null);

            FuzzySet temp;

            temp = new FuzzySet("h_veryHigh", 0, 200);
            temp.LineColour = new SolidBrush(Color.Black);
            temp.AddPoint(0, 0);
            temp.AddPoint(160, 0);
            temp.AddPoint(170, 1);
            temp.AddPoint(200, 1);
            Alt.Add(temp);

            temp = new FuzzySet("h_high", 0, 200);
            temp.LineColour = new SolidBrush(Color.Green);
            temp.AddPoint(0, 0);
            temp.AddPoint(75, 0);
            temp.AddPoint(105, 1);
            temp.AddPoint(140, 1);
            temp.AddPoint(170, 0);
            temp.AddPoint(200, 0);
            Alt.Add(temp);

            temp = new FuzzySet("h_low", 0, 200);
            temp.LineColour = new SolidBrush(Color.Blue);
            temp.AddPoint(0, 0);
            temp.AddPoint(20, 0);
            temp.AddPoint(50, 1);
            temp.AddPoint(75, 1);
            temp.AddPoint(100, 0);
            temp.AddPoint(200, 0);
            Alt.Add(temp);

            temp = new FuzzySet("h_landing", 0, 200);
            temp.LineColour = new SolidBrush(Color.DeepPink);
            temp.AddPoint(0, 1);
            temp.AddPoint(30, 1);
            temp.AddPoint(40, 0);
            temp.AddPoint(200, 0);
            Alt.Add(temp);

            //XDISTANCE
            temp = new FuzzySet("x_veryLeft", -200, 200);
            temp.LineColour = new SolidBrush(Color.Maroon);
            temp.AddPoint(-200, 1);
            temp.AddPoint(-100, 1);
            temp.AddPoint(-40, 0);
            temp.AddPoint(200, 0);
            XDist.Add(temp);

            temp = new FuzzySet("x_left", -200, 200);
            temp.AddPoint(-200, 0); temp.LineColour = new SolidBrush(Color.Lime);
            temp.AddPoint(-160, 0);
            temp.AddPoint(-20, 1);
            temp.AddPoint(-5, 1);
            temp.AddPoint(20, 0);
            temp.AddPoint(200, 0);
            XDist.Add(temp);


            temp = new FuzzySet("x_right", -200, 200);
            temp.AddPoint(-200, 0); temp.LineColour = new SolidBrush(Color.Navy);
            temp.AddPoint(-200, 0);
            temp.AddPoint(-20, 0);
            temp.AddPoint(5, 1);
            temp.AddPoint(40, 1);
            temp.AddPoint(160, 0);
            temp.AddPoint(200, 0);
            XDist.Add(temp);

            temp = new FuzzySet("x_veryRight", -200, 200);
            temp.AddPoint(-200, 0); temp.LineColour = new SolidBrush(Color.Red);
            temp.AddPoint(-200, 0);
            temp.AddPoint(30, 0);
            temp.AddPoint(100, 1);
            temp.AddPoint(200, 1);
            XDist.Add(temp);

        }

        private void SetupRuleSets()
        {
            RuleSetThrottle = new FuzzyCollection("Throttle Rules", null);
            RuleSetThrustVec = new FuzzyCollection("Thrust Vector Rules", null);

            int tRules = 10;
            int tvRules = 10;

            for (int i = 0; i < tRules; i++)
            {
                RuleSetThrottle.Add(new FuzzySet("Rule" + i.ToString(), 0, 100) { LineColour = ruleColours[i] });
                RuleSetThrottle["Rule" + i.ToString()].SetRangeWithPoints(0, 100);
            }

            for (int i = 0; i < tvRules; i++)
            {
                RuleSetThrustVec.Add(new FuzzySet("Rule" + i.ToString(), 0, 100) { LineColour = ruleColours[i] });
                RuleSetThrustVec["Rule" + i.ToString()].SetRangeWithPoints(-5, 5);
            }
        }

        private void SetupThrottleSets()
        {
            Throttle = new FuzzyCollection("Throttle Sets", null);
            ThrustVec = new FuzzyCollection("ThrustVector Sets", null);

            FuzzySet temp;

            //THROTTLE
            temp = new FuzzySet("t_hard", 0, 100);
            temp.LineColour = new SolidBrush(Color.LightCoral);
            temp.AddPoint(0, 100);
            temp.AddPoint(0, 0);
            temp.AddPoint(91, 0);
            temp.AddPoint(95, 1);
            temp.AddPoint(100, 1);
            Throttle.Add(temp);

            temp = new FuzzySet("t_med", 0, 100);
            temp.LineColour = new SolidBrush(Color.MediumBlue);
            temp.AddPoint(0, 0);
            temp.AddPoint(75, 0);
            temp.AddPoint(86, 1);
            temp.AddPoint(90, 1);
            temp.AddPoint(95, 0);
            temp.AddPoint(100, 0);
            Throttle.Add(temp);

            temp = new FuzzySet("t_soft", 0, 100);
            temp.LineColour = new SolidBrush(Color.LawnGreen);
            temp.AddPoint(0, 0);
            temp.AddPoint(66, 0);
            temp.AddPoint(70, 1);
            temp.AddPoint(80, 1);
            temp.AddPoint(85, 0);
            temp.AddPoint(100, 0);
            Throttle.Add(temp);

            temp = new FuzzySet("t_off", 0, 100);
            temp.LineColour = new SolidBrush(Color.Indigo);
            temp.AddPoint(0, 0);
            temp.AddPoint(40, 0);
            
            //this will float up....
            //temp.AddPoint(60, 1);
            //temp.AddPoint(65, 1);

            //and this will fix it
            temp.AddPoint(50, 1);
            temp.AddPoint(55, 1);

            temp.AddPoint(68, 0);
            temp.AddPoint(100, 0);
            Throttle.Add(temp);

            //THRUST VEC
            temp = new FuzzySet("p_fwdHard", -5, 5);
            temp.LineColour = new SolidBrush(Color.Green);
            temp.AddPoint(-5, 1);
            temp.AddPoint(-2, 0);
            temp.AddPoint(5, 0);
            ThrustVec.Add(temp);

            //THRUST VEC
            temp = new FuzzySet("p_fwdMax", -5, 5);
            temp.LineColour = new SolidBrush(Color.BlueViolet);
            temp.AddPoint(-5, 1);
            temp.AddPoint(-4, 1);
            temp.AddPoint(-3, 0);
            temp.AddPoint(5, 0);
            ThrustVec.Add(temp);


            temp = new FuzzySet("p_fwd", -5, 5);
            temp.LineColour = new SolidBrush(Color.Red);
            temp.AddPoint(-5, 0);
            temp.AddPoint(-4, 0);
            temp.AddPoint(-2, 1);
            temp.AddPoint(0, 0);
            temp.AddPoint(5, 0);
            ThrustVec.Add(temp);

            temp = new FuzzySet("p_back", -5, 5);
            temp.LineColour = new SolidBrush(Color.Black);
            temp.AddPoint(-5, 0);
            temp.AddPoint(-1, 0);
            temp.AddPoint(2, 1);
            temp.AddPoint(4, 0);
            temp.AddPoint(5, 0);
            ThrustVec.Add(temp);

            temp = new FuzzySet("p_backHard", -5, 5);
            temp.LineColour = new SolidBrush(Color.Orange);
            temp.AddPoint(-5, 0);
            temp.AddPoint(3, 0);
            temp.AddPoint(5, 1);
            ThrustVec.Add(temp);

        }

        private void SetupVelocitySets()
        {
            YVel = new FuzzyCollection("Vertical Speed", null);
            XVel = new FuzzyCollection("Horizontal Speed", null);

            FuzzySet temp;

            //VERTICAL
            temp = new FuzzySet("y_up", -50, 20);
            temp.LineColour = new SolidBrush(Color.IndianRed);
            temp.AddPoint(-50, 0);
            temp.AddPoint(-3, 0);
            temp.AddPoint(10, 1);
            temp.AddPoint(20, 1);
            YVel.Add(temp);

            temp = new FuzzySet("y_slow", -50, 20);
            temp.LineColour = new SolidBrush(Color.Gold);
            temp.AddPoint(-50, 0);
            temp.AddPoint(-16, 0);
            temp.AddPoint(-5, 1);
            temp.AddPoint(-4, 1);
            temp.AddPoint(-1, 0);
            temp.AddPoint(20, 0);
            YVel.Add(temp);

            temp = new FuzzySet("y_med", -50, 20);
            temp.LineColour = new SolidBrush(Color.Fuchsia);
            temp.AddPoint(-50, 0);
            temp.AddPoint(-28, 0);
            temp.AddPoint(-20, 1);
            temp.AddPoint(-12, 1);
            temp.AddPoint(-6, 0);
            temp.AddPoint(20, 0);
            YVel.Add(temp);

            temp = new FuzzySet("y_fast", -50, 20);
            temp.LineColour = new SolidBrush(Color.ForestGreen);
            temp.AddPoint(-50, 1);
            temp.AddPoint(-30, 1);
            temp.AddPoint(-22, 0);
            temp.AddPoint(20, 0);
            YVel.Add(temp);


            //HORIZONTAL
            temp = new FuzzySet("x_f_fast", -50, 50);
            temp.LineColour = new SolidBrush(Color.DimGray);
            temp.AddPoint(-50, 0);
            temp.AddPoint(20, 0);
            temp.AddPoint(25, 1);
            temp.AddPoint(50, 1);
            XVel.Add(temp);

            temp = new FuzzySet("x_f_med", -50, 50);
            temp.LineColour = new SolidBrush(Color.BlueViolet);
            temp.AddPoint(-50, 0);
            temp.AddPoint(1, 0);
            temp.AddPoint(10, 1);
            temp.AddPoint(15, 1);
            temp.AddPoint(23, 0);
            temp.AddPoint(50, 0);
            XVel.Add(temp);

            temp = new FuzzySet("x_safe", -50, 50);
            temp.LineColour = new SolidBrush(Color.Chartreuse);
            temp.AddPoint(-50, 0);
            temp.AddPoint(-2, 0);
            temp.AddPoint(0, 1);
            temp.AddPoint(2, 0);
            temp.AddPoint(50, 0);
            XVel.Add(temp);

            temp = new FuzzySet("x_b_slow", -50, 50);
            temp.LineColour = new SolidBrush(Color.Brown);
            temp.AddPoint(-50, 0);
            temp.AddPoint(-14, 0);
            temp.AddPoint(-10, 1);
            temp.AddPoint(-6, 1);
            temp.AddPoint(-3, 0);
            temp.AddPoint(50, 0);
            XVel.Add(temp);


            temp = new FuzzySet("x_b_med", -50, 50);
            temp.LineColour = new SolidBrush(Color.CadetBlue);
            temp.AddPoint(-50, 0);
            temp.AddPoint(-23, 0);
            temp.AddPoint(-15, 1);
            temp.AddPoint(-10, 1);
            temp.AddPoint(-1, 0);
            temp.AddPoint(50, 0);
            XVel.Add(temp);

            temp = new FuzzySet("x_b_fast", -50, 50);
            temp.LineColour = new SolidBrush(Color.Red);
            temp.AddPoint(-50, 1);
            temp.AddPoint(-25, 1);
            temp.AddPoint(-20, 0);
            temp.AddPoint(50, 0);
            XVel.Add(temp);
        }


    }
}
