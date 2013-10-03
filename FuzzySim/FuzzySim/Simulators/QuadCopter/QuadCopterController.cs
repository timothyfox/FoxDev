using CFLS;

namespace FuzzySim.Simulators
{
    using System.Collections.Generic;
    using Core;

    /*      ---- Rememeber! ----
     * 
     *      1. Declare FuzzyCollections 
     *      2. Initialize FuzzyCollections - InitializeController()
     *      3. Populate FuzzyCollections with FuzzySets - InitializeFuzzySets() -> 'WrapperMethod'()
     *      4. Add FuzzyCollections to return collection - GetFuzzyLogic()
     */
    
    class QuadCopterController : AIController
    {

        /// <summary>
        /// The variables used for this simulation (Press 'F12') on 'HarrierVars'!
        /// </summary>
        private SimVars.QuadCopterVars quad;

        //RULESETS - You will need two seperate rule sets for your Throttle, and Thrust-Vector
        private FuzzyCollection RuleSetThrottle;
        private FuzzyCollection RuleSetThrustVec;

        //ACCUMULATORS
        private FuzzyCollection ThrottleAccum;
        private FuzzyCollection ThrustVecAccum;

        //FUZZY SETS
        private FuzzyCollection YOUR_COLLECTIONS_HERE; //Declare your FuzzySet Collections
        //
        //
        //


        double _throttle;
        private double _tv;

        public override void ButtonAPress()
        {
            _throttle--;
        }
        public override void ButtonBPress()
        {
            _throttle++;
        }
        public override void ButtonCPress() { }
        public override void ButtonDPress() { }
        public override void ButtonEPress()
        {
            _tv--;
        }
        public override void ButtonFPress()
        {
            _tv++;
        }
        public override void ButtonRandomPress()
        {
            if (quad != null)
            {
                quad.X = (double)new System.Random().Next(5, 300);
                quad.Y = (double)new System.Random().Next(50, 200);
            }
        }

        public override List<FuzzyCollection> GetFuzzyLogic()
        {
            List<FuzzyCollection> sendToOutputWindow = new List<FuzzyCollection>();

            //
            //Your Declared FuzzyCollections need to be added to 'sendToOutputWindow'
            //

            sendToOutputWindow.Add(ThrottleAccum);
            sendToOutputWindow.Add(ThrustVecAccum);

            sendToOutputWindow.Add(RuleSetThrottle);
            sendToOutputWindow.Add(RuleSetThrustVec);

            return sendToOutputWindow;
        }

        public override void CalculateFuzzyLogic()
        {
            //START
            quad = ((QuadCopterSim)Globals.Simulator).QuadCopter;

            double height = quad.Y;
            double speedY = quad.YVel;
            double speedX = quad.XVel;
            double safeX = quad.X - quad.MidSafeX - 40;

            //FuzzySet throttleOutput = ;
            //FuzzySet thrustVectorOutput = ;

            //A clue... ;)
            //throttleOutput.Clear();
            //thrustVectorOutput.Clear();
            //throttleOutput.SetRangeWithPoints(0, 100);
            //thrustVectorOutput.SetRangeWithPoints(-5, 5);


            //WRITE YOUR RULES LIKE THIS....
            //(dont forget to pass the 
            //accumulator variable by reference, and also pass the Rule itself
            //Hot Tip: COMMENT YOUR RULES SO YOU KNOW WHAT THEY DO!

            //if height is high and speed is med, throttle soft		:R1
            //YourRuleSet["Rule0"] = Rule.AND(height, "Your Height Set[high]", speedY, "Your Y Speed Set[medium]", ref throttleOutput, "Your Throttle Set[soft]", YourRuleSet["Rule0"]);	


            //Switch for how to adjust throttle settings ('AutoPilot' Checkbox on form)
            if (!Manual)
            {
                quad.Throttle = _throttle;
                quad.ThrustVector = _tv;
            }
            else
            {
               quad.Throttle = Operations.DeFuzzifyCOG(ThrottleAccum["ThrottleOutput"]);
               quad.ThrustVector = Operations.DeFuzzifyCOG(ThrustVecAccum["ThrustVector"]);
            }

            //THESE ARE UP TO YOU TO TUNE!!
            quad.Throttle += 20;

            //END
            ((QuadCopterSim)Globals.Simulator).QuadCopter = quad;
        }

        /// <summary>
        /// Initialize the controller
        /// </summary>
        public override void InitializeController()
        {
            _throttle = 60;
            _tv = 0;

            //Write your sets here...


            SetupRuleSets();

            SetupAccumulators();
        }


        /// <summary>
        /// Set up Throttle and ThrustVector Output sets 
        /// </summary>
        private void SetupAccumulators()
        {
            //Throttle Accumulator
            ThrottleAccum = new FuzzyCollection("Throttle Output", null)
                                {
                                    new FuzzySet("ThrottleOutput", 0, 100)
                                };
            ThrottleAccum["ThrottleOutput"].SetRangeWithPoints(0, 100);


            //ThrustVector Accumulator
            ThrustVecAccum = new FuzzyCollection("Thrust Vector Output", null)
                                 {
                                     new FuzzySet("ThrustVector", -5, 5)
                                 };
            ThrustVecAccum["ThrustVector"].SetRangeWithPoints(-5, 5);
        }//accumulators are now ready to use


        /// <summary>
        /// Initializes and sets up Rule Sets
        /// </summary>
        private void SetupRuleSets()
        {
            RuleSetThrottle = new FuzzyCollection("Throttle Rules", null);
            RuleSetThrustVec = new FuzzyCollection("Thrust Vector Rules", null);

            // Changes these as you add rules....
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
        }// Rulesets are now ready to use
    }
}
